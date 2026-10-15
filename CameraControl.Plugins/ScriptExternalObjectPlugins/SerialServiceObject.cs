using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace CameraControl.Plugins.ScriptExternalObjectPlugins
{
	public sealed class SerialServiceObject : IScriptExternalObject, IDisposable
	{
		private const char START_REQUEST = '\a';
		private const char REQUEST_VALUE_SEPARATOR = '=';
		private const char END_REQUEST = '\f';
		private const char START_RESPONSE = '\a';
		private const char RESPONSE_VALUE_SEPARATOR = ':';
		private const char END_RESPONSE = '\f';
		private const int RESPONSE_OK = 0;
		private const int RESPONSE_ERROR = 2;
		private const int RESPONSE_UNKNOWN_NAME = 1;

		private readonly SerialPort _port;
		private bool _waitingForRequest;
		private string _stillToLog;

		public SerialServiceObject (SerialServiceProvider provider, string name, IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> parameters)
		{
			Name = name;
			Provider = provider;
			ProviderParameters = parameters;

			var stopBits = (string)provider.Parameters[4].GetValue(parameters);
			_port = new SerialPort()
			{
				PortName = (string)provider.Parameters[0].GetValue(parameters),
				BaudRate = (int)(long)provider.Parameters[1].GetValue(parameters),
				DataBits = (int)(long)provider.Parameters[2].GetValue(parameters),
				Parity = (Parity)Enum.Parse(typeof(Parity), (string)provider.Parameters[3].GetValue(parameters)),
				StopBits = stopBits == "1" ? StopBits.One : stopBits == "1.5" ? StopBits.OnePointFive : StopBits.Two,
				Handshake = Handshake.None,
				NewLine = "\n",
#if DEBUG
				ReadTimeout = Debugger.IsAttached ? 20000 : 2000,
#else
				ReadTimeout = 2000,
#endif
				WriteTimeout = 500,
			};
			_port.DataReceived += MonitorDebugOutput;
			if (!(bool)provider.Parameters[5].GetValue(parameters))
			{
				Activate();
			}
		}

		public void Dispose ()
		{
			if (_port.IsOpen)
			{
				_port.Close();
			}
		}

		public IScriptExternalObjectProvider Provider { get; }

		public IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> ProviderParameters { get; }

		public string Name
		{
			get;
		}

		public Uri BaseUrl
		{
			get;
		}

		public IEnumerable<ScriptExternalObjectProperty> Properties
		{
			get;
			private set;
		} = Array.Empty<ScriptExternalObjectProperty>();

		public bool IsActivated
		{
			get;
			private set;
		}

		public void Activate ()
		{
			if (!IsActivated)
			{
				try
				{
					if (!_port.IsOpen)
					{
						_port.Open();
					}
				}
				catch (Exception ex)
				{
					throw new Exception($"Cannot open serial port {_port.PortName}: {ex.Message}");
				}

				var properties = SendRequest(null, "=Index");
				Properties = ScriptExternalObjectProperty.Parse(properties);
				IsActivated = true;
			}
		}

		public object Read (string propertyName)
		{
			var property = GetProperty(propertyName);
			var stringValue = SendRequest(property, property.Name);
			return property.FromStringValue(stringValue);
		}

		public void Write (string propertyName, object value)
		{
			var property = GetProperty(propertyName);
			var stringValue = property.ToStringValue(value);
			SendRequest(property, property.Name + REQUEST_VALUE_SEPARATOR + stringValue);
		}

		public void Trigger (string propertyName)
		{
			var property = GetProperty(propertyName);
			SendRequest(property, property.Name);
		}

		public bool IsEventRaised (string propertyName)
		{
			var property = GetProperty(propertyName);
			var stringValue = SendRequest(property, property.Name);
			return (bool)property.FromStringValue(stringValue);
		}

		private ScriptExternalObjectProperty GetProperty (string propertyName)
		{
			var result = (from p in Properties
						  where p.Name == propertyName
						  select p).FirstOrDefault()
						  ?? throw new ArgumentException($"Device '{Name}' has no property '{propertyName}'");
			return result;
		}

		private string SendRequest (ScriptExternalObjectProperty property, string request)
		{
			if (!_port.IsOpen)
			{
				IsActivated = false;
				if (_stillToLog != null)
				{
					AddToLog("", true);
				}
				Log.Debug($"Port {_port.PortName} has been closed unexpectedly; re-activating device '{Name}'");
				Activate();
			}
			try
			{
				_waitingForRequest = true;
				_port.WriteLine(START_REQUEST + request + END_REQUEST);
			}
			catch (Exception ex)
			{
				_waitingForRequest = false;
				var msg = $"Cannot communicate with via the serial port {_port.PortName}: {ex.Message}";
				Log.Error(msg, ex);
				throw new Exception(msg);
			}
			string response = null;
			while (true)
			{
				try
				{
					if (_port.BytesToRead > 0)
					{
						var received = _port.ReadExisting();
						int start = -1;
						if (response == null)
						{
							start = received.IndexOf(START_RESPONSE);
							if (start < 0)
							{
								AddToLog(received, false);
								continue;
							}
							else
							{
								if (start > 0 || _stillToLog != null)
								{
									AddToLog(received.Substring(0, start), true);
								}
								response = "";
							}
						}

						var end = received.IndexOf(END_RESPONSE, start + 1);
						if (end < 0)
						{
							response += received.Substring(start + 1);
						}
						else
						{
							response += received.Substring(start + 1, end - start - 1);
							end++;
							while (end < received.Length)
							{
								if (received[end] == '\r' || received[end] == '\n')
								{
									end++;
								}
								else
								{
									AddToLog(received.Substring(end), received.IndexOf('\n', end) > 0);
									break;
								}
							}
							_waitingForRequest = false;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					_waitingForRequest = false;
					var msg = $"Cannot communicate with via the serial port {_port.PortName}: {ex.Message}";
					Log.Error(msg, ex);
					throw new Exception(msg);
				}
			}
			var separator = response.IndexOf(RESPONSE_VALUE_SEPARATOR);
			if (!int.TryParse(separator < 0 ? response : response.Substring(0, separator), out var code))
			{
				var msg = $"Invalid response received from the serial port {_port.PortName}";
				Log.Error(msg);
				throw new Exception(msg);
			}
			else if (code != RESPONSE_OK)
			{
				var msg = code == RESPONSE_UNKNOWN_NAME
					? $"Error while processing request by '{Name}': unknown property/trigger/event '{property?.Name ?? "=Index"}'"
					: $"Error while processing request for '{property?.Name ?? "=Index"}' by '{Name}'";
				Log.Error(msg);
				throw new Exception(msg);
			}
			return separator < 0 || separator == response.Length - 1 ? null : response.Substring(separator + 1);
		}

		private void MonitorDebugOutput (object sender, SerialDataReceivedEventArgs e)
		{
			if (!_waitingForRequest)
			{
				var received = _port.ReadExisting();
				AddToLog(received, e.EventType == SerialData.Eof);
			}
		}
		private void AddToLog (string toLog, bool logNow)
		{
			if (_stillToLog == null)
			{
				_stillToLog = $"{_port.PortName}: ";
			}
			_stillToLog += toLog;

			if (logNow)
			{
				Log.Debug(_stillToLog.TrimEnd());
				_stillToLog = null;
			}
		}
	}
}

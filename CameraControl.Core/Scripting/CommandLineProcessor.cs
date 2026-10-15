#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.Core.Scripting
{
	public class CommandLineProcessor : IDisposable
	{
		public CommandLineProcessor (ICameraDevice targetDevice = null, Func<bool> isCancellationRequested = null)
		{
			_targetDevice = targetDevice;
			_alwaysUseScriptCamera = _targetDevice != null;
			_isCancellationRequested = isCancellationRequested;
		}
		private Func<bool> _isCancellationRequested;

		public CommandLineProcessor (string targetDevice)
			: this(GetCamera(targetDevice))
		{
		}

		public void Dispose ()
		{
			ScriptExecutionCompleted();
		}

		public void ScriptExecutionCompleted ()
		{
			DiscardSyncEvents(this);
		}

		public const string SingleCommandDocumentationUrl = "https://www.digicamcontrol.com/doc/userguide/singlecmd";

		private const string _seeHelp = "; use 'help' or (in tcl) 'echo [ dcc help ]' for assistence.";
		public object Pharse (string[] args)
		{
			var cmd = args[0].ToLower().Trim();
			switch (cmd)
			{
				case "help":
					return $@"Application control

All commands apply to the application and the camera and session that is
currently selected in the application. The commands may change what is
visible in the user interface. The camera/capture commands may be
applied to a different camera and session (see below).

List
	Cameras
		Lists all connected cameras.
	Cmds
		Lists all available window commands. Commands are executed via 'do'.
	Session
		Lists all properties of the session.
	SyncEvents
		List all synchronisation events that are used in one or more running
		tcl scripts or single line commands.

Get
	Camera
		Returns the serial number of the camera.
	Session
		Returns the name of the session.
	Session.<name>
		Returns the value of the property <name> of the session. See
		'List Session' for valid property names.

Set
	Camera <id>
		Select a camera as the currently selected camera in the application.
		<id> is either the serial number of the camera or the device name
		with spaces replaced by '_'. If no camera has been selected via
		'Set ScriptCamera' (see below), the camera/capture control commands
		apply to the selected camera.
	Session <name>
		Select the session as the currently selected session in the application.
	Session.<name> <value>
		Sets the value of the property <name> of the session. See
		'List Session' for valid property names.

Do
	<name>
		Execute the command <name> associated with an action that is typically
		started from one of the (open) windows of the application. See
		'List Cmds' for valid command names.

SyncEvent <name>
	Commands related to a synchronisation event with name <name> (a name without
	spaces). A script uses a synchronisation event to ensure that a command in
	the script is started at the same time as a command in one or more other
	scripts that run in parallel.

	Use
		Indicates that the script is going to wait for the event after the next
		couple of commands have been completed. E.g., after one or more captures
		of the ScriptCamera (see below).
	Wait
	Wait <milliseconds>
		Pauses the script. Once all scripts that use this event have executed
		the 'SyncEvent <name> Wait' command, all paused scripts resume
		execution. The parameter <milliseconds> is optional and must be zero or
		larger. If it is specified, the script only waits the indicated number
		of milliseconds before it cancels the wait. The command returns 'true'
		if the wait has been completed, false if it was cancelled.
	Discard
		Indicates that the script will no longer wait for the event. If this
		was the last script that used the event, the event is removed from
		the application. If a script stops running, the application will call
		'SyncEvent <name> Discard' if the script has not yet done so.
	After <seconds>
		Indicates that the resumption of scripts after 'SyncEvent <name> Wait'
		should not occur before <seconds> after now. Overwrites the
		date/time specified by NotBefore.
	NotBefore <seconds>
		Indicates that the resumption of scripts after 'SyncEvent <name> Wait'
		should not occur before a particular date/time. The date/time is
		specified as seconds since 1-1-1970, the same as returned by tcl
		commands like 'clock scan ""2026-09-22 18:07:00""'. Overwrites the
		date/time specified by After.
	Interval -
	Interval <seconds>
		Indicates that the resumption of scripts after 'SyncEvent <name> Wait'
		should not occur before <seconds> after the resumption after a previous
		'SyncEvent <name> Wait' event. Can be specified in addition to After
		or NotBefore. Disable this feature by specifying '-' instead of
		<seconds>.

Camera/capture control

All commands apply to the camera controlled by the script. By default that
is the camera selected in the application. Select a different camera via
'Set ScriptCamera' or 'Set Camera' (see application control).

List
	Camera
		Lists the camera-specific settings.
    Property
        Lists the DCC camera properties.
	Camera.Session
		Lists all properties of the session that is used for the camera
		controlled by the script.
    LiveView
        Lists the DCC live view settings of the camera controlled by the script.
	Camera.<name>
		Lists the allowed values for the parameter <name>. See 'List Camera' 
		for valid parameter names.

Get
	ScriptCamera
		Returns the serial number of the camera controlled by the script.
	Camera.<name>
		Returns the value of the camera-specific setting <name>. See
		'List Camera' for valid setting names.
    Property.<name>
        Returns the value of the DCC camera property <name>. See 'List Property'
		for valid property names.
	Camera.Session
		Returns the name of the session that is used for the camera.
	Camera.Session.<name>
		Returns the value of the property <name> of the session that is used
		for the camera. See 'List Camera.Session' for valid property names.
	Transfer
		Returns where the captured photos and videos are stored:
		on the PC and/or camera.
    LiveView.<name>
        Returns the value of the DCC live view setting for the camera. See 
		'List LiveView' for valid property names.
	Camera.RecordCondition
		Returns empty if video recording can be started,or an error condition
		(text) otherwise.
	LastCaptured
		Returns the file name of the last captured photo. If a capture is in
		progress '-' will be returned.

Set
	ScriptCamera
	ScriptCamera <id>
		Select a camera as the camera to be controlled by this script.
		<id> is either the serial number of the camera or the device name
		with spaces replaced by '_'. If no <id> is specified, the currently
		selected camera in the application will be used.
	Camera.<name> <value>
		Assigns the value of the camera-specific setting <name>. The <value> 
		is case sensitive, with spaces in the value replaced by '_'.
		See 'List Camera' for valid setting names. 
    Property.<name> <value>
        Assigns the value of the DCC camera property <name>. The <value> 
		is case sensitive, with spaces in the value replaced by '_'.
		See 'List Property' for valid property names.
	Camera.Session <name>
		Selects the session to use for the camera. See 'List Sessions' for valid
		session names.
	Camera.Session.<name> <value>
		Assigns the value of the property <name> of the session that is used
		for the camera. See 'List Camera.Session' for valid property names.
	Transfer <value>
		Select where the captured photos and videos are stored. The <value> is one
		of (case insensitive):
			Save_to_PC_only
			Save_to_camera_only
			Save_to_PC_and_camera
    LiveView.<name> <value>
        Assign the value of the DCC live view setting for the camera. The
		<value> is case sensitive, with spaces in the value replaced by '_'.
		See 'List LiveView' for valid property names.

Capture
	Capture a still image. Autofocus is used if necessary and if the camera is
	configured to use autofocus.

Capture <full path of image file>
	Shorthand for three commands:
		Set Camera.Session.Folder <full path of image file directory>
		Set Camera.Session.FileNameTemplate <image file name without extension>
		Capture

CaptureNoAf
	Capture a still image without using autofocus.

StartRecord
	Start recording a video. For some cameras the live view window should be
	open. For backward compatibility, 'Do StartRecord' is supported as an alias
	for this command.

StopRecord
	Finish recording the video.
	For backward compatibility, 'Do StopRecord' is supported as an alias
	for this command.


For more information visit {SingleCommandDocumentationUrl}.
Unless stated otherwise commands, parameter names and values are case
insensitive."
						.Replace("\t", "    ").Replace("\r", "").Split('\n');

				case "capturenoaf":
				case "capture":
					{
						var device = GetDevice();
						if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
						{
							var session = device.AttachedPhotoSession as PhotoSession ?? ServiceProvider.Settings.DefaultSession;

							// fix if space is in file name 
							//http://digicamcontrol.com/phpbb/viewtopic.php?f=4&t=1929&p=5486#p5486
							var file = string.Join(" ", args.Skip(1)).Trim();
							if (file.Contains(":\\") || file.StartsWith(@"\\"))
							{
								session.Folder = Path.GetDirectoryName(file);
								session.FileNameTemplate = Path.GetFileNameWithoutExtension(file);
							}
							else
							{
								session.FileNameTemplate = file;
							}

						}
						if (cmd == "capturenoaf")
							CameraHelper.CaptureNoAf(device);
						else
							CameraHelper.CaptureWithError(device);
						device.WaitForCamera(30000);
						return null;
					}

				case "startrecord":
					return CameraHelper.StartRecordVideo(GetDevice());
				case "stoprecord":
					return CameraHelper.StopRecordVideo(GetDevice());

				//case "startbulb":
				//    CameraHelper.Capture(GetDevice());
				//    return null;
				//case "stopbulb":
				case "set":
					Set(args.Skip(1).ToArray());
					Thread.Sleep(200);
					return null;
				case "do":
					return DoCmd(args.Skip(1).ToArray());
				case "get":
					return Get(args.Skip(1).ToArray());
				case "list":
					return List(args.Skip(1).ToArray());
				case "syncevent":
					return SyncEventCmd(args.Skip(1).ToArray());
				default:
					throw new Exception($"Unknow parameter {cmd}; ");
			}
		}

		private string DoCmd (string[] args)
		{
			var device = GetDevice();
			var arg = args[0].ToLower().Trim();
			switch (arg)
			{
				case "startrecord":
					return CameraHelper.StartRecordVideo(device);
				case "stoprecord":
					return CameraHelper.StopRecordVideo(device);
				default:
					ServiceProvider.WindowsManager.ExecuteCommand(args[0]);
					// cammand with _ are special commands 
					if (!args[0].Contains("_") && !ServiceProvider.Settings.Actions.Select((x) => x.Name).Contains(args[0]))
						throw new Exception(string.Format("Invalid command {0}", args[0]));
					break;
			}
			return "";
		}

		private static readonly IEnumerable<string> _noValues = Array.Empty<string>();

		private object List (string[] args)
		{
			var device = GetDevice();
			var arg = args[0].ToLower().Trim();

			switch (arg)
			{
				case "syncevents":
					lock (_syncEvents)
					{
						var list = new List<string>();
						foreach (var syncEvent in from ev in _syncEvents.Values
												  orderby ev.Name.ToLower()
												  select ev)
						{
							syncEvent.List(list);
						}
						return string.Join("\n", list);
					}
				case "shutterspeed":
					return device.ShutterSpeed?.Values ?? _noValues;
				case "iso":
					return device.IsoNumber.Values;
				case "exposurecompensation":
					return device.ExposureCompensation?.Values ?? _noValues;
				case "aperture":
					return device.FNumber?.Values ?? _noValues;
				case "focusmode":
					return device.FocusMode?.Values ?? _noValues;
				case "whitebalance":
					return device.WhiteBalance?.Values ?? _noValues;
				case "mode":
					return device.Mode?.Values ?? _noValues;
				case "compressionsetting":
					return device.CompressionSetting?.Values ?? _noValues;
				case "sessions":
					return ServiceProvider.Settings.PhotoSessions.Select(x => x.Name).ToArray();
				case "cmds":
					return ServiceProvider.WindowsManager.WindowCommands.Select(x => x.Name).ToList();
				case "cameras":
					return
						ServiceProvider.DeviceManager.ConnectedDevices.Where(x => x.IsConnected)
							.Select(x => x.SerialNumber)
							.ToArray();
				case "session":
					{
						return (from prop in GetProperties(typeof(PhotoSession)) select "session." + prop.Name.ToLower() + "=" + prop.GetValue(ServiceProvider.Settings.DefaultSession, null)).ToList();
					}
				case "property":
					{
						var properties = device.LoadProperties();
						return (from prop in GetProperties(typeof(CameraProperty)) select "property." + prop.Name.ToLower() + "=" + prop.GetValue(properties, null)).ToList();
					}
				case "liveview":
					{
						var settings = device.LoadProperties().LiveviewSettings;
						return settings == null
							? new List<string>()
							: (from prop in GetProperties(typeof(LiveviewSettings)) select "liveview." + prop.Name.ToLower() + "=" + prop.GetValue(settings, null)).ToList();
					}
				case "camera.session":
					{
						var session = device.AttachedPhotoSession ?? ServiceProvider.Settings.DefaultSession;
						return session == null
							? new List<string>()
							: (from prop in GetProperties(typeof(PhotoSession)) select "camera.session." + prop.Name.ToLower() + "=" + prop.GetValue(session, null)).ToList();
					}
				case "camera":
					{
						IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
						List<string> res = new List<string>();
						foreach (PropertyInfo info in props)
						{
							if (info.PropertyType.Name.StartsWith("PropertyValue"))
							{
								dynamic valp = info.GetValue(device, null);
								object val = valp?.Value;
								if (val != null)
								{
									res.Add("camera." + info.Name.ToLower() + "=" + val);
								}
							}
						}
						foreach (PropertyValue<long> property in device.AdvancedProperties)
						{
							if (!string.IsNullOrEmpty(property.Name) && property.Value != null)
							{
								res.Add("camera." + property.Name.ToLower().Replace(" ", "_") + "=" + property.Value);
							}
						}
						res.Add("camera." + "exposurestatus" + "=" + device.ExposureStatus);
						if (device.AttachedPhotoSession is PhotoSession session)
						{
							res.Add("camera." + "session" + "=" + session.Name);
						}
						return res;
					}
				default:
					if (arg.StartsWith("camera."))
					{
						IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
						foreach (PropertyInfo info in props)
						{
							if (info.PropertyType.Name.StartsWith("PropertyValue") &&
								(arg.Split('.')[1].ToLower().Replace(" ", "_") == info.Name.ToLower()))
							{
								dynamic valp = info.GetValue(device, null);
								if (valp != null)
									return valp.Values;
								else
									return _noValues;
							}
						}
						foreach (PropertyValue<long> property in device.AdvancedProperties)
						{
							if (!string.IsNullOrEmpty(property.Name) && (arg.Split('.')[1].ToLower() == property.Name.ToLower().Replace(" ", "_")))
							{
								return property.Value != null ? property.Values : _noValues;
							}
						}
					}
					throw new Exception($"Unknow parameter {arg}");
			}
		}

		private object Get (string[] args)
		{
			var device = GetDevice();
			var arg = args[0].ToLower().Trim();

			switch (arg)
			{
				case "transfer":
					{
						CameraProperty property = ServiceProvider.DeviceManager.SelectedCameraDevice.LoadProperties();
						if (ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
						{
							if (ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam)
							{
								return "Save to PC only";
							}
							else if (property.NoDownload)
							{
								return "Save to camera only";
							}
							else
							{
								return "Save to PC and camera";
							}
						}

						return (property.NoDownload) ? "Save to camera only" : "Save to PC and camera";
					}
				case "shutterspeed":
					return device.ShutterSpeed?.Value;
				case "iso":
					return device.IsoNumber?.Value;
				case "exposurecompensation":
					return device.ExposureCompensation?.Value;
				case "aperture":
					return device.FNumber?.Value;
				case "focusmode":
					return device.FocusMode?.Value;
				case "whitebalance":
					return device.WhiteBalance?.Value;
				case "mode":
					return device.Mode?.Value;
				case "compressionsetting":
					return device.CompressionSetting?.Value;
				case "lastcaptured":
					{
						if (ServiceProvider.DeviceManager.LastCapturedImage.TryGetValue(device, out var image)
							&& image != "-")
						{
							return Path.GetFileName(image);
						}
						else
						{
							return "-";
						}
					}
				case "session":
					return ServiceProvider.Settings.DefaultSession.Name;
				case "camera.session":
					return (device.AttachedPhotoSession as PhotoSession ?? ServiceProvider.Settings.DefaultSession).Name;
				case "camera.exposurestatus":
					return device.ExposureStatus;
				case "camera.recordcondition":
					return device.GetProhibitionCondition(OperationEnum.RecordMovie);
				case "camera":
					return (_alwaysUseScriptCamera ? device : ServiceProvider.DeviceManager.SelectedCameraDevice).SerialNumber;
				case "scriptcamera":
					return device.SerialNumber;
				default:
					if (arg.StartsWith("session.") || arg.StartsWith("camera.session."))
					{
						var session = (arg[0] == 'c' ? device.AttachedPhotoSession : null) ?? ServiceProvider.DeviceManager.SelectedCameraDevice;
						var propName = arg.Substring((arg[0] == 'c' ? "camera.session." : "session.").Length);
						foreach (PropertyInfo prop in GetProperties(typeof(PhotoSession)))
						{
							if (propName == prop.Name.ToLower())
							{
								return session == null ? null : prop.GetValue(session, null);
							}
						}
					}
					if (arg.StartsWith("property."))
					{
						var properties = device.LoadProperties();
						foreach (PropertyInfo prop in GetProperties(typeof(CameraProperty)))
						{
							if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
							{
								return prop.GetValue(properties, null);
							}
						}
					}
					if (arg.StartsWith("liveview."))
					{
						var settings = device.LoadProperties().LiveviewSettings;
						foreach (PropertyInfo prop in GetProperties(typeof(LiveviewSettings)))
						{
							if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
							{
								return settings == null ? null : prop.GetValue(settings, null);
							}
						}
					}
					if (arg.StartsWith("camera."))
					{
						IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
						foreach (PropertyInfo info in props)
						{
							if (info.PropertyType.Name.StartsWith("PropertyValue") &&
								(arg.Split('.')[1].ToLower().Replace("_", " ") == info.Name.ToLower())
								)
							{
								dynamic valp = info.GetValue(device, null);
								object val = valp.Value;
								if (val != null)
								{
									return val;
								}
							}
						}
						foreach (PropertyValue<long> property in device.AdvancedProperties)
						{
							if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower().Replace("_", " ") == property.Name.ToLower()))
							{
								return property.Value;
							}
						}
					}
					throw new Exception($"Unknow parameter {arg}");
			}
		}


		private void Set (string[] args)
		{
			var device = GetDevice();
			args = args.ToArray().Aggregate("", (current, s) => current + s + " ").Split('|');

			string arg;
			string param;
			for (int k = 0; k < args.Length; k++)
			{
				bool notFound = true;
				arg = args[k].Split(' ')[0];
				param = args[k].Skip(arg.Length).ToArray().Aggregate("", (current, s) => current + s).Trim();
				arg = arg.Trim().ToLower();
				switch (arg)
				{
					case "transfer":
						{
							CameraProperty property = device.LoadProperties();
							var val = param.Replace("_", " ").ToLower();
							switch (val)
							{
								case "save to pc only":
									if (device.GetCapability(CapabilityEnum.CaptureInRam))
									{
										device.CaptureInSdRam = true;
										property.NoDownload = false;
									}
									else
										throw new Exception(string.Format("Value {0} for property {1} is not supported", val, arg));
									break;
								case "save to camera only":
									property.NoDownload = true;
									device.CaptureInSdRam = false;
									break;
								case "save to pc and camera":
									property.NoDownload = false;
									device.CaptureInSdRam = false;
									break;
								default:
									throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
							}
							property.CaptureInSdRam = device.CaptureInSdRam;
						}
						break;
					case "shutterspeed":
						{
							var val = param;
							if (val.Equals("bulb"))
							{
								val = "Bulb";
							}
							// if the value not found check 
							if (!device.ShutterSpeed.Values.Contains(val))
								if (!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
								{
									val += "s";
								}

							if (!device.ShutterSpeed.Values.Contains(val))
								throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
							device.ShutterSpeed.SetValue(val);
						}
						break;
					case "iso":
						if (!device.IsoNumber.Values.Contains(param))
							throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
						device.IsoNumber.SetValue(param);
						break;
					case "exposurecompensation":
						if (!device.ExposureCompensation.Values.Contains(param))
							throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
						device.ExposureCompensation.SetValue(param);
						break;
					case "aperture":
						{
							var val = param;
							if (!val.Contains("."))
								val = val + ".0";
							if (!device.FNumber.Values.Contains(val))
								throw new Exception(string.Format("Wrong value {0} for property aperture", val));
							device.FNumber.SetValue(param);
						}
						break;
					case "focusmode":
						if (!device.FocusMode.Values.Contains(param))
							throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
						device.FocusMode.SetValue(param);
						break;
					case "whitebalance":
						if (device?.WhiteBalance != null)
						{
							if (!device.WhiteBalance.Values.Contains(param) == true)
								throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
							device.WhiteBalance.SetValue(param);
						}
						break;
					case "mode":
						if (!device?.Mode?.Values?.Contains(param) == true)
							throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
						device.Mode.SetValue(param);
						break;
					case "compressionsetting":
						if (!device.CompressionSetting.Values.Contains(param))
							throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
						device.CompressionSetting.SetValue(param);
						break;
					case "camera":
						{
							var camera = GetCamera(param);
							if (camera != null)
							{
								ServiceProvider.DeviceManager.SelectedCameraDevice = camera;
							}
						}
						break;
					case "scriptcamera":
						{
							var camera = GetCamera(param);
							if (camera != null)
							{
								_targetDevice = camera;
							}
						}
						break;
					case "session":
						foreach (var session in ServiceProvider.Settings.PhotoSessions)
						{
							if (session.Name.ToLower() == param.ToLower())
							{
								ServiceProvider.Settings.DefaultSession = session;
								notFound = false;
								break;// return;
							}
						}
						if (notFound)
							throw new Exception($"Unknow session name {param}");
						else
							break;
					case "camera.session":
						if (string.IsNullOrEmpty(param))
						{
							device.AttachedPhotoSession = null;
							var properties = device.LoadProperties();
							properties.PhotoSessionName = null;
							notFound = false;
						}
						else
						{
							foreach (var session in ServiceProvider.Settings.PhotoSessions)
							{
								if (session.Name.ToLower() == param.ToLower())
								{
									device.AttachedPhotoSession = session;
									var properties = device.LoadProperties();
									properties.PhotoSessionName = session.Name;
									notFound = false;
									break;
								}
							}
						}
						if (notFound)
							throw new Exception($"Unknow session name {param}");
						else
							break;
					default:
						if (arg.StartsWith("session.") || arg.StartsWith("camera.session."))
						{
							var session = (arg[0] == 'c' ? device.AttachedPhotoSession : null) ?? ServiceProvider.Settings.DefaultSession;
							var paramName = arg.Substring((arg[0] == 'c' ? "camera.session." : "session.").Length);

							var val = param;
							foreach (PropertyInfo prop in GetProperties(typeof(PhotoSession)))
							{
								if (paramName == prop.Name.ToLower())
								{
									if (prop.PropertyType == typeof(string))
									{
										prop.SetValue(session, val, null);
										notFound = false;
									}
									else if (prop.PropertyType == typeof(bool))
									{
										val = val.ToLower().Trim();
										if (val != "true" && val != "false" && val != "0" && val != "1")
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
										prop.SetValue(session, (val == "true" || val == "1"), null);
										notFound = false;
									}
									else if (prop.PropertyType == typeof(int))
									{
										int i = 0;
										if (int.TryParse(val, out i))
										{
											prop.SetValue(session, i, null);
											notFound = false;
										}
										else
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
									}
									break;
								}
							}
						}
						else if (arg.StartsWith("property."))
						{
							var val = param;
							var paramName = arg.Substring("property.".Length);
							var properties = device.LoadProperties();
							foreach (PropertyInfo prop in GetProperties(typeof(CameraProperty)))
							{
								if (paramName == prop.Name.ToLower())
								{
									if (prop.PropertyType == typeof(string))
									{
										notFound = false;
										prop.SetValue(properties, val, null);
									}
									else if (prop.PropertyType == typeof(bool))
									{
										val = val.ToLower().Trim();
										if (val != "true" && val != "false" && val != "0" && val != "1")
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
										notFound = false;
										prop.SetValue(properties, (val == "true" || val == "1"), null);
									}
									else if (prop.PropertyType == typeof(int))
									{
										int i = 0;
										if (int.TryParse(val, out i))
										{
											notFound = false;
											prop.SetValue(properties, i, null);
										}
										else
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
									}
									break;
								}
							}
						}
						else if (arg.StartsWith("liveview."))
						{
							var val = param;
							var paramName = arg.Substring("liveview.".Length);
							foreach (PropertyInfo prop in GetProperties(typeof(LiveviewSettings)))
							{
								if (paramName == prop.Name.ToLower())
								{
									if (prop.PropertyType == typeof(string))
									{
										notFound = false;
										prop.SetValue(device.LoadProperties().LiveviewSettings, val, null);
									}
									else if (prop.PropertyType == typeof(bool))
									{
										val = val.ToLower().Trim();
										if (val != "true" && val != "false" && val != "0" && val != "1")
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
										notFound = false;
										prop.SetValue(ServiceProvider.Settings.DefaultSession, (val == "true" || val == "1"), null);
									}
									else if (prop.PropertyType == typeof(int))
									{
										int i = 0;
										if (int.TryParse(val, out i))
										{
											notFound = false;
											prop.SetValue(device.LoadProperties().LiveviewSettings, i, null);
										}
										else
											throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
									}
									break;
								}
							}
						}
						else if (arg.StartsWith("camera."))
						{
							IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
							foreach (PropertyInfo info in props)
							{
								if (info.PropertyType.Name.StartsWith("PropertyValue") &&
									(arg.Split('.')[1].Replace("_", " ") == info.Name.ToLower())
									)
								{
									dynamic valp = info.GetValue(device, null);
									if (!valp.Values.Contains(param.Replace("_", " ")))
										throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
									valp.Value = param.Replace("_", " ");
									notFound = false;
									break;
								}
							}
							if (notFound)
								foreach (PropertyValue<long> property in device.AdvancedProperties)
								{
									if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].Replace("_", " ") == property.Name.ToLower()))
									{
										if (!property.Values.Contains(param.Replace("_", " ")))
											throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
										property.Value = param.Replace("_", " ");
										notFound = false;
										break;
									}
								}
						}
						if (notFound)
							throw new Exception($"Unknow parameter {arg}");
						break;
				}
			}
		}

		private ICameraDevice _targetDevice;
		/// <summary>
		/// Indicates whether the "camera" in "get camera" is the one for which the processor is created, rather than the
		/// globally selected one. That is how the processor worked before the distinction between camera and scriptcamera was
		/// introduced.
		/// </summary>
		private bool _alwaysUseScriptCamera;

		private ICameraDevice GetDevice ()
		{
			if (_targetDevice != null)
				return _targetDevice;
			return ServiceProvider.DeviceManager.SelectedCameraDevice;
		}

		private static ICameraDevice GetCamera (string camera)
		{
			if (string.IsNullOrEmpty(camera))
				return null;
			foreach (var cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
			{
				if ((PhotoUtils.IsNumeric(camera) && cameraDevice.SerialNumber == camera) || cameraDevice.DeviceName.Replace(" ", "_") == camera.Replace(" ", "_"))
				{
					return cameraDevice;
				}
			}
			throw new Exception($"Unknow camera {camera}");
		}

		private IEnumerable<PropertyInfo> GetProperties (Type type)
		{
			foreach (var prop in type.GetProperties())
			{
				if ((prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool))
					&& prop.GetCustomAttribute<TclScriptIgnoreAttribute>() == null)
				{
					yield return prop;
				}
			}
		}

		private string SyncEventCmd (string[] args)
		{
			if (args.Length == 0)
			{
				throw new Exception("Missing event name");
			}
			if (args.Length == 1)
			{
				throw new Exception($"Missing command for event {args[0]}");
			}
			var command = args[1].ToLower().Trim();
			switch (command)
			{
				case "use":
					EvalSyncEvent(args[0], this, use => use.IsUsing = true);
					return "";
				case "wait":
					if (args.Length == 2)
					{
						var waitCompleted = true;
						EvalSyncEvent(args[0], this, use => waitCompleted = use.Wait(-1));
						return waitCompleted ? "true" : "false";
					}
					break;
				case "discard":
					EvalSyncEvent(args[0], this, use => use.Discard(), false);
					return "";
				case "after":
				case "notbefore":
					break;
				case "interval":
					if (args.Length == 3 && args[2] == "-")
					{
						EvalSyncEvent(args[0], this, use => use.Event.SetInterval(0));
						return "";
					}
					break;
				default:
					throw new Exception(string.Format("Invalid command {0}", args[0]));
			}

			if (args.Length == 2)
			{
				throw new Exception($"Missing value for command {args[1]} for event {args[0]}");
			}
			if (!long.TryParse(args[2], out var value))
			{
				throw new Exception($"Value for command {args[1]} for event {args[0]} must be an integer");
			}
			if (value < 0)
			{
				throw new Exception($"Value for command {args[1]} for event {args[0]} must be zero or larger");
			}

			switch (command)
			{
				case "wait":
					{
						var waitCompleted = true;
						EvalSyncEvent(args[0], this, use => waitCompleted = use.Wait(value));
						return waitCompleted ? "true" : "false";
					}
				case "after":
					EvalSyncEvent(args[0], this, use => use.Event.SetAfter(value));
					break;
				case "notbefore":
					EvalSyncEvent(args[0], this, use => use.Event.SetNotBefore(value));
					break;
				case "interval":
					EvalSyncEvent(args[0], this, use => use.Event.SetInterval(value));
					break;
			}
			return "";
		}

		private sealed class SyncEvent
		{
			internal SyncEvent (string name)
			{
				Name = name;
				_syncEvents[name] = this;
			}

			internal string Name
			{
				get;
			}

			internal Dictionary<CommandLineProcessor, SyncEventUse> Users
			{
				get;
			} = new Dictionary<CommandLineProcessor, SyncEventUse>();


			internal void Remove ()
			{
				_syncEvents.Remove(Name);
			}

			internal void List (List<string> result)
			{
				result.Add(Name + ":");
				result.Add($"    #Use={(from u in Users.Values where u.IsUsing select u).Count()}");
				result.Add($"    #Wait={(from u in Users.Values where u.IsWaiting select u).Count()}");
				var notBefore = _lastSyncTime.AddSeconds(_interval);
				if (notBefore < _notBefore)
				{
					notBefore = _notBefore;
				}
				if (notBefore > DateTime.UtcNow)
				{
					result.Add($"    NotBefore={notBefore:yyyy-MM-dd HH:mm:ss} UTC");
				}
				if (_interval > 0)
				{
					result.Add($"    Interval={_interval}");
				}
			}

			private DateTime _lastSyncTime = DateTime.MinValue;
			private long _interval = 0;
			private DateTime _notBefore = DateTime.MinValue;
			private Timer _wait;

			internal bool TryFire ()
			{
				if ((from u in Users.Values
					 where !u.IsWaiting && u.IsUsing
					 select u).Any())
				{
					_wait?.Stop();
					return false;
				}

				var now = DateTime.UtcNow;
				if (_wait == null || !_wait.Enabled)
				{
					var notBefore = _lastSyncTime.AddSeconds(_interval);
					if (notBefore < _notBefore)
					{
						notBefore = _notBefore;
					}
					if (notBefore > now)
					{
						if (_wait == null)
						{
							_wait = new Timer()
							{
								AutoReset = false
							};
							_wait.Elapsed += (s, e) =>
							{
								TryFire();
							};
						}
						_wait.Interval = (notBefore - now).TotalMilliseconds;
						_wait.Start();
						return false;
					}
				}

				lock (_syncEvents)
				{
					_lastSyncTime = now;
					foreach (var user in Users.Values)
					{
						user.StopWaiting();
					}
				}
				return true;
			}

			internal void SetAfter (long seconds)
			{
				var notBefore = DateTime.UtcNow.AddSeconds(seconds);
				if (notBefore != _notBefore)
				{
					_notBefore = notBefore;
					TryFire();
				}
			}

			internal void SetNotBefore (long seconds)
			{
				var notBefore = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
				if (notBefore != _notBefore)
				{
					_notBefore = notBefore;
					TryFire();
				}
			}

			internal void SetInterval (long seconds)
			{
				if (seconds <= 0)
				{
					if (_interval == 0)
					{
						return;
					}
				}
				else if (_interval == seconds)
				{
					return;
				}
				_interval = seconds;
				TryFire();
			}
		}

		private sealed class SyncEventUse
		{
			internal SyncEventUse (SyncEvent @event, CommandLineProcessor processor)
			{
				_processor = processor;
				Event = @event;
				Event.Users[processor] = this;
			}
			private readonly CommandLineProcessor _processor;

			internal SyncEvent Event { get; }

			private bool _isUsing;
			internal bool IsUsing
			{
				get => _isUsing;
				set
				{
					if (value && !_isUsing)
					{
						_isUsing = true;
						Event.TryFire();
					}
				}
			}

			private readonly AutoResetEvent _wait = new AutoResetEvent(false);
			internal bool IsWaiting { get; private set; }

			internal void Discard ()
			{
				lock (_syncEvents)
				{
					Event.Users.Remove(_processor);
					if (Event.Users.Count == 0)
					{
						Event.Remove();
					}
					else
					{
						Event.TryFire();
					}
					_wait.Dispose();
				}
			}

			internal bool Wait (long timeout)
			{
				if (!IsUsing)
				{
					throw new Exception($"The script is not using event {Event.Name}");
				}
				IsWaiting = true;
				var waitCompleted = true;
				if (Event.TryFire())
				{
					_wait.Reset();
				}
				else
				{
					var waitUntil = timeout < 0
						? DateTime.MaxValue
						: DateTime.UtcNow.AddMilliseconds(timeout);
					while ((waitCompleted = DateTime.UtcNow < waitUntil)
						   && !(_processor._isCancellationRequested?.Invoke() ?? false))
					{
						if (_wait.WaitOne(100))
						{
							break;
						}
					}
				}
				IsWaiting = false;
				return waitCompleted;
			}

			internal void StopWaiting ()
			{
				if (IsWaiting)
				{
					_wait.Set();
				}
			}
		}

		private static Dictionary<string, SyncEvent> _syncEvents = new Dictionary<string, SyncEvent>(StringComparer.OrdinalIgnoreCase);

		private static void EvalSyncEvent (string eventName, CommandLineProcessor processor, Action<SyncEventUse> command, bool forceCreate = true)
		{
			SyncEventUse use;
			lock (_syncEvents)
			{
				if (!_syncEvents.TryGetValue(eventName, out var syncEvent))
				{
					if (!forceCreate)
					{
						return;
					}
					syncEvent = new SyncEvent(eventName);
				}
				if (!syncEvent.Users.TryGetValue(processor, out use))
				{
					use = new SyncEventUse(syncEvent, processor);
				}
			}
			command(use);
		}

		private void DiscardSyncEvents (CommandLineProcessor processor)
		{
			lock (_syncEvents)
			{
				foreach (var syncEvent in _syncEvents.ToList())
				{
					if (syncEvent.Value.Users.TryGetValue(processor, out var use))
					{
						use.Discard();
					}
				}
			}
		}
	}
}

using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace CameraControl.Plugins.ScriptExternalObjectPlugins
{
	public sealed class RestServiceObject : IScriptExternalObject
	{
		public RestServiceObject (RestServiceProvider provider, string name, IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> parameters)
		{
			Name = name;
			Provider = provider;
			ProviderParameters = parameters;

			BaseUrl = new Uri((string)provider.Parameters[0].GetValue(parameters));
			if (!(bool)provider.Parameters[1].GetValue(parameters))
			{
				Activate();
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
				var properties = RetrieveValue(null, "OPTIONS");
				Properties = ScriptExternalObjectProperty.Parse(properties);
				IsActivated = true;
			}
		}

		public object Read (string propertyName)
		{
			var property = GetProperty(propertyName);
			var json = RetrieveValue(property.Name, "GET");
			return property.FromJsonValue(json);
		}

		public void Write (string propertyName, object value)
		{
			var property = GetProperty(propertyName);
			var json = property.ToJsonValue(value);
			SendValue(property.Name, json, "PUT");
		}

		public void Trigger (string propertyName)
		{
			var property = GetProperty(propertyName);
			SendValue(property.Name, null, "POST");
		}

		public bool IsEventRaised (string propertyName)
		{
			var property = GetProperty(propertyName);
			var json = RetrieveValue(property.Name, "POST");
			return (bool)property.FromJsonValue(json);
		}

		private ScriptExternalObjectProperty GetProperty (string propertyName)
		{
			var result = (from p in Properties
						  where p.Name == propertyName
						  select p).FirstOrDefault()
						  ?? throw new ArgumentException($"Web service '{Name}' has no property '{propertyName}'");
			return result;
		}

		private string RetrieveValue (string propertyName, string method)
		{
			try
			{
				var request = WebRequest.CreateHttp(propertyName == null ? BaseUrl.ToString() : BaseUrl.ToString() + propertyName);
				request.Method = method;
				request.Accept = "application/json";
				var response = request.GetResponse();
				using (var stream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				var msg = $"Cannot get the {(propertyName == null ? "metadata" : $"value for property '{propertyName}'")} from the web service '{Name}'";
				Log.Error(msg, ex);
				throw new Exception(msg);
			}
		}

		private void SendValue (string propertyName, string value, string method)
		{
			try
			{
				var request = WebRequest.CreateHttp(BaseUrl.ToString() + propertyName);
				request.Method = method;
				if (value != null)
				{
					request.ContentType = "application/json";
					request.ContentLength = value.Length;
					using (var stream = request.GetRequestStream())
					{
						using (var writer = new StreamWriter(stream))
						{
							writer.Write(value);
						}
					}
				}
				_ = request.GetResponse();
			}
			catch (Exception ex)
			{
				var msg = value == null
					? $"Cannot touch the property '{propertyName}' via the web service '{Name}'"
					: $"Cannot set the value for property '{propertyName}' via the web service '{Name}'";
				Log.Error(msg, ex);
				throw new Exception(msg);
			}
		}
	}
}

using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace CameraControl.Plugins.ScriptExternalObjectPlugins
{
	/// <summary>
	/// Provider to connect to a web service that exposes an object to be used as external object in scripts. The web
	/// service should be designed:
	/// <list type="bullet">
	/// <item>The web service has a base URL, e.g., http://localhost/object. If the
	/// service receives a OPTIONS request at this URL, it should return an array of property, trigger and event descriptions that can be parsed
	/// by <see cref="ScriptExternalObjectProperty.Parse(string)"/>.
	/// </item>
	/// <item>The value of each read-only or read/write property can be obtained by sending a GET request to the base URL + property name,
	/// e.g., http://localhost/object/propname. The service should return the value as a single JSON value: true/false for
	/// boolean, number for long, "quoted string" for a string.
	/// </item>
	/// <item>The value of each write-only or read/write property can be modified by sending a PUT request to the base URL + property name.
	/// The body of the request contains the value as a single JSON value: true/false for boolean, number for long,
	/// "quoted string" for a string.
	/// </item>
	/// <item>
	/// A trigger property is touched by sending a POST request to the base URL + property name. The body of the request is empty.
	/// </item>
	/// <item>
	/// An event property is probed by sending a POST request to the base URL + property name. The service should return a boolean
	/// (true/false) indicating whether the event has been raised. If a true value is returned, the service should auto-reset the event
	/// and return false on subsequent requests until the event is raised again.
	/// </item>
	/// <item>
	/// The names of the properties are considered case sensitive.
	/// </item>
	/// </list>
	/// </summary>
	public sealed class RestServiceProvider : IScriptExternalObjectProvider
	{
		public string DisplayName
			=> "REST web service";

		public string Description
			=> $"A web service that exposes data and/or methods via the REST protocol. The web service should comply with the technical requirements for use by this application.";

		public IReadOnlyList<ScriptExternalObjectCreateParameter> Parameters
			=> _Parameters;
		private static readonly ScriptExternalObjectCreateParameter[] _Parameters = new ScriptExternalObjectCreateParameter[]
		{
			new ScriptExternalObjectCreateParameter ("URL", ScriptExternalObjectCreateParameter.ValueType.String)
			{
				Description = "The URL of the web service, e.g., 'http://localhost:42042/labcontrol' or 'https://service.io/service/dcc/'.",
				IsConstant = false
			},
			ScriptExternalObjectCreateParameter.DelayActivation
		};

		public IScriptExternalObject CreateInstance (string name, Dictionary<ScriptExternalObjectCreateParameter, object> parameters)
		{
			var url = (string)_Parameters[0].GetValue(parameters);
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				throw new ArgumentException($"The URL '{url}' specified for parameter {_Parameters[0].Name} is invalid");
			}
			url = url.EndsWith("/") ? url : url + "/";
			parameters[_Parameters[0]] = url;
			return new RestServiceObject(this, name, parameters);
		}
	}
}

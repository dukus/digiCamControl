using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System.Collections.Generic;

namespace CameraControl.Plugins.ScriptExternalObjectPlugins
{
	/// <summary>
	/// Provider to connect to a device via a USB/serial port that exposes an object to be used as external object in
	/// scripts. The device should implement the specific serial communication protocol:
	/// <list type="bullet">
	/// <item>The application (digiCamControl) sends requests to the device by writing text to a serial port. Each request starts with a '\a' and ends with a '\f' character.</item>
	/// <item>A request <c>=Index</c> requests the property, trigger and event descriptions that can be parsed by <see cref="ScriptExternalObjectProperty.Parse(string)"/>.</item>
	/// <item>A request <c>Property_name</c> request the value of a property. The response should contain the value converted to text; for a boolean the values are "true" and "false".</item>
	/// <item>A request <c>Property_name=value</c> requests to assign the value to a property. The response does not include a return value.</item>
	/// <item>A request <c>Trigger_name</c> request the activation of a trigger. The response does not include a return value.</item>
	/// <item>A request <c>Event_name</c> request the state of an event. A boolean value should be returned that indicates whether the event was raised.
	/// Next time the event state is requested, the returned value should be false (unless the event has been raised again).</item>
	/// <item>Each request is answered by the device by writing a response to the serial port. A response starts with a '\a' and ends with a
	/// '\f' character. A response consists of two parts, separated by a ':' character. The first part is a error/OK code:
	/// 200 if the request could be processed, 400 if that was not possible, 404 if the request was read correctly but
	/// references an unknown property, trigger or event. The second part is the return value. If no return value is
	/// requested, the ':' character can be omitted.</item>
	/// <item>
	/// The device can write other (debug) text to the serial port. All text that is not a response to a request is logged if a newline '\n' is received,
	/// or if a response to a request is received.
	/// </item>
	/// </list>
	/// </summary>
	public sealed class SerialServiceProvider : IScriptExternalObjectProvider
	{
		public string DisplayName
			=> "Serial device";

		public string Description
			=> $"A device that exposes data and/or methods via a serial (COM) port. The service offered by the device should comply with the technical requirements for use by this application.";

		public IReadOnlyList<ScriptExternalObjectCreateParameter> Parameters
			=> _Parameters;
		private static readonly ScriptExternalObjectCreateParameter[] _Parameters = new ScriptExternalObjectCreateParameter[]
		{
			new ScriptExternalObjectCOMPortParameter (),
			new ScriptExternalObjectCreateParameter ("Baud rate", ScriptExternalObjectCreateParameter.ValueType.Long)
			{
				Description = "The baud rate of the serial communication. Default is 9600.",
				DefaultValue = 9600L
			},
			new ScriptExternalObjectCreateParameter ("Data bits", ScriptExternalObjectCreateParameter.ValueType.Long)
			{
				Description = "The data bits of the serial communication. Default is 8.",
				DefaultValue = 8L,
			},
			new ScriptExternalObjectCreateParameter ("Parity", ScriptExternalObjectCreateParameter.ValueType.String)
			{
				Description = "The parity of the serial communication. Default is None.",
				DefaultValue = "None",
				EnumerationValues = new string[]
				{
					"None",
					"Even",
					"Odd",
					"Mark",
					"Space"
				},
			},
			new ScriptExternalObjectCreateParameter ("Stop bits", ScriptExternalObjectCreateParameter.ValueType.String)
			{
				Description = "The stop bits of the serial communication. Default is 1.",
				DefaultValue = "1",
				EnumerationValues = new string[]
				{
					"1",
					"1.5",
					"2"
				},
			},
			ScriptExternalObjectCreateParameter.DelayActivation,
		};

		public IScriptExternalObject CreateInstance (string name, Dictionary<ScriptExternalObjectCreateParameter, object> parameters)
			=> new SerialServiceObject(this, name, parameters);
	}
}

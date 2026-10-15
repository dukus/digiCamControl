using System.Text.Json.Serialization;

namespace ScriptExternalObjectRestService.Components.ScriptExternalObjectDemo
{
	public sealed class ScriptExternalObjectProperties : List<ScriptExternalObjectProperties.ScriptExternalObjectProperty>
	{
		public class ScriptExternalObjectProperty
		{
			public required string Name
			{
				get; set;
			}

			[JsonConverter(typeof(JsonStringEnumConverter<ValueType>))]
			public enum ValueType
			{
				Boolean,
				Long,
				String,
				Trigger,
				Event
			}

			public ValueType Type { get; set; }

			public string? Description { get; set; }

			public List<string>? EnumerationValues { get; set; }

			public bool? CanRead { get; set; }

			public bool? CanWrite { get; set; }
		}
	}
}

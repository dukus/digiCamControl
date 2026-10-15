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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace CameraControl.Core.Classes
{
	public class ScriptExternalObjectProperty
	{
		public ScriptExternalObjectProperty (string name, ValueType type, bool canRead, bool canWrite)
		{
			Name = name;
			ScriptName = Name.Replace(' ', '_');
			Type = type;
			CanRead = (canRead || type == ValueType.Event) && type != ValueType.Trigger;
			CanWrite = (canWrite || type == ValueType.Trigger) && type != ValueType.Event;
		}

		/// <summary>
		/// Name of the property.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Name of the property s used in scripts.
		/// </summary>
		public string ScriptName { get; }

		/// <summary>
		/// Description of the parameter (optional).
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Supported types of the value of the property.
		/// </summary>
		public enum ValueType
		{
			/// <summary>
			/// The property has a <see langword="bool"/> as value.
			/// </summary>
			Boolean = ScriptExternalObjectCreateParameter.ValueType.Boolean,
			/// <summary>
			/// The property has a <see langword="long"/> as value.
			/// </summary>
			Long = ScriptExternalObjectCreateParameter.ValueType.Long,
			/// <summary>
			/// The property has a <see langword="string"/> as value.
			/// </summary>
			String = ScriptExternalObjectCreateParameter.ValueType.String,
			/// <summary>
			/// A property representing a functionality in the external object. The functionality is triggered/started once the
			/// property is touched.
			/// </summary>
			Trigger,
			/// <summary>
			/// A property representing an event that is triggered at the external object. The property has a
			/// <see langword="bool"/> as value that is <see langword="true"/> the first time the property's value is read after
			/// the event has been raised.
			/// </summary>
			Event
		}

		/// <summary>
		/// Type of the property's value.
		/// </summary>
		public ValueType Type
		{
			get;
		}

		/// <summary>
		/// If <see cref="Type"/> is <see cref="ValueType.String"/> or <see cref="ValueType.Long"/>: the allowed values of the
		/// parameter. Returns <see langword="null"/> if all values are allowed.
		/// </summary>
		public IReadOnlyList<string> EnumerationValues { get; set; }

		/// <summary>
		/// Indicates wether the value of the property can be read.
		/// </summary>
		public bool CanRead { get; }
		/// <summary>
		/// Indicates wether the value of the property can be assigned.
		/// </summary>
		public bool CanWrite { get; }


		/// <summary>
		/// Get the value for this property in the native type (boolean, long, string) from the JSON representation of the
		/// value. Also verifies if the value is one of the <see cref="EnumerationValues"/>.
		/// </summary>
		/// <param name="value">Value in JSON format.</param>
		/// <returns>Value in native type.</returns>
		public object FromJsonValue (string value)
			=> FromTextValue(value, true);

		/// <summary>
		/// Get the value for this property in the native type (boolean, long, string) from the string representation of the
		/// value. Also verifies if the value is one of the <see cref="EnumerationValues"/>.
		/// </summary>
		/// <param name="value">Value in JSON format.</param>
		/// <returns>Value in native type.</returns>
		public object FromStringValue (string value)
			=> FromTextValue(value, false);

		/// <summary>
		/// Get the value for this property in the native type (boolean, long, string) from the JSON representation of the
		/// value. Also verifies if the value is one of the <see cref="EnumerationValues"/>.
		/// </summary>
		/// <param name="value">Value in JSON format.</param>
		/// <returns>Value in native type.</returns>
		private object FromTextValue (string value, bool stringsAreQuoted)
		{
			value = value?.Trim();
			switch (Type)
			{
				case ValueType.Boolean:
				case ValueType.Event:
					if (value?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false)
					{
						return true;
					}
					else if (value?.Equals("false", StringComparison.OrdinalIgnoreCase) ?? false)
					{
						return false;
					}
					else
					{
						throw new ArgumentException("Invalid value for a boolean property");
					}

				case ValueType.Long:
					if (string.IsNullOrEmpty(value) ||
						!long.TryParse(value, out var longValue))
					{
						throw new ArgumentException("Invalid value for a long property");
					}
					if (EnumerationValues != null && EnumerationValues.Count > 0
						&& !EnumerationValues.Contains(value))
					{
						throw new ArgumentException("Value is outside the allowed range for the property");
					}
					return longValue;

				case ValueType.String:
					if (string.IsNullOrEmpty(value) || value == "null")
					{
						if (EnumerationValues != null && EnumerationValues.Count > 0)
						{
							throw new ArgumentException("Value is outside the allowed range for the property");
						}
						return null;
					}
					if (stringsAreQuoted)
					{
						if (value[0] != '"' || value[value.Length - 1] != '"' || value.Length == 1)
						{
							throw new ArgumentException("Invalid value for a string property");
						}
						value = value.Substring(1, value.Length - 2);
					}
					if (EnumerationValues != null && EnumerationValues.Count > 0
						&& !EnumerationValues.Contains(value))
					{
						throw new ArgumentException("Value is outside the allowed range for the property");
					}
					return value;

				default:
					throw new NotImplementedException("Unknown property type");
			}
		}

		/// <summary>
		/// Convert the value for this property from the native type (boolean, long, string) to its JSON representation. Also
		/// verifies if the value is one of the <see cref="EnumerationValues"/>.
		/// </summary>
		/// <param name="value">Value in native type.</param>
		/// <returns>JSON representation.</returns>
		public string ToJsonValue (object value)
			=> ToTextValue(value, true);

		/// <summary>
		/// Convert the value for this property from the native type (boolean, long, string) to its string representation.
		/// Also verifies if the value is one of the <see cref="EnumerationValues"/>.
		/// </summary>
		/// <param name="value">Value in native type.</param>
		/// <returns>JSON representation.</returns>
		public string ToStringValue (object value)
			=> ToTextValue(value, false);

		private string ToTextValue (object value, bool stringsAreQuoted)
		{
			switch (Type)
			{
				case ValueType.Boolean:
					return Convert.ToBoolean(value) ? "true" : "false";

				case ValueType.Long:
					var longValue = Convert.ToInt64(value).ToString();
					if (EnumerationValues != null && EnumerationValues.Count > 0
						&& !EnumerationValues.Contains(longValue))
					{
						throw new ArgumentException("Value is outside the allowed range for the property");
					}
					return longValue;

				case ValueType.String:
					if (value == null)
					{
						if (EnumerationValues != null && EnumerationValues.Count > 0)
						{
							throw new ArgumentException("Value is outside the allowed range for the property");
						}
						return "null";
					}
					else
					{
						if (EnumerationValues != null && EnumerationValues.Count > 0
							&& !EnumerationValues.Contains(value))
						{
							throw new ArgumentException("Value is outside the allowed range for the property");
						}
					}
					return stringsAreQuoted ?
						$"\"{(value as string).Replace("\"", "\\\"")}\""
						: value as string;

				default:
					throw new NotImplementedException("Unknown property type");
			}
		}

		/// <summary>
		/// Deserialize a JSON description of a list of properties.
		/// </summary>
		/// <param name="json">JSON description</param>
		/// <returns>The deserialized properties.</returns>
		public static List<ScriptExternalObjectProperty> Parse (string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				return new List<ScriptExternalObjectProperty>();
			}
			var parsed = JsonConvert.DeserializeObject<List<AsJson>>(json);
			return (from p in parsed
					select new ScriptExternalObjectProperty(p.Name, p.Type, p.CanRead ?? false, p.CanWrite ?? false)
					{
						Description = p.Description,
						EnumerationValues = p.EnumerationValues
					}).ToList();
		}

		private sealed class AsJson
		{
			public string Name { get; set; }

			[JsonConverter(typeof(StringEnumConverter))]
			public ValueType Type { get; set; }

			public string Description { get; set; }

			public List<string> EnumerationValues { get; set; }

			/// <summary>
			/// Omit this in case of triggers and events
			/// </summary>
			public bool? CanRead { get; set; }
			/// <summary>
			/// Omit this in case of triggers and events
			/// </summary>
			public bool? CanWrite { get; set; }
		}
	}
}

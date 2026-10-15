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
using System;
using System.Collections.Generic;
#endregion

namespace CameraControl.Core.Classes
{
	public class ScriptExternalObjectCreateParameter
	{
		public ScriptExternalObjectCreateParameter (string name, ValueType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		/// Name of the parameter.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Description of the parameter (optional).
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Supported types of the value of the parameter.
		/// </summary>
		public enum ValueType
		{
			/// <summary>
			/// The parameter has a <see cref="bool"/> as value.
			/// </summary>
			Boolean,
			/// <summary>
			/// The parameter has a <see cref="long"/> as value.
			/// </summary>
			Long,
			/// <summary>
			/// The parameter has a <see cref="string"/> as value.
			/// </summary>
			String
		}
		/// <summary>
		/// Type of the parameter's value.
		/// </summary>
		public ValueType Type { get; }

		/// <summary>
		/// Indicates whether the parameter is required. The default is <see langword="true"/>.
		/// </summary>
		public bool IsRequired
		{
			get; set;
		} = true;

		/// <summary>
		/// If <see cref="Type"/> is <see cref="ValueType.String"/> or <see cref="ValueType.Long"/>: the allowed values of the
		/// parameter. Returns <see langword="null"/> if all values are allowed.
		/// </summary>
		public IReadOnlyList<string> EnumerationValues { get; set; }

		/// <summary>
		/// Method that updates the <see cref="EnumerationValues"/>. It is called just before the parameter is used.
		/// </summary>
		public virtual void RefreshEnumerationValues ()
		{
		}

		/// <summary>
		/// The default value for the parameter.
		/// </summary>
		public object DefaultValue { get; set; }

		/// <summary>
		/// Indicates that the parameter does not depend on any external parameters. Default is <see langword="true"/>.
		/// Returns <see langword="false"/> for, e.g., COM-port numbers that may be different next time the applicatiion is
		/// run. The application allows to create a new object with the same parameters as a registered one, with different
		/// values for the non-constant parameters.
		/// </summary>
		public bool IsConstant { get; set; } = true;

		/// <summary>
		/// Common parameter to delay activation.
		/// </summary>
		public static ScriptExternalObjectCreateParameter DelayActivation
		{
			get;
		} = new ScriptExternalObjectCreateParameter("Delay activation", ValueType.Boolean)
		{
			Description = "Postpone activation of the service/device. An inactive service/device is registered but is not available for scripting and has no associated buttons in the user interface.",
			IsRequired = false,
			DefaultValue = false,
			IsConstant = false,
		};

		/// <summary>
		/// Get the value of the parameter from a collection.
		/// </summary>
		/// <param name="parameters">Collection of parameter values.</param>
		/// <returns>
		/// The parameter value as specified or, if not present, the default value for the parameter or parameter's value
		/// type.
		/// </returns>
		public object GetValue (IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> parameters)
		{
			if (parameters.TryGetValue(this, out var value))
			{
				return value;
			}
			if (DefaultValue != null)
			{
				return DefaultValue;
			}
			switch (Type)
			{
				case ValueType.Boolean:
					return default(bool);
				case ValueType.Long:
					return default(long);
				case ValueType.String:
					return default(string);
				default:
					throw new NotImplementedException();
			}
		}
	}
}

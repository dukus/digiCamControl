using CameraControl.Core.Classes;
using System.Collections.Generic;

namespace CameraControl.Core.Interfaces
{
	public interface IExternalServiceScriptObject
	{
		/// <summary>
		/// Name of the instance, used to address the instance in scripting.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Description of the properties of the instance.
		/// </summary>
		IEnumerable<ScriptExternalObjectProperty> Properties
		{
			get;
		}

		/// <summary>
		/// Indicates whether the object is activated.
		/// </summary>
		bool IsActivated
		{
			get;
		}

		/// <summary>
		/// Activate an inactive object.
		/// </summary>
		void Activate ();

		/// <summary>
		/// Read the value of a property.
		/// </summary>
		/// <param name="propertyName">Name of one of the readable <see cref="Properties"/>.</param>
		/// <returns>
		/// The value of the property, of the type as specified by <see cref="ScriptExternalObjectCreateParameter.Type"/>.
		/// </returns>
		object Read (string propertyName);

		/// <summary>
		/// Write the value of a property.
		/// </summary>
		/// <param name="propertyName">Name of one of the writable <see cref="Properties"/>.</param>
		/// <param name="value">
		/// The value of the property, of the type as specified by <see cref="ScriptExternalObjectCreateParameter.Type"/>.
		/// </param>
		void Write (string propertyName, object value);

		/// <summary>
		/// Trigger the functionality exposed via a property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of one of the <see cref="Properties"/> of type <see cref="ScriptExternalObjectProperty.ValueType.Trigger"/>.
		/// </param>
		void Trigger (string propertyName);

		/// <summary>
		/// Indicates whether an event has been raised.
		/// </summary>
		/// <param name="propertyName">
		/// Name of one of the <see cref="Properties"/> of type <see cref="ScriptExternalObjectProperty.ValueType.Event"/>.
		/// </param>
		bool IsEventRaised (string propertyName);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CameraControl.Core.Interfaces
{
	/// <summary>
	/// Interface that should be implemented for a <see cref="IExternalServiceProvider"/> and optionally by a
	/// <see cref="IScriptExternalObject"/> to inform the application whether to add buttons to the UI.
	/// </summary>
	public interface IUIButtonProvider
	{
		/// <summary>
		/// Get the name of the provider. If the <see cref="INotifyPropertyChanged"/> is also implemented, the property must
		/// be public to support data binding.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Open a separate window to show the details and/or configuration of the buttons.
		/// </summary>
		void ShowWindow ();

		/// <summary>
		/// List the buttons that should be displayed on the button window. May return <see langword="null"/> if the provider
		/// does not support buttons.
		/// </summary>
		IEnumerable<IUIButton> Buttons
		{
			get;
		}

		/// <summary>
		/// Event that is raised when the <see cref="Buttons"/> collection changes (not the properties of the buttons). The
		/// event is never raised if <see cref="Buttons"/> returns <see langword="null"/>.
		/// </summary>
		event EventHandler ButtonsChanged;
	}
}

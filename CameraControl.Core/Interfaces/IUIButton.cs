using System.ComponentModel;

namespace CameraControl.Core.Interfaces
{
	/// <summary>
	/// A button that should be displayed in the button window. If any of the properties may change during the lifetime of
	/// the button, the <see cref="INotifyPropertyChanged"/> interface should be implemented as well. In that case the
	/// properties of this interface should be implemented as public properties of the button.
	/// </summary>
	public interface IUIButton
	{
		/// <summary>
		/// Get the button provider that provided this button.
		/// </summary>
		IUIButtonProvider Provider
		{
			get;
		}

		/// <summary>
		/// Get the title of the button.
		/// </summary>
		string Title
		{
			get;
		}

		/// <summary>
		/// Get an explanation of the button.
		/// </summary>
		string Description
		{
			get;
		}

		/// <summary>
		/// Indicates whether the button is enabled. A button can be temporarily disabled, but if it is permanently disabled
		/// as a result of the configuration of the <see cref="IExternalServiceProvider"/> (or other button provider), the
		/// button should not be presented to the UI.
		/// </summary>
		bool IsEnabled
		{
			get;
		}

		/// <summary>
		/// Called if the button is clicked.
		/// </summary>
		void Click ();
	}
}

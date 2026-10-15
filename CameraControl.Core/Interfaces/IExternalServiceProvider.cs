using System.ComponentModel;

namespace CameraControl.Core.Interfaces
{
	/// <summary>
	/// Interface implemented by a provider that provides an extension to the single command system used in scripting as an
	/// <see cref="IScriptExternalObject"/> instance and that exposes <see cref="IUIButton"/>s to be added to the
	/// application's user interface. A provider implements this interface rather than
	/// <see cref="IScriptExternalObjectProvider"/> if the default creation of <see cref="IScriptExternalObject"/>
	/// instances, exposition of triggers as <see cref="IUIButton"/>s and/or import/export of the configuration are
	/// insufficient. If the service configuration can be part of a DCC project, the
	/// <see cref="IDCCProjectSettingsProvider"/> interface should be implemented.
	/// </summary>
	public interface IExternalServiceProvider : IUIButtonProvider
	{
		/// <summary>
		/// Indicates whether the object is activated. If the activation is not constant, a change of this property should be
		/// passed via <see cref="INotifyPropertyChanged"/>
		/// </summary>
		bool IsActivated
		{
			get;
		}

		/// <summary>
		/// Get the object that can be used to control the data and functionality of the provider via script. Returns
		/// <see langword="null"/> is scripting is not supported.
		/// </summary>
		IExternalServiceScriptObject ScriptObject
		{
			get;
		}
	}
}

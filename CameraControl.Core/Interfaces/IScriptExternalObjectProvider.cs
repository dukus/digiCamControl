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
using CameraControl.Core.Classes;
using System.Collections.Generic;
#endregion

namespace CameraControl.Core.Interfaces
{
	/// <summary>
	/// Interface implemented by a provider that provides an extension to the single command system used in scripting. It
	/// is a factory for <see cref="IScriptExternalObject"/> instances. A provider that implements this interface relies on
	/// the application to handle the creation of the <see cref="IScriptExternalObject"/> instance, exposing triggers as
	/// <see cref="IUIButton"/>s and import/export of the configuration. If more control is required, the
	/// <see cref="IExternalServiceProvider"/> interface should be implemented instead.
	/// </summary>
	public interface IScriptExternalObjectProvider
	{
		/// <summary>
		/// Name of the plugin to use in the UI.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Explanation of the functionality of the <see cref="IScriptExternalObject"/> created by the plugin.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Description of the parameter required/accepted to create an <see cref="IScriptExternalObject"/>. The parameters
		/// are displayed in the UI in the order returned.
		/// </summary>
		IReadOnlyList<ScriptExternalObjectCreateParameter> Parameters
		{
			get;
		}

		/// <summary>
		/// Create the <see cref="IScriptExternalObject"/>.
		/// </summary>
		/// <param name="name">Name of the instance, used to address the instance in scripting.</param>
		/// <param name="parameters">Parameters to create the instance.</param>
		/// <returns>
		/// The instance. If the instance cannot be created, an exception should be thrown that reports the reason.
		/// </returns>
		IScriptExternalObject CreateInstance (string name, Dictionary<ScriptExternalObjectCreateParameter, object> parameters);
	}


}

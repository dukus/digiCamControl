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
using System.Text;
#endregion

namespace CameraControl.Core.Interfaces
{
	/// <summary>
	/// Interface that is supported by any singleton that provides some type of settings that can be exchanged via a DCC
	/// project file.
	/// </summary>
	public interface IDCCProjectSettingsProvider
	{
		/// <summary>
		/// Get the name of the group of settings
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Indicates whether the settings may refer to data in additional files.
		/// </summary>
		bool ReferencesLocalFiles
		{
			get;
		}

		/// <summary>
		/// Get the current settings to be stored (as JSON) in the DCC project.
		/// </summary>
		/// <param name="projectFile">
		/// Helper to convert data to values to be stored in the project file.
		/// </param>
		/// <returns>
		/// The configuration and a list of paths (as returned from <see cref="DCCProjectFile.MakeProjectSettingsFilePath"/>)
		/// of files that contain additional information and that have to be included in the project. Both may be
		/// <see langword="null"/>. Use <see cref="DCCProjectFile.Serialize"/> to convert the configuration values into
		/// strings.
		/// </returns>
		(Dictionary<string, string> settings, IEnumerable<string> filesToInclude) GetProjectSettings (DCCProjectFile projectFile);

		/// <summary>
		/// Replace/update the current instance according to the specified project settings.
		/// </summary>
		/// <param name="settings">
		/// A configuration previously returned by <see cref="GetProjectSettings"/>. Use
		/// <see cref="DCCProjectFile.Deserialize"/> to convert the configuration string values into values of the correct
		/// type.
		/// </param>
		/// <param name="projectFile">
		/// Helper to convert values as stored in the project file to data to be used in the application.
		/// </param>
		/// <param name="errors">Collector of non-fatal import errors.</param>
		void ApplyProjectSettings (Dictionary<string, string> settings, DCCProjectFile projectFile, StringBuilder errors);
	}
}

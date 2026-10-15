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
using CameraControl.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
#endregion

namespace CameraControl.Core.Classes
{
	public sealed class DCCProjectFile
	{
		public DCCProjectFile (string filePath)
		{
			FullPath = Path.GetFullPath(filePath);
			if (FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				FullPath = FullPath.Substring(0, FullPath.Length - 4);
				CreateZip = true;
			}
			ProjectDirectory = Path.GetDirectoryName(filePath);
		}

		public bool CreateZip { get; }

		public string FullPath { get; }

		public string ProjectDirectory { get; }

		public static string Serialize (object value)
		{
			if (value == null || value.GetType().IsValueType)
			{
				// This is not correct for structs like Rect, but those are unlikely to be present in settings.
				return value?.ToString();
			}
			return JsonConvert.SerializeObject(value, _settings);
		}
		public static object Deserialize (Type valueType, string data)
		{
			if (data == null)
			{
				return null;
			}
			else if (valueType.IsValueType)
			{
				return Convert.ChangeType(data, valueType);
			}
			return JsonConvert.DeserializeObject(data, valueType, _settings);
		}
		private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
		{
			DefaultValueHandling = DefaultValueHandling.Include,
			NullValueHandling = NullValueHandling.Include,
		};

		/// <summary>
		/// Turn a path to a file or directory into a path relative to the project file (if possible). The result should be
		/// stored in the project settings rather than the absolute path.
		/// </summary>
		/// <param name="filePath">File path to convert.</param>
		/// <returns>
		/// File path to store in the settings and to return from <see cref="IDCCProjectSettingsProvider.GetProjectSettings"/>
		/// .
		/// </returns>
		public string MakeProjectSettingsFilePath (string filePath)
		{
			if (!Path.IsPathRooted(filePath))
			{
				filePath = Path.GetFullPath(filePath);
			}
			if (Path.GetPathRoot(filePath) == Path.GetPathRoot(FullPath))
			{
				return HttpUtility.UrlDecode(new Uri(FullPath).MakeRelativeUri(new Uri(filePath)).ToString()).Replace('/', Path.DirectorySeparatorChar);
			}
			return filePath;
		}

		/// <summary>
		/// Get the absolute file path from a file path stored in the project settings.
		/// </summary>
		/// <param name="projectSettingsFilePath">
		/// File path as stored in the settings and passed to <see cref="IDCCProjectSettingsProvider.ApplyProjectSettings"/>.
		/// </param>
		/// <returns></returns>
		public string GetAbsoluteFilePath (string projectSettingsFilePath)
		{
			if (!Path.IsPathRooted(projectSettingsFilePath))
			{
				return Path.GetFullPath(Path.Combine(ProjectDirectory, projectSettingsFilePath));
			}
			return projectSettingsFilePath;
		}

		/// <summary>
		/// Get the full type name with generic parameters resolved.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetFullName (Type type)
		{
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsGenericType || typeInfo.IsGenericTypeDefinition)
			{
				var name = type.GetGenericTypeDefinition().FullName;
				var idx = name.IndexOf('`');
				return name.Substring(0, idx) + "<" +
					(type.GenericTypeArguments.Length > 0
						? string.Join(",", from p in type.GenericTypeArguments
										   select (p is null ? "" : p.FullName))
						: string.Join(",", from p in type.GetTypeInfo().GenericTypeParameters
										   select ""))
					+ ">";
			}
			return type.FullName;
		}
	}
}

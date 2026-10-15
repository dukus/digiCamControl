using CameraControl.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CameraControl.Core.Classes
{
	/// <summary>
	/// Base class for an implementation of a DCC project settings provider. The settings are the public read/write
	/// properties of the derived class that are not marked with <see cref="JsonIgnoreAttribute"/> or
	/// <see cref="DCCProjectIgnoreAttribute"/>. <see cref="JsonPropertyAttribute.PropertyName"/> can be used to specify an
	/// alternative name for the property. Mark file paths with the <see cref="FilePathAttribute"/>. The property values
	/// are copied for storage in the project file, and the values deserialized from the project files are assigned to the
	/// properties. This is a shallow copy: if a property's value is a class or struct, its properties are not copied or
	/// analyzed but (de)serialized directly.
	/// </summary>
	public abstract class DCCProjectSettingsProvider : IDCCProjectSettingsProvider
	{
		private readonly Type _type;
		private readonly object _instance;
		private readonly string _name;
		private readonly bool _referencesLocalFiles;

		public DCCProjectSettingsProvider (string name)
		{
			_instance = this;
			_type = GetType();
			_name = name;
			_referencesLocalFiles = (from p in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
									 where p.CanWrite && p.CanRead
										&& p.GetCustomAttribute<FilePathAttribute>() != null
									 	&& p.GetCustomAttribute<JsonIgnoreAttribute>() == null
									 	&& p.GetCustomAttribute<DCCProjectIgnoreAttribute>() == null
									 select p).Any();
		}

		protected DCCProjectSettingsProvider (string name, Type type, object instance)
		{
			_instance = instance;
			_type = type;
			_name = name;
			_referencesLocalFiles = (from p in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
									 where p.CanWrite && p.CanRead
										&& p.GetCustomAttribute<FilePathAttribute>() != null
									 	&& p.GetCustomAttribute<JsonIgnoreAttribute>() == null
									 	&& p.GetCustomAttribute<DCCProjectIgnoreAttribute>() == null
									 select p).Any();
		}

		/// <summary>
		/// Called before the settings are copied to the project file.
		/// </summary>
		/// <param name="projectFile">
		/// Helper to convert data to values to be stored in the project file.
		/// </param>
		/// <returns>
		/// Indicates whether the settings can be copied (<see langword="true"/>) rather than being ignored (<see
		/// langword="false"/>).
		/// </returns>
		protected virtual bool BeforeGetSettings (DCCProjectFile projectFile)
			=> true;

		/// <summary>
		/// Called before the settings from the project file are assigned.
		/// </summary>
		/// <param name="projectFile">
		/// Helper to convert values as stored in the project file to data to be used in the application.
		/// </param>
		/// <returns>
		/// Indicates whether the settings can be assigned (<see langword="true"/>) rather than being ignored (<see
		/// langword="false"/>).
		/// </returns>
		protected virtual bool BeforeApplyProjectSettings (DCCProjectFile projectFile)
			=> true;

		/// <summary>
		/// Called after the settings from the project file have been applied.
		/// </summary>
		/// <param name="projectFile">
		/// Helper to convert values as stored in the project file to data to be used in the application.
		/// </param>
		protected virtual void AfterApplyProjectSettings (DCCProjectFile projectFile)
		{
		}

		string IDCCProjectSettingsProvider.Name => _name;

		bool IDCCProjectSettingsProvider.ReferencesLocalFiles => _referencesLocalFiles;

		(Dictionary<string, string> settings, IEnumerable<string> filesToInclude) IDCCProjectSettingsProvider.GetProjectSettings (DCCProjectFile projectFile)
		{
			if (!BeforeGetSettings(projectFile))
			{
				return (null, null);
			}

			var settings = new Dictionary<string, string>();
			var filesToInclude = new List<string>();
			foreach (var property in (from p in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
									  where p.CanWrite && p.CanRead
										 && p.GetCustomAttribute<JsonIgnoreAttribute>() == null
									 	 && p.GetCustomAttribute<DCCProjectIgnoreAttribute>() == null
									  select p))
			{
				var name = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;

				var value = property.GetValue(_instance);
				if (value != null && property.GetCustomAttribute<FilePathAttribute>() != null)
				{
					var filePath = value.ToString();
					bool fileExists = false;
					try
					{
						fileExists = File.Exists(filePath);
					}
					catch
					{
					}
					if (fileExists)
					{
						settings.Add(name + ":exists", "1");
					}

					value = projectFile.MakeProjectSettingsFilePath(filePath);
					if (fileExists && !Path.IsPathRooted(value as string))
					{
						filesToInclude.Add(value as string);
					}
				}
				settings.Add(name, DCCProjectFile.Serialize(value));
			}
			return (settings, filesToInclude);
		}

		void IDCCProjectSettingsProvider.ApplyProjectSettings (Dictionary<string, string> settings, DCCProjectFile projectFile, StringBuilder errors)
		{
			if (!BeforeApplyProjectSettings(projectFile))
			{
				return;
			}
			foreach (var property in (from p in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
									  where p.CanWrite && p.CanRead
										 && p.GetCustomAttribute<JsonIgnoreAttribute>() == null
										  && p.GetCustomAttribute<DCCProjectIgnoreAttribute>() == null
									  select p))
			{
				var name = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
				if (settings.TryGetValue(name, out var stringValue))
				{
					var value = stringValue == null
									? null
									: DCCProjectFile.Deserialize(property.PropertyType, stringValue);
					if (value != null && property.GetCustomAttribute<FilePathAttribute>() != null)
					{
						var fullPath = projectFile.GetAbsoluteFilePath(value.ToString());
						value = fullPath;
						if (settings.TryGetValue(name + ":exists", out var fileExists)
							&& fileExists == "1"
							&& !File.Exists(fullPath))
						{
							errors.AppendLine($"File '{fullPath}', used by '{_name}', is not present");
						}
					}
					property.SetValue(_instance, value);
				}
			}
			AfterApplyProjectSettings(projectFile);
		}

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class FilePathAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class DCCProjectIgnoreAttribute : Attribute
		{
		}
	}

	public sealed class DCCProjectSettingsProvider<SettingsClass> : DCCProjectSettingsProvider
	{
		public DCCProjectSettingsProvider (string name, SettingsClass settings)
			: base(name, typeof(SettingsClass), settings)
		{
		}
	}
}

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
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;








#endregion

namespace CameraControl.Core.Classes
{
	/// <summary>
	/// Global collection of all items that form a project: a collection of projects that can be exported, copied to other
	/// machines and imported to provide DCC with the same functionality. Some projects are stored elsewhere as well (e.g.,
	/// sessions) - kept that way not to break the work of existing users. New in this project are global collection of all
	/// external devices and services, custom buttons and scripts that together form an extension of the UI and script
	/// interpreters of the application. New elements are added via various windows of the application, and the elements
	/// are used in multiple places. It is assumed that all machines have the same (version of the) application and
	/// plugins.
	/// </summary>
	public class DCCProject
	{
		public DCCProject ()
		{
			ExternalServiceProviders.CollectionChanged += UpdateButtonsChanged;
		}

		#region Stored elsewhere
		public ObservableCollection<PhotoSession> PhotoSessions
			=> ServiceProvider.Settings.PhotoSessions;

		public AsyncObservableCollection<CameraProperty> CameraProperties
			=> ServiceProvider.Settings.CameraProperties.Items;

		public ObservableCollection<CameraPreset> CameraPresets
			=> ServiceProvider.Settings.CameraPresets;

		public AsyncObservableCollection<IExternalServiceProvider> ExternalServiceProviders
			=> ServiceProvider.PluginManager.ExternalServiceProviders;

		public AsyncObservableCollection<IDCCProjectSettingsProvider> SettingsProviders
			=> ServiceProvider.PluginManager.DCCProjectSettingsProviders;
		#endregion

		#region External objects
		private readonly List<ExternalObject> _externalObjects = new List<ExternalObject>();

		/// <summary>
		/// Collection of <see cref="IScriptExternalObject"/> created from a <see cref="IScriptExternalObjectProvider"/>.
		/// </summary>
		public IReadOnlyList<ExternalObject> ExternalObjects
			=> _externalObjects;

		public sealed class ExternalObject : INotifyPropertyChanged
		{
			internal ExternalObject (IScriptExternalObject scriptObject, IUIButtonProvider buttonProvider)
			{
				ScriptObject = scriptObject;
				ButtonProvider = buttonProvider;
				if (scriptObject is INotifyPropertyChanged npc)
				{
					npc.PropertyChanged += (s, e) =>
					{
						if (e.PropertyName == nameof(ScriptObject.IsActivated))
						{
							NotifyPropertyChanged(nameof(IsActivated));
						}
					};
				}
			}

			public IScriptExternalObject ScriptObject
			{
				get;
			}

			public IUIButtonProvider ButtonProvider
			{
				get;
			}

			public bool IsActivated
				=> ScriptObject.IsActivated;

			public void Activate ()
			{
				if (!IsActivated)
				{
					ScriptObject.Activate();
					NotifyPropertyChanged(nameof(IsActivated));
					if (IsActivated
						&& ButtonProvider is ScriptExternalObjectUIButtonProvider helper)
					{
						helper.VerifyButtons();
					}
				}
			}

			#region Implementation of INotifyPropertyChanged
			public event PropertyChangedEventHandler PropertyChanged;
			private void NotifyPropertyChanged (string info)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
			}
			#endregion
		}

		/// <summary>
		/// Get the names of the <see cref="IExternalServiceProvider"/> with
		/// <see cref="IExternalServiceProvider.ScriptObject"/> and of the <see cref="IScriptExternalObject"/>, as used in
		/// scripts (with spaces replaced by '_'). Also includes the active state.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<(string scriptName, bool isActivated)> AllExternalObjects
		{
			get
			{
				foreach (var o in _externalObjects)
				{
					yield return (o.ScriptObject.Name.Replace(' ', '_'), o.ScriptObject.IsActivated);
				}
				foreach (var p in ServiceProvider.Project.ExternalServiceProviders)
				{
					if (p.ScriptObject != null)
					{
						yield return (p.ScriptObject.Name.Replace(' ', '_'), p.ScriptObject.IsActivated);
					}
				}
			}
		}


		/// <summary>
		/// Get the <see cref="IExternalServiceScriptObject"/> from either <see cref="ExternalObjects"/> or from the
		/// <see cref="ExternalServiceProviders"/>.
		/// </summary>
		/// <param name="name">Name of the external object (case insensitive).</param>
		/// <returns>The external object or <see langword="null"/>.s</returns>
		public IExternalServiceScriptObject GetExternalObject (string name)
		{
			var normalised = name.Replace(' ', '_').ToLower();
			IExternalServiceScriptObject result = null;
			lock (_externalObjects)
			{
				result = (from o in _externalObjects
						  where o.ScriptObject.Name.Replace(' ', '_').ToLower() == normalised
						  select o.ScriptObject).FirstOrDefault();
			}
			if (result == null)
			{
				result = (from p in ServiceProvider.Project.ExternalServiceProviders
						  where p.Name.Replace(' ', '_').ToLower() == normalised
						  select p.ScriptObject).FirstOrDefault();
			}
			return result;
		}

		public bool IsValidExternalObjectName (string name)
		{
			var normalised = name.Replace(' ', '_').ToLower();
			lock (_externalObjects)
			{
				if ((from o in _externalObjects
					 where o.ScriptObject.Name.Replace(' ', '_').ToLower() == normalised
					 select o).Any())
				{
					return false;
				}
			}
			return !(from p in ServiceProvider.Project.ExternalServiceProviders
					 where p.Name.Replace(' ', '_').ToLower() == normalised
					 select p).Any();
		}

		public ExternalObject CreateExternalObject (IScriptExternalObjectProvider provider, string name, Dictionary<ScriptExternalObjectCreateParameter, object> editedExternalObjectParameters)
		{
			lock (_externalObjects)
			{
				ExternalObject result = null;
				var scriptObject = provider.CreateInstance(name, new Dictionary<ScriptExternalObjectCreateParameter, object>(editedExternalObjectParameters));
				if (scriptObject != null)
				{
					var buttonProvider = scriptObject as IUIButtonProvider ?? new ScriptExternalObjectUIButtonProvider(scriptObject);
					_externalObjects.Add(result = new ExternalObject(scriptObject, buttonProvider));
					buttonProvider.ButtonsChanged += UpdateUIButtons;
					if ((buttonProvider.Buttons?.Count() ?? 0) > 0)
					{
						UpdateUIButtons(buttonProvider, EventArgs.Empty);
					}
				}
				return result;
			}
		}

		public bool RemoveExternalObject (IScriptExternalObject externalObject)
		{
			var result = false;
			if (externalObject != null)
			{
				IUIButtonProvider buttonProvider = null;
				lock (_externalObjects)
				{
					for (var i = 0; i < _externalObjects.Count; i++)
					{
						if (_externalObjects[i].ScriptObject == externalObject)
						{
							buttonProvider = _externalObjects[i].ButtonProvider;
							buttonProvider.ButtonsChanged -= UpdateUIButtons;
							_externalObjects.RemoveAt(i);
							result = true;
							break;
						}
					}
				}
				if (buttonProvider != null)
				{
					AddRemoveButtons(buttonProvider, null);
				}
				if (externalObject is IDisposable dispose)
				{
					dispose.Dispose();
				}
			}
			return result;
		}
		#endregion

		#region Tcl script state
		private readonly List<TclScript> _tclScripts = new List<TclScript>();

		public IReadOnlyList<TclScript> TclScripts
			=> _tclScripts;

		public TclScript CreateTclScript ()
		{
			lock (_tclScripts)
			{
				_lastTclScriptIndex++;
				var tclScript = new TclScript(_lastTclScriptIndex);
				tclScript.ButtonsChanged += UpdateUIButtons;
				_tclScripts.Add(tclScript);
				return tclScript;
			}
		}
		private int _lastTclScriptIndex;

		public void RemoveTclScript (TclScript tclScript)
		{
			lock (_tclScripts)
			{
				if (!_tclScripts.Remove(tclScript))
				{
					return;
				}
			}
			tclScript.ButtonsChanged -= UpdateUIButtons;
			AddRemoveButtons(tclScript, null);
		}

		public sealed class TclScript : IUIButtonProvider, INotifyPropertyChanged
		{
			internal TclScript (int index)
			{
				ScriptName = $"Tcl script #{index}";
			}

			public string ScriptName
			{
				get;
			}

			public string Name
				=> ScriptFilePath != null
						? $"{Path.GetFileName(ScriptFilePath)} [{ScriptName}]"
						: ScriptName;

			private string _scriptFilePath;
			[DCCProjectSettingsProvider.FilePath]
			public string ScriptFilePath
			{
				get => _scriptFilePath;
				set
				{
					if (_scriptFilePath != value)
					{
						_scriptFilePath = value;
						NotifyPropertyChanged(nameof(Name));
						NotifyPropertyChanged(nameof(ScriptFilePath));
					}
				}
			}

			[DCCProjectSettingsProvider.DCCProjectIgnore]
			public bool IsModifiedInEditor
			{
				get;
				set;
			}

			private bool _showInButtons;
			public bool ShowInButtons
			{
				get => _showInButtons;
				set
				{
					if (_showInButtons != value)
					{
						_showInButtons = value;
						if (_showInButtons)
						{
							_buttons = new UIButton[]
							{
								new UIButton ()
								{
									Provider = this,
									Title = "Run",
									IsEnabled = !IsRunning,
									OnClick = RunRequested
								},
								new UIButton ()
								{
									Provider = this,
									Title = "Stop",
									IsEnabled = IsRunning,
									OnClick = StopRequested
								}
							};
						}
						else
						{
							_buttons = Array.Empty<UIButton>();
						}
						ButtonsChanged?.Invoke(this, EventArgs.Empty);
					}
				}
			}

			private bool _isRunning;
			[DCCProjectSettingsProvider.DCCProjectIgnore]
			public bool IsRunning
			{
				get => _isRunning;
				set
				{
					if (_isRunning != value)
					{
						_isRunning = value;
						if (_buttons != null)
						{
							_buttons[0].IsEnabled = !_isRunning;
							_buttons[1].IsEnabled = _isRunning;
						}
						NotifyPropertyChanged(nameof(IsRunning));
					}
				}
			}

			private Action _runRequested;
			[DCCProjectSettingsProvider.DCCProjectIgnore]
			public Action RunRequested
			{
				get => _runRequested;
				set
				{
					_runRequested = value;
					if (_buttons != null)
					{
						_buttons[0].OnClick = _runRequested;
					}
				}
			}

			private Action _stopRequested;
			[DCCProjectSettingsProvider.DCCProjectIgnore]
			public Action StopRequested
			{
				get => _stopRequested;
				set
				{
					_stopRequested = value;
					if (_buttons != null)
					{
						_buttons[1].OnClick = _stopRequested;
					}
				}
			}

			[DCCProjectSettingsProvider.DCCProjectIgnore]
			public Action SaveScript
			{
				get;
				set;
			}

			private UIButton[] _buttons;
			public IEnumerable<IUIButton> Buttons
				=> _buttons;

			public event EventHandler ButtonsChanged;

			private sealed class UIButton : IUIButton, INotifyPropertyChanged
			{
				public IUIButtonProvider Provider
				{
					get;
					internal set;
				}

				public string Title
				{
					get;
					internal set;
				}

				public string Description
				{
					get;
					internal set;
				}

				private bool _isEnabled;
				public bool IsEnabled
				{
					get => _isEnabled;
					set
					{
						if (_isEnabled != value)
						{
							_isEnabled = value;
							NotifyPropertyChanged(nameof(IsEnabled));
						}
					}
				}

				internal Action OnClick
				{
					get;
					set;
				}

				public void Click ()
					=> OnClick?.Invoke();

				#region Implementation of INotifyPropertyChanged

				public event PropertyChangedEventHandler PropertyChanged;
				private void NotifyPropertyChanged (string info)
				{
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
				}
				#endregion
			}

			public void ShowWindow ()
				=> ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_ShowTclScript, this);

			#region Implementation of INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;
			private void NotifyPropertyChanged (string info)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
			}
			#endregion
		}
		#endregion

		#region UIButtons collection taken from various sources
		private readonly AsyncObservableCollection<IUIButton> _buttons = new AsyncObservableCollection<IUIButton>();

		public AsyncObservableCollection<IUIButton> UIButtons
			=> _buttons;

		private void UpdateButtonsChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (IUIButtonProvider provider in e.OldItems)
				{
					provider.ButtonsChanged -= UpdateUIButtons;
					AddRemoveButtons(provider, null);
				}
			}
			if (e.NewItems != null)
			{
				foreach (IUIButtonProvider provider in e.NewItems)
				{
					provider.ButtonsChanged += UpdateUIButtons;
					AddRemoveButtons(provider, provider.Buttons);
				}
			}
		}

		private void UpdateUIButtons (object sender, EventArgs _)
		{
			if (sender is IUIButtonProvider provider)
			{
				AddRemoveButtons(provider, provider.Buttons);
			}
		}

		private void AddRemoveButtons (IUIButtonProvider provider, IEnumerable<IUIButton> buttons)
		{
			var present = buttons?.ToList() ?? new List<IUIButton>();
			var toRemove = new List<IUIButton>();
			lock (_buttons)
			{
				foreach (var button in _buttons)
				{
					if (button.Provider == provider)
					{
						if (present.Contains(button))
						{
							present.Remove(button);
						}
						else
						{
							toRemove.Add(button);
						}
					}
				}
			}
			foreach (var button in toRemove)
			{
				_buttons.Remove(button);
			}
			foreach (var button in present)
			{
				_buttons.Add(button);
			}
		}
		#endregion

		#region Project
		public static void ExportProject (string projectFilePath, bool includeFiles,
			IEnumerable<TclScript> scripts, bool saveScripts,
			IEnumerable<IScriptExternalObject> externalObjects,
			IEnumerable<IExternalServiceProvider> serviceProviders,
			IEnumerable<PhotoSession> sessions,
			IEnumerable<CameraProperty> cameraProperties,
			IEnumerable<CameraPreset> cameraPresets,
			IEnumerable<IDCCProjectSettingsProvider> otherSettings)
		{
			var projectSettings = new ProjectSettings(new DCCProjectFile(projectFilePath), includeFiles);

			projectSettings.ExportTclScripts(scripts, saveScripts);
			projectSettings.ExportExternalObjects(externalObjects);
			projectSettings.ExportServiceProviders(serviceProviders);
			projectSettings.ExportSessions(sessions);
			projectSettings.ExportCameraProperties(cameraProperties);
			projectSettings.ExportCameraPresets(cameraPresets);
			projectSettings.ExportOtherSettings(otherSettings);

			projectSettings.Save();
		}

		public static string ImportProject (string projectFilePath, bool replaceScripts, bool replaceExternalObjects)
		{
			var errors = new StringBuilder();

			try
			{
				var projectSettings = ProjectSettings.Read(projectFilePath);

				projectSettings.ImportTclScripts(errors, replaceScripts);
				projectSettings.ImportExternalObjects(errors, replaceExternalObjects);
				projectSettings.ImportServiceProviders(errors);
				projectSettings.ImportSessions(errors);
				projectSettings.ImportCameraProperties(errors);
				projectSettings.ImportCameraPresets(errors);
				projectSettings.ImportOtherSettings(errors);
			}
			catch (Exception ex)
			{
				errors.AppendLine($"Error while importing the project: {ex.Message}");
			}
			if (errors.Length > 0)
			{
				Log.Error(errors.ToString());
				return errors.ToString();
			}
			return null;
		}

		private class ProjectSettings
		{
			private DCCProjectFile _projectFile;
			private readonly HashSet<string> _filesToInclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			private readonly bool _includeFiles;
			private readonly DCCProject _project;

			public ProjectSettings (DCCProjectFile projectFile, bool includeFiles)
				: this()
			{
				_projectFile = projectFile;
				_includeFiles = includeFiles;
			}

			public ProjectSettings ()
			{
				_project = ServiceProvider.Project;
			}

			internal void Save ()
			{
				var json = DCCProjectFile.Serialize(this);
				Directory.CreateDirectory(_projectFile.ProjectDirectory);

				if (_includeFiles || _projectFile.CreateZip)
				{
					var zipFilePath = _projectFile.FullPath + ".zip";
					if (File.Exists(zipFilePath))
					{
						File.Delete(zipFilePath);
					}
					using (var zip = new ZipFile(zipFilePath))
					{
						zip.AddEntry(Path.GetFileName(_projectFile.FullPath), json);
						if (_includeFiles)
						{
							foreach (var file in _filesToInclude)
							{
								zip.AddFile(Path.GetFullPath(Path.Combine(_projectFile.ProjectDirectory, file)), Path.GetDirectoryName(file) ?? "");
							}
						}
						zip.Save();
					}
				}
				else
				{
					File.WriteAllText(_projectFile.FullPath, json);
				}
			}

			internal static ProjectSettings Read (string projectFilePath)
			{
				string json = null;
				DCCProjectFile projectFile;
				if (projectFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					projectFile = new DCCProjectFile(projectFilePath.Substring(0, projectFilePath.Length - 4));
					try
					{
						var projectFileName = Path.GetFileName(projectFile.FullPath);
						using (var zip = ZipFile.Read(projectFilePath))
						{
							foreach (var entry in zip.Entries)
							{
								if (entry.FileName.Equals(projectFileName, StringComparison.OrdinalIgnoreCase))
								{
									using (Ionic.Crc.CrcCalculatorStream stream = entry.OpenReader())
									{
										using (var reader = new StreamReader(stream))
										{
											json = reader.ReadToEnd();
										}
									}
								}
								else
								{
									entry.Extract(projectFile.ProjectDirectory, ExtractExistingFileAction.OverwriteSilently);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Error($"Cannot unzip {projectFilePath}: {ex.Message}");
						throw;
					}
				}
				else
				{
					projectFile = new DCCProjectFile(projectFilePath);
					json = File.ReadAllText(projectFile.FullPath);
				}
				ProjectSettings projectSettings;
				try
				{
					projectSettings = JsonConvert.DeserializeObject(json, typeof(ProjectSettings)) as ProjectSettings;
					projectSettings._projectFile = projectFile;
					return projectSettings;
				}
				catch (Exception ex)
				{
					Log.Error($"Cannot read the project from {projectFile.FullPath}: {ex.Message}");
					throw;
				}
			}

			public List<Dictionary<string, string>> TclScripts
			{
				get; set;
			}
			internal void ExportTclScripts (IEnumerable<TclScript> scripts, bool saveScripts)
			{
				if (scripts != null)
				{
					foreach (var script in scripts)
					{
						if (_project.TclScripts.Contains(script))
						{
							if (saveScripts && script.IsModifiedInEditor)
							{
								script.SaveScript?.Invoke();
							}
							TclScripts = Serialize(new DCCProjectSettingsProvider<TclScript>(null, script), TclScripts);
						}
					}
				}
			}
			internal void ImportTclScripts (StringBuilder errors, bool replaceScripts)
			{
				if (replaceScripts)
				{
					lock (_project._tclScripts)
					{
						foreach (var script in _project._tclScripts)
						{
							_project.AddRemoveButtons(script, null);
						}
						_project._tclScripts.Clear();
					}
					ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_CloseAllTclScripts);
				}
				if (TclScripts != null)
				{
					var verify = false;
					foreach (var data in TclScripts)
					{
						(new DCCProjectSettingsProvider<TclScript>(null, _project.CreateTclScript()) as IDCCProjectSettingsProvider)
							.ApplyProjectSettings(data, _projectFile, errors);
						verify = true;
					}
					if (verify)
					{
						ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_VerifyTclScriptsPresent);
					}
				}
			}


			public List<Dictionary<string, string>> ExternalProviders
			{
				get; set;
			}
			internal void ExportServiceProviders (IEnumerable<IExternalServiceProvider> serviceProviders)
			{
				if (serviceProviders != null)
				{
					foreach (var provider in serviceProviders)
					{
						if (ServiceProvider.Project.ExternalServiceProviders.Contains(provider))
						{
							// Always add the provider to the project, so its presence will be checked on import
							ExternalProviders = Serialize(provider as IDCCProjectSettingsProvider, ExternalProviders,
								(":Type", DCCProjectFile.GetFullName(provider.GetType()))
							);
						}
					}
				}
			}
			internal void ImportServiceProviders (StringBuilder errors)
			{
				if (ExternalProviders != null)
				{
					foreach (var settings in ExternalProviders)
					{
						var providerType = settings[":Type"];
						var provider = (from p in _project.ExternalServiceProviders
										where DCCProjectFile.GetFullName(p.GetType()) == providerType
										select p).FirstOrDefault();
						if (provider == null)
						{
							errors.AppendLine($"External service {providerType} is not present");
							continue;
						}
						if (provider is IDCCProjectSettingsProvider settingsProvider)
						{
							UpdateUIButtonChanged(settingsProvider, false);
							try
							{
								settingsProvider.ApplyProjectSettings(settings, _projectFile, errors);
							}
							catch (Exception ex)
							{
								errors.AppendLine($"External service {providerType} not properly initialised: {ex.Message}");
							}
							finally
							{
								UpdateUIButtonChanged(settingsProvider, true);
							}
						}
						else if (settings.Count > 1)
						{
							errors.AppendLine($"External service {providerType} does not support the import of settings");
							continue;
						}
					}
				}
			}

			public List<Dictionary<string, string>> ExternalObjects
			{
				get; set;
			}
			internal void ExportExternalObjects (IEnumerable<IScriptExternalObject> externalObjects)
			{
				if (externalObjects != null)
				{
					foreach (var externalObject in externalObjects)
					{
						if ((from e in _project._externalObjects
							 where e.ScriptObject == externalObject
							 select e).Any())
						{
							if (ExternalObjects == null)
							{
								ExternalObjects = new List<Dictionary<string, string>>();
							}
							var settings = new Dictionary<string, string>();
							settings[":" + nameof(externalObject.Name)] = externalObject.Name;
							settings[":" + nameof(externalObject.Provider)] = DCCProjectFile.GetFullName(externalObject.Provider.GetType());
							foreach (var param in externalObject.ProviderParameters)
							{
								settings[param.Key.Name] = param.Value?.ToString();
							}
							ExternalObjects.Add(settings);
						}
					}
				}
			}
			internal void ImportExternalObjects (StringBuilder errors, bool replaceExternalObjects)
			{
				if (replaceExternalObjects)
				{
					lock (_project._externalObjects)
					{
						foreach (var externalObject in _project._externalObjects)
						{
							_project.AddRemoveButtons(externalObject.ButtonProvider, null);
						}
						_project._externalObjects.Clear();
					}
					ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_RemoveAllExternalObjects);
				}
				if (ExternalObjects != null)
				{
					var verify = false;
					lock (_project._externalObjects)
					{
						foreach (var settings in ExternalObjects)
						{
							verify |= DeserializeExternalObject(settings, errors);
						}
					}
					if (verify)
					{
						ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_VerifyExternalObjectsPresent);
					}
				}
			}
			private bool DeserializeExternalObject (Dictionary<string, string> settings, StringBuilder errors)
			{
				var verify = false;

				var name = settings[":" + nameof(IScriptExternalObject.Name)];
				var current = (from e in _project._externalObjects
							   where e.ScriptObject.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
							   select e).FirstOrDefault();
				if (current != null)
				{
					_project.AddRemoveButtons(current.ButtonProvider, null);
					_project._externalObjects.Remove(current);
					verify = true;
				}

				var providerType = settings[":" + nameof(IScriptExternalObject.Provider)];
				var provider = (from p in ServiceProvider.PluginManager.ExternalObjectProviders
								where DCCProjectFile.GetFullName(p.GetType()) == providerType
								select p).FirstOrDefault();
				if (provider == null)
				{
					errors.AppendLine($"External service/object '{name}' ({providerType}) not created as its provider is not present");
					return verify;
				}

				var parameters = new Dictionary<ScriptExternalObjectCreateParameter, object>();
				foreach (var param in provider.Parameters)
				{
					if (settings.TryGetValue(param.Name, out var stringValue))
					{
						object value = null;
						if (stringValue != null)
						{
							param.RefreshEnumerationValues();
							if (param.EnumerationValues != null && param.EnumerationValues.Count > 0)
							{
								if (!param.EnumerationValues.Contains(stringValue))
								{
									if (param.IsConstant)
									{
										errors.AppendLine($"External service/object '{name}' ({providerType}) not created as the parameter {param.Name}'s value is invalid");
										return verify;
									}
									else
									{
										errors.AppendLine($"External service/object '{name}' ({providerType}): parameter {param.Name}'s value is invalid and needs to be updated.");
									}
								}
							}
							switch (param.Type)
							{
								case ScriptExternalObjectCreateParameter.ValueType.Boolean:
									value = Convert.ToBoolean(stringValue);
									break;
								case ScriptExternalObjectCreateParameter.ValueType.Long:
									value = Convert.ToInt64(stringValue);
									break;
								case ScriptExternalObjectCreateParameter.ValueType.String:
									value = stringValue;
									break;
								default:
									errors.AppendLine($"External service/object '{name}' ({providerType}) not created as the parameter {param.Name}'s value is not available");
									return verify;
							}
						}
						parameters[param] = value;
					}
				}

				try
				{
					_project.CreateExternalObject(provider, name, parameters);
				}
				catch (Exception ex)
				{
					errors.AppendLine($"External service/object '{name}' ({providerType}) not created: {ex.Message}");
				}
				return true;
			}



			public List<Dictionary<string, string>> Sessions
			{
				get; set;
			}
			internal void ExportSessions (IEnumerable<PhotoSession> sessions)
			{
				if (sessions != null)
				{
					foreach (var session in sessions)
					{
						if (ServiceProvider.Project.PhotoSessions.Contains(session))
						{
							Sessions = Serialize(new DCCProjectSettingsProvider<PhotoSession>(session.Name, session), Sessions);
						}
					}
				}
			}
			internal void ImportSessions (StringBuilder errors)
			{
				if (Sessions != null)
				{
					foreach (var settings in Sessions)
					{
						var name = DCCProjectFile.Deserialize(typeof(string), settings[nameof(PhotoSession.Name)]) as string;
						var session = (from s in _project.PhotoSessions
									   where name.Equals(s.Name, StringComparison.OrdinalIgnoreCase)
									   select s).FirstOrDefault();
						var isNew = false;
						if (session == null)
						{
							session = PhotoSession.CreateFromDefault();
							isNew = true;
						}
						try
						{
							(new DCCProjectSettingsProvider<PhotoSession>(name, session) as IDCCProjectSettingsProvider).ApplyProjectSettings(settings, _projectFile, errors);
						}
						catch (Exception ex)
						{
							errors.AppendLine($"Session '{name}' not {(isNew ? "created" : "properly initialised")}: {ex.Message}");
						}
						if (isNew)
						{
							_project.PhotoSessions.Add(session);
						}
					}
				}
			}


			public List<Dictionary<string, string>> CameraProperties
			{
				get; set;
			}
			internal void ExportCameraProperties (IEnumerable<CameraProperty> cameraProperties)
			{
				if (cameraProperties != null)
				{
					foreach (var cameraProperty in cameraProperties)
					{
						if (ServiceProvider.Project.CameraProperties.Contains(cameraProperty))
						{
							CameraProperties = Serialize(new DCCProjectSettingsProvider<CameraProperty>(cameraProperty.DeviceName, cameraProperty), CameraProperties);
						}
					}
				}
			}
			internal void ImportCameraProperties (StringBuilder errors)
			{
				if (CameraProperties != null)
				{
					foreach (var settings in CameraProperties)
					{
						var name = DCCProjectFile.Deserialize(typeof(string), settings[nameof(CameraProperty.DeviceName)]) as string;
						var serial = DCCProjectFile.Deserialize(typeof(string), settings[nameof(CameraProperty.SerialNumber)]) as string;
						var properties = (from s in _project.CameraProperties
										  where s.DeviceName == name && s.SerialNumber == serial
										  select s).FirstOrDefault();
						var isNew = false;
						if (properties == null)
						{
							properties = new CameraProperty();
							isNew = true;
						}
						try
						{
							(new DCCProjectSettingsProvider<CameraProperty>(name, properties) as IDCCProjectSettingsProvider).ApplyProjectSettings(settings, _projectFile, errors);
						}
						catch (Exception ex)
						{
							errors.AppendLine($"Camera property for '{name}' not {(isNew ? "created" : "properly initialised")}: {ex.Message}");
						}
						if (isNew)
						{
							_project.CameraProperties.Add(properties);
						}
					}
				}
			}


			public List<Dictionary<string, string>> CameraPresets
			{
				get; set;
			}
			internal void ExportCameraPresets (IEnumerable<CameraPreset> cameraPresets)
			{
				if (cameraPresets != null)
				{
					foreach (var cameraPreset in cameraPresets)
					{
						if (ServiceProvider.Project.CameraPresets.Contains(cameraPreset))
						{
							CameraPresets = Serialize(new DCCProjectSettingsProvider<CameraPreset>(cameraPreset.Name, cameraPreset), CameraPresets);
						}
					}
				}
			}
			internal void ImportCameraPresets (StringBuilder errors)
			{
				if (CameraPresets != null)
				{
					foreach (var settings in CameraPresets)
					{
						var name = DCCProjectFile.Deserialize(typeof(string), settings[nameof(CameraPreset.Name)]) as string;
						var preset = (from s in _project.CameraPresets
									  where s.Name == name
									  select s).FirstOrDefault();
						var isNew = false;
						if (preset == null)
						{
							preset = new CameraPreset();
							isNew = true;
						}
						try
						{
							(new DCCProjectSettingsProvider<CameraPreset>(name, preset) as IDCCProjectSettingsProvider).ApplyProjectSettings(settings, _projectFile, errors);
						}
						catch (Exception ex)
						{
							errors.AppendLine($"Camera presets '{name}' not {(isNew ? "created" : "properly initialised")}: {ex.Message}");
						}
						if (isNew)
						{
							_project.CameraPresets.Add(preset);
						}
					}
				}
			}


			public List<Dictionary<string, string>> Settings
			{
				get; set;
			}
			internal void ExportOtherSettings (IEnumerable<IDCCProjectSettingsProvider> otherSettings)
			{
				if (otherSettings != null)
				{
					foreach (var settings in otherSettings)
					{
						Settings = Serialize(settings, Settings, (":Name", settings.Name));
					}
				}
			}
			internal void ImportOtherSettings (StringBuilder errors)
			{
				if (Settings != null)
				{
					foreach (var settings in Settings)
					{
						var name = settings[":Name"];
						var provider = (from s in _project.SettingsProviders
										where s.Name == name
										select s).FirstOrDefault();
						if (provider == null)
						{
							errors.AppendLine($"Settings '{name}' are not present");
							continue;
						}
						UpdateUIButtonChanged(provider, false);
						try
						{
							provider.ApplyProjectSettings(settings, _projectFile, errors);
						}
						catch (Exception ex)
						{
							errors.AppendLine($"Settings '{name}' not properly initialised: {ex.Message}");
						}
						UpdateUIButtonChanged(provider, true);
					}
				}
			}

			private List<Dictionary<string, string>> Serialize (IDCCProjectSettingsProvider provider, List<Dictionary<string, string>> list, params (string key, string value)[] extra)
			{
				var (data, files) = provider is null
					? (null, null)
					: provider.GetProjectSettings(_projectFile);
				if (extra.Length > 0)
				{
					if (data == null)
					{
						data = new Dictionary<string, string>();
					}
					foreach (var e in extra)
					{
						data[e.key] = e.value;
					}
				}
				if (data != null)
				{
					if (list == null)
					{
						list = new List<Dictionary<string, string>>();
					}
					list.Add(data);
					if (_includeFiles && files != null)
					{
						_filesToInclude.UnionWith(files);
					}
				}
				return list;
			}

			private void UpdateUIButtonChanged (object provider, bool subscribe)
			{
				if (provider is IUIButtonProvider uiButtonProvider)
				{
					if (subscribe)
					{
						uiButtonProvider.ButtonsChanged += _project.UpdateUIButtons;
						_project.UpdateUIButtons(uiButtonProvider, EventArgs.Empty);
					}
					else
					{
						uiButtonProvider.ButtonsChanged -= _project.UpdateUIButtons;
					}
				}
			}
		}
		#endregion
	}
}

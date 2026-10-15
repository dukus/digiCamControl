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
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ExportCustomControlsConfigurationDlg.xaml
	/// </summary>
	public partial class ExportProjectDlg : Window
	{
		public static void ExportProject ()
		{
			var dlg = new ExportProjectDlg();
			dlg.ShowDialog();
		}

		private readonly Dictionary<PhotoSession, ExportProjectDlg_CCNode> _sessions = new Dictionary<PhotoSession, ExportProjectDlg_CCNode>();
		private readonly Dictionary<CameraProperty, ExportProjectDlg_CCNode> _cameraProperties = new Dictionary<CameraProperty, ExportProjectDlg_CCNode>();
		private readonly Dictionary<CameraPreset, ExportProjectDlg_CCNode> _cameraPresets = new Dictionary<CameraPreset, ExportProjectDlg_CCNode>();
		private readonly Dictionary<IExternalServiceProvider, ExportProjectDlg_CCNode> _serviceProviders = new Dictionary<IExternalServiceProvider, ExportProjectDlg_CCNode>();
		private readonly Dictionary<IScriptExternalObject, ExportProjectDlg_CCNode> _externalObjects = new Dictionary<IScriptExternalObject, ExportProjectDlg_CCNode>();
		private readonly Dictionary<DCCProject.TclScript, ExportProjectDlg_CCNode> _scripts = new Dictionary<DCCProject.TclScript, ExportProjectDlg_CCNode>();
		private readonly Dictionary<IDCCProjectSettingsProvider, ExportProjectDlg_CCNode> _settings = new Dictionary<IDCCProjectSettingsProvider, ExportProjectDlg_CCNode>();

		public ExportProjectDlg ()
		{
			DataContext = this;

			var filesToSave = new List<ExportProjectDlg_CCNode>();
			var filesToInclude = new List<ExportProjectDlg_CCNode>();

			var topNode = new ExportProjectDlg_CCNode(null, "Sessions", false);
			ItemsToSelect.Add(topNode);
			foreach (var session in from s in ServiceProvider.Project.PhotoSessions
									orderby s.Name.ToLower()
									select s)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var node = new ExportProjectDlg_CCNode(topNode, session.Name);
				topNode.Children.Add(node);
				_sessions[session] = node;
			}

			topNode = new ExportProjectDlg_CCNode(null, "Camera properties", false);
			ItemsToSelect.Add(topNode);
			foreach (var cameraProperties in from s in ServiceProvider.Project.CameraProperties
											 where s.SerialNumber != null
											 orderby s.DeviceName.ToLower()
											 select s)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var node = new ExportProjectDlg_CCNode(topNode, cameraProperties.DeviceName);
				topNode.Children.Add(node);
				_cameraProperties[cameraProperties] = node;
			}

			topNode = new ExportProjectDlg_CCNode(null, "Camera presets", false);
			ItemsToSelect.Add(topNode);
			foreach (var cameraPreset in from s in ServiceProvider.Project.CameraPresets
										 orderby s.Name.ToLower()
										 select s)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var node = new ExportProjectDlg_CCNode(topNode, cameraPreset.Name);
				topNode.Children.Add(node);
				_cameraPresets[cameraPreset] = node;
			}

			topNode = new ExportProjectDlg_CCNode(null, "Services and devices", false);
			ItemsToSelect.Add(topNode);
			var childList = new List<ExportProjectDlg_CCNode>();
			foreach (var provider in ServiceProvider.Project.ExternalServiceProviders)
			{
				if (provider is IDCCProjectSettingsProvider)
				{
					topNode.IsSelected = topNode.IsEnabled = true;
					var node = new ExportProjectDlg_CCNode(topNode, provider.Name);
					childList.Add(node);
					_serviceProviders[provider] = node;
				}
			}
			foreach (var externalObject in ServiceProvider.Project.ExternalObjects)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var node = new ExportProjectDlg_CCNode(topNode, externalObject.ScriptObject.Name);
				childList.Add(node);
				_externalObjects[externalObject.ScriptObject] = node;
			}
			childList.Sort((x, y) => x.Title.ToLower().CompareTo(y.Title.ToLower()));
			topNode.Children = new ObservableCollection<ExportProjectDlg_CCNode>(childList);

			topNode = new ExportProjectDlg_CCNode(null, "Scripts", false);
			ItemsToSelect.Add(topNode);
			foreach (var script in from s in ServiceProvider.Project.TclScripts
								   where !string.IsNullOrEmpty(s.ScriptFilePath)
								   orderby Path.GetFileName(s.ScriptFilePath).ToLower(), s.ScriptName.ToLower()
								   select s)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var title = $"{Path.GetFileName(script.ScriptFilePath)} - {script.ScriptName}{(script.IsModifiedInEditor ? " *" : "")}";
				var node = new ExportProjectDlg_CCNode(topNode, title);
				topNode.Children.Add(node);
				_scripts[script] = node;

				filesToInclude.Add(node);
				if (script.IsModifiedInEditor)
				{
					filesToSave.Add(node);
				}
			}
			foreach (var script in from s in ServiceProvider.Project.TclScripts
								   where string.IsNullOrEmpty(s.ScriptFilePath)
								   orderby s.ScriptName.ToLower()
								   select s)
			{
				var title = $"??? - {script.ScriptName}{(script.IsModifiedInEditor ? " * " : "")}";
				var node = new ExportProjectDlg_CCNode(topNode, title, false);
				topNode.Children.Add(node);
			}

			topNode = new ExportProjectDlg_CCNode(null, "Other", false);
			ItemsToSelect.Add(topNode);
			foreach (var settings in from s in ServiceProvider.Project.SettingsProviders
									 orderby s.Name.ToLower()
									 select s)
			{
				topNode.IsSelected = topNode.IsEnabled = true;
				var node = new ExportProjectDlg_CCNode(topNode, settings.Name);
				topNode.Children.Add(node);
				_settings[settings] = node;

				if (settings.ReferencesLocalFiles)
				{
					filesToInclude.Add(node);
				}
			}

			void _EnableSaveFiles ()
			{
				cb_save_files.IsEnabled = (from s in filesToSave
										   where s.IsSelected == true
										   select s).Any();
			}
			foreach (var node in filesToSave)
			{
				node.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(ExportProjectDlg_CCNode.IsSelected))
					{
						_EnableSaveFiles();
					}
				};
			}

			void _EnableCreateZip ()
			{
				cb_include_files.IsEnabled = (from s in filesToInclude
											  where s.IsSelected == true
											  select s).Any();
			}
			foreach (var node in filesToInclude)
			{
				node.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(ExportProjectDlg_CCNode.IsSelected))
					{
						_EnableCreateZip();
					}
				};
			}

			InitializeComponent();
			_EnableSaveFiles();
			_EnableCreateZip();
		}

		private void btn_export_Click (object sender, RoutedEventArgs e)
		{
			var includeFiles = cb_include_files.IsEnabled && cb_include_files.IsChecked == true;
			var dlg = new SaveFileDialog()
			{
				Filter = includeFiles
					? "Project file (*.dccproject.zip)|*.dccproject.zip|All files|*.*"
					: "Project file (*.dccproject, *.dccproject.zip)|*.dccproject;*.dccproject.zip|All files|*.*"
			};
			if (dlg.ShowDialog() == true)
			{
				try
				{
					DCCProject.ExportProject(
						dlg.FileName, includeFiles,
						from n in _scripts where n.Value.IsSelected == true select n.Key, cb_save_files.IsChecked == true,
						from n in _externalObjects where n.Value.IsSelected == true select n.Key,
						from n in _serviceProviders where n.Value.IsSelected == true select n.Key,
						from n in _sessions where n.Value.IsSelected == true select n.Key,
						from n in _cameraProperties where n.Value.IsSelected == true select n.Key,
						from n in _cameraPresets where n.Value.IsSelected == true select n.Key,
						from n in _settings where n.Value.IsSelected == true select n.Key
					);
					Close();
				}
				catch (Exception exception)
				{
					MessageBox.Show("Error exporting project " + exception.Message);
					Log.Error("Error exporting project", exception);
				}
			}
		}

		public ObservableCollection<ExportProjectDlg_CCNode> ItemsToSelect
		{
			get;
		} = new ObservableCollection<ExportProjectDlg_CCNode>();
	}

	// XAML does not support nested types???

	public sealed class ExportProjectDlg_CCNode : INotifyPropertyChanged
	{
		private readonly ExportProjectDlg_CCNode _parent;

		internal ExportProjectDlg_CCNode (ExportProjectDlg_CCNode parent, string title, bool canBeSelected = true)
		{
			_parent = parent;
			Title = title;
			_isSelected = IsEnabled = canBeSelected;
		}

		public string Title
		{
			get;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

		private bool? _isSelected;
		public bool? IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					if (IsThreeState && value == null)
					{
						value = !_isSelected.Value;
					}
					SetIsSelected(value, true, true);
				}
			}
		}
		private void SetIsSelected (bool? value, bool updateParent, bool updateChildren)
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				NotifyPropertyChanged(nameof(IsSelected));

				if (_isSelected.HasValue)
				{
					if (updateChildren)
					{
						foreach (var child in Children)
						{
							if (child.IsEnabled)
							{
								child.SetIsSelected(_isSelected, false, true);
							}
						}
					}

					if (_parent != null && updateParent)
					{
						var anyTrue = (from c in _parent.Children
									   where c.IsSelected == true && c.IsEnabled
									   select c).Any();
						var anyFalse = (from c in _parent.Children
										where c.IsSelected == false && c.IsEnabled
										select c).Any();
						if (anyTrue && !anyFalse)
						{
							_parent.SetIsSelected(true, true, false);
						}
						else if (!anyTrue && anyFalse)
						{
							_parent.SetIsSelected(false, true, false);
						}
						else
						{
							_parent.SetIsSelected(null, true, false);
						}
					}
				}
			}
		}

		public bool IsThreeState
			=> Children.Count > 0;

		public ObservableCollection<ExportProjectDlg_CCNode> Children
		{
			get;
			set;
		} = new ObservableCollection<ExportProjectDlg_CCNode>();

		#region Implementation of INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}

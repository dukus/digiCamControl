using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace CameraControl.Plugins.ToolPlugins
{
	public class ArduinoPlugin : IToolPlugin, IExternalServiceProvider, IDCCProjectSettingsProvider, INotifyPropertyChanged
	{
		private ArduinoWindow _window;
		private ArduinoViewModel _viewModel;

		public bool Execute ()
		{
			if (_window == null || !_window.IsVisible)
			{
				_window = new ArduinoWindow()
				{
					DataContext = _viewModel,
					Owner = ServiceProvider.PluginManager.SelectedWindow as Window
				};
				_window.Show();
			}
			else
			{
				_window.Activate();
			}
			return true;
		}

		public string Title { get; set; }

		public string Id
		{
			get { return "{5B6842B3-E486-4A3E-A0C3-26988B6F0123}"; }
		}

		public void Init ()
		{
			_viewModel = new ArduinoViewModel();
			InitViewModel();
		}
		private void InitViewModel ()
		{
			_viewModel.UIButtonsChanged += (s, e) => ButtonsChanged?.Invoke(this, e);
			if (_viewModel.Active)
			{
				_viewModel.OpenPort();
				if (_viewModel.ButtonsStartup)
				{
					_viewModel.ShowButtons();
				}
			}
			_viewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_viewModel.Active))
				{
					NotifyPropertyChanged(nameof(IsActivated));
					ButtonsChanged?.Invoke(this, EventArgs.Empty);
				}
			};
			ScriptObject = new ArduinoScriptObject(this, _viewModel);
			NotifyPropertyChanged(nameof(IsActivated));
		}

		public ArduinoPlugin ()
		{
			Title = "Arduino (Serial)";
		}

		public string Name
			=> Title;

		void IUIButtonProvider.ShowWindow ()
			=> Execute();

		public IEnumerable<IUIButton> Buttons
			=> ((_viewModel?.Active ?? false) ? _viewModel?.UIButtons : null) ?? Array.Empty<IUIButton>();

		public event EventHandler ButtonsChanged;

		public bool IsActivated => _viewModel?.Active ?? false;

		public IExternalServiceScriptObject ScriptObject
		{
			get;
			private set;
		}

		bool IDCCProjectSettingsProvider.ReferencesLocalFiles => false;

		public (Dictionary<string, string> settings, IEnumerable<string> filesToInclude) GetProjectSettings (DCCProjectFile _)
		{
			var settings = new Dictionary<string, string>();
			foreach (var valuePair in _viewModel.PluginSetting.Values)
			{
				settings[valuePair.Name] = valuePair.Value;
				if (valuePair.IsDisabled)
				{
					settings[valuePair.Name + ":d"] = "1";
				}
			}
			return (settings, null);
		}

		public void ApplyProjectSettings (Dictionary<string, string> settings, DCCProjectFile _, StringBuilder __)
		{
			var values = new List<ValuePair>();
			foreach (var settingsPair in settings)
			{
				values.Add(new ValuePair()
				{
					Name = settingsPair.Key,
					Value = settingsPair.Value,
					IsDisabled = settings.ContainsKey(settingsPair.Key + ":d"),
				});
			}

			var isWindowVisible = _window?.IsVisible ?? false;
			_window?.Close();
			_window = null;

			if (_viewModel != null)
			{
				_viewModel.Active = false;
			}

			_viewModel = new ArduinoViewModel(values);
			InitViewModel();

			if (isWindowVisible)
			{
				Execute();
			}
		}

		#region Implementation of INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}

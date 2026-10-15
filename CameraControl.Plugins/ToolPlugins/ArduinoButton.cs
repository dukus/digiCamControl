using CameraControl.Core;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;
using System;
using System.Linq;
using System.Windows;

namespace CameraControl.Plugins.ToolPlugins
{
	public class ArduinoButton : BaseFieldClass, IUIButton
	{
		private bool _visible;
		private string _title;
		private string _command;


		public bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				NotifyPropertyChanged("Visible");
				NotifyPropertyChanged(nameof(Visibility));
			}
		}

		public Visibility Visibility
		{
			get { return Visible && !string.IsNullOrEmpty(Command) ? Visibility.Visible : Visibility.Collapsed; }
		}


		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyPropertyChanged("Title");
			}
		}

		public string Command
		{
			get { return _command; }
			set
			{
				_command = value;
				NotifyPropertyChanged("Command");
				NotifyPropertyChanged(nameof(Visibility));
			}
		}

		public string Description
			=> null;

		public bool IsEnabled
			=> true;


		internal Action<ArduinoButton> OnClick
		{
			get;
			set;
		}

		private IUIButtonProvider _provider;
		IUIButtonProvider IUIButton.Provider
		{
			get
			{
				if (_provider == null)
				{
					_provider = (from p in ServiceProvider.PluginManager.ToolPlugins
								 where p is ArduinoPlugin
								 select p as IExternalServiceProvider).FirstOrDefault();
				}
				return _provider;
			}
		}

		public void Click ()
		{
			OnClick?.Invoke(this);
		}
	}
}

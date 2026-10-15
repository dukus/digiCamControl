using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

namespace CameraControl.Plugins.ToolPlugins
{
	public class ArduinoCommandViewModel : ViewModelBase
	{
		private List<ArduinoButton> _buttons;
		public List<ArduinoButton> Buttons
		{
			get => _buttons;
			set
			{
				_buttons = value;
				foreach (var button in _buttons)
				{
					button.OnClick = Execute;
					button.PropertyChanged += (s, e) =>
					{
						if (e.PropertyName == nameof(button.Visibility))
						{
							UIButtonsChanged?.Invoke(this, EventArgs.Empty);
						}
					};
				}
			}
		}

		public RelayCommand<ArduinoButton> ExecuteCommand { get; set; }
		public ArduinoViewModel ArduinoViewModel { get; set; }

		public ArduinoCommandViewModel ()
		{
			ExecuteCommand = new RelayCommand<ArduinoButton>(Execute);
			var buttons = new List<ArduinoButton>();
			for (int i = 0; i < 16; i++)
			{
				var button = new ArduinoButton()
				{
					Title = "Button " + (i + 1),
					Visible = true
				};
				buttons.Add(button);
			}
			Buttons = buttons;
		}

		private void Execute (ArduinoButton obj)
		{
			if (ArduinoViewModel.Active)
				ArduinoViewModel.Send(obj.Command);
		}

		public event EventHandler UIButtonsChanged;
	}
}

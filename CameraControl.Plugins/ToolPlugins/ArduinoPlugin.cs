using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoPlugin : IToolPlugin
    {
        private ArduinoWindow _window;
        private ArduinoViewModel _viewModel;

        public bool Execute()
        {
            if (_window == null || !_window.IsVisible)
            {
                _window = new ArduinoWindow();
                _window.DataContext = _viewModel;
                _window.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
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

        public void Init()
        {
            _viewModel = new ArduinoViewModel();
            if (_viewModel.Active)
            {
                _viewModel.OpenPort();
                if (_viewModel.ButtonsStartup)
                {
                    _viewModel.ShowButtons();
                }
            }
        }

        public ArduinoPlugin()
        {
            Title = "Arduino (Serial)";
        }
    }
}

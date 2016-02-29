using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    public class CombineZpPlugin : IToolPlugin
    {
        private CombineZpWindow _window;
        public bool Execute()
        {
            if (_window == null || !_window.IsVisible)
            {
                _window = new CombineZpWindow();
                _window.DataContext = new CombineZpViewModel(_window);
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
            get { return "{F3155291-D688-49B8-B22D-E74A2D5E020E}"; }
        }

        public CombineZpPlugin()
        {
            Title = "Combine Zp";
        }
    }
}

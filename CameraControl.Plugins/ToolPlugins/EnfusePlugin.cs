using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    public class EnfusePlugin : IToolPlugin
    {
        private EnfusePluginWindow _window;
        public bool Execute()
        {
            if (_window == null || !_window.IsVisible)
            {
                _window = new EnfusePluginWindow();
                _window.DataContext = new EnfusePluginViewModel(_window);
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
            get { return "{2495C316-F222-4AAC-8F2C-65DF524D75CC}"; }
        }

        public void Init()
        {
            
        }

        public EnfusePlugin()
        {
            Title = "Enfuse";
        }

    }
}

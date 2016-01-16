using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    public class EnfusePlugin : IToolPlugin
    {
        public bool Execute()
        {
            EnfusePluginWindow window = new EnfusePluginWindow();
            window.DataContext = new EnfusePluginViewModel();
            window.ShowDialog();
            return true;
        }

        public string Title { get; set; }
        
        public string Id
        {
            get { return "{2495C316-F222-4AAC-8F2C-65DF524D75CC}"; }
        }

        public EnfusePlugin()
        {
            Title = "Enfuse";
        }

    }
}

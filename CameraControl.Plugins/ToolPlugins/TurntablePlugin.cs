using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    class TurntablePlugin : IToolPlugin
    {
        public bool Execute()
        {
           TurntableWindows wnd=new TurntableWindows();
            wnd.DataContext = new TurntableViewModel();
            wnd.Show();
            return true;
        }

        public string Title { get; set; }

        public TurntablePlugin()
        {
            Title = "Turntable";
        }
    }
}

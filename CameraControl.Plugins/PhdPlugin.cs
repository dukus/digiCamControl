using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;
using CameraControl.Plugins.Astro;

namespace CameraControl.Plugins
{
    public class PhdPlugin : IToolPlugin
    {
        #region Implementation of IToolPlugin

        public bool Execute()
        {
            PHDWnd wnd = new PHDWnd();
            wnd.Show();
            return true;
        }

        public string Title { get; set; }

        #endregion

        public PhdPlugin()
        {
            Title = "PHD Guiding";
        }
    }
}

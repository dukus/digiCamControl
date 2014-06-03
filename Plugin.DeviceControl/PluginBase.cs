using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace Plugin.DeviceControl
{ 
    public class PluginBase : IPlugin
    {
        #region Implementation of IPlugin

        public bool Register()
        {
            ServiceProvider.PluginManager.ToolPlugins.Add(new WaterDropWnd());
            return true;
        }

        #endregion
    }
}

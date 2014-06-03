using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Plugins.ExportPlugins;
using CameraControl.Plugins.ExternalDevices;
using CameraControl.Plugins.MainWindowPlugins;


namespace CameraControl.Plugins
{
    public class Plugins : IPlugin
    {
        #region Implementation of IPlugin

        public bool Register()
        {
            try
            {
                ServiceProvider.PluginManager.ExportPlugins.Add(new ExportToZip());
                ServiceProvider.PluginManager.ExportPlugins.Add(new ExportToFolder());
                ServiceProvider.PluginManager.MainWindowPlugins.Add(new SimpleMainWindow());
                ServiceProvider.PluginManager.ToolPlugins.Add(new PhdPlugin());
                ServiceProvider.ExternalDeviceManager.ExternalDevices.Add(new SerialPortShutterRelease());
                ServiceProvider.ExternalDeviceManager.ExternalDevices.Add(new DSUSBShutterRelease());
                ServiceProvider.ExternalDeviceManager.ExternalDevices.Add(new MultiCameraBoxShutterRelease());
                ServiceProvider.ExternalDeviceManager.ExternalDevices.Add(new DCCUSBShutterRelease());
            }
            catch (Exception exception)
            {
                Log.Error("Error loadings plugins ", exception);
            }
            return true;
        }

        #endregion
    }
}

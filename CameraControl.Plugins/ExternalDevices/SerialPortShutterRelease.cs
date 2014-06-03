using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ExternalDevices
{
    public class SerialPortShutterRelease : IExternalDevice
    {

        #region Implementation of IExternalShutterReleaseSource

        public string Name { get; set; }
        
        public bool Capture(CustomConfig config)
        {
            return true;
        }

        public bool Focus(CustomConfig config)
        {
            return true;
        }

        public bool CanExecute(CustomConfig config)
        {
            return true;
        }

        public UserControl GetConfig(CustomConfig config)
        {
            return new SerialPortShutterReleaseConfig(config);
        }

        public SourceEnum DeviceType { get; set; }
        public bool OpenShutter(CustomConfig config)
        {
            if (config.AttachedObject != null)
                CloseShutter(config);
            SerialPort serialPort = new SerialPort(config.Get("Port"));
            serialPort.Open();
            serialPort.RtsEnable = true;
            config.AttachedObject = serialPort;
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            if (config.AttachedObject == null)
                return false;
            SerialPort serialPort = config.AttachedObject as SerialPort;
            if (serialPort == null) throw new ArgumentNullException("serialPort");
            serialPort.RtsEnable = false;
            serialPort.Close();
            config.AttachedObject = null;
            return true;
        }

        public bool AssertFocus(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        public bool DeassertFocus(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        #endregion

        public SerialPortShutterRelease()
        {
            Name = "Serial Port Shutter Release";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }
    }
}

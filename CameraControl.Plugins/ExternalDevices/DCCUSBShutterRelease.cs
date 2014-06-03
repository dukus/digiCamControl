using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.ExternalDevices
{
    internal class DCCUSBShutterRelease : IExternalDevice
    {
        private object _locker = new object();

        #region Implementation of IExternalDevice

        public string Name { get; set; }


        public bool Capture(CustomConfig config)
        {
            SendCommand("0", config.Get("Port"));
            SendCommand(config.Get("IrRemote") == "True" ? "7" : "5", config.Get("Port"));
            return true;
        }

        public bool Focus(CustomConfig config)
        {
            SendCommand("0", config.Get("Port"));
            SendCommand("6", config.Get("Port"));
            return true;
        }

        public bool CanExecute(CustomConfig config)
        {
            return true;
        }

        public UserControl GetConfig(CustomConfig config)
        {
            return new DCCUSBShutterReleaseConfig(config);
        }

        public SourceEnum DeviceType { get; set; }

        public bool OpenShutter(CustomConfig config)
        {
            SendCommand("0", config.Get("Port"));
            SendCommand(config.Get("IrRemote") == "True" ? "7" : "2", config.Get("Port"));
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            SendCommand(config.Get("IrRemote") == "True" ? "7" : "3", config.Get("Port"));
            return true;
        }

        public bool AssertFocus(CustomConfig config)
        {
            SendCommand("1", config.Get("Port"));
            return true;
        }

        public bool DeassertFocus(CustomConfig config)
        {
            SendCommand("4", config.Get("Port"));
            return true;
        }

        #endregion

        public DCCUSBShutterRelease()
        {
            Name = "DCCUSB Shutter Release";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }

        private void SendCommand(string cmd, string port)
        {
            lock (_locker)
            {
                try
                {
                    SerialPort sp = new SerialPort();
                    if (sp.IsOpen)
                        sp.Close();
                    sp.PortName = port;
                    sp.BaudRate = 9600;
                    sp.WriteTimeout = 3500;
                    sp.Open();
                    sp.Write(cmd);
                    sp.Close();
                }
                catch (Exception exception)
                {
                    Log.Error("Error sending serial command ", exception);
                    StaticHelper.Instance.SystemMessage = "Error sending serial command " + exception.Message;
                }
            }
        }
    }
}

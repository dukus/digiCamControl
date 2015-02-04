#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

#endregion

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
            try
            {
                return new SerialPortShutterReleaseConfig(config);
            }
            catch (Exception exception)
            {
                Log.Error("", exception);
            }
            return null;
        }

        public SourceEnum DeviceType { get; set; }

        public bool OpenShutter(CustomConfig config)
        {
            try
            {
                if (config.AttachedObject != null)
                    CloseShutter(config);
                SerialPort serialPort = new SerialPort(config.Get("Port"));
                serialPort.Open();
                serialPort.RtsEnable = true;
                config.AttachedObject = serialPort;
            }
            catch (Exception ex)
            {
                Log.Debug("Comm OpenShutter", ex);
                StaticHelper.Instance.SystemMessage = "Error Shutter " + ex.Message;
            }
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            try
            {
                if (config.AttachedObject == null)
                    return false;
                SerialPort serialPort = config.AttachedObject as SerialPort;
                if (serialPort == null) throw new ArgumentNullException("serialPort");
                serialPort.RtsEnable = false;
                serialPort.Close();
                config.AttachedObject = null;
            }
            catch (Exception ex)
            {
                Log.Debug("Comm CloseShutter", ex);
                StaticHelper.Instance.SystemMessage = "Error Shutter " + ex.Message;
            }
            return true;
        }

        public bool AssertFocus(CustomConfig config)
        {
            //throw new NotImplementedException();
            return true;
        }

        public bool DeassertFocus(CustomConfig config)
        {
            //throw new NotImplementedException();
            return true;
        }

        #endregion

        public SerialPortShutterRelease()
        {
            Name = "Serial Port Shutter Release";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }
    }
}
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

#endregion

namespace CameraControl.Plugins.ExternalDevices
{
    public class DSUSBShutterRelease : IExternalDevice
    {
        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_Open();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern void DSUSB_Close();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_Reset();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_ShutterOpen();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_ShutterClose();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_FocusAssert();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_FocusDeassert();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern int DSUSB_ShutterStatus(ref int status);

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_LEDRed();

        [DllImport("ShoestringDSUSB_DLL.dll")]
        public static extern bool DSUSB_LEDGreen();

        #region Implementation of IExternalDevice

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
            return null;
        }

        public SourceEnum DeviceType { get; set; }

        public bool OpenShutter(CustomConfig config)
        {
            if (!DSUSB_Reset() && !DSUSB_Open())
                throw new Exception(string.Format("Error connect device {0} ", config.Name));
            DSUSB_LEDRed();
            if (!OpenShutter())
                throw new Exception(string.Format("Error open shutter device {0} ", config.Name));
            DSUSB_FocusAssert();
            DSUSB_LEDGreen();
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            DSUSB_FocusDeassert();
            if (!DSUSB_ShutterClose())
                throw new Exception(string.Format("Error close shutter device {0}", config.Name));
            DSUSB_LEDRed();
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

        private bool OpenShutter()
        {
            int status = 0;
            for (int index = 1; index <= 5; ++index)
            {
                DSUSB_ShutterOpen();
                DSUSB_ShutterStatus(ref status);
                if (status == 2)
                    return true;
                Thread.Sleep(500);
            }
            return false;
        }

        public DSUSBShutterRelease()
        {
            Name = "DSUSB Shutter Release";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }
    }
}
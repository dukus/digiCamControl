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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CameraControl.Devices;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

#endregion

namespace CameraControl.Core.Classes
{
    public class TriggerClass
    {
        public WebServer WebServer { get; set; }
        public KeyboardHook KeyboardHook { get; set; }
        private static bool _eventIsBusy = false;
        private static bool _altPressed = false;
        private static bool _ctrlPressed = false;

        public TriggerClass()
        {
            WebServer = new WebServer();
            KeyboardHook = new KeyboardHook("DCC");
        }

        public void Start()
        {
            //_hookID = SetHook(_proc);
            try
            {
                if (ServiceProvider.Settings.UseWebserver)
                {
                    WebServer.Start(ServiceProvider.Settings.WebserverPort);
                }
                KeyboardHook.KeyDownEvent += KeyDown;
                KeyboardHook.KeyUpEvent += KeyUp;
            }
            catch (Exception)
            {
                Log.Error("Unable to start webserver");
            }
        }

        private void KeyUp(KeyboardHookEventArgs e)
        {
            _altPressed = e.isAltPressed;
            _ctrlPressed = e.isCtrlPressed;

        }

        private void KeyDown(KeyboardHookEventArgs e)
        {
            _altPressed = e.isAltPressed;
            _ctrlPressed = e.isCtrlPressed;
            if (_eventIsBusy)
                return;
            Task.Factory.StartNew(() => KeyDownThread(e));
        }

        private void KeyDownThread(KeyboardHookEventArgs e)
        {
            _eventIsBusy = true;
            try
            {
                Key inputKey = KeyInterop.KeyFromVirtualKey((int)e.Key);
                foreach (var item in ServiceProvider.Settings.Actions)
                {
                    if (!item.Global)
                        continue;
                    if (item.Alt == e.isAltPressed && item.Ctrl == e.isCtrlPressed && item.KeyEnum == inputKey)
                        ServiceProvider.WindowsManager.ExecuteCommand(item.Name);
                }
                ICameraDevice lastDevice = null;
                foreach (ICameraDevice device in ServiceProvider.DeviceManager.ConnectedDevices)
                {
                    if (lastDevice != null)
                        lastDevice.WaitForCamera(1500);

                    // wait for camera to finish last transfer with timeot of 1.5 sec
                    device.WaitForCamera(1500);
                    // skip camera is camera is still busy
                    if (device.IsBusy)
                        continue;
                    CameraProperty property = device.LoadProperties();
                    if (property.KeyTrigger.KeyEnum != Key.None && property.KeyTrigger.Alt == e.isAltPressed &&
                        property.KeyTrigger.Ctrl == e.isCtrlPressed &&
                        property.KeyTrigger.KeyEnum == inputKey)
                    {
                        CameraHelper.Capture(device);
                        lastDevice = device;
                    }
                }
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Key trigger ", exception);
            }
            _eventIsBusy = false;
        }

        public void Stop()
        {
            WebServer.Stop();
        }

        public static void KeyDown(KeyEventArgs e)
        {
            foreach (var item in ServiceProvider.Settings.Actions)
            {
                if (item.KeyEnum == e.Key && item.Alt == _altPressed && item.Ctrl == _ctrlPressed)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(item.Name);
                    e.Handled = true;
                }
            }

        }

    }
}
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
using System.IO;
using System.Linq;
using System.Net;
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
        private Process _ngrok_process = null;
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
                KeyboardHook.KeyDownEvent += KeyDown;
                KeyboardHook.KeyUpEvent += KeyUp;
                if (ServiceProvider.Settings.UseWebserver)
                {
                    WebServer.Start(ServiceProvider.Settings.WebserverPort);
                    string file = Path.Combine(Settings.ApplicationFolder, "ngrok.exe");
                    _ngrok_process = PhotoUtils.Run(file, "http " + ServiceProvider.Settings.WebserverPort,
                        ProcessWindowStyle.Hidden);
                    if (_ngrok_process == null)
                        return;
                    Thread.Sleep(2000);
                    using (var client = new WebClient())
                    {
                        string data = client.DownloadString("http://127.0.0.1:4040/api/tunnels");
                        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                        string url = json.tunnels[0].public_url;
                        if (!string.IsNullOrEmpty(url))
                        {
                            client.DownloadString(
                                string.Format("http://digicamcontrol.com/remote/submit.php?id={0}&url={1}",
                                    ServiceProvider.Settings.ClientId, url));
                        }
                    }
                }
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
            {
                Log.Debug("Evcent busy !");
                return;
            }
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
            if (_ngrok_process!=null && !_ngrok_process.HasExited)
            {
                _ngrok_process.Kill();
            }
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
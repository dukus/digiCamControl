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
using System.Text;
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Nikon;
using CameraControl.ViewModel;
using CameraControl.XSplit;

#endregion

namespace CameraControl.windows
{
    public class LiveViewManager : IWindow
    {
        private static object _locker = new object();

        private Dictionary<object, LiveViewWnd> _register;
        private static Dictionary<ICameraDevice, bool> _recordtoRam;
        private static Dictionary<ICameraDevice, bool> _hostMode;
        private static Dictionary<ICameraDevice, CameraPreset> _presets;

        public LiveViewManager()
        {
            _register = new Dictionary<object, LiveViewWnd>();
            _recordtoRam = new Dictionary<ICameraDevice, bool>();
            _hostMode = new Dictionary<ICameraDevice, bool>();
            _presets = new Dictionary<ICameraDevice, CameraPreset>();
            try
            {
                // xsplit plugin support 
                var plugin = TimedBroadcasterPlugin.CreateInstance(
                    "F752DC1B-438E-4014-914B-48F249D4C8F1", null, 1380, 112, 50);

                if (plugin != null)
                {
                    plugin.StartTimer();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start XSplit ", exception);
            }
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            if (param == null)
                param = ServiceProvider.DeviceManager.SelectedCameraDevice;
            lock (_locker)
            {
                switch (cmd)
                {
                    case WindowsCmdConsts.LiveViewWnd_Show:
                        {
                            if (!_register.ContainsKey(param))
                            {
                                Application.Current.Dispatcher.Invoke(new Action(delegate
                                {
                                    LiveViewWnd wnd = new LiveViewWnd();
                                    ServiceProvider.Settings.ApplyTheme(wnd);
                                    _register.Add(param, wnd);
                                    wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                                }));
                            }
                            NikonBase nikonBase = param as NikonBase;
                            if (nikonBase != null && ServiceProvider.Settings.EasyLiveViewControl)
                            {
                                CameraPreset preset = new CameraPreset();
                                preset.Get(nikonBase);
                                if (!_presets.ContainsKey(nikonBase))
                                    _presets.Add(nikonBase, preset);
                                else
                                    _presets[nikonBase] = preset;
                                if (nikonBase.ShutterSpeed.Value == "Bulb")
                                {
                                    nikonBase.ShutterSpeed.Value =
                                        nikonBase.ShutterSpeed.Values[nikonBase.ShutterSpeed.Values.Count / 2];
                                }
                                nikonBase.FocusMode.Value = nikonBase.FocusMode.Values[0];
                                nikonBase.FNumber.Value = nikonBase.FNumber.Values[0];
                            }
                            _register[param].ExecuteCommand(cmd, param);
                        }
                        break;
                    case WindowsCmdConsts.LiveViewWnd_Hide:
                    {
                        if (_register.ContainsKey(param))
                            _register[param].ExecuteCommand(cmd, param);
                        var nikonBase = param as NikonBase;
                        if (ServiceProvider.Settings.EasyLiveViewControl)
                        {
                            if (nikonBase != null && _presets.ContainsKey(nikonBase))
                            {
                                nikonBase.ShutterSpeed.Value = _presets[nikonBase].GetValue("ShutterSpeed");
                                nikonBase.FNumber.Value = _presets[nikonBase].GetValue("FNumber");
                                nikonBase.FocusMode.Value = _presets[nikonBase].GetValue("FocusMode");
                            }
                        }
                    }
                        break;
                    case CmdConsts.All_Minimize:
                    case CmdConsts.All_Close:
                        foreach (var liveViewWnd in _register)
                        {
                            liveViewWnd.Value.ExecuteCommand(cmd, param);
                        }
                        break;
                    default:
                        foreach (var liveViewWnd in _register)
                        {
                            if (cmd.StartsWith("LiveView"))
                            {
                                liveViewWnd.Value.ExecuteCommand(cmd, param);
                            }
                        }
                        break;
                }
            }
        }

        public bool IsVisible { get; private set; }

        #endregion

        public static void StartLiveView(ICameraDevice device)
        {
            // some nikon cameras can set af to manual
            //force to capture in ram
            if (device is NikonBase)
            {
                //if (!_recordtoRam.ContainsKey(device))
                //    _recordtoRam.Add(device, device.CaptureInSdRam);
                //else
                //    _recordtoRam[device] = device.CaptureInSdRam;
                //device.CaptureInSdRam = true;
                //if (!_hostMode.ContainsKey(device))
                //    _hostMode.Add(device, device.HostMode);
                //else
                //    _hostMode[device] = device.HostMode;
                //device.HostMode = true;
            }
            device.StartLiveView();
        }

        public static void StopLiveView(ICameraDevice device)
        {
            if (device == null || !device.IsConnected)
                return;
            device.StopLiveView();
            if (device is NikonBase)
            {
                //if (_recordtoRam.ContainsKey(device))
                //    device.CaptureInSdRam = _recordtoRam[device];
                //if (_hostMode.ContainsKey(device))
                //    device.HostMode = _hostMode[device];
            }
        }

        public static LiveViewData GetLiveViewImage(ICameraDevice device)
        {
            return device.GetLiveViewImage();
        }
    }
}
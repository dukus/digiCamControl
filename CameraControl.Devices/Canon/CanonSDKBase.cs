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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using CameraControl.Devices.Classes;
using Canon.Eos.Framework;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Internal.SDK;

#endregion

namespace CameraControl.Devices.Canon
{
    public class CanonSDKBase : BaseMTPCamera
    {
        private EosLiveImageEventArgs _liveViewImageData = null;
        private bool _recording = false;

        public EosCamera Camera = null;
        private System.Timers.Timer _shutdownTimer = new System.Timers.Timer(1000*60);

        protected Dictionary<uint, string> _shutterTable = new Dictionary<uint, string>
                                                               {
                                                                   {0x0C, "Bulb"},
                                                                   {0x10, "30"},
                                                                   {0x13, "25"},
                                                                   {0x14, "20"},
                                                                   {0x15, "20"},
                                                                   {0x18, "15"},
                                                                   {0x1B, "13"},
                                                                   {0x1C, "10"},
                                                                   {0x1D, "10"},
                                                                   {0x20, "8"},
                                                                   {0x23, "6"},
                                                                   {0x24, "6"},
                                                                   {0x25, "5"},
                                                                   {0x28, "4"},
                                                                   {0x2B, "3.2"},
                                                                   {0x2C, "3"},
                                                                   {0x2D, "2.5"},
                                                                   {0x30, "2"},
                                                                   {0x33, "1.6"},
                                                                   {0x34, "1.5"},
                                                                   {0x35, "1.3"},
                                                                   {0x38, "1"},
                                                                   {0x3B, "0.8"},
                                                                   {0x3C, "0.7"},
                                                                   {0x3D, "0.6"},
                                                                   {0x40, "0.5"},
                                                                   {0x43, "0.4"},
                                                                   {0x44, "0.3"},
                                                                   {0x45, "0.3"},
                                                                   {0x48, "1/4"},
                                                                   {0x4B, "1/5"},
                                                                   {0x4C, "1/6"},
                                                                   {0x4D, "1/5"},
                                                                   {0x50, "1/8"},
                                                                   {0x53, "1/10"},
                                                                   {0x54, "1/10"},
                                                                   {0x55, "1/13"},
                                                                   {0x58, "1/15"},
                                                                   {0x5B, "1/20"},
                                                                   {0x5C, "1/20"},
                                                                   {0x5D, "1/25"},
                                                                   {0x60, "1/30"},
                                                                   {0x63, "1/40"},
                                                                   {0x64, "1/45"},
                                                                   {0x65, "1/50"},
                                                                   {0x68, "1/60"},
                                                                   {0x6B, "1/80"},
                                                                   {0x6C, "1/90"},
                                                                   {0x6D, "1/100"},
                                                                   {0x70, "1/125"},
                                                                   {0x73, "1/160"},
                                                                   {0x74, "1/180"},
                                                                   {0x75, "1/200"},
                                                                   {0x78, "1/250"},
                                                                   {0x7B, "1/320"},
                                                                   {0x7C, "1/350"},
                                                                   {0x7D, "1/400"},
                                                                   {0x80, "1/500"},
                                                                   {0x83, "1/640"},
                                                                   {0x84, "1/750"},
                                                                   {0x85, "1/800"},
                                                                   {0x88, "1/1000"},
                                                                   {0x8B, "1/1250"},
                                                                   {0x8C, "1/1500"},
                                                                   {0x8D, "1/1600"},
                                                                   {0x90, "1/2000"},
                                                                   {0x93, "1/2500"},
                                                                   {0x94, "1/3000"},
                                                                   {0x95, "1/3200"},
                                                                   {0x98, "1/4000"},
                                                                   {0x9B, "1/5000"},
                                                                   {0x9C, "1/6000"},
                                                                   {0x9D, "1/6400"},
                                                                   {0xA0, "1/8000"},
                                                               };

        protected Dictionary<int, string> _apertureTable = new Dictionary<int, string>
                                                               {
                                                                   {0x08, "1.0"},
                                                                   {0x0B, "1.1"},
                                                                   {0x0C, "1.2"},
                                                                   {0x0D, "1.0"},
                                                                   {0x10, "1.4"},
                                                                   {0x13, "1.6"},
                                                                   {0x14, "1.8"},
                                                                   {0x15, "1.8"},
                                                                   {0x18, "2.0"},
                                                                   {0x1B, "2.2"},
                                                                   {0x1C, "2.5"},
                                                                   {0x1D, "2.5"},
                                                                   {0x20, "2.8"},
                                                                   {0x23, "3.2"},
                                                                   {0x24, "3.5"},
                                                                   {0x25, "3.5"},
                                                                   {0x28, "4.0"},
                                                                   {0x2B, "4.5"},
                                                                   {0x2C, "4.5"},
                                                                   {0x2D, "5.0"},
                                                                   {0x30, "5.6"},
                                                                   {0x33, "6.3"},
                                                                   {0x34, "6.7"},
                                                                   {0x35, "7.1"},
                                                                   {0x38, "8.0"},
                                                                   {0x3B, "9.0"},
                                                                   {0x3C, "9.5"},
                                                                   {0x3D, "10.0"},
                                                                   {0x40, "11.0"},
                                                                   {0x43, "13.0"},
                                                                   {0x44, "13.0"},
                                                                   {0x45, "14.0"},
                                                                   {0x48, "16.0"},
                                                                   {0x4B, "18.0"},
                                                                   {0x4C, "19.0"},
                                                                   {0x4D, "20.0"},
                                                                   {0x50, "22.0"},
                                                                   {0x53, "25.0"},
                                                                   {0x54, "27.0"},
                                                                   {0x55, "29.0"},
                                                                   {0x58, "32.0"},
                                                                   {0x5B, "36.0"},
                                                                   {0x5C, "38.0"},
                                                                   {0x5D, "40.0"},
                                                                   {0x60, "45.0"},
                                                                   {0x63, "51.0"},
                                                                   {0x64, "54.0"},
                                                                   {0x65, "57.0"},
                                                                   {0x68, "64.0"},
                                                                   {0x6B, "72.0"},
                                                                   {0x6C, "76.0"},
                                                                   {0x6D, "80.0"},
                                                                   {0x70, "91.0"},
                                                               };

        protected Dictionary<long, string> _exposureModeTable = new Dictionary<long, string>()
                                                                    {
                                                                        {0, "P"},
                                                                        {1, "Tv"},
                                                                        {2, "Av"},
                                                                        {3, "M"},
                                                                        {4, "Bulb"},
                                                                        {5, "A-DEP"},
                                                                        {6, "DEP"},
                                                                        {7, "Camera settings registered"},
                                                                        {8, "Lock"},
                                                                        {9, "Auto"},
                                                                        {10, "Night scene Portrait"},
                                                                        {11, "Sport"},
                                                                        {12, "Portrait"},
                                                                        {13, "Landscape"},
                                                                        {14, "Close-Up"},
                                                                        {15, "Flash Off"},
                                                                        {19, "Creative Auto"},
                                                                        {21, "Photo in Movie"},
                                                                    };

        protected Dictionary<uint, string> _isoTable = new Dictionary<uint, string>()
                                                           {
                                                               {0x00000000, "Auto"},
                                                               {0x00000028, "6"},
                                                               {0x00000030, "12"},
                                                               {0x00000038, "25"},
                                                               {0x00000040, "50"},
                                                               {0x00000048, "100"},
                                                               {0x0000004B, "125"},
                                                               {0x0000004D, "160"},
                                                               {0x00000050, "200"},
                                                               {0x00000053, "250"},
                                                               {0x00000055, "320"},
                                                               {0x00000058, "400"},
                                                               {0x0000005B, "500"},
                                                               {0x0000005D, "640"},
                                                               {0x00000060, "800"},
                                                               {0x00000063, "1000"},
                                                               {0x00000065, "1250"},
                                                               {0x00000068, "1600"},
                                                               {0x00000070, "3200"},
                                                               {0x00000078, "6400"},
                                                               {0x00000080, "12800"},
                                                               {0x00000088, "25600"},
                                                               {0x00000090, "51200"},
                                                               {0x00000098, "102400"},
                                                           };

        protected Dictionary<uint, string> _ec = new Dictionary<uint, string>()
                                                     {
                                                         {0x18, "+3.0"},
                                                         {0x15, "+2 2/3"},
                                                         {0x14, "+2.5"},
                                                         {0x13, "+2 1/3"},
                                                         {0x10, "+2.0"},
                                                         {0x0D, "+1 2/3"},
                                                         {0x0C, "+1.5"},
                                                         {0x0B, "+1 1/3"},
                                                         {0x08, "+1"},
                                                         {0x05, "+2/3"},
                                                         {0x04, "+0.5"},
                                                         {0x03, "+1/3"},
                                                         {0x00, "0.0"},
                                                         {0xFD, "-1/3"},
                                                         {0xFC, "-0.5"},
                                                         {0xFB, "-2/3"},
                                                         {0xF8, "-1"},
                                                         {0xF5, "-1 1/3"},
                                                         {0xF4, "-1.5"},
                                                         {0xF3, "-1 2/3"},
                                                         {0xF0, "-2"},
                                                         {0xED, "-2 1/3"},
                                                         {0xEC, "-2.5"},
                                                         {0xEB, "-3 2/3"},
                                                         {0xE8, "-3"},
                                                     };

        protected Dictionary<uint, string> _wbTable = new Dictionary<uint, string>()
                                                          {
                                                              {0, "Auto"},
                                                              {1, "Daylight"},
                                                              {2, "Cloudy"},
                                                              {3, "Tungsten"},
                                                              {4, "Fluorescent"},
                                                              {5, "Flash"},
                                                              {6, "Manual"},
                                                              {8, "Shade"},
                                                              {9, "Color temperature"},
                                                              {10, "Custom white balance: PC-1"},
                                                              {11, "Custom white balance: PC-2"},
                                                              {12, "Custom white balance: PC-3"},
                                                              {15, "Manual 2"},
                                                              {16, "Manual 3"},
                                                              {18, "Manual 4"},
                                                              {19, "Manual 5"},
                                                              {20, "Custom white balance: PC-4"},
                                                              {21, "Custom white balance: PC-5"},
                                                          };

        protected Dictionary<uint, string> _meteringTable = new Dictionary<uint, string>()
                                                                {
                                                                    {1, "Spot metering"},
                                                                    {3, "Evaluative metering"},
                                                                    {4, "Partial metering"},
                                                                    {5, "Center-weighted averaging metering"},
                                                                    {0xFFFFFFFF, "Not valid/no settings changes"},
                                                                };

        protected Dictionary<uint, string> _focusModeTable = new Dictionary<uint, string>()
                                                                 {
                                                                     {0, "One-Shot AF"},
                                                                     {1, "AI Servo AF"},
                                                                     {2, "AI Focus AF"},
                                                                     {3, "Manual Focus"},
                                                                     {0xFFFFFFFF, "Not valid/no settings changes"},
                                                                 };

        public CanonSDKBase()
        {
        }

        public override bool CaptureInSdRam
        {
            get { return base.CaptureInSdRam; }
            set
            {
                base.CaptureInSdRam = value;
                if (IsConnected && Camera != null)
                {
                    if (!base.CaptureInSdRam)
                    {
                        Camera.SavePicturesToCamera();
                    }
                    else
                    {
                        Camera.SavePicturesToHost(Path.GetTempPath());
                    }
                }
            }

        }

        public override DateTime DateTime
        {
            get
            {
                try
                {
                    return Camera.GetDate();
                }
                catch (Exception e)
                {
                    Log.Error("   Unable to get time", e);
                }
                return DateTime.MinValue;
            }
            set
            {
                try
                {
                    Camera.SetDate(value);
                    NotifyPropertyChanged("DateTime");

                }
                catch (Exception e)
                {
                    Log.Error("Unable to set time",e);
                }
            }
        }

        public bool Init(EosCamera camera)
        {
            try
            {
                PreventShutDown = true;
                IsBusy = true;
                Camera = camera;
                Camera.IsErrorTolerantMode = false;
                DeviceName = Camera.DeviceDescription;
                PortName = camera.PortName;
                Manufacturer = "Canon Inc.";
                Camera.SetEventHandlers();
                Camera.EnsureOpenSession();
                Camera.Error += _camera_Error;
                Camera.Shutdown += _camera_Shutdown;
                Camera.LiveViewPaused += Camera_LiveViewPaused;
                Camera.LiveViewUpdate += Camera_LiveViewUpdate;
                Camera.PictureTaken += Camera_PictureTaken;
                Capabilities.Add(CapabilityEnum.Bulb);
                Capabilities.Add(CapabilityEnum.LiveView);
                Capabilities.Add(CapabilityEnum.CaptureInRam);
                Capabilities.Add(CapabilityEnum.RecordMovie);
                Capabilities.Add(CapabilityEnum.SimpleManualFocus);
                IsConnected = true;
                LoadProperties();
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(200);
                    OnCameraInitDone();
                });
                thread.Start();
                _shutdownTimer.Elapsed += _shutdownTimer_Elapsed;
                return true;
            }
            catch (Exception exception)
            {
                Log.Error("Error initialize EOS camera object ", exception);
                return false;
            }
        }

        private void _shutdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsBusy)
                return;
            if (Monitor.TryEnter(Locker, 10))
            {
                try
                {
                    Camera_WillShutdown(null, null);
                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
        }

        public void LoadProperties()
        {
            InitMode();
            InitShutterSpeed();
            InitFNumber();
            InitIso();
            InitEc();
            InitWb();
            InitMetering();
            InitFocus();
            InitOther();
            InitCompression();
            Battery = (int) Camera.BatteryLevel + 20;
            IsBusy = false;
            CaptureInSdRam = true;
            Camera.PropertyChanged += Camera_PropertyChanged;
            Camera.WillShutdown += Camera_WillShutdown;
            SerialNumber = Camera.SerialNumber;
            AddAditionalProps();
        }

        private void Camera_WillShutdown(object sender, EventArgs e)
        {
            try
            {
                if (PreventShutDown)
                {
                    Camera.SendCommand(Edsdk.CameraCommand_ExtendShutDownTimer);
                }
            }
            catch (Exception exception)
            {
                Log.Debug("PreventShutDown", exception);
            }
        }

        public virtual void AddAditionalProps()
        {
            AdvancedProperties.Add(InitDriveMode());
            AdvancedProperties.Add(InitFlahEc());
            AdvancedProperties.Add(InitBracket());
            AdvancedProperties.Add(InitAEBracket());
            foreach (PropertyValue<long> value in AdvancedProperties)
            {
                value.SetValue((long)Camera.GetProperty(value.Code), false);
            }
        }

        private PropertyValue<long> InitFlahEc()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Flash Compensation",
                IsEnabled = true,
                Code = Edsdk.PropID_FlashCompensation,
                SubType = typeof(UInt32),
                DisableIfWrongValue = true
            };

            var data = GetSettingsList(Edsdk.PropID_FlashCompensation);
            
            res.ValueChanged += FlashExposureCompensation_ValueChanged;
            try
            {
                foreach (KeyValuePair<uint, string> keyValuePair in _ec)
                {
                    if (data.Count == 0)
                    {
                        res.AddValues(keyValuePair.Value, (int)keyValuePair.Key);
                    }
                    else
                    {
                        if (data.Contains((int)keyValuePair.Key))
                            res.AddValues(keyValuePair.Value, (int)keyValuePair.Key);
                    }
                }
                res.ReloadValues();
                res.IsEnabled = true;
                res.SetValue((int)Camera.GetProperty(Edsdk.PropID_FlashCompensation), false);
            }
            catch (Exception exception)
            {
                Log.Debug("Error get EC", exception);
            }
            return res;
        }

        private void FlashExposureCompensation_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_FlashCompensation, val);
            }
            catch (Exception exception)
            {
                Log.Debug("Error set EC to camera", exception);
            }
        }

        private PropertyValue<long> InitDriveMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Drive Mode",
                IsEnabled = true,
                Code = Edsdk.PropID_DriveMode,
                SubType = typeof (UInt32),
                DisableIfWrongValue = true
            };
            var r = Camera.GetPropertyDescription(Edsdk.PropID_DriveMode);
            var rr = r.PropDesc.Take(r.NumElements).ToArray();
            if (rr.Contains(0))
                res.AddValues("Single-Frame Shooting", 0);
            if (rr.Contains(1))
                res.AddValues("Continuous Shooting", 1);
            if (rr.Contains(2))
                res.AddValues("Video", 2);
            //res.AddValues("Not used", 3);
            if (rr.Contains(4))
                res.AddValues("High-Speed Continuous Shooting", 4);
            if (rr.Contains(5))
                res.AddValues("Low-Speed Continuous Shooting", 5);
            if (rr.Contains(6))
                res.AddValues("Silent single shooting", 6);
            if (rr.Contains(7))
                res.AddValues("10-Sec Self-Timer plus continuous shots", 7);
            if (rr.Contains(10))
                res.AddValues("10-Sec Self-Timer", 10);
            if (rr.Contains(11))
                res.AddValues("2-Sec Self-Timer", 11);
            if (rr.Contains(12))
                res.AddValues("14fps super high speed", 12);
            if (rr.Contains(13))
                res.AddValues("Silent single shooting", 13);
            if (rr.Contains(14))
                res.AddValues("Silent contin shooting", 14);
            if (rr.Contains(15))
                res.AddValues("Silent HS continuous", 15);
            if (rr.Contains(16))
                res.AddValues("Silent LS continuous", 16);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => Camera.SetProperty(res.Code, val); 
            return res;
        }

        private PropertyValue<long> InitFlash()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Flash",
                IsEnabled = true,
                Code = Edsdk.PropID_FlashOn,
                SubType = typeof(UInt32),
                DisableIfWrongValue = true
            };
            res.AddValues("No flash", 0);
            res.AddValues("Flash", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => Camera.SetProperty(res.Code, val);
            return res;
        }

        private PropertyValue<long> InitBracket()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Bracketing",
                IsEnabled = false,
                Code = Edsdk.PropID_Bracket,
                SubType = typeof(UInt32),
                DisableIfWrongValue = true
            };
            res.AddValues("Bracket off", 0xFFFFFFFF);
            res.AddValues("AE bracket", 1);
            res.AddValues("ISO bracket", 2);
            res.AddValues("WB bracket", 3);
            res.AddValues("FE bracket", 4);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => Camera.SetProperty(res.Code, val);
            return res;
        }

        private PropertyValue<long> InitAEBracket()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "AE Bracketing",
                IsEnabled = true,
                Code = Edsdk.PropID_AEBracket,
                SubType = typeof(UInt32),
                DisableIfWrongValue = true
            };
            var r = Camera.GetPropertyDescription(Edsdk.PropID_AEBracket);
            res.AddValues("Bracket off", 0);
            for (int i = 1; i < r.NumElements; i++)
            {
                res.AddValues("Step "+i, r.PropDesc[i]);
            }
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) =>
                {
                    Camera.SetProperty(res.Code, val);
                };
            return res;
        }
        private void InitOther()
        {
            LiveViewImageZoomRatio = new PropertyValue<long> {Name = "LiveViewImageZoomRatio"};
            LiveViewImageZoomRatio.AddValues("All", 1);
            LiveViewImageZoomRatio.AddValues("5x", 5);
            //LiveViewImageZoomRatio.AddValues("10x", 10);
            LiveViewImageZoomRatio.SetValue("All");
            LiveViewImageZoomRatio.ReloadValues();
            LiveViewImageZoomRatio.ValueChanged += LiveViewImageZoomRatio_ValueChanged;
        }

        private void InitCompression()
        {
            var data = GetSettingsList(Edsdk.PropID_ImageQuality);
            CompressionSetting = new PropertyValue<long>();
            CompressionSetting.AddValues("Large Fine JPEG", (int) new EosImageQuality(){PrimaryCompressLevel = EosCompressLevel.Fine,PrimaryImageFormat = EosImageFormat.Jpeg,PrimaryImageSize = EosImageSize.Large,SecondaryCompressLevel = EosCompressLevel.Unknown,SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown}.ToBitMask());
            CompressionSetting.AddValues("Large Normal JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Normal, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Large, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Medium Fine JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Fine, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Middle, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Medium Normal JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Normal, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Middle, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Small Fine JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Fine, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Small2, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Small Normal JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Normal, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Small2, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Smaller JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Fine, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Small3, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("Tiny JPEG", (int)new EosImageQuality() { PrimaryCompressLevel = EosCompressLevel.Fine, PrimaryImageFormat = EosImageFormat.Jpeg, PrimaryImageSize = EosImageSize.Small4, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown }.ToBitMask());
            CompressionSetting.AddValues("RAW + Large Fine JPEG", (int)new EosImageQuality() {PrimaryImageFormat =EosImageFormat.Cr2,PrimaryCompressLevel = EosCompressLevel.Lossless, PrimaryImageSize = EosImageSize.Large, SecondaryImageSize =EosImageSize.Large,SecondaryCompressLevel = EosCompressLevel.Fine, SecondaryImageFormat = EosImageFormat.Jpeg }.ToBitMask());
            CompressionSetting.AddValues("RAW", (int)new EosImageQuality() { PrimaryImageFormat = EosImageFormat.Cr2, PrimaryCompressLevel = EosCompressLevel.Lossless, PrimaryImageSize = EosImageSize.Large, SecondaryCompressLevel = EosCompressLevel.Unknown, SecondaryImageFormat = EosImageFormat.Unknown, SecondaryImageSize = EosImageSize.Unknown}.ToBitMask());
            CompressionSetting.Filter(data.Select(i => (long) i).ToList());
            CompressionSetting.SetValue((int) Camera.ImageQuality.ToBitMask());
            CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
        }

        private void CompressionSetting_ValueChanged(object sender, string key, long val)
        {
            Camera.ImageQuality = EosImageQuality.Create(val);
        }


        private void LiveViewImageZoomRatio_ValueChanged(object sender, string key, long val)
        {
            Camera.SetProperty(Edsdk.PropID_Evf_Zoom, val);
            //Camera.LiveViewqueue.Enqueue(() => Camera.SetProperty(Edsdk.PropID_Evf_Zoom, val));
        }


        private void Camera_PropertyChanged(object sender, EosPropertyEventArgs e)
        {
            if (IsBusy)
                return;
            lock (Locker)
            {

                try
                {
                    // Log.Debug("Property changed " + e.PropertyId.ToString("X"));
                    switch (e.PropertyId)
                    {
                        case Edsdk.PropID_ExposureCompensation:
                            ExposureCompensation.SetValue((int) Camera.GetProperty(Edsdk.PropID_ExposureCompensation),
                                false);
                            break;
                        case Edsdk.PropID_AEMode:
                            ReInitFNumber(true);
                            ReInitShutterSpeed();
                            Mode.SetValue((uint)Camera.GetProperty(Edsdk.PropID_AEMode), false);
                            break;
                        case Edsdk.PropID_WhiteBalance:
                            WhiteBalance.SetValue(Camera.GetProperty(Edsdk.PropID_WhiteBalance), false);
                            break;
                        case Edsdk.PropID_ISOSpeed:
                            IsoNumber.SetValue(Camera.GetProperty(Edsdk.PropID_ISOSpeed), false);
                            break;
                        case Edsdk.PropID_Tv:
                            ShutterSpeed.SetValue(Camera.GetProperty(Edsdk.PropID_Tv), false);
                            break;
                        case Edsdk.PropID_Av:
                            FNumber.SetValue((int) Camera.GetProperty(Edsdk.PropID_Av), false);
                            break;
                        case Edsdk.PropID_MeteringMode:
                            ExposureMeteringMode.SetValue((int) Camera.GetProperty(Edsdk.PropID_MeteringMode), false);
                            break;
                        case Edsdk.PropID_AFMode:
                            FocusMode.SetValue((int) Camera.GetProperty(Edsdk.PropID_AFMode), false);
                            break;
                        case Edsdk.PropID_ImageQuality:
                            int i = (int) Camera.GetProperty(Edsdk.PropID_ImageQuality);
                            CompressionSetting.SetValue((int) Camera.ImageQuality.ToBitMask(), false);
                            break;
                        case Edsdk.PropID_BatteryLevel:
                            Battery = (int) Camera.BatteryLevel + 20;
                            break;
                        case Edsdk.PropID_AEBracketType:
                            int ae = (int)Camera.GetProperty(Edsdk.PropID_AEBracketType);
                            //ResetShutterButton();
                            break;
                        case Edsdk.PropID_Bracket:
                            int br = (int)Camera.GetProperty(Edsdk.PropID_Bracket);
                            //ResetShutterButton();
                            break;

                        case Edsdk.PropID_FocusInfo:
                            //ResetShutterButton();
                            break;
                    }
                    foreach (
                        PropertyValue<long> advancedProperty in
                            AdvancedProperties.Where(advancedProperty => advancedProperty.Code == e.PropertyId))
                    {
                        advancedProperty.SetValue((long) Camera.GetProperty(advancedProperty.Code), false);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Error set property " + e.PropertyId.ToString("X"), exception);
                }
            }
        }

        private void Camera_PictureTaken(object sender, EosImageEventArgs e)
        {
            try
            {
                Log.Debug("Picture taken event received type" + e.GetType().ToString());
                PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                                                  {
                                                      WiaImageItem = null,
                                                      //EventArgs =
                                                      //  new PortableDeviceEventArgs(new PortableDeviceEventType()
                                                      //  {
                                                      //      ObjectHandle =
                                                      //        (uint)longeventParam
                                                      //  }),
                                                      CameraDevice = this,
                                                      FileName = "IMG0000.jpg",
                                                      Handle =e.Pointer.ToInt32() != 0?(object) e.Pointer: e
                                                  };

                EosFileImageEventArgs file = e as EosFileImageEventArgs;
                if (file != null)
                {
                    args.FileName = Path.GetFileName(file.ImageFilePath);
                }
                EosMemoryImageEventArgs memory = e as EosMemoryImageEventArgs;
                if (memory != null)
                {
                    if (!string.IsNullOrEmpty(memory.FileName))
                        args.FileName = Path.GetFileName(memory.FileName);
                }
                OnPhotoCapture(this, args);
                OnCaptureCompleted(this, new EventArgs());
            }
            catch (Exception exception)
            {
                Log.Error("EOS Picture taken event error", exception);
            }
        }

        private void Camera_LiveViewUpdate(object sender, EosLiveImageEventArgs e)
        {
            _liveViewImageData = e;
        }

        private void Camera_LiveViewPaused(object sender, EventArgs e)
        {
            try
            {
                //Camera.ResetShutterButton();
                //Camera.TakePictureNoAf();
                //Camera.ResetShutterButton();
                //Camera.ResumeLiveview();
            }
            catch (Exception exception)
            {
                IsBusy = false;
                Log.Debug("Live view pause error", exception);
            }
        }

        private void _camera_Shutdown(object sender, EventArgs e)
        {
            IsConnected = false;
            OnCameraDisconnected(this, new DisconnectCameraEventArgs {StillImageDevice = null, EosCamera = Camera});
        }

        public override void Close()
        {
            HaveLiveView = false;
            Camera.Error -= _camera_Error;
            Camera.Shutdown -= _camera_Shutdown;
            Camera.LiveViewPaused -= Camera_LiveViewPaused;
            Camera.LiveViewUpdate -= Camera_LiveViewUpdate;
            Camera.PictureTaken -= Camera_PictureTaken;
            Camera.PropertyChanged -= Camera_PropertyChanged;
            // this block the application
            //Camera.Dispose();
            Camera = null;
        }


        private void _camera_Error(object sender, EosExceptionEventArgs e)
        {
            try
            {
                Log.Error("Canon error", e.Exception);
                StaticHelper.Instance.SystemMessage = e.Exception.Message;
            }
            catch (Exception exception)
            {
                Log.Error("Error get camera error", exception);
                StaticHelper.Instance.SystemMessage = exception.Message;
            }
        }

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            //StillImageDevice = new StillImageDevice(deviceDescriptor.WpdId);
            //StillImageDevice.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
            //StillImageDevice.DeviceEvent += _stillImageDevice_DeviceEvent;
            Capabilities.Add(CapabilityEnum.Bulb);
            Capabilities.Add(CapabilityEnum.LiveView);

            IsConnected = true;
            return true;
        }


        private void InitShutterSpeed()
        {
            ShutterSpeed = new PropertyValue<long>();
            ShutterSpeed.Name = "ShutterSpeed";
            ShutterSpeed.ValueChanged += ShutterSpeed_ValueChanged;
            ReInitShutterSpeed();
        }

        private void ReInitShutterSpeed()
        {
            lock (Locker)
            {
                try
                {
                    ShutterSpeed.Clear();
                    var data = GetSettingsList(Edsdk.PropID_Tv);
                    foreach (KeyValuePair<uint, string> keyValuePair in _shutterTable)
                    {
                        if (data.Count > 0)
                        {
                            if (data.Contains((int) keyValuePair.Key))
                                ShutterSpeed.AddValues(keyValuePair.Value, keyValuePair.Key);
                        }
                        else
                        {
                            ShutterSpeed.AddValues(keyValuePair.Value, keyValuePair.Key);
                        }
                    }
                    ShutterSpeed.ReloadValues();
                    long value = Camera.GetProperty(Edsdk.PropID_Tv);
                    if (value == 0)
                    {
                        ShutterSpeed.IsEnabled = false;
                    }
                    else
                    {
                        ShutterSpeed.IsEnabled = true;
                        ShutterSpeed.SetValue(Camera.GetProperty(Edsdk.PropID_Tv), false);
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("EOS Shutter speed init", ex);
                }
            }
        }

        private void ShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_Tv, val);
            }
            catch (Exception exception)
            {
                Log.Error("Error set property sP", exception);
            }
        }

        #region F number

        private void InitFNumber()
        {
            FNumber = new PropertyValue<long> {IsEnabled = true, Name = "FNumber"};
            FNumber.ValueChanged += FNumber_ValueChanged;
            ReInitFNumber(true);
        }

        private void FNumber_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_Av, val);
            }
            catch (Exception exception)
            {
                Log.Error("Error set property AV", exception);
            }
        }

        private void ReInitFNumber(bool trigervaluchange)
        {
            try
            {
                var data = GetSettingsList(Edsdk.PropID_Av);
                long value = Camera.GetProperty(Edsdk.PropID_Av);
                bool shouldinit = FNumber.Values.Count == 0;

                if (data.Count > 0)
                    FNumber.Clear();

                if (shouldinit && data.Count == 0)
                {
                    foreach (KeyValuePair<int, string> keyValuePair in _apertureTable)
                    {
                        FNumber.AddValues(keyValuePair.Value, keyValuePair.Key);
                    }
                }
                else
                {
                    foreach (
                        KeyValuePair<int, string> keyValuePair in
                            _apertureTable.Where(keyValuePair => data.Count > 0).Where(
                                keyValuePair => data.Contains(keyValuePair.Key)))
                    {
                        FNumber.AddValues(keyValuePair.Value, keyValuePair.Key);
                    }
                }
                FNumber.ReloadValues();
                if (value == 0)
                {
                    FNumber.IsEnabled = false;
                }
                else
                {
                    FNumber.SetValue((int) value, false);
                    FNumber.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error set aperture ", ex);
            }
        }

        #endregion

        private void InitIso()
        {
            IsoNumber = new PropertyValue<long>();
            IsoNumber.ValueChanged += IsoNumber_ValueChanged;
            ReInitIso();
        }

        private void IsoNumber_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.PauseLiveview();
                Camera.SetProperty(Edsdk.PropID_ISOSpeed, val);
                Camera.ResumeLiveview();
            }
            catch (Exception exception)
            {
                Log.Debug("Error set ISO to camera", exception);
            }
        }

        public List<int> GetSettingsList(uint PropID)
        {
            if (Camera.Handle!= IntPtr.Zero)
            {
                    //get the list of possible values
                    Edsdk.EdsPropertyDesc des = new Edsdk.EdsPropertyDesc();
                    var Error = Edsdk.EdsGetPropertyDesc(Camera.Handle, PropID, out des);
                    return des.PropDesc.Take(des.NumElements).ToList();
            }
            return new List<int>();
        }

        private void ReInitIso()
        {
            try
            {
                var data = GetSettingsList(Edsdk.PropID_ISOSpeed);
                long value = Camera.GetProperty(Edsdk.PropID_ISOSpeed);
                bool shouldinit = IsoNumber.Values.Count == 0;
               
                if (data.Count > 0)
                    IsoNumber.Clear();

                if (shouldinit && data.Count == 0)
                {
                    foreach (KeyValuePair<uint, string> keyValuePair in _isoTable)
                    {
                        IsoNumber.AddValues(keyValuePair.Value, (int) keyValuePair.Key);
                    }
                }
                else
                {
                    foreach (
                        KeyValuePair<uint, string> keyValuePair in
                            _isoTable.Where(keyValuePair => data.Count > 0).Where(
                                keyValuePair => data.Contains((int) keyValuePair.Key)))
                    {
                        IsoNumber.AddValues(keyValuePair.Value, (int) keyValuePair.Key);
                    }
                }
                IsoNumber.ReloadValues();
                IsoNumber.SetValue((int)value, false);
                IsoNumber.IsEnabled = true;

            }
            catch (Exception ex)
            {
                Log.Debug("Error set iso ", ex);
            }
        }

        private void InitMode()
        {
           Mode = new PropertyValue<long>();
            Mode.ValueChanged += Mode_ValueChanged;
            try
            {
                foreach (KeyValuePair<long, string> keyValuePair in _exposureModeTable)
                {
                    Mode.AddValues(keyValuePair.Value, keyValuePair.Key);
                }
                Mode.ReloadValues();
                Mode.SetValue((uint) Camera.GetProperty(Edsdk.PropID_AEMode), false);
                Mode.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Log.Debug("Error set aperture ", ex);
            }
        }

        private void Mode_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_AEModeSelect, val);
                //Camera.SetProperty(Edsdk.PropID_AEMode, val);
                //Thread.Sleep(200);
                //ReInitFNumber(true);
                //ReInitShutterSpeed();
            }
            catch (Exception exception)
            {
                Log.Debug("Error set EC to camera", exception);
            }
        }

        private void InitEc()
        {
            var data = GetSettingsList(Edsdk.PropID_ExposureCompensation);
            ExposureCompensation = new PropertyValue<long>();
            ExposureCompensation.ValueChanged += ExposureCompensation_ValueChanged;
            try
            {
                foreach (KeyValuePair<uint, string> keyValuePair in _ec)
                {
                    if (data.Count == 0)
                    {
                        ExposureCompensation.AddValues(keyValuePair.Value, (int)keyValuePair.Key);                        
                    }
                    else
                    {
                        if (data.Contains((int)keyValuePair.Key))
                            ExposureCompensation.AddValues(keyValuePair.Value, (int)keyValuePair.Key);
                    }
                }
                ExposureCompensation.ReloadValues();
                ExposureCompensation.IsEnabled = true;
                ExposureCompensation.SetValue((int) Camera.GetProperty(Edsdk.PropID_ExposureCompensation), false);
            }
            catch (Exception exception)
            {
                Log.Debug("Error get EC", exception);
            }
        }

        private void InitWb()
        {
            var data = GetSettingsList(Edsdk.PropID_WhiteBalance);
            WhiteBalance = new PropertyValue<long>();
            WhiteBalance.ValueChanged += WhiteBalance_ValueChanged;
            try
            {
                foreach (KeyValuePair<uint, string> keyValuePair in _wbTable)
                {
                    if (data.Count > 0 && data.Contains((int) keyValuePair.Key))
                        WhiteBalance.AddValues(keyValuePair.Value, (int) keyValuePair.Key);
                }
                WhiteBalance.ReloadValues();
                WhiteBalance.IsEnabled = true;
                WhiteBalance.SetValue((long) Camera.GetProperty(Edsdk.PropID_WhiteBalance), false);
            }
            catch (Exception exception)
            {
                Log.Debug("Error get EC", exception);
            }
        }

        private void WhiteBalance_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_WhiteBalance, val);
            }
            catch (Exception exception)
            {
                Log.Debug("Error set WB to camera", exception);
            }
        }

        private void InitMetering()
        {
            var data = GetSettingsList(Edsdk.PropID_MeteringMode);
            ExposureMeteringMode = new PropertyValue<long>();
            ExposureMeteringMode.ValueChanged += ExposureMeteringMode_ValueChanged;
            try
            {
                foreach (KeyValuePair<uint, string> keyValuePair in _meteringTable)
                {
                    if (data.Contains((int) keyValuePair.Key))
                        ExposureMeteringMode.AddValues(keyValuePair.Value, (int) keyValuePair.Key);
                }
                ExposureMeteringMode.ReloadValues();
                ExposureMeteringMode.IsEnabled = true;
                ExposureMeteringMode.SetValue((int) Camera.GetProperty(Edsdk.PropID_MeteringMode), false);
            }
            catch (Exception exception)
            {
                Log.Debug("Error get metering", exception);
            }
        }

        private void InitFocus()
        {
            var data = GetSettingsList(Edsdk.PropID_AFMode);
            FocusMode = new PropertyValue<long>();
            FocusMode.ValueChanged += FocusMode_ValueChanged;
            try
            {
                foreach (KeyValuePair<uint, string> keyValuePair in _focusModeTable)
                {
                    if (data.Contains((int) keyValuePair.Key))
                        FocusMode.AddValues(keyValuePair.Value, (int) keyValuePair.Key);
                }
                FocusMode.ReloadValues();
                FocusMode.IsEnabled = true;
                FocusMode.SetValue((int) Camera.GetProperty(Edsdk.PropID_AFMode), false);
            }
            catch (Exception exception)
            {
                Log.Debug("Error get focus mode", exception);
            }
        }

        private void FocusMode_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_AFMode, val);
            }
            catch (Exception exception)
            {
                Log.Debug("Error set focus mode to camera", exception);
            }
        }

        private void ExposureMeteringMode_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_MeteringMode, val);
            }
            catch (Exception exception)
            {
                Log.Debug("Error set metering mode to camera", exception);
            }
        }

        private void ExposureCompensation_ValueChanged(object sender, string key, long val)
        {
            try
            {
                Camera.SetProperty(Edsdk.PropID_ExposureCompensation, val);
            }
            catch (Exception exception)
            {
                Log.Debug("Error set EC to camera", exception);
            }
        }

        public override void CapturePhoto()
        {
            Log.Debug("EOS capture start");
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                if (Camera.IsInHostLiveViewMode)
                {
                    if (Camera.ImageQuality.SecondaryImageFormat != EosImageFormat.Unknown)
                    {
                        throw new Exception("RAW+JPG capture in live view not supported");
                    }
                    Camera.PauseLiveview();
                    Camera.ResetShutterButton();
                    ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_TakePicture));
                    Camera.ResetShutterButton();
                }
                else
                {
                    Camera.TakePicture();
                }
            }
            catch (COMException comException)
            {
                IsBusy = false;
                Camera.ResumeLiveview();
                ErrorCodes.GetException(comException);
            }
            catch
            {
                Camera.ResumeLiveview();
                IsBusy = false;
                throw;
            }
            finally
            {
                Monitor.Exit(Locker);
            }
            Log.Debug("EOS capture end");
        }

        private uint ResetShutterButton()
        {
            Camera.SendCommand(Edsdk.CameraCommand_DoEvfAf, 0);
            //ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_DoEvfAf, 0));
            return Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton,
                (int) Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF);
        }

        public void PressButton()
        {
            ResetShutterButton();
            ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Completely));
        }

        public void PressHalfButton()
        {
            ResetShutterButton();
            ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Halfway));
        }

        public void ReleaseButton()
        {
           Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton,(int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF);
        }


        public override void CapturePhotoNoAf()
        {
            Log.Debug("EOS capture start");
            Monitor.Enter(Locker);
            try
            {
                if (Camera.IsInHostLiveViewMode && Camera.ImageQuality.SecondaryImageFormat != EosImageFormat.Unknown)
                {
                    throw new Exception("RAW+JPG capture in live view not supported");
                }
                IsBusy = true;
                Camera.PauseLiveview();
                ErrorCodes.GetCanonException(ResetShutterButton());
                ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Completely_NonAF));
                ResetShutterButton();

                //if (Camera.IsInHostLiveViewMode)
                //{
                //    Camera.TakePictureInLiveview();
                //}
                //else
                //{
                //    Camera.TakePicture();
                //}
            }
            catch (COMException comException)
            {
                IsBusy = false;
                ErrorCodes.GetException(comException);
            }
            catch
            {
                IsBusy = false;
                throw;
            }
            finally
            {
                Monitor.Exit(Locker);
            }
            Log.Debug("EOS capture end");
        }

        public override void StartBulbMode()
        {
            Camera.BulbStart();
        }

        public override void EndBulbMode()
        {
            Camera.BulbEnd();
        }

        public override void StartRecordMovie()
        {
            lock (Locker)
            {
                _recording = true;
                ResetShutterButton();
                Camera.StartRecord();
            }
        }

        public override void StopRecordMovie()
        {
            lock (Locker)
            {
                Camera.StopRecord();
                _recording = false;
            }
        }

        public override void Focus(int x, int y)
        {
            lock (Locker)
            {
                ResetShutterButton();
                if (_liveViewImageData != null)
                {
                    x -= (_liveViewImageData.ZommBounds.Width/2);
                    y -= (_liveViewImageData.ZommBounds.Height/2);
                }
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                Camera.LiveViewqueue.Enqueue(
                    () =>
                        Camera.SetPropertyIntegerArrayData(Edsdk.PropID_Evf_ZoomPosition,
                            new uint[] {(uint) x, (uint) y}));
            }
        }

        public override LiveViewData GetLiveViewImage()
        {

                LiveViewData viewData = new LiveViewData();
                if (Monitor.TryEnter(Locker, 10))
                {
                    try
                    {
                        if (Camera == null)
                            return viewData;
                        Camera.DownloadEvf();
                        if (_liveViewImageData != null)
                        {
                            //DeviceReady();
                            viewData.HaveFocusData = true;
                            viewData.ImageDataPosition = 0;
                            viewData.ImageData = _liveViewImageData.ImageData;
                            viewData.ImageHeight = _liveViewImageData.ImageSize.Height;
                            viewData.ImageWidth = _liveViewImageData.ImageSize.Width;
                            viewData.LiveViewImageHeight = 100;
                            viewData.LiveViewImageWidth = 100;
                            viewData.FocusX = _liveViewImageData.ZommBounds.X +
                                              (_liveViewImageData.ZommBounds.Width / 2);
                            viewData.FocusY = _liveViewImageData.ZommBounds.Y +
                                              (_liveViewImageData.ZommBounds.Height / 2);
                            viewData.FocusFrameXSize = _liveViewImageData.ZommBounds.Width;
                            viewData.FocusFrameYSize = _liveViewImageData.ZommBounds.Height;
                            viewData.MovieIsRecording = _recording;
                        }
                    }
                    catch (Exception)
                    {
                        //Log.Error("Error get live view image ", e);
                    }
                    finally
                    {
                        Monitor.Exit(Locker);
                    }
                }
            return viewData;
        }

        public override void StartLiveView()
        {
            _recording = false;
            ResetShutterButton();
            //if (!Camera.IsInLiveViewMode) 
            Camera.StartLiveView(EosLiveViewAutoFocus.LiveMode);
        }

        public void StartLiveViewCamera()
        {
            _recording = false;
            ResetShutterButton();
            //if (!Camera.IsInLiveViewMode) 
            Camera.StartLiveViewCamera();
        }

        public override void StopLiveView()
        {
            if (Camera == null)
                return;
            ResetShutterButton();
            //if (Camera.IsInLiveViewMode)
            Camera.StopLiveView();
        }

        public override void AutoFocus()
        {
            ResetShutterButton();
            Camera.PauseLiveview();
            
            try
            {
                ErrorCodes.GetCanonException(Camera.SendCommand(Edsdk.CameraCommand_DoEvfAf, 1));
         //       ErrorCodes.GetCanonException(
         //Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton,
         //                 (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF));
         //       ErrorCodes.GetCanonException(
         //           Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton,
         //               (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Halfway));
         //       ErrorCodes.GetCanonException(
         //                Camera.SendCommand(Edsdk.CameraCommand_PressShutterButton,
         //                                 (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF));

            }
            finally
            {
                Camera.ResumeLiveview();                
            }
        }

        public override int Focus(int step)
        {
            ResetShutterButton();

            int focus = 0;
            var res = Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf,
                (int) (step < 0 ? Edsdk.EvfDriveLens_Near1 : Edsdk.EvfDriveLens_Far1));
            //if (i%10 == 0)
            //    Camera.DownloadEvfInternal();
            if (res == Edsdk.EDS_ERR_OK)
                focus += step < 0 ? -1 : 1;

            //Camera.SendCommand(Edsdk.CameraCommand_DoEvfAf, 0);
            return focus;
        }

        public override void Focus(FocusDirection direction, FocusAmount amount)
        {
            ResetShutterButton();
            switch (direction)
            {
                case FocusDirection.Far:
                    switch (amount)
                    {
                        case FocusAmount.Small:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int) Edsdk.EvfDriveLens_Far1);
                            break;
                        case FocusAmount.Medium:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int) Edsdk.EvfDriveLens_Far2);
                            break;
                        case FocusAmount.Large:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int) Edsdk.EvfDriveLens_Far3);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(amount), amount, null);
                    }
                    break;
                case FocusDirection.Near:
                    switch (amount)
                    {
                        case FocusAmount.Small:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int)Edsdk.EvfDriveLens_Near1);
                            break;
                        case FocusAmount.Medium:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int)Edsdk.EvfDriveLens_Near2);
                            break;
                        case FocusAmount.Large:
                            Camera.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int)Edsdk.EvfDriveLens_Near3);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(amount), amount, null);
                    }
                    break;

            }
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            if (deviceObject.Handle is IntPtr)
            {
                Camera.DeleteItem((IntPtr)deviceObject.Handle);
            }
            return true;
        }

        public override void ReleaseResurce(object o)
        {
            if (o is IntPtr)
            {
                Edsdk.EdsRelease((IntPtr)o);
            }
        }

        public override void TransferFileThumb(object o, string filename)
        {
            base.TransferFileThumb(o, filename);
        }

        public override void TransferFile(object o, string filename)
        {
            lock (Locker)
            {
                IsBusy = true;
                if (o is IntPtr)
                {
                    Log.Debug("Pointer file transfer started");
                    try
                    {
                        IsBusy = true;
                        Camera.PauseLiveview();
                        Log.Debug("Camera.PauseLiveview();");
                        var transporter = new EosImageTransporter();
                        transporter.ProgressEvent += (i) => TransferProgress = (uint) i;
                        Log.Debug("TransportAsFileName");
                        transporter.TransportAsFileName((IntPtr) o, filename, Camera.Handle);
                        Log.Debug("TransportAsFileName DONE");
                        Camera.ResumeLiveview();
                        Log.Debug("Camera.ResumeLiveview(); DONE");
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Error transfer memory file", exception);
                        File.Delete(filename);
                    }
                }
                EosFileImageEventArgs file = o as EosFileImageEventArgs;
                if (file != null)
                {
                    Log.Debug("File transfer started");
                    try
                    {
                        if (File.Exists(file.ImageFilePath))
                        {
                            File.Copy(file.ImageFilePath, filename, true);
                            File.Delete(file.ImageFilePath);
                        }
                        else
                        {
                            Log.Error("Base file not found " + file.ImageFilePath);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Transfer error ", exception);
                    }
                }
                EosMemoryImageEventArgs memory = o as EosMemoryImageEventArgs;
                if (memory != null)
                {
                    Log.Debug("Memory file transfer started");
                    try
                    {
                        using (FileStream fileStream = File.Create(filename, (int) memory.ImageData.Length))
                        {
                            fileStream.Write(memory.ImageData, 0, memory.ImageData.Length);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Error transfer memory file", exception);
                    }
                }
                string fil = o as string;
                if (fil != null)
                {
                    GetFile(fil, filename);
                }
            }
        }

        public override string GetProhibitionCondition(OperationEnum operationEnum)
        {
            switch (operationEnum)
            {
                case OperationEnum.Capture:
                    break;
                case OperationEnum.RecordMovie:
                    if (Camera.GetProperty(Edsdk.PropID_Record) == 4)
                        return "LabelRecordInProgres";
                    if (Camera.LiveViewDevice==EosLiveViewDevice.None)
                        return "LabelWrongLiveViewType";
                    break;
                case OperationEnum.AutoFocus:
                    break;
                case OperationEnum.ManualFocus:
                    break;
                case OperationEnum.LiveView:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("operationEnum", operationEnum, null);
            }
            return "";
        }

        //private bool ArrayContainValue(IEnumerable<int> data, uint value)
        //{
        //    return ArrayContainValue(data, (int) value);
        //}

        //private bool ArrayContainValue(IEnumerable<int> data, int value)
        //{
        //    return data.Any(i => i == (int) value);
        //}

        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            List<DeviceObject> list = new List<DeviceObject>();
            int count = 0;
            Edsdk.EdsGetChildCount(Camera.Handle, out count);
            for (int i = 0; i < count; i++)
            {
                IntPtr volumePtr;
                Edsdk.EdsGetChildAtIndex(Camera.Handle, i, out volumePtr);
                Edsdk.EdsVolumeInfo vinfo;
                Edsdk.EdsGetVolumeInfo(volumePtr, out vinfo);
                if (vinfo.StorageType == 0)
                    throw new Exception("No memory card inserted");
                //ignore the HDD
                if (vinfo.szVolumeLabel != "HDD")
                {
                    //get all child entries on this volume
                    list.AddRange(GetChildren(volumePtr, loadThumbs));
                }
                Edsdk.EdsRelease(volumePtr);
            }
            return new AsyncObservableCollection<DeviceObject>(list);
        }

        private List<DeviceObject> GetChildren(IntPtr ptr, bool loadThumbs)
        {
            int ChildCount;
            //get children of first pointer
            List<DeviceObject> list = new List<DeviceObject>();
            Edsdk.EdsGetChildCount(ptr, out ChildCount);
            if (ChildCount > 0)
            {
                //if it has children, create an array of entries
                for (int i = 0; i < ChildCount; i++)
                {
                    IntPtr ChildPtr;
                    //get children of children
                    Edsdk.EdsGetChildAtIndex(ptr, i, out ChildPtr);
                    //get the information about this children
                    Edsdk.EdsDirectoryItemInfo ChildInfo;
                    Edsdk.EdsGetDirectoryItemInfo(ChildPtr, out ChildInfo);

                    if (ChildInfo.isFolder == 0)
                    {
                        //if it's not a folder, create thumbnail and safe it to the entry
                        if (loadThumbs)
                        {
                            IntPtr stream;
                            Edsdk.EdsCreateMemoryStream(0, out stream);
                            Edsdk.EdsDownloadThumbnail(ChildPtr, stream);
                            list.Add(new DeviceObject()
                            {
                                FileName = ChildInfo.szFileName,
                                ThumbData = GetImage(stream, Edsdk.EdsImageSource.Thumbnail),
                                Handle = ChildInfo.szFileName
                            });
                            if (stream != IntPtr.Zero)
                                Edsdk.EdsRelease(stream);
                        }
                        else
                        {
                            list.Add(new DeviceObject()
                            {
                                FileName = ChildInfo.szFileName,
                                Handle = ChildPtr
                            });
                        }
                    }
                    else
                    {
                        //if it's a folder, check for children with recursion
                        list.AddRange(GetChildren(ChildPtr, loadThumbs));
                        Edsdk.EdsRelease(ChildPtr);
                    }
                    //release current children

                }
            }
            return list;
        }

        public void GetFile(string fileName,string outFileName)
        {
            int count = 0;
            Edsdk.EdsGetChildCount(Camera.Handle, out count);
            for (int i = 0; i < count; i++)
            {
                IntPtr volumePtr;
                Edsdk.EdsGetChildAtIndex(Camera.Handle, i, out volumePtr);
                Edsdk.EdsVolumeInfo vinfo;
                Edsdk.EdsGetVolumeInfo(volumePtr, out vinfo);
                //ignore the HDD
                if (vinfo.szVolumeLabel != "HDD")
                {
                    GetFile(volumePtr, fileName, outFileName);
                }
                Edsdk.EdsRelease(volumePtr);
            }
        }

        private void GetFile(IntPtr ptr,string fileName, string outfileName)
        {
            int ChildCount;
            //get children of first pointer
            List<DeviceObject> list = new List<DeviceObject>();
            Edsdk.EdsGetChildCount(ptr, out ChildCount);
            if (ChildCount > 0)
            {
                //if it has children, create an array of entries
                for (int i = 0; i < ChildCount; i++)
                {
                    IntPtr ChildPtr;
                    //get children of children
                    Edsdk.EdsGetChildAtIndex(ptr, i, out ChildPtr);
                    //get the information about this children
                    Edsdk.EdsDirectoryItemInfo ChildInfo;
                    Edsdk.EdsGetDirectoryItemInfo(ChildPtr, out ChildInfo);

                    if (ChildInfo.isFolder == 0)
                    {
                        if(ChildInfo.szFileName==fileName)
                        {
                            Camera._transporter.TransportAsFileName(ChildPtr, outfileName, Camera.Handle);
                        }
                    }
                    else
                    {
                        //if it's a folder, check for children with recursion
                        GetFile(ChildPtr, fileName, outfileName);
                    }
                    //release current children
                    Edsdk.EdsRelease(ChildPtr);
                }
            }
        }

        private byte[] GetImage(IntPtr img_stream, Edsdk.EdsImageSource imageSource)
        {
            IntPtr stream = IntPtr.Zero;
            IntPtr img_ref = IntPtr.Zero;
            IntPtr streamPointer = IntPtr.Zero;
            Edsdk.EdsImageInfo imageInfo;
            Edsdk.EdsSize outputSize = new Edsdk.EdsSize();
            Byte[] buffer;
            Byte temp;

            //create reference to image
            Edsdk.EdsCreateImageRef(img_stream, out img_ref);
            //get information about image
            Edsdk.EdsGetImageInfo(img_ref, imageSource, out imageInfo);

            //calculate size, stride and buffersize
            outputSize.width = imageInfo.EffectiveRect.width;
            outputSize.height = imageInfo.EffectiveRect.height;
            int Stride = ((outputSize.width*3) + 3) & ~3;
            uint bufferSize = (uint) (outputSize.height*Stride);

            //Init buffer
            buffer = new Byte[bufferSize];
            //Create memory stream to buffer
            Edsdk.EdsCreateMemoryStreamFromPointer(buffer, bufferSize, out stream);
            //copy image into buffer
            Edsdk.EdsGetImage(img_ref, imageSource, Edsdk.EdsTargetImageType.RGB, imageInfo.EffectiveRect, outputSize,
                              stream);

            //makes RGB out of BGR
            if (outputSize.width%4 == 0)
            {
                for (int t = 0; t < bufferSize; t += 3)
                {
                    temp = buffer[t];
                    buffer[t] = buffer[t + 2];
                    buffer[t + 2] = temp;
                }
            }
            else
            {
                int Padding = Stride - (outputSize.width*3);
                for (int y = outputSize.height - 1; y > -1; y--)
                {
                    int RowStart = (outputSize.width*3)*y;
                    int TargetStart = Stride*y;

                    Array.Copy(buffer, RowStart, buffer, TargetStart, outputSize.width*3);

                    for (int t = TargetStart; t < TargetStart + (outputSize.width*3); t += 3)
                    {
                        temp = buffer[t];
                        buffer[t] = buffer[t + 2];
                        buffer[t + 2] = temp;
                    }
                }
            }

            //create pointer to image data
            Edsdk.EdsGetPointer(stream, out streamPointer);
            //Release all ressources
            Edsdk.EdsRelease(img_stream);
            Edsdk.EdsRelease(img_ref);
            Edsdk.EdsRelease(stream);
            try
            {
                var bitmap = new Bitmap(outputSize.width, outputSize.height, Stride, PixelFormat.Format24bppRgb,
                                        streamPointer);
                using (MemoryStream memostream = new MemoryStream())
                {
                    bitmap.Save(memostream, ImageFormat.Bmp);
                    memostream.Close();

                    return memostream.ToArray();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error loading image ", exception);
            }
            return new byte[0];
        }

        public override void FormatStorage(object storageId)
        {
            try
            {
                lock (Locker)
                {
                    Camera.Lock();
                    int count = 0;
                    Log.Debug("EdsGetChildCount");
                    if (Edsdk.EdsGetChildCount(Camera.Handle, out count) != Edsdk.EDS_ERR_OK)
                        throw new DeviceException("Error EdsGetChildCount");
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr volumePtr;
                        Log.Debug("EdsGetChildAtIndex");
                        if (Edsdk.EdsGetChildAtIndex(Camera.Handle, i, out volumePtr) != Edsdk.EDS_ERR_OK)
                            throw new DeviceException("Error EdsGetChildAtIndex");
                        Edsdk.EdsVolumeInfo vinfo;
                        Log.Debug("EdsGetVolumeInfo");
                        if (Edsdk.EdsGetVolumeInfo(volumePtr, out vinfo) != Edsdk.EDS_ERR_OK)
                            throw new DeviceException("Error EdsGetVolumeInfo");
                        //ignore the HDD
                        if (vinfo.szVolumeLabel != "HDD")
                        {
                            Log.Debug("EdsFormatVolume");
                            if (Edsdk.EdsFormatVolume(volumePtr) != Edsdk.EDS_ERR_OK)
                                throw new DeviceException("Error EdsFormatVolume");
                        }
                        Log.Debug("EdsRelease");
                        if (Edsdk.EdsRelease(volumePtr) != Edsdk.EDS_ERR_OK)
                            throw new DeviceException("Error EdsRelease");
                    }
                }
            }
            finally
            {
                Camera.Unlock();
            }
        }

        public override string ToStringCameraData()
        {
            /* Canon SDK adds a EosCamera object
                        BatteryLevel (long) and BatteryQuality (string)
                        Copyright (string)
                        FirmwareVersion (string)
                        ImageQuality
                            PrimaryCompressLevel
                            PrimateImageFormat
                            PrimateImageSize
                        IsInLiveViewMode (bool)
            */

            StringBuilder c = new StringBuilder(base.ToStringCameraData() + "\n\tType..................Canon (SDK)");

            if (Camera != null)
            {
                c.AppendFormat("\n\tFirmware version......{0}", Camera.FirmwareVersion);
                c.AppendFormat("\n\tDate/Time.............{0}", DateTime.ToString());
                c.AppendFormat("\n\tBattery...............{0,3}%, quality {1}, (note base class value is this + 20)", Camera.BatteryLevel, Camera.BatteryQuality);
                c.AppendFormat("\n\tIs in LiveView Mode...{0}", Camera.IsInLiveViewMode ? "Yes" : "No");
                c.Append("\n\tImage Quality - Primary");
                c.AppendFormat("\n\t  Format..............{0}", Camera.ImageQuality.PrimaryImageFormat);
                c.AppendFormat("\n\t  Size................{0}", Camera.ImageQuality.PrimaryImageSize);
                c.AppendFormat("\n\t  Compression.........{0}", Camera.ImageQuality.PrimaryCompressLevel);
                if (!Camera.ImageQuality.SecondaryImageFormat.Equals(EosImageFormat.Unknown))
                {
                    c.Append("\n\tImage Quality - Secondary");
                    c.AppendFormat("\n\t  Format..............{0}", Camera.ImageQuality.SecondaryImageFormat);
                    c.AppendFormat("\n\t  Size................{0}", Camera.ImageQuality.SecondaryImageSize);
                    c.AppendFormat("\n\t  Compression.........{0}", Camera.ImageQuality.SecondaryCompressLevel);
                }
                c.AppendFormat("\n\tArtist................{0}", Camera.Artist);
                c.AppendFormat("\n\tCopyright (c).........{0}", Camera.Copyright);
                c.AppendFormat("\n\tDepth Preview.........{0}", Camera.DepthOfFieldPreview ? "Yes" : "No");
                c.AppendFormat("\n\tWhite Balance.........{0}", Camera.WhiteBalance);
            }
            return c.ToString();
        }
    }
}
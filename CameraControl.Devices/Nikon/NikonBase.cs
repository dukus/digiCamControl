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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Xml;
using PortableDeviceLib;
using PortableDeviceLib.Model;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.Devices.Nikon
{
    /// <summary>
    /// Base Nikon driver
    /// </summary>
    public class NikonBase : BaseMTPCamera
    {
        public const uint CONST_CMD_AfDrive = 0x90C1;
        public const uint CONST_CMD_StartLiveView = 0x9201;
        public const uint CONST_CMD_EndLiveView = 0x9202;
        public const uint CONST_CMD_GetLiveViewImage = 0x9203;
        public const uint CONST_CMD_InitiateCaptureRecInMedia = 0x9207;
        public const uint CONST_CMD_AfAndCaptureRecInSdram = 0x90CB;
        public const uint CONST_CMD_InitiateCaptureRecInSdram = 0x90C0;
        public const uint CONST_CMD_ChangeAfArea = 0x9205;
        public const uint CONST_CMD_MfDrive = 0x9204;
        public const uint CONST_CMD_GetLargeThumb = 0x90C4;

        public const uint CONST_CMD_GetEvent = 0x90C7;

        public const uint CONST_CMD_DeviceReady = 0x90C8;
        public const uint CONST_CMD_ChangeCameraMode = 0x90C2;
        public const uint CONST_CMD_StartMovieRecInCard = 0x920A;
        public const uint CONST_CMD_EndMovieRec = 0x920B;

        public const uint CONST_PROP_ImageSize = 0x5003;
        public const uint CONST_PROP_Fnumber = 0x5007;
        public const uint CONST_PROP_MovieFnumber = 0xD1A9;
        public const uint CONST_PROP_ExposureIndex = 0x500F;
        public const uint CONST_PROP_ExposureIndexEx = 0xD0B4;
        public const uint CONST_PROP_MovieExposureIndex = 0xD1AA;
        public const uint CONST_PROP_ExposureTime = 0x500D;
        public const uint CONST_PROP_ShutterSpeed = 0xD100;
        public const uint CONST_PROP_MovieShutterSpeed = 0xD1A8;
        public const uint CONST_PROP_WhiteBalance = 0x5005;
        public const uint CONST_PROP_ExposureProgramMode = 0x500E;
        public const uint CONST_PROP_ExposureBiasCompensation = 0x5010;
        public const uint CONST_PROP_MovieExposureBiasCompensation = 0xD1AB;
        public const uint CONST_PROP_DateTime = 0x5011;
        public const uint CONST_PROP_LiveViewImageZoomRatio = 0xD1A3;
        public const uint CONST_PROP_AFModeSelect = 0xD161;
        public const uint CONST_PROP_AfModeAtLiveView = 0xD061;
        public const uint CONST_PROP_CompressionSetting = 0x5004;
        public const uint CONST_PROP_ExposureMeteringMode = 0x500B;
        public const uint CONST_PROP_FocusMode = 0x500A;
        public const uint CONST_PROP_LiveViewStatus = 0xD1A2;
        public const uint CONST_PROP_LiveViewSelector = 0xD1A6;
        public const uint CONST_PROP_ExposureIndicateStatus = 0xD1B1;
        public const uint CONST_PROP_RecordingMedia = 0xD10B;
        public const uint CONST_PROP_NoiseReduction = 0xD06B;
        public const uint CONST_PROP_ApplicationMode = 0xD1F0;
        public const uint CONST_PROP_RawCompressionType = 0xD016;
        public const uint CONST_PROP_RawCompressionBitMode = 0xD149;
        public const uint CONST_PROP_ActivePicCtrlItem = 0xD200;
        public const uint CONST_PROP_ColorSpace = 0xD032;
        public const uint CONST_PROP_WbTuneFluorescentType = 0xD14F;
        public const uint CONST_PROP_WbColorTemp = 0xD01E;
        public const uint CONST_PROP_ISOAutoHighLimit = 0xD183;
        public const uint CONST_PROP_FlashCompensation = 0xD124;

        public const uint CONST_Event_DevicePropChanged = 0x4006;
        public const uint CONST_Event_StoreFull = 0x400A;
        public const uint CONST_Event_CaptureComplete = 0x400D;
        public const uint CONST_Event_CaptureCompleteRecInSdram = 0xC102;
        public const uint CONST_Event_ObsoleteEvent = 0xC104;


        protected Dictionary<uint, string> _isoTable = new Dictionary<uint, string>()
                                                           {
                                                               {0x0064, "100"},
                                                               {0x007D, "125"},
                                                               {0x00A0, "160"},
                                                               {0x00C8, "200"},
                                                               {0x00FA, "250"},
                                                               {0x0140, "320"},
                                                               {0x0190, "400"},
                                                               {0x01F4, "500"},
                                                               {0x0280, "640"},
                                                               {0x0320, "800"},
                                                               {0x03E8, "1000"},
                                                               {0x04E2, "1250"},
                                                               {0x0640, "1600"},
                                                               {0x07D0, "2000"},
                                                               {0x09C4, "2500"},
                                                               {0x0C80, "3200"},
                                                               {0x0FA0, "4000"},
                                                               {0x1388, "5000"},
                                                               {0x1900, "6400"},
                                                               {0x1F40, "Hi 0.3"},
                                                               {0x2710, "Hi 0.7"},
                                                               {0x3200, "Hi 1"},
                                                               {0x6400, "Hi 2"},
                                                           };

        protected Dictionary<byte, string> _autoIsoTable = new Dictionary<byte, string>()
                                                           {
                                                               {0, "200"},
                                                               {1, "400"},
                                                               {2, "800"},
                                                               {3, "1600"},
                                                               {4, "3200"},
                                                               {5, "6400"},
                                                               {6, "Hi 1.7"},
                                                               {7, "Hi 2"},
                                                           };

        protected Dictionary<uint, string> _shutterTable = new Dictionary<uint, string>
                                                               {
                                                                   {1, "1/6400"},
                                                                   {2, "1/4000"},
                                                                   {3, "1/3200"},
                                                                   {4, "1/2500"},
                                                                   {5, "1/2000"},
                                                                   {6, "1/1600"},
                                                                   {8, "1/1250"},
                                                                   {10, "1/1000"},
                                                                   {12, "1/800"},
                                                                   {13, "1/750"},
                                                                   {15, "1/640"},
                                                                   {20, "1/500"},
                                                                   {25, "1/400"},
                                                                   {28, "1/350"},
                                                                   {31, "1/320"},
                                                                   {40, "1/250"},
                                                                   {50, "1/200"},
                                                                   {55, "1/180"},
                                                                   {62, "1/160"},
                                                                   {80, "1/125"},
                                                                   {100, "1/100"},
                                                                   {111, "1/90"},
                                                                   {125, "1/80"},
                                                                   {166, "1/60"},
                                                                   {200, "1/50"},
                                                                   {222, "1/45"},
                                                                   {250, "1/40"},
                                                                   {333, "1/30"},
                                                                   {400, "1/25"},
                                                                   {500, "1/20"},
                                                                   {666, "1/15"},
                                                                   {769, "1/13"},
                                                                   {1000, "1/10"},
                                                                   {1250, "1/8"},
                                                                   {1666, "1/6"},
                                                                   {2000, "1/5"},
                                                                   {2500, "1/4"},
                                                                   {3333, "1/3"},
                                                                   {4000, "1/2.5"},
                                                                   {5000, "1/2"},
                                                                   {6250, "1/1.6"},
                                                                   {6666, "1/1.5"},
                                                                   {7692, "1/1.3"},
                                                                   {10000, "1s"},
                                                                   {13000, "1.3s"},
                                                                   {15000, "1.5s"},
                                                                   {16000, "1.6s"},
                                                                   {20000, "2s"},
                                                                   {25000, "2.5s"},
                                                                   {30000, "3s"},
                                                                   {40000, "4s"},
                                                                   {50000, "5s"},
                                                                   {60000, "6s"},
                                                                   {80000, "8s"},
                                                                   {100000, "10s"},
                                                                   {130000, "13s"},
                                                                   {150000, "15s"},
                                                                   {200000, "20s"},
                                                                   {250000, "25s"},
                                                                   {300000, "30s"},
                                                                   {0xFFFFFFFF, "Bulb"},
                                                               };

        private Dictionary<int, string> _exposureModeTable = new Dictionary<int, string>()
                                                                   {
                                                                       {1, "M"},
                                                                       {2, "P"},
                                                                       {3, "A"},
                                                                       {4, "S"},
                                                                       {0x8010, "[Scene mode] AUTO"},
                                                                       {0x8011, "[Scene mode] Portrait"},
                                                                       {0x8012, "[Scene mode] Landscape"},
                                                                       {0x8013, "[Scene mode] Close up"},
                                                                       {0x8014, "[Scene mode] Sports"},
                                                                       {0x8016, "[Scene mode] Flash prohibition AUTO"},
                                                                       {0x8017, "[Scene mode] Child"},
                                                                       {0x8018, "[Scene mode] SCENE"},
                                                                       {0x8019, "[EffectMode] EFFECTS"},
                                                                   };

        private Dictionary<uint, string> _wbTable = new Dictionary<uint, string>()
                                                            {
                                                                {2, "Auto"},
                                                                {4, "Daylight"},
                                                                {5, "Fluorescent"},
                                                                {6, "Incandescent"},
                                                                {7, "Flash"},
                                                                {0x8010, "Cloudy"},
                                                                {0x8011, "Shade"},
                                                                {0x8012, "Kelvin"},
                                                                {0x8013, "Custom"},
                                                                {0x8016, "Natural Light Auto"}
                                                            };

        protected Dictionary<int, string> _csTable = new Dictionary<int, string>()
                                                         {
                                                             {0, "JPEG (BASIC)"},
                                                             {1, "JPEG (NORMAL)"},
                                                             {2, "JPEG (FINE)"},
                                                             {3, "TIFF (RGB)"},
                                                             {4, "RAW"},
                                                             {5, "RAW + JPEG (BASIC)"},
                                                             {6, "RAW + JPEG (NORMAL)"},
                                                             {7, "RAW + JPEG (FINE)"}
                                                         };

        private Dictionary<int, string> _emmTable = new Dictionary<int, string>
                                                        {
                                                            {2, "Center-weighted metering"},
                                                            {3, "Multi-pattern metering"},
                                                            {4, "Spot metering"},
                                                            {0x8010, "Highlight-weighted"}
                                                        };

        private Dictionary<uint, string> _fmTable = new Dictionary<uint, string>()
                                                        {
                                                            {1, "[M] Manual focus"},
                                                            {0x8010, "[S] Single AF servo"},
                                                            {0x8011, "[C] Continuous AF servo"},
                                                            {0x8012, "[A] AF servo mode automatic switching"},
                                                            {0x8013, "[F] Constant AF servo"},
                                                        };

        public bool LiveViewOn
        {
            get { return _liveViewOn; }
            set
            {
                _liveViewOn = value;
                FocusMode = LiveViewOn ? LiveViewFocusMode : NormalFocusMode;
                SetMovieMode();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera swich is in movie mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [live view is in movie mode]; otherwise, <c>false</c>.
        /// </value>
        public bool LiveViewMovieOn
        {
            get { return _liveViewMovieOn; }
            set
            {
                _liveViewMovieOn = value;
                SetMovieMode();
            }
        }

        public PropertyValue<long> LiveViewFocusMode { get; set; }
        public PropertyValue<long> NormalFocusMode { get; set; }

        public PropertyValue<long> NormalFNumber { get; set; }
        public PropertyValue<long> MovieFNumber { get; set; }

        public PropertyValue<long> NormalIsoNumber { get; set; }
        public PropertyValue<long> MovieIsoNumber { get; set; }

        public PropertyValue<long> NormalShutterSpeed { get; set; }
        public PropertyValue<long> MovieShutterSpeed { get; set; }

        public PropertyValue<long> NormalExposureCompensation { get; set; }
        public PropertyValue<long> MovieExposureCompensation { get; set; }

        public NikonBase()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            SlowDownEventTimer();
        }

        private void SetMovieMode()
        {
            if (LiveViewOn && LiveViewMovieOn)
            {
                FNumber =  MovieFNumber ;
                IsoNumber = MovieIsoNumber;
                ShutterSpeed = MovieShutterSpeed;
                ExposureCompensation = MovieExposureCompensation;
            }
            else
            {
                FNumber = NormalFNumber;
                IsoNumber = NormalIsoNumber;
                ShutterSpeed = NormalShutterSpeed;
                ExposureCompensation = NormalExposureCompensation;
            }
        }

        private void SlowDownEventTimer()
        {
            _timer.Interval = 1000/10;
        }

        private void SpeedUpEventTimer()
        {
            _timer.Interval = 1000/10;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timer.Stop();
                lock (Locker)
                {
                   ThreadPool.QueueUserWorkItem(GetEvent);
                }
            }
            catch (Exception)
            {
            }
            //_timer.Start();
        }

        public PictureControl GetPictureControl(byte slotnum)
        {
            PictureControl control = new PictureControl();
            MTPDataResponse result = ExecuteReadDataEx(0x90CC, slotnum, 0);
            if (result.Data != null && result.Data.Length > 30)
            {
                control.IsLoaded = true;
                control.ItemNumber = slotnum;
                control.Monocrome = result.Data[1] == 1;
                control.CustomFlag = result.Data[2];
                string name = Encoding.ASCII.GetString(result.Data, 3, 20);
                control.RegistrationName = name.Contains("\0") ? name.Split('\0')[0] : name;
                if (!control.Monocrome)
                {
                    control.QuickAdjustFlag = result.Data[23];
                    control.QuickAdjust = (sbyte) result.Data[24];
                    control.Saturation = (sbyte) result.Data[25];
                    control.Hue = (sbyte) result.Data[26];
                }
                else
                {
                    control.FilterEffects = result.Data[23];
                    control.Toning = result.Data[24];
                    control.ToningDensity = result.Data[25];
                    //control.Hue = result.Data[27];
                }
                control.Sharpening = (sbyte) result.Data[27];
                control.Contrast = (sbyte) result.Data[28];
                control.Brightness = (sbyte) result.Data[29];
                control.CustomCurveFlag = result.Data[30];
                if (control.CustomCurveFlag == 1)
                    result.Data.CopyTo(control.CustomCurveData, 31);
            }
            else
            {
                return null;
            }
            return control;
        }

        public void SetPictureControl(PictureControl control)
        {
        }

        protected virtual PropertyValue<long> InitHostMode()
        {
            var res = new PropertyValue<long>() { Name = "Lock", IsEnabled = true };
            res.AddValues("OFF", 0);
            res.AddValues("ON", 1);
            res.ReloadValues();
            res.Value = "OFF";
            res.ValueChanged += delegate (object sender, string key, long val)
            {
                ExecuteWithNoData(CONST_CMD_ChangeCameraMode, (uint) (key == "OFF" ? 0 : 1));
                Mode.IsEnabled = key == "ON";
                GetEvent(null);
                ReadDeviceProperties(CONST_PROP_ExposureProgramMode);
            };
            return res;
        }

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            try
            {
                IsBusy = true;
                Capabilities.Add(CapabilityEnum.CaptureInRam);
                Capabilities.Add(CapabilityEnum.CaptureNoAf);
                StillImageDevice = deviceDescriptor.StillImageDevice;
                // check if is mtp device 
                StillImageDevice imageDevice = StillImageDevice as StillImageDevice;
                if (imageDevice != null)
                    imageDevice.DeviceEvent += _stillImageDevice_DeviceEvent;
                HaveLiveView = true;
                DeviceReady();
                DeviceName = StillImageDevice.Model;
                Manufacturer = StillImageDevice.Manufacturer;
                IsConnected = true;
                CaptureInSdRam = true;
                PropertyChanged += NikonBase_PropertyChanged;
                var ser = StillImageDevice.SerialNumber;
                Log.Debug("Serial number" + ser ?? "");
                if (ser != null && ser.Length >= 7)
                {
                    SerialNumber = ser.Substring(0, 7);
                    // there in some cases the leading zero order differs
                    if (SerialNumber == "0000000")
                    {
                        SerialNumber =  ser.Substring(ser.Length-7,7);    
                    }
                }
                // load advanced properties in a separated thread to speed up camera connection
                var thread = new Thread(LoadProperties) {Priority = ThreadPriority.Lowest};
                thread.Start();
            }
            catch (Exception exception)
            {
                Log.Error("Error initialize device", exception);
            }
            return true;
        }

        public void LoadProperties()
        {
            try
            {
                InitIso();
                InitShutterSpeed();
                InitFNumber();
                ReadDeviceProperties(CONST_PROP_LiveViewSelector);
                InitMode();
                InitWhiteBalance();
                InitExposureCompensation();
                InitCompressionSetting();
                InitExposureMeteringMode();
                InitFocusMode();
                InitOther();
                ReadDeviceProperties(CONST_PROP_BatteryLevel);
                ReadDeviceProperties(CONST_PROP_ExposureIndicateStatus);
                AddAditionalProps();
                ReInitShutterSpeed();
                ReadDeviceProperties(CONST_PROP_LiveViewStatus);
                _timer.Start();
                OnCameraInitDone();
                
            }
            catch (Exception exception)
            {
                Log.Error("Error load device properties", exception);
            }
            IsBusy = false;
        }

        public virtual void AddAditionalProps()
        {
            AdvancedProperties.Add(InitImageSize());
            AdvancedProperties.Add(InitRawQuality());
            AdvancedProperties.Add(InitBurstNumber());
            AdvancedProperties.Add(InitStillCaptureMode());
            AdvancedProperties.Add(InitOnOffProperty("Auto Iso", 0xD054));
            AdvancedProperties.Add(InitFlash());
            AdvancedProperties.Add(InitOnOffProperty("Long exp NR", CONST_PROP_NoiseReduction));
            AdvancedProperties.Add(InitNRHiIso());
            AdvancedProperties.Add(InitExposureDelay());
            AdvancedProperties.Add(InitLock());
            AdvancedProperties.Add(InitPictControl());
            AdvancedProperties.Add(InitRawBit());
            AdvancedProperties.Add(InitColorSpace()); //12
            AdvancedProperties.Add(InitWbTuneFluorescentType());
            AdvancedProperties.Add(InitWbColorTemp());
            AdvancedProperties.Add(InitOnOffProperty("Application mode", CONST_PROP_ApplicationMode));
            AdvancedProperties.Add(InitAutoIsoHight());
            AdvancedProperties.Add(CenterWeightedExRange());
            AdvancedProperties.Add(FlashCompensation());
            AdvancedProperties.Add(FlashSyncSpeed());
            AdvancedProperties.Add(CaptureAreaCrop());
            AdvancedProperties.Add(ActiveDLighting());
            AdvancedProperties.Add(HDRMode());
            AdvancedProperties.Add(HDREv());
            AdvancedProperties.Add(HDRSmoothing());
            AdvancedProperties.Add(ActiveSlot());
            AdvancedProperties.Add(LensSort());
            AdvancedProperties.Add(InitHostMode());
            try
            {
                var deviceinfo = LoadDeviceData(ExecuteReadDataEx(0x1001));
                foreach (PropertyValue<long> value in AdvancedProperties)
                {
                    if (!deviceinfo.PropertyExist(value.Code))
                        value.Available = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to check advanced proprties", ex);
            }
            foreach (PropertyValue<long> value in AdvancedProperties)
            {
                ReadDeviceProperties(value.Code);
            }
        }

        protected virtual PropertyValue<long> LensSort()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Lens Sort",
                IsEnabled = false,
                Code = 0xD0E1,
                SubType = typeof(sbyte)
            };
            res.AddValues("Not mounted", 0);
            res.AddValues("CPU lens mounted", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> ActiveSlot()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Active Slot",
                IsEnabled = false,
                Code = 0xD1F2,
                SubType = typeof(sbyte)
            };
            res.AddValues("Card not inserted", 0);
            res.AddValues("CF slot", 1);
            res.AddValues("SD slot", 2);
            res.AddValues("CF slot & SD slot", 3);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> FlashSyncSpeed()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Flash sync speed",
                IsEnabled = true,
                Code = 0xD074,
                SubType = typeof(sbyte)
            };
            res.AddValues("1/320 sec. (auto FP)", 0);
            res.AddValues("1/250 sec. (auto FP)", 1);
            res.AddValues("1/250 sec.", 2);
            res.AddValues("1/200 sec.", 3);
            res.AddValues("1/160 sec.", 4);
            res.AddValues("1/125 sec.", 5);
            res.AddValues("1/100 sec.", 6);
            res.AddValues("1/80 sec.", 7);
            res.AddValues("1/60 sec.", 8);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> HDRSmoothing()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "HDR intensity",
                IsEnabled = true,
                Code = 0xD132,
                SubType = typeof(sbyte)
            };
            res.AddValues("High", 0);
            res.AddValues("Normal", 1);
            res.AddValues("Low", 2);
            res.AddValues("Auto", 3);
            res.AddValues("Extra high", 4);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> HDREv()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "HDR Exposure deviation",
                IsEnabled = true,
                Code = 0xD131,
                SubType = typeof(sbyte)
            };
            res.AddValues("Auto", 0);
            res.AddValues("1 EV", 1);
            res.AddValues("2 EV", 2);
            res.AddValues("3 EV", 3);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> HDRMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "HDRMode",
                IsEnabled = true,
                Code = 0xD130,
                SubType = typeof(sbyte)
            };
            res.AddValues("OFF", 0);
            res.AddValues("ON (single)", 1);
            res.AddValues("ON (sequence)", 2);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }


        protected virtual PropertyValue<long> ActiveDLighting()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Active D-Lighting",
                IsEnabled = true,
                Code = 0xD14E,
                SubType = typeof(sbyte)
            };
            res.AddValues("Extra high", 0);
            res.AddValues("High", 1);
            res.AddValues("Normal", 2);
            res.AddValues("Low", 3);
            res.AddValues("Not performed", 4);
            res.AddValues("Auto", 5);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> CaptureAreaCrop()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Capture area crop",
                IsEnabled = true,
                Code = 0xD030,
                SubType = typeof(sbyte)
            };
            res.AddValues("FX format (36x24)", 0);
            res.AddValues("DX format (24x16)", 1);
            res.AddValues("5:4 (30x24)", 2);
            res.AddValues("1.2x (30x20)", 3);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }
        
        protected virtual PropertyValue<long> FlashCompensation ()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Ext. flash compensation",
                IsEnabled = true,
                Code = CONST_PROP_FlashCompensation,
                SubType = typeof (sbyte)
            };
            for (decimal i = -18; i <= 18; i++)
            {
                if (i > 0)
                    res.AddValues("+"+Decimal.Round(i/6, 1).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US")), (long) i);
                else
                    res.AddValues(Decimal.Round(i / 6, 1).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US")), (long)i);
            }
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((sbyte)val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> CenterWeightedExRange()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Center-weighted area",
                IsEnabled = true,
                Code = 0xD059,
                SubType = typeof(byte)
            };
            res.AddValues("6 mm", 0);
            res.AddValues("8 mm", 1);
            res.AddValues("10 mm", 2);
            res.AddValues("10 mm", 3);
            res.AddValues("Average on the entire screen", 4);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        public virtual PropertyValue<long> InitAutoIsoHight()
        {
            var res = new PropertyValue<long>();
            try
            {
                DeviceReady();
                byte datasize = 1;
                res.Name = "ISO Auto High Limit";
                res.SubType = typeof (byte);
                res.Code = CONST_PROP_ISOAutoHighLimit;
                res.ValueChanged +=
                    (sender, key, val) =>
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)val }, res.Code);

                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                    CONST_PROP_ISOAutoHighLimit);
                int type = BitConverter.ToInt16(result.Data, 2);
                byte formFlag = result.Data[(2*datasize) + 5];
                byte defval = result.Data[datasize + 5];

                foreach (KeyValuePair<byte, string> pair in _autoIsoTable)
                {
                    res.AddValues(pair.Value, pair.Key);
                }
                res.ReloadValues();
                res.SetValue(defval, false);
            }
            catch (Exception)
            {
            }
            return res;
        }



        protected virtual PropertyValue<long> InitColorSpace()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Color space",
                                              IsEnabled = true,
                                              Code = CONST_PROP_ColorSpace,
                                              SubType = typeof (byte)
                                          };
            res.AddValues("sRGB", 0);
            res.AddValues("Adobe RGB", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitWbTuneFluorescentType()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Fluorescent light type",
                                              IsEnabled = WhiteBalance.NumericValue == 5,
                                              Code = CONST_PROP_WbTuneFluorescentType,
                                              SubType = typeof (byte),
                                              DisableIfWrongValue = false
                                          };
            res.AddValues("Sodium lamp mixed light", 0);
            res.AddValues("Cool white fluorescent lamp", 1);
            res.AddValues("Warm white fluorescent lamp", 2);
            res.AddValues("White fluorescent lamp", 3);
            res.AddValues("Day white fluorescent lamp", 4);
            res.AddValues("Daylight fluorescent lamp", 5);
            res.AddValues("High color-temperature mercury lamp", 6);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitWbColorTemp()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Temperature",
                IsEnabled = WhiteBalance.NumericValue == 32786,
                Code = CONST_PROP_WbColorTemp,
                SubType = typeof (byte),
                DisableIfWrongValue = false
            };
            res.AddValues("2500 K", 0);
            res.AddValues("2560 K", 1);
            res.AddValues("2630 K", 2);
            res.AddValues("2700 K", 3);
            res.AddValues("2780 K", 4);
            res.AddValues("2860 K", 5);
            res.AddValues("2940 K", 6);
            res.AddValues("3030 K", 7);
            res.AddValues("3130 K", 8);
            res.AddValues("3230 K", 9);
            res.AddValues("3330 K", 10);
            res.AddValues("3450 K", 11);
            res.AddValues("3570 K", 12);
            res.AddValues("3700 K", 13);
            res.AddValues("3850 K", 14);
            res.AddValues("4000 K", 15);
            res.AddValues("4170 K", 16);
            res.AddValues("4350 K", 17);
            res.AddValues("4550 K", 18);
            res.AddValues("4760 K", 19);
            res.AddValues("5000 K", 20);
            res.AddValues("5260 K", 21);
            res.AddValues("5560 K", 22);
            res.AddValues("5880 K", 23);
            res.AddValues("6250 K", 24);
            res.AddValues("6670 K", 25);
            res.AddValues("7140 K", 26);
            res.AddValues("7690 K", 27);
            res.AddValues("8330 K", 28);
            res.AddValues("9090 K", 29);
            res.AddValues("10000 K", 30);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                    res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitRawBit()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Raw Recording bit mode",
                                              IsEnabled = true,
                                              Code = CONST_PROP_RawCompressionBitMode,
                                              SubType = typeof (byte),
                                              DisableIfWrongValue = true
                                          };
            res.AddValues("12-bit recording", 0);
            res.AddValues("14-bit recording", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitPictControl()
        {
            var res = new PropertyValue<long>()
            {
                Name = "Picture control",
                IsEnabled = true,
                Code = CONST_PROP_ActivePicCtrlItem,
                SubType = typeof(UInt16)
            };
            res.AddValues("Standard", 1);
            res.AddValues("Neutral", 2);
            res.AddValues("Vivid", 3);
            res.AddValues("Monochrome", 4);
            res.AddValues("Portrait", 5);
            res.AddValues("Landscape", 6);
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);

            try
            {
                for (byte i = 201; i < 210; i++)
                {
                    PictureControl control = GetPictureControl(i);
                    if (control != null && (control.IsLoaded && !string.IsNullOrWhiteSpace(control.RegistrationName)))
                        res.AddValues(control.RegistrationName, i);
                    else
                        res.AddValues("Custom picture control " + (i - 200), i);
                }
            }
            catch (Exception)
            {
                
            }
            res.ReloadValues();
            return res;
        }

        protected virtual PropertyValue<long> InitImageSize()
        {
            var res = new PropertyValue<long>() {Name = "Image Size", IsEnabled = true, Code = CONST_PROP_ImageSize};
            res.ValueChanged += ImageSize_ValueChanged;

            MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc, res.Code);

            //ErrorCodes.GetException(result.ErrorCode);

            if (result.Data != null && result.Data.Length > 112)
            {
                res.AddValues(Encoding.Unicode.GetString(result.Data, 51, 20), 0);
                res.AddValues(Encoding.Unicode.GetString(result.Data, 72, 20), 0);
                res.AddValues(Encoding.Unicode.GetString(result.Data, 93, 20), 0);
                res.SetValue(Encoding.Unicode.GetString(result.Data, 27, 20), false);
            }
            res.ReloadValues();
            return res;
        }

        private void ImageSize_ValueChanged(object sender, string key, long val)
        {
            if (CompressionSetting != null &&
                (CompressionSetting.Value != null && CompressionSetting.Value.Contains("RAW")))
                return;
            List<byte> vals = new List<byte>() {10};
            vals.AddRange(Encoding.Unicode.GetBytes(key));
            SetProperty(CONST_CMD_SetDevicePropValue, vals.ToArray(), CONST_PROP_ImageSize);
        }

        protected virtual PropertyValue<long> InitRawQuality()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Raw Compression",
                                              IsEnabled = true,
                                              Code = CONST_PROP_RawCompressionType,
                                              SubType = typeof (byte),
                                              DisableIfWrongValue = true
                                          };
            res.AddValues("Lossless compressed RAW", 0);
            res.AddValues("Compressed RAW", 1);
            res.AddValues("Uncompressed RAW", 2);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitOnOffProperty(string name, uint code)
        {
            var res = new PropertyValue<long>() {Name = name, IsEnabled = true, Code = code};
            res.AddValues("OFF", 0);
            res.AddValues("ON", 1);
            res.ReloadValues();
            res.ValueChanged += (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) val},
                                                                  res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitLock()
        {
            var res = new PropertyValue<long>() {Name = "Lock", IsEnabled = true};
            res.AddValues("OFF", 0);
            res.AddValues("ON", 1);
            res.ReloadValues();
            res.Value = "OFF";
            res.ValueChanged += delegate(object sender, string key, long val)
                                    {
                                        if (key == "OFF")
                                        {
                                            UnLockCamera();
                                        }
                                        else
                                        {
                                            LockCamera();
                                        }
                                    };
            return res;
        }

        protected virtual PropertyValue<long> InitFlash()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {Name = "Flash", IsEnabled = true, Code = 0x500C, SubType = typeof (UInt16)};
            res.AddValues("Flash prohibited", 0x0002);
            res.AddValues("Red-eye reduction", 0x0004);
            res.AddValues("Normal synchronization", 0x8010);
            res.AddValues("Slow synchronization", 0x8011);
            res.AddValues("Rear synchronization", 0x8012);
            res.AddValues("Red-eye reduction slow synchronization", 0x8013);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitStillCaptureMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Still Capture Mode",
                                              IsEnabled = true,
                                              Code = 0x5013,
                                              SubType = typeof (UInt16)
                                          };
            res.AddValues("Single shot (single-frame shooting)", 0x0001);
            res.AddValues("Continuous shot (continuous shooting)", 0x0002);
            res.AddValues("Self-timer", 0x8011);
            res.AddValues("Quick-response remote", 0x8014);
            res.AddValues("2s delayed remote", 0x8015);
            res.AddValues("Quiet shooting", 0x8016);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitBurstNumber()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Burst Number",
                                              IsEnabled = true,
                                              Code = 0x5018,
                                              SubType = typeof (UInt16)
                                          };
            for (int i = 1; i < 100; i++)
            {
                res.AddValues(i.ToString(), i);
            }
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }


        protected virtual PropertyValue<long> InitNRHiIso()
        {
            PropertyValue<long> res = new PropertyValue<long>() {Name = "High ISO NR", IsEnabled = true, Code = 0xD070};
            res.AddValues("Not performed", 0);
            res.AddValues("Low", 1);
            res.AddValues("Normal", 2);
            res.AddValues("High", 3);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) val}, res.Code);
            return res;
        }

        protected virtual PropertyValue<long> InitExposureDelay()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {Name = "Exposure delay mode", IsEnabled = true, Code = 0xD06A};
            res.AddValues("OFF", 0);
            res.AddValues("ON", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) val}, res.Code);
            return res;
        }

        protected void NikonBase_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CaptureInSdRam")
            {
                SetProperty(CONST_CMD_SetDevicePropValue, CaptureInSdRam ? new[] {(byte) 1} : new[] {(byte) 0},
                    CONST_PROP_RecordingMedia);
                ReadDeviceProperties(CONST_PROP_RecordingMedia);
            }
        }

        private void _stillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_DEVICE_REMOVED)
            {
                _timer.Stop();
                StillImageDevice.Disconnect();
                StillImageDevice.IsConnected = false;
                IsConnected = false;
                OnCameraDisconnected(this, new DisconnectCameraEventArgs {StillImageDevice = StillImageDevice});
            }
            else
            {
                Thread thread = new Thread(GetEvent);
                thread.Start();
            }
        }

        private void InitOther()
        {
            LiveViewImageZoomRatio = new PropertyValue<long> {Name = "LiveViewImageZoomRatio"};
            LiveViewImageZoomRatio.SubType = typeof(int);
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.AddValues("25%", 1);
            LiveViewImageZoomRatio.AddValues("33%", 2);
            LiveViewImageZoomRatio.AddValues("50%", 3);
            LiveViewImageZoomRatio.AddValues("66%", 4);
            LiveViewImageZoomRatio.AddValues("100%", 5);
            LiveViewImageZoomRatio.AddValues("200%", 6);
            LiveViewImageZoomRatio.SetValue("All");
            LiveViewImageZoomRatio.ReloadValues();
            LiveViewImageZoomRatio.ValueChanged += LiveViewImageZoomRatio_ValueChanged;
        }

        private void LiveViewImageZoomRatio_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                            CONST_PROP_LiveViewImageZoomRatio);
            }
        }

        protected virtual void InitIso()
        {
            lock (Locker)
            {
                NormalIsoNumber = new PropertyValue<long>();
                NormalIsoNumber.Name = "IsoNumber";
                NormalIsoNumber.SubType = typeof (int);
                NormalIsoNumber.ValueChanged += IsoNumber_ValueChanged;
                NormalIsoNumber.Clear();
                try
                {
                    DeviceReady();
                    MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc, CONST_PROP_ExposureIndex);
                    //IsoNumber.IsEnabled = result.Data[4] == 1;
                    UInt16 defval = BitConverter.ToUInt16(result.Data, 7);
                    for (int i = 0; i < result.Data.Length - 12; i += 2)
                    {
                        UInt16 val = BitConverter.ToUInt16(result.Data, 12 + i);
                        NormalIsoNumber.AddValues(_isoTable.ContainsKey(val) ? _isoTable[val] : val.ToString(), val);
                    }
                    NormalIsoNumber.ReloadValues();
                    NormalIsoNumber.SetValue(defval, false);
                    IsoNumber = NormalIsoNumber;
                }
                catch (Exception)
                {
                    NormalIsoNumber.IsEnabled = false;
                }

                MovieIsoNumber = new PropertyValue<long>();
                MovieIsoNumber.Name = "IsoNumber";
                MovieIsoNumber.SubType = typeof (int);
                MovieIsoNumber.ValueChanged += MovieIsoNumber_ValueChanged;
                MovieIsoNumber.Clear();
                try
                {
                    MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc, CONST_PROP_MovieExposureIndex);
                    //IsoNumber.IsEnabled = result.Data[4] == 1;
                    UInt16 defval = BitConverter.ToUInt16(result.Data, 7);
                    for (int i = 0; i < result.Data.Length - 12; i += 2)
                    {
                        UInt16 val = BitConverter.ToUInt16(result.Data, 12 + i);
                        MovieIsoNumber.AddValues(_isoTable.ContainsKey(val) ? _isoTable[val] : val.ToString(CultureInfo.InvariantCulture), val);
                    }
                    MovieIsoNumber.ReloadValues();
                    MovieIsoNumber.SetValue(defval, false);
                }
                catch (Exception)
                {
                    MovieIsoNumber.IsEnabled = false;
                }
            }
        }

        private void IsoNumber_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((int)val),
                            CONST_PROP_ExposureIndex);
            }
        }

        private void MovieIsoNumber_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((int)val),
                            CONST_PROP_MovieExposureIndex);
            }
        }

        protected virtual void InitShutterSpeed()
        {
            NormalShutterSpeed = new PropertyValue<long>();
            NormalShutterSpeed.Name = "ShutterSpeed";
            NormalShutterSpeed.ValueChanged += ShutterSpeed_ValueChanged;
            MovieShutterSpeed = new PropertyValue<long>();
            MovieShutterSpeed.Name = "ShutterSpeed";
            MovieShutterSpeed.ValueChanged += MovieShutterSpeed_ValueChanged;
            ReInitShutterSpeed();
        }

        private void ShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "M" || Mode.Value == "S"))
                if (key == "Bulb")
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(0xFFFFFFFF),
                        CONST_PROP_ShutterSpeed);
                else
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                        CONST_PROP_ExposureTime);
        }

        private void MovieShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "M" || Mode.Value == "S"))
                if (key == "Bulb")
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(0xFFFFFFFF),
                        CONST_PROP_MovieShutterSpeed);
                else
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                        CONST_PROP_MovieShutterSpeed);
        }

        protected virtual void ReInitShutterSpeed()
        {
            lock (Locker)
            {
                DeviceReady();
                try
                {
                    byte datasize = 4;
                    var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                     CONST_PROP_ExposureTime);
                    if (result.Data == null)
                        return;
                    NormalShutterSpeed.Clear();
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2 * datasize) + 5];
                    UInt32 defval = BitConverter.ToUInt32(result.Data, datasize + 5);
                    for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                    {
                        UInt32 val = BitConverter.ToUInt32(result.Data, ((2 * datasize) + 6 + 2) + i);
                        NormalShutterSpeed.AddValues(_shutterTable.ContainsKey(val) ? _shutterTable[val] : val.ToString(), val);
                    }
                    // force to add Bulb mode for some cameras which not support it
                    if (Mode != null && (Mode.Value == "S" || Mode.Value == "M") && !NormalShutterSpeed.Values.Contains("Bulb"))
                        NormalShutterSpeed.AddValues("Bulb", 0xFFFFFFFF);
                    NormalShutterSpeed.ReloadValues();
                    NormalShutterSpeed.SetValue(defval, false);
                    ShutterSpeed = NormalShutterSpeed;
                }
                catch (Exception)
                {
                }
                try
                {
                    byte datasize = 4;
                    var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                     CONST_PROP_MovieShutterSpeed);
                    if (result.Data == null || result.Data.Length==0)
                        return;
                    MovieShutterSpeed.Clear();
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2 * datasize) + 5];
                    UInt32 defval = BitConverter.ToUInt32(result.Data, datasize + 5);
                    for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                    {
                        UInt32 val = BitConverter.ToUInt32(result.Data, ((2 * datasize) + 6 + 2) + i);
                        MovieShutterSpeed.AddValues("1/" + (val-0x10000),val);
                    }
                    // force to add Bulb mode for some cameras which not support it
                    if (Mode != null && (Mode.Value == "S" || Mode.Value == "M") && !MovieShutterSpeed.Values.Contains("Bulb"))
                        MovieShutterSpeed.AddValues("Bulb", 0xFFFFFFFF);

                    MovieShutterSpeed.SetValue(defval, false);
                }
                catch (Exception)
                {
                }
            }
        }

        private void InitMode()
        {
            try
            {
                DeviceReady();
                byte datasize = 2;
                Mode = new PropertyValue<long>();
                Mode.SubType = typeof(uint);
                Mode.Name = "Mode";
                Mode.IsEnabled = false;
                Mode.ValueChanged += Mode_ValueChanged;
                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                 CONST_PROP_ExposureProgramMode);
                int type = BitConverter.ToInt16(result.Data, 2);
                byte formFlag = result.Data[(2 * datasize) + 5];
                UInt16 defval = BitConverter.ToUInt16(result.Data, datasize + 5);
                for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                {
                    UInt16 val = BitConverter.ToUInt16(result.Data, ((2 * datasize) + 6 + 2) + i);
                    Mode.AddValues(_exposureModeTable.ContainsKey(val) ? _exposureModeTable[val] : val.ToString(), val);
                }
                Mode.ReloadValues();
                Mode.SetValue(defval);
            }
            catch (Exception)
            {
            }
        }


        private void Mode_ValueChanged(object sender, string key, long val)
        {
            try
            {
                switch (key)
                {
                    case "M":
                        ShutterSpeed.IsEnabled = true;
                        NormalFNumber.IsEnabled = true;
                        //ReInitShutterSpeed();
                        break;
                    case "P":
                        ShutterSpeed.IsEnabled = false;
                        FNumber.IsEnabled = false;
                        break;
                    case "A":
                        ShutterSpeed.IsEnabled = false;
                        NormalFNumber.IsEnabled = true;
                        break;
                    case "S":
                        ShutterSpeed.IsEnabled = true;
                        FNumber.IsEnabled = false;
                        //ReInitShutterSpeed();
                        break;
                    default:
                        ShutterSpeed.IsEnabled = false;
                        FNumber.IsEnabled = false;
                        break;
                }
                if (Mode.IsEnabled)
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                        CONST_PROP_ExposureProgramMode);
            }
            catch (Exception ex)
            {
                Log.Error("Usable to set camera mode", ex);
            }
        }

        protected virtual void InitFNumber()
        {
            NormalFNumber = new PropertyValue<long> {IsEnabled = true, Name = "FNumber"};
            NormalFNumber.ValueChanged += NormalFNumber_ValueChanged;
            NormalFNumber.SubType = typeof (int);
            MovieFNumber = new PropertyValue<long> { IsEnabled = true, Name = "FNumber" };
            MovieFNumber.ValueChanged += MovieFNumber_ValueChanged;
            MovieFNumber.SubType = typeof(int);
            ReInitFNumber(false);
        }

        private void MovieFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((int)val),
                            CONST_PROP_MovieFnumber);
        }

        private void NormalFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((int)val),
                            CONST_PROP_Fnumber);
        }


        protected void ReInitFNumber(bool trigervaluchange)
        {
            try
            {
                DeviceReady();
                const byte datasize = 2;
                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc, CONST_PROP_Fnumber);
                if (result.Data != null)
                {
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2*datasize) + 5];
                    UInt16 defval = BitConverter.ToUInt16(result.Data, datasize + 5);
                    NormalFNumber.Clear();
                    for (int i = 0; i < result.Data.Length - ((2*datasize) + 6 + 2); i += datasize)
                    {
                        UInt16 val = BitConverter.ToUInt16(result.Data, ((2*datasize) + 6 + 2) + i);
                        string s =  (val/100.0).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US"));
                        NormalFNumber.AddValues(s, val);
                    }
                    NormalFNumber.ReloadValues();
                    NormalFNumber.SetValue(defval, trigervaluchange);
                    FNumber = NormalFNumber;
                }
                else
                {
                    
                }
                result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc, CONST_PROP_MovieFnumber);
                if (result.Data != null && result.Data.Length > 0)
                {
                    MovieFNumber.Clear();
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2 * datasize) + 5];
                    UInt16 defval = BitConverter.ToUInt16(result.Data, datasize + 5);
                    for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                    {
                        UInt16 val = BitConverter.ToUInt16(result.Data, ((2 * datasize) + 6 + 2) + i);
                        string s = (val / 100.0).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US"));
                        MovieFNumber.AddValues(s, val);
                    }
                    MovieFNumber.ReloadValues();
                    MovieFNumber.SetValue(defval, trigervaluchange);
                }
            }
            catch (Exception)
            {
            }
        }

        private void InitWhiteBalance()
        {
            try
            {
                DeviceReady();
                byte datasize = 2;
                WhiteBalance = new PropertyValue<long>();
                WhiteBalance.Name = "WhiteBalance";
                WhiteBalance.ValueChanged += WhiteBalance_ValueChanged;
                var result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc, CONST_PROP_WhiteBalance);
                if (result.Data != null && result.Data.Length > 0)
                {
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2*datasize) + 5];
                    UInt16 defval = BitConverter.ToUInt16(result.Data, datasize + 5);
                    for (int i = 0; i < result.Data.Length - ((2*datasize) + 6 + 2); i += datasize)
                    {
                        UInt16 val = BitConverter.ToUInt16(result.Data, ((2*datasize) + 6 + 2) + i);
                        WhiteBalance.AddValues(_wbTable.ContainsKey(val) ? _wbTable[val] : val.ToString(), val);
                    }
                    WhiteBalance.ReloadValues();
                    WhiteBalance.SetValue(defval, false);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error init white balance", ex);
            }
        }

        private void WhiteBalance_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) val),
                        CONST_PROP_WhiteBalance);
            if (AdvancedProperties.Count > 13)
                AdvancedProperties[13].IsEnabled = val == 5;
            if (AdvancedProperties.Count > 14)
                AdvancedProperties[14].IsEnabled = val == 32786;
        }

        public void InitExposureCompensation()
        {
            try
            {
                DeviceReady();
                byte datasize = 2;
                NormalExposureCompensation = new PropertyValue<long>();
                NormalExposureCompensation.SubType = typeof(int);
                NormalExposureCompensation.Name = "ExposureCompensation";
                NormalExposureCompensation.ValueChanged += ExposureCompensation_ValueChanged;
                MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc,
                                                           CONST_PROP_ExposureBiasCompensation);
                Int16 defval = BitConverter.ToInt16(result.Data, datasize + 5);
                for (int i = 0; i < result.Data.Length - ((2*datasize) + 6 + 2); i += datasize)
                {
                    Int16 val = BitConverter.ToInt16(result.Data, ((2*datasize) + 6 + 2) + i);
                    decimal d = val;
                    string s = Decimal.Round(d/1000, 1).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US"));
                    if (d > 0)
                        s = "+" + s;
                    NormalExposureCompensation.AddValues(s, val);
                }
                NormalExposureCompensation.ReloadValues();
                NormalExposureCompensation.SetValue(defval, false);
                ExposureCompensation = NormalExposureCompensation;
            }
            catch (Exception)
            {
            }
            try
            {
                DeviceReady();
                byte datasize = 2;
                MovieExposureCompensation = new PropertyValue<long>();
                MovieExposureCompensation.SubType = typeof(int);
                MovieExposureCompensation.Name = "ExposureCompensation";
                MovieExposureCompensation.ValueChanged += MovieExposureCompensation_ValueChanged;
                MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc,
                                                           CONST_PROP_MovieExposureBiasCompensation);
                Int16 defval = BitConverter.ToInt16(result.Data, datasize + 5);
                for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                {
                    Int16 val = BitConverter.ToInt16(result.Data, ((2 * datasize) + 6 + 2) + i);
                    decimal d = val;
                    string s = Decimal.Round(d / 1000, 1).ToString("0.0", CultureInfo.CreateSpecificCulture("en-US"));
                    if (d > 0)
                        s = "+" + s;
                    MovieExposureCompensation.AddValues(s, val);
                }
                MovieExposureCompensation.ReloadValues();
                MovieExposureCompensation.SetValue(defval, false);
            }
            catch (Exception)
            {
            }

        }


        private void MovieExposureCompensation_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                            CONST_PROP_MovieExposureBiasCompensation);
            }
        }

        private void ExposureCompensation_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                            CONST_PROP_ExposureBiasCompensation);
            }
        }

        protected virtual void InitCompressionSetting()
        {
            try
            {
                DeviceReady();
                byte datasize = 1;
                CompressionSetting = new PropertyValue<long>();
                CompressionSetting.SubType = typeof(int);
                CompressionSetting.Name = "CompressionSetting ";
                CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                 CONST_PROP_CompressionSetting);
                if(result.Data.Length==0)
                    return;
                int type = BitConverter.ToInt16(result.Data, 2);
                byte formFlag = result.Data[(2 * datasize) + 5];
                byte defval = result.Data[datasize + 5];
                for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                {
                    byte val = result.Data[((2 * datasize) + 6 + 2) + i];
                    CompressionSetting.AddValues(_csTable.ContainsKey(val) ? _csTable[val] : val.ToString(), val);
                }
                CompressionSetting.ReloadValues();
                CompressionSetting.SetValue(defval, false);
            }
            catch (Exception)
            {
            }
        }

        protected void CompressionSetting_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((byte) val),
                        CONST_PROP_CompressionSetting);
        }

        public void InitExposureMeteringMode()
        {
            try
            {
                DeviceReady();
                byte datasize = 2;
                ExposureMeteringMode = new PropertyValue<long>();
                ExposureMeteringMode.SubType = typeof(int);
                ExposureMeteringMode.Name = "ExposureMeteringMode";
                ExposureMeteringMode.ValueChanged += ExposureMeteringMode_ValueChanged;
                MTPDataResponse result = ExecuteReadDataEx(CONST_CMD_GetDevicePropDesc, CONST_PROP_ExposureMeteringMode);
                UInt16 defval = BitConverter.ToUInt16(result.Data, datasize + 5);
                for (int i = 0; i < result.Data.Length - ((2*datasize) + 6 + 2); i += datasize)
                {
                    UInt16 val = BitConverter.ToUInt16(result.Data, ((2*datasize) + 6 + 2) + i);
                    ExposureMeteringMode.AddValues(_emmTable.ContainsKey(val) ? _emmTable[val] : val.ToString(), val);
                }
                ExposureMeteringMode.ReloadValues();
                ExposureMeteringMode.SetValue(defval, false);
            }
            catch (Exception)
            {
            }
        }

        private void ExposureMeteringMode_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) val),
                        CONST_PROP_ExposureMeteringMode);
        }

        protected virtual void InitFocusMode()
        {
            try
            {
                Log.Debug("InitFocusMode 1");
                DeviceReady();
                NormalFocusMode = new PropertyValue<long>();
                NormalFocusMode.Name = "FocusMode";
                NormalFocusMode.Code = CONST_PROP_AFModeSelect;
                NormalFocusMode.IsEnabled = true;
                NormalFocusMode.AddValues("AF-S", 0);
                NormalFocusMode.AddValues("AF-C", 1);
                NormalFocusMode.AddValues("AF-A", 2);
                NormalFocusMode.AddValues("MF (hard)", 3);
                NormalFocusMode.AddValues("MF (soft)", 4);
                NormalFocusMode.ReloadValues();
                NormalFocusMode.ValueChanged += NormalFocusMode_ValueChanged;
                FocusMode = NormalFocusMode;
                ReadDeviceProperties(NormalFocusMode.Code);
                Log.Debug("InitFocusMode 2");
                LiveViewFocusMode = new PropertyValue<long>();
                LiveViewFocusMode.Name = "FocusMode";
                LiveViewFocusMode.Code = CONST_PROP_AfModeAtLiveView;
                LiveViewFocusMode.IsEnabled = true;
                LiveViewFocusMode.AddValues("AF-S", 0);
                LiveViewFocusMode.AddValues("AF-F", 2);
                LiveViewFocusMode.AddValues("MF (hard)", 3);
                LiveViewFocusMode.AddValues("MF (soft)", 4);
                LiveViewFocusMode.ReloadValues();
                LiveViewFocusMode.ValueChanged += LiveViewFocusMode_ValueChanged;
                ReadDeviceProperties(LiveViewFocusMode.Code);
                Log.Debug("InitFocusMode 3");
            }
            catch (Exception exception)
            {
                Log.Error("Unable to init focus mode property", exception);
            }
        }

        void LiveViewFocusMode_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((sbyte)val),
                        CONST_PROP_AfModeAtLiveView);            
        }

        private void NormalFocusMode_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((sbyte) val),
                        CONST_PROP_AFModeSelect);
        }

        public override void StartLiveView()
        {
            lock (Locker)
            {
                //DeviceReady();
                //check if the live view already is started if yes returning without doing anything
                MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus);
                //ErrorCodes.GetException(response.ErrorCode);
                if (response.Data != null && response.Data.Length > 0 && response.Data[0] > 0)
                    return;
                DeviceReady();
                // this should improve focus staking performance
                // anyway
                //LockCamera();
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_StartLiveView));
                DeviceReady();
                LiveViewImageZoomRatio.SetValue();
            }
        }

        public override void StopLiveView()
        {
            lock (Locker)
            {
                DeviceReady();
                //UnLockCamera();
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_EndLiveView));
                DeviceReady();
            }
        }

        public override LiveViewData GetLiveViewImage()
        {
            _timer.Stop();
            LiveViewData viewData = new LiveViewData();
            if (Monitor.TryEnter(Locker, 10))
            {
                try
                {
                    //DeviceReady();
                    viewData.HaveFocusData = true;

                    const int headerSize = 384;

                    var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetLiveViewImage);
                    if (result.ErrorCode == ErrorCodes.MTP_Not_LiveView)
                    {
                        _timer.Start();
                        viewData.IsLiveViewRunning = false;
                        viewData.ImageData = null;
                        return viewData;                       
                    }
                    if (result.Data == null || result.Data.Length <= headerSize)
                    {
                        _timer.Start();
                        viewData.ImageData = null;
                        return viewData;
                    }
                    int cbBytesRead = result.Data.Length;
                    GetAdditionalLiveViewData(viewData, result.Data);
                    viewData.ImageDataPosition = 384;
                    viewData.ImageData = result.Data;
                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            _timer.Start();
            return viewData;
        }

        [Obsolete("Use GetAdditionalLiveViewData instead.", false)]
        protected virtual void GetAditionalLIveViewData(LiveViewData viewData, byte[] result)
        {
            GetAdditionalLiveViewData(viewData, result);
        }

        protected virtual void GetAdditionalLiveViewData(LiveViewData viewData, byte[] result)
        {
            viewData.LiveViewImageWidth = ToInt16(result, 0);
            viewData.LiveViewImageHeight = ToInt16(result, 2);

            viewData.ImageWidth = ToInt16(result, 4);
            viewData.ImageHeight = ToInt16(result, 6);

            viewData.FocusFrameXSize = ToInt16(result, 16);
            viewData.FocusFrameYSize = ToInt16(result, 18);

            viewData.FocusX = ToInt16(result, 20);
            viewData.FocusY = ToInt16(result, 22);
            
            viewData.MovieTimeRemain = ToDeciaml(result, 56);

            viewData.Focused = result[40] != 1;
            if (result[29] == 1)
                viewData.Rotation = -90;
            if (result[29] == 2)
                viewData.Rotation = 90;
            viewData.MovieIsRecording = result[60] == 1;
        }

        public override int Focus(int step)
        {
            if (step == 0)
                return 0;
            DeviceReady();
            uint resp =(step > 0? ExecuteWithNoData(CONST_CMD_MfDrive, 0x00000002, (uint) step)
                                        : ExecuteWithNoData(CONST_CMD_MfDrive, 0x00000001, (uint) -step));
            ErrorCodes.GetException(resp);
            DeviceReady();
            return step;
        }


        public override void Focus(FocusDirection direction, FocusAmount amount)
        {
            switch (direction)
            {
                case FocusDirection.Far:
                    switch (amount)
                    {
                        case FocusAmount.Small:
                            Focus(10);
                            break;
                        case FocusAmount.Medium:
                            Focus(100);
                            break;
                        case FocusAmount.Large:
                            Focus(500);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(amount), amount, null);
                    }
                    break;
                case FocusDirection.Near:
                    switch (amount)
                    {
                        case FocusAmount.Small:
                            Focus(-10);
                            break;
                        case FocusAmount.Medium:
                            Focus(-100);
                            break;
                        case FocusAmount.Large:
                            Focus(-500);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(amount), amount, null);
                    }
                    break;

            }
        }
    

        public override void AutoFocus()
        {
            lock (Locker)
            {
                DeviceReady();
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_AfDrive));
            }
        }

        public override void CapturePhoto()
        {
            SpeedUpEventTimer();
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                DeviceReady();
                ErrorCodes.GetException(CaptureInSdRam
                                            ? ExecuteWithNoData(CONST_CMD_AfAndCaptureRecInSdram)
                                            : ExecuteWithNoData(CONST_CMD_InitiateCapture));
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
        }

        public override void CapturePhotoNoAf()
        {
            SpeedUpEventTimer();
            lock (Locker)
            {
                MTPDataResponse val = null;
                byte oldval = 0;
                try
                {
                    IsBusy = true;
                    DeviceReady();
                    MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus);
                    //ErrorCodes.GetException(response.ErrorCode);
                    // test if live view is on 
                    if (response.Data != null && response.Data.Length > 0 && response.Data[0] > 0)
                    {
                        if (CaptureInSdRam)
                        {
                            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF));
                            return;
                        }
                        else
                        {
                            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia, 0xFFFFFFFF, 0x0000));
                            return;
                        }
                        StopLiveView();
                    }

                    val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect );
                    if (val.Data != null && val.Data.Length > 0)
                        oldval = val.Data[0];
                    SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 4}, CONST_PROP_AFModeSelect);
                    DeviceReady();
                    ErrorCodes.GetException(CaptureInSdRam
                                                ? ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF)
                                                : ExecuteWithNoData(CONST_CMD_InitiateCapture));
                    //DeviceReady();
                }
                catch
                {
                    IsBusy = false;
                    throw;
                }
                finally
                {
                    //IsBusy = false;
                    if (val != null && (val.Data != null && val.Data.Length > 0))
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] {oldval}, CONST_PROP_AFModeSelect);
                }
            }
        }

        public override void Focus(int x, int y)
        {
            lock (Locker)
            {
                //DeviceReady();
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_ChangeAfArea, (uint) x, (uint) y));
            }
        }

        public override void Close()
        {
            lock (Locker)
            {
                try
                {
                    IsConnected = false;
                    _timer.Stop();
                    HaveLiveView = false;
                    StillImageDevice.Disconnect();
                }
                catch (Exception exception)
                {
                    Log.Error("Close camera error", exception);
                }
            }
        }


        //private byte _liveViewImageZoomRatio;

        //public override PropertyValue<int> LiveViewImageZoomRatio
        //{
        //  get
        //  {
        //    if (_stillImageDevice == null)
        //      return 0;
        //    lock (Locker)
        //    {
        //      byte[] data = _stillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
        //                                                      CONST_PROP_LiveViewImageZoomRatio,
        //                                                      -1);
        //      if (data != null && data.Length == 1)
        //      {
        //        _liveViewImageZoomRatio = data[0];
        //        ////NotifyPropertyChanged("LiveViewImageZoomRatio");
        //      }
        //    }
        //    return _liveViewImageZoomRatio;
        //  }
        //  set
        //  {
        //    lock (Locker)
        //    {
        //      if (_stillImageDevice.ExecuteWriteData(CONST_CMD_SetDevicePropValue, new[] {value},
        //                                             CONST_PROP_LiveViewImageZoomRatio, -1) == 0)
        //        _liveViewImageZoomRatio = value;
        //      NotifyPropertyChanged("LiveViewImageZoomRatio");
        //    }
        //  }
        //}

        public override void ReadDeviceProperties(uint prop)
        {
            //lock (Locker)
            //{
                try
                {
                    HaveLiveView = true;
                    switch (prop)
                    {
                        case CONST_PROP_Fnumber:
                            //FNumber.SetValue(_stillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_Fnumber));
                            ReInitFNumber(false);
                            break;
                        case CONST_PROP_MovieFnumber:
                            //FNumber.SetValue(_stillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_Fnumber));
                            ReInitFNumber(false);
                            break;
                        case CONST_PROP_ExposureIndex:
                            NormalIsoNumber.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                CONST_PROP_ExposureIndex), false);
                            break;
                    case CONST_PROP_ExposureIndexEx:
                        NormalIsoNumber.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                            CONST_PROP_ExposureIndexEx), false);
                        break;
                    case CONST_PROP_MovieExposureIndex:
                            MovieFNumber.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                CONST_PROP_MovieExposureIndex), false);
                            break;
                        case CONST_PROP_ExposureTime:
                            NormalShutterSpeed.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                   CONST_PROP_ExposureTime), false);
                            break;
                        case CONST_PROP_MovieShutterSpeed:
                            MovieShutterSpeed.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                   CONST_PROP_MovieShutterSpeed), false);
                            break;
                        case CONST_PROP_WhiteBalance:
                            WhiteBalance.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                   CONST_PROP_WhiteBalance), false);
                            break;
                        case CONST_PROP_ExposureProgramMode:
                            Mode.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                           CONST_PROP_ExposureProgramMode), true);
                            break;
                        case CONST_PROP_ExposureBiasCompensation:
                            NormalExposureCompensation.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                           CONST_PROP_ExposureBiasCompensation),
                                                          false);
                            break;
                        case CONST_PROP_MovieExposureBiasCompensation:
                            MovieExposureCompensation.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                           CONST_PROP_MovieExposureBiasCompensation),
                                                          false);
                            break;

                        case CONST_PROP_CompressionSetting:
                            CompressionSetting.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                         CONST_PROP_CompressionSetting),
                                                        false);
                            break;
                        case CONST_PROP_ExposureMeteringMode:
                            ExposureMeteringMode.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                                           CONST_PROP_ExposureMeteringMode),
                                                          false);
                            break;
                        case CONST_PROP_AFModeSelect:
                            NormalFocusMode.SetValue(
                                StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect),
                                false);
                            NormalFocusMode.IsEnabled = NormalFocusMode.NumericValue != 3;
                            break;
                        case CONST_PROP_AfModeAtLiveView:
                            LiveViewFocusMode.SetValue(
                                StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AfModeAtLiveView),
                                false);
                            LiveViewFocusMode.IsEnabled = LiveViewFocusMode.NumericValue != 3;
                            break;
                        case CONST_PROP_BatteryLevel:
                            {
                                var data = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_BatteryLevel);
                                if (data.Data != null && data.Data.Length > 0)
                                    Battery = data.Data[0];
                            }
                            break;
                        case CONST_PROP_ExposureIndicateStatus:
                            {
                                var data =
                                    StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                     CONST_PROP_ExposureIndicateStatus);
                                if (data.Data != null && data.Data.Length > 0)
                                {
                                    sbyte i =
                                        unchecked(
                                            (sbyte)data.Data[0]);
                                    ExposureStatus = Convert.ToInt32(i);
                                }
                            }
                            break;
                        case CONST_PROP_LiveViewStatus:
                        {
                            MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus);
                            if (response.Data != null && response.Data.Length > 0 )
                            {
                                LiveViewOn = response.Data[0] == 1;
                            }
                            else
                            {
                                LiveViewOn = false;
                            }
                            break;
                        }
                        case CONST_PROP_LiveViewSelector:
                        {
                            MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewSelector);
                            if (response.Data != null && response.Data.Length > 0)
                            {
                                LiveViewMovieOn = response.Data[0] == 1;
                            }
                            else
                            {
                                LiveViewMovieOn = false;
                            }
                            break;
                        }
                        default:
                            // imrovements from: http://digicamcontrol.com/forum/testingbug-reports/buglet-nikonbasecs
                            foreach (PropertyValue<long> advancedProperty in AdvancedProperties.Where(advancedProperty => advancedProperty.Code == prop))
                            {
                                if (advancedProperty.Name == "Image Size")
                                {
                                    var val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                        advancedProperty.Code);
                                    if (val.Data != null && val.Data.Length > 0)
                                    {
                                        advancedProperty.SetValue(
                                            Encoding.Unicode.GetString(val.Data, 1, 20), false);
                                    }
                                }
                                else
                                {
                                    advancedProperty.SetValue(
                                        StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                            advancedProperty.Code), false);
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ReadDeviceProperties error", ex);
                }
            //}
        }

        public override void LockCamera()
        {
            ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 1);
            Mode.IsEnabled = true;
        }

        public override void UnLockCamera()
        {
            Mode.IsEnabled = false;
            ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 0);
        }

        public override void StartRecordMovie()
        {
            base.StartRecordMovie();
            DeviceReady();
            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_StartMovieRecInCard));
        }

        public override void StopRecordMovie()
        {
            base.StopRecordMovie();
            DeviceReady();
            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_EndMovieRec));
        }

        public override void SetCameraField(CameraFieldType cameraFieldType, string comment)
        {
            switch (cameraFieldType)
            {
                case CameraFieldType.Comment:
                    if (string.IsNullOrEmpty(comment))
                    {
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0}, 0xD091);
                    }
                    else
                    {
                        comment = comment.Length > 36 ? comment.Substring(0, 36) : comment.PadRight(36);
                        List<byte> vals = new List<byte>();
                        List<byte> valsnew = new List<byte>();
                        valsnew.Add(36);
                        vals.AddRange(Encoding.UTF8.GetBytes(comment));
                        foreach (byte val in vals)
                        {
                            valsnew.Add(val);
                            valsnew.Add(0);
                        }
                        valsnew.Add(0);
                        valsnew.Add(0);
                        SetProperty(CONST_CMD_SetDevicePropValue, valsnew.ToArray(), 0xD090);
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, 0xD091);
                    }
                    break;
                case CameraFieldType.Artist:
                    if (string.IsNullOrEmpty(comment))
                    {
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0, (byte) 0, (byte) 0}, 0xD072);
                    }
                    else
                    {
                        comment = comment.Length > 36 ? comment.Substring(0, 36) : comment.PadRight(36);
                        List<byte> vals = new List<byte>();
                        List<byte> valsnew = new List<byte>();
                        valsnew.Add(36);
                        vals.AddRange(Encoding.UTF8.GetBytes(comment));
                        foreach (byte val in vals)
                        {
                            valsnew.Add(val);
                            valsnew.Add(0);
                        }
                        valsnew.Add(0);
                        valsnew.Add(0);
                        SetProperty(CONST_CMD_SetDevicePropValue, valsnew.ToArray(), 0xD072);
                    }

                    break;
                case CameraFieldType.Copyright:
                    if (string.IsNullOrEmpty(comment))
                    {
                        SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0, (byte) 0, (byte) 0}, 0xD073);
                    }
                    else
                    {
                        comment = comment.Length > 53 ? comment.Substring(0, 53) : comment.PadRight(53);
                        List<byte> vals = new List<byte>();
                        List<byte> valsnew = new List<byte>();
                        valsnew.Add(54);
                        vals.AddRange(Encoding.UTF8.GetBytes(comment));
                        foreach (byte val in vals)
                        {
                            valsnew.Add(val);
                            valsnew.Add(0);
                        }
                        valsnew.Add(0);
                        valsnew.Add(0);
                        SetProperty(CONST_CMD_SetDevicePropValue, valsnew.ToArray(), 0xD073);
                    }
                    break;
                default:
                    //throw new ArgumentOutOfRangeException("cameraFieldType");
                    break;
            }
        }

        private DateTime _dateTime;
        private bool _liveViewOn;
        private bool _liveViewMovieOn;

        public override DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                try
                {
                    string datestring = _dateTime.ToString("yyyyMMddTHHmmss");
                    List<byte> vals = new List<byte>();
                    List<byte> valsnew = new List<byte>();
                    valsnew.Add(16);
                    vals.AddRange(Encoding.UTF8.GetBytes(datestring));
                    foreach (byte val in vals)
                    {
                        valsnew.Add(val);
                        valsnew.Add(0);
                    }
                    valsnew.Add(0);
                    valsnew.Add(0);
                    SetProperty(CONST_CMD_SetDevicePropValue, valsnew.ToArray(), CONST_PROP_DateTime);
                }
                catch (Exception exception)
                {
                    Log.Error("Error set camera DateTime", exception);
                }
                NotifyPropertyChanged("DateTime");
            }
        }

        private void GetEvent(object state)
        {
            try
            {
                if (_eventIsbusy)
                    return;
                _eventIsbusy = true;
                //DeviceReady();
                MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetEvent);

                if (response.Data == null || response.Data.Length == 0)
                {
                    Log.Debug("Get event error :" + response.ErrorCode.ToString("X"));
                    _eventIsbusy = false;
                    return;
                }
                int eventCount = BitConverter.ToInt16(response.Data, 0);
                if (eventCount > 0)
                {
                    for (int i = 0; i < eventCount; i++)
                    {
                        try
                        {
                            uint eventCode = BitConverter.ToUInt16(response.Data, 6*i + 2);
                            ushort eventParam = BitConverter.ToUInt16(response.Data, 6*i + 4);
                            int longeventParam = BitConverter.ToInt32(response.Data, 6*i + 4);
                            switch (eventCode)
                            {
                                case CONST_Event_DevicePropChanged:
                                    ReadDeviceProperties(eventParam);
                                    break;
                                case CONST_Event_ObjectAddedInSdram:
                                case CONST_Event_ObjectAdded:
                                    {
                                        Log.Debug("CONST_Event_ObjectAddedInSdram" + eventCode.ToString("X"));
                                        MTPDataResponse objectdata = ExecuteReadDataEx(CONST_CMD_GetObjectInfo,
                                                                                       (uint) longeventParam);
                                        string filename = "DSC_0000.JPG";
                                        if (objectdata.Data != null)
                                        {
                                            filename = Encoding.Unicode.GetString(objectdata.Data, 53, 12*2);
                                            if (filename.Contains("\0"))
                                                filename = filename.Split('\0')[0];
                                        }
                                        else
                                        {
                                            Log.Error("Error getting file name");
                                        }
                                        Log.Debug("File name" + filename);
                                        PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                                                                          {
                                                                              WiaImageItem = null,
                                                                              EventArgs =
                                                                                  new PortableDeviceEventArgs(new PortableDeviceEventType
                                                                                                                  ()
                                                                                                                  {
                                                                                                                      ObjectHandle
                                                                                                                          =
                                                                                                                          (
                                                                                                                          uint
                                                                                                                          )
                                                                                                                          longeventParam
                                                                                                                  }),
                                                                              CameraDevice = this,
                                                                              FileName = filename,
                                                                              Handle = (uint) longeventParam
                                                                          };
                                        OnPhotoCapture(this, args);
                                    }
                                    break;
                                case CONST_Event_CaptureComplete:
                                case CONST_Event_CaptureCompleteRecInSdram:
                                    {
                                        SlowDownEventTimer();
                                        OnCaptureCompleted(this, new EventArgs());
                                    }
                                    break;
                                case CONST_Event_ObsoleteEvent:
                                    break;
                                default:
                                    //Console.WriteLine("Unknown event code " + eventCode.ToString("X"));
                                    Log.Debug("Unknown event code :" + eventCode.ToString("X") + "|" +
                                              longeventParam.ToString("X"));
                                    break;
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Event queue processing error ", exception);
                        }
                    }
                }
            }
            catch (InvalidComObjectException)
            {
                //return;
            }
            catch (Exception)
            {
                //Log.Error("Event exception ", exception);
            }
            _eventIsbusy = false;
            _timer.Start();
        }

        public void DeviceReady()
        {
            DeviceReady(0);
        }

        public void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        public void DeviceReady(int retrynum)
        {
            //uint cod = Convert.ToUInt32(_stillImageDevice.ExecuteWithNoData(CONST_CMD_DeviceReady));
            while (true)
            {
                if (retrynum > 50)
                    return;
                ulong cod = (ulong)ExecuteWithNoData(CONST_CMD_DeviceReady);
                if (cod != 0 && cod != ErrorCodes.MTP_OK)
                {
                    if (cod == ErrorCodes.MTP_Device_Busy || cod == 0x800700AA)
                    {
                       Console.WriteLine("Device not ready");
                        Thread.Sleep(5);
                        retrynum++;
                    }
                    else
                    {
                       Console.WriteLine("Device ready code #0" + cod.ToString("X"));
                    }
                }
                return;
            }
        }

        public override void FormatStorage(object storageId)
        {
            DeviceReady();
            base.FormatStorage(storageId);
        }

        public override void ResetDevice()
        {
            StillImageDevice imageDevice = StillImageDevice as StillImageDevice;
            if (imageDevice != null)
            {
              var resp=  imageDevice.ResetDevice();
                Console.WriteLine(resp.ToString("X"));
            }
        }

        public override string GetProhibitionCondition(OperationEnum operationEnum)
        {
            switch (operationEnum)
            {
                case OperationEnum.Capture:
                    return "";
                case OperationEnum.RecordMovie:
                    MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, 0xD0A4);
                    if (response.Data != null && response.Data.Length > 0)
                    {
                        Int32 resp = BitConverter.ToInt32(response.Data, 0);
                        if (resp == 0)
                            return string.Empty;
                        if (StaticHelper.GetBit(resp, 0))
                            return "LabelNoCardInserted";
                        if (StaticHelper.GetBit(resp, 1))
                            return "LabelCardError";
                        if (StaticHelper.GetBit(resp, 2))
                            return "LabelCardNotFormatted";
                        if (StaticHelper.GetBit(resp, 3))
                            return "LabelNoFreeAreaInCard";
                        if (StaticHelper.GetBit(resp, 7))
                            return "LabelCardBufferNotEmpty";
                        if (StaticHelper.GetBit(resp, 8))
                            return "LabelPcBufferNotEmpty";
                        if (StaticHelper.GetBit(resp, 9))
                            return "LabelBufferNotEmpty";
                        if (StaticHelper.GetBit(resp, 10))
                            return "LabelRecordInProgres";
                        if (StaticHelper.GetBit(resp, 11))
                            return "LabelCardProtected";
                        if (StaticHelper.GetBit(resp, 12))
                            return "LabelDuringEnlargedDisplayLiveView";
                        if (StaticHelper.GetBit(resp, 13))
                            return "LabelWrongLiveViewType";
                        if (StaticHelper.GetBit(resp, 14))
                            return "";
                        //return "LabelNotInApplicationMode";
                    }
                    return "";
                case OperationEnum.AutoFocus:
                    if (FocusMode != null && FocusMode.Value != null && FocusMode.Value.Contains("[M]"))
                        return "LabelMFError";
                    // check if not Single AF servo
                    return "";
                case OperationEnum.ManualFocus:
                    if (FocusMode != null && FocusMode.Value != null && FocusMode.Value.Contains("[M]"))
                        return "LabelMFError";
                    MTPDataResponse responselFocus = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, 0xD061);
                    if (responselFocus != null && (responselFocus.Data != null && responselFocus.Data.Length > 0))
                    {
                        var resp = responselFocus.Data[0];
                        if (resp == 2)
                            return "LabelNotAFSError";
                    }
                    // check if not Single AF servo
                    return "";
                case OperationEnum.LiveView:
                    MTPDataResponse responsel = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, 0xD1A4);
                    if (responsel.Data != null && responsel.Data.Length > 0)
                    {
                        Int32 resp = BitConverter.ToInt32(responsel.Data, 0);
                        if (resp == 0)
                            return string.Empty;
                        if (StaticHelper.GetBit(resp, 0))
                            return "LabelDestinationCardError";
                        if (StaticHelper.GetBit(resp, 2))
                            return "LabelSequenceError";
                        if (StaticHelper.GetBit(resp, 4))
                            return "LabelFullyPressedButtonError";
                        if (StaticHelper.GetBit(resp, 5))
                            return "LabelApertureValueError";
                        if (StaticHelper.GetBit(resp, 6))
                            return "LabelBulbError";
                        if (StaticHelper.GetBit(resp, 7))
                            return "LabelDuringCleaningMirror";
                        if (StaticHelper.GetBit(resp, 8))
                            return "LabelDuringInsufficiencyBattery";
                        if (StaticHelper.GetBit(resp, 9))
                            return "LabelTTLError";
                        if (StaticHelper.GetBit(resp, 11))
                            return "LabelNonCPULEnseError";
                        if (StaticHelper.GetBit(resp, 12))
                            return "LabelImageInRAM";
                        if (StaticHelper.GetBit(resp, 13))
                            return "LabelMirrorUpError2";
                        if (StaticHelper.GetBit(resp, 14))
                            return "LabelNoCardInsertedError";
                        if (StaticHelper.GetBit(resp, 15))
                            return "LabelCommandProcesingError";
                        if (StaticHelper.GetBit(resp, 16))
                            return "LabelShoutingInProgress";
                        if (StaticHelper.GetBit(resp, 17))
                            return "LabelOverHeatedError";
                        if (StaticHelper.GetBit(resp, 18))
                            return "LabelCardProtectedError";
                        if (StaticHelper.GetBit(resp, 19))
                            return "LabelCardError";
                        if (StaticHelper.GetBit(resp, 20))
                            return "LabelCardNotFormatted";
                        if (StaticHelper.GetBit(resp, 21))
                            return "LabelBulbError";
                        if (StaticHelper.GetBit(resp, 22))
                            return "LabelMirrorUpError";
                    }
                    return "";
                default:
                    throw new ArgumentOutOfRangeException("operationEnum");
            }
        }

        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            DeviceReady();
            return base.GetObjects(storageId, loadThumbs);
        }

        public override void TransferFileThumb(object o, string filename)
        {
            lock (Locker)
            {

                using (var fs = File.Open(filename, FileMode.Create))
                {
                    MTPDataResponse result = StillImageDevice.ExecuteReadBigData(CONST_CMD_GetLargeThumb,fs,
                        (total, current) =>
                        {
                            double i = (double) current/total;
                            TransferProgress =
                                Convert.ToUInt32(i*100);
                        }, Convert.ToUInt32(o));
                    if (result.Data != null)
                        fs.Write(result.Data, 0, result.Data.Length);
                }
                _timer.Start();
                TransferProgress = 0;
            }
        }

        public override string ToStringCameraData()
        {
            StringBuilder c = new StringBuilder(base.ToString() + "\n\tType..................Nikon(" + ")");
            c.AppendFormat("\n\tLiveView:");
            c.AppendFormat("\n\t  On..................{0}", LiveViewOn ? "Yes" : "No");
            c.AppendFormat("\n\t  Focus Mode..........{0}", LiveViewFocusMode);
            c.AppendFormat("\n\tNormal:");
            c.AppendFormat("\n\t  Focus Mode..........{0}", NormalFocusMode);
            c.AppendFormat("\n\texposure..............{0} f{1} +/-{2}, ISO{3}",
                    NormalShutterSpeed,
                    NormalFNumber,
                    NormalExposureCompensation,
                    NormalIsoNumber);

            return c.ToString();
        }
    }
}
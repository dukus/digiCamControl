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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using WIA;

#endregion

namespace CameraControl.Devices.Others
{
    public class WiaCameraDevice : BaseCameraDevice
    {
        private Dictionary<int, string> ShutterTable = new Dictionary<int, string>
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
                                                               {15, "1/640"},
                                                               {20, "1/500"},
                                                               {25, "1/400"},
                                                               {31, "1/320"},
                                                               {40, "1/250"},
                                                               {50, "1/200"},
                                                               {62, "1/160"},
                                                               {80, "1/125"},
                                                               {100, "1/100"},
                                                               {125, "1/80"},
                                                               {166, "1/60"},
                                                               {200, "1/50"},
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
                                                               {7692, "1/1.3"},
                                                               {10000, "1s"},
                                                               {13000, "1.3s"},
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
                                                               {-1, "Bulb"},
                                                           };

        private Dictionary<int, string> ExposureModeTable = new Dictionary<int, string>()
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

        private Dictionary<int, string> WbTable = new Dictionary<int, string>()
                                                      {
                                                          {2, "Auto"},
                                                          {4, "Daylight"},
                                                          {5, "Fluorescent "},
                                                          {6, "Incandescent"},
                                                          {7, "Flash"},
                                                          {32784, "Cloudy"},
                                                          {32785, "Shade"},
                                                          {32786, "Kelvin"},
                                                          {32787, "Custom"}
                                                      };

        private Dictionary<int, string> CSTable = new Dictionary<int, string>()
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

        private Dictionary<int, string> EMMTable = new Dictionary<int, string>
                                                       {
                                                           {2, "Center-weighted metering"},
                                                           {3, "Multi-pattern metering"},
                                                           {4, "Spot metering"}
                                                       };

        private Dictionary<uint, string> FMTable = new Dictionary<uint, string>()
                                                       {
                                                           {1, "[M] Manual focus"},
                                                           {0x8010, "[S] Single AF servo"},
                                                           {0x8011, "[C] Continuous AF servo"},
                                                           {0x8012, "[A] AF servo mode automatic switching"},
                                                           {0x8013, "[F] Constant AF servo"},
                                                       };

        #region Implementation of ICameraDevice

        private string _displayName;

        public override string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    return DeviceName + " (" + SerialNumber + ")" + "(WIA)";
                return _displayName;
            }
            set
            {
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }


        //public override bool GetCapability(CapabilityEnum capabilityEnum)
        //{
        //  return false;
        //}


        private Device Device { get; set; }
        internal new object Locker = new object(); // object used to lock multi  
        public DeviceManager DeviceManager { get; set; }

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            IsBusy = false;
            //the device not connected
            try
            {
                ConnectToWiaDevice(deviceDescriptor);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect camera using wia driver", exception);
                return false;
            }
            DeviceManager = new DeviceManager();
            DeviceManager.RegisterEvent(Conts.wiaEventItemCreated, deviceDescriptor.WiaId);
            DeviceManager.OnEvent += DeviceManager_OnEvent;

            try
            {
                Device = deviceDescriptor.WiaDevice;
                DeviceName = Device.Properties["Description"].get_Value();
                Manufacturer = Device.Properties["Manufacturer"].get_Value();
                SerialNumber = StaticHelper.GetSerial(Device.Properties["PnP ID String"].get_Value());
            }
            catch (Exception ex)
            {
                Log.Debug("Init error", ex);
            }
            IsConnected = true;
            try
            {
                try
                {
                    Property apertureProperty = Device.Properties[Conts.CONST_PROP_F_Number];
                    if (apertureProperty != null)
                    {
                        foreach (var subTypeValue in apertureProperty.SubTypeValues)
                        {
                            double d = (int) subTypeValue;
                            string s = (d/100).ToString("0.0");
                            FNumber.AddValues(s, (int) d);
                            FNumber.ReloadValues();
                            if ((int) subTypeValue == (int) apertureProperty.get_Value())
                                FNumber.SetValue((int) d);
                        }
                    }
                }
                catch (COMException)
                {
                    FNumber.IsEnabled = false;
                }

                try
                {
                    Property isoProperty = Device.Properties[Conts.CONST_PROP_ISO_Number];
                    if (isoProperty != null)
                    {
                        foreach (var subTypeValue in isoProperty.SubTypeValues)
                        {
                            IsoNumber.AddValues(subTypeValue.ToString(), (int) subTypeValue);
                            IsoNumber.ReloadValues();
                            if ((int) subTypeValue == (int) isoProperty.get_Value())
                                IsoNumber.SetValue((int) subTypeValue);
                        }
                    }
                }
                catch (COMException)
                {
                    IsoNumber.IsEnabled = false;
                }

                try
                {
                    Property shutterProperty = Device.Properties[Conts.CONST_PROP_Exposure_Time];
                    if (shutterProperty != null)
                    {
                        foreach (int subTypeValue in shutterProperty.SubTypeValues)
                        {
                            if (ShutterTable.ContainsKey((int) subTypeValue))
                                ShutterSpeed.AddValues(ShutterTable[(int) subTypeValue], (int) subTypeValue);
                        }
                        ShutterSpeed.ReloadValues();
                        ShutterSpeed.SetValue(shutterProperty.get_Value());
                    }
                }
                catch (COMException)
                {
                    ShutterSpeed.IsEnabled = false;
                }

                try
                {
                    Property wbProperty = Device.Properties[Conts.CONST_PROP_WhiteBalance];
                    if (wbProperty != null)
                    {
                        foreach (var subTypeValue in wbProperty.SubTypeValues)
                        {
                            if (WbTable.ContainsKey((int) subTypeValue))
                                WhiteBalance.AddValues(WbTable[(int) subTypeValue], (int) subTypeValue);
                        }
                        WhiteBalance.ReloadValues();
                        WhiteBalance.SetValue(wbProperty.get_Value());
                    }
                }
                catch (COMException)
                {
                    WhiteBalance.IsEnabled = false;
                }

                try
                {
                    Property modeProperty = Device.Properties[Conts.CONST_PROP_ExposureMode];
                    if (modeProperty != null)
                    {
                        foreach (var subTypeValue in modeProperty.SubTypeValues)
                        {
                            if (ExposureModeTable.ContainsKey((int) subTypeValue))
                                Mode.AddValues(ExposureModeTable[(int) subTypeValue], Convert.ToUInt32(subTypeValue));
                        }
                        Mode.ReloadValues();
                        Mode.SetValue(Convert.ToUInt32(modeProperty.get_Value()));
                    }
                    Mode.IsEnabled = false;
                }
                catch (COMException)
                {
                    Mode.IsEnabled = false;
                }

                try
                {
                    Property ecProperty = Device.Properties[Conts.CONST_PROP_ExposureCompensation];
                    if (ecProperty != null)
                    {
                        foreach (var subTypeValue in ecProperty.SubTypeValues)
                        {
                            decimal d = (int) subTypeValue;
                            string s = decimal.Round(d/1000, 1).ToString();
                            if (d > 0)
                                s = "+" + s;
                            ExposureCompensation.AddValues(s, (int) subTypeValue);
                        }
                        ExposureCompensation.ReloadValues();
                        ExposureCompensation.SetValue(ecProperty.get_Value());
                    }
                }
                catch (COMException)
                {
                    ExposureCompensation.IsEnabled = false;
                }

                try
                {
                    Property csProperty = Device.Properties[Conts.CONST_PROP_CompressionSetting];
                    if (csProperty != null)
                    {
                        foreach (var subTypeValue in csProperty.SubTypeValues)
                        {
                            if (CSTable.ContainsKey((int) subTypeValue))
                                CompressionSetting.AddValues(CSTable[(int) subTypeValue], (int) subTypeValue);
                        }
                        CompressionSetting.ReloadValues();
                        CompressionSetting.SetValue(csProperty.get_Value());
                    }
                }
                catch (COMException)
                {
                    CompressionSetting.IsEnabled = false;
                }

                try
                {
                    Property emmProperty = Device.Properties[Conts.CONST_PROP_ExposureMeteringMode];
                    if (emmProperty != null)
                    {
                        foreach (var subTypeValue in emmProperty.SubTypeValues)
                        {
                            if (EMMTable.ContainsKey((int) subTypeValue))
                                ExposureMeteringMode.AddValues(EMMTable[(int) subTypeValue], (int) subTypeValue);
                        }
                        ExposureMeteringMode.ReloadValues();
                        ExposureMeteringMode.SetValue(emmProperty.get_Value());
                    }
                }
                catch (COMException)
                {
                    CompressionSetting.IsEnabled = false;
                }

                try
                {
                    Property fmProperty = Device.Properties[Conts.CONST_PROP_FocusMode];
                    if (fmProperty != null)
                    {
                        foreach (int subTypeValue in fmProperty.SubTypeValues)
                        {
                            uint subval = Convert.ToUInt16(subTypeValue);
                            if (FMTable.ContainsKey(subval))
                                FocusMode.AddValues(FMTable[subval], subval);
                        }
                        FocusMode.ReloadValues();
                        FocusMode.SetValue(Convert.ToUInt16((int) fmProperty.get_Value()));
                    }
                }
                catch (COMException)
                {
                    FocusMode.IsEnabled = false;
                }

                try
                {
                    Battery = Device.Properties[Conts.CONST_PROP_BatteryStatus].get_Value();
                }
                catch (COMException)
                {
                    Battery = 0;
                }
                IsConnected = true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                IsConnected = false;
            }
            HaveLiveView = true;
            //Capabilities.Add(CapabilityEnum.LiveView);
            return true;
        }

        private void ConnectToWiaDevice(DeviceDescriptor deviceDescriptor, int retries_left = 6)
        {
            if (deviceDescriptor.WiaDevice == null)
            {
                Thread.Sleep(500);
                try
                {
                    deviceDescriptor.WiaDevice = deviceDescriptor.WiaDeviceInfo.Connect();
                    deviceDescriptor.CameraDevice = this;
                    Thread.Sleep(250);
                }
                catch (COMException e)
                {
                    if ((uint) e.ErrorCode == ErrorCodes.WIA_ERROR_BUSY && retries_left > 0)
                    {
                        int retry_in_secs = 2*(7 - retries_left);
                        Thread.Sleep(1000*retry_in_secs);
                        Log.Debug("Connection to wia failed, Retrying to connect in " + retry_in_secs + " seconds");
                        ConnectToWiaDevice(deviceDescriptor, retries_left - 1);
                    }
                    else
                    {
                        Log.Error("Could not connect to wia device.", e);
                        throw e;
                    }
                }
            }
        }

        private void DeviceManager_OnEvent(string eventId, string deviceId, string itemId)
        {
            Item tem = Device.GetItem(itemId);
            ImageFile imageFile = (ImageFile) tem.Transfer("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                EventArgs = imageFile,
                CameraDevice = this,
                FileName = "00000." + imageFile.FileExtension,
                Handle = new object[] {imageFile, itemId}
            };
            OnPhotoCapture(this, args);
            OnCaptureCompleted(this, new EventArgs());
        }

        public WiaCameraDevice()
        {
            FNumber = new PropertyValue<long>();
            FNumber.ValueChanged += FNumber_ValueChanged;
            IsoNumber = new PropertyValue<long>();
            IsoNumber.ValueChanged += IsoNumber_ValueChanged;
            ShutterSpeed = new PropertyValue<long>();
            ShutterSpeed.ValueChanged += ShutterSpeed_ValueChanged;
            WhiteBalance = new PropertyValue<long>();
            WhiteBalance.ValueChanged += WhiteBalance_ValueChanged;
            Mode = new PropertyValue<long>();
            Mode.ValueChanged += Mode_ValueChanged;
            CompressionSetting = new PropertyValue<long>();
            CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
            ExposureCompensation = new PropertyValue<long>();
            ExposureCompensation.ValueChanged += ExposureCompensation_ValueChanged;
            ExposureMeteringMode = new PropertyValue<long>();
            ExposureMeteringMode.ValueChanged += ExposureMeteringMode_ValueChanged;
            FocusMode = new PropertyValue<long>();
            FocusMode.IsEnabled = false;
        }

        private void ExposureMeteringMode_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_ExposureMeteringMode].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void CompressionSetting_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_CompressionSetting].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void ExposureCompensation_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_ExposureCompensation].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void Mode_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                switch (key)
                {
                    case "M":
                        ShutterSpeed.IsEnabled = true;
                        FNumber.IsEnabled = true;
                        break;
                    case "P":
                        ShutterSpeed.IsEnabled = false;
                        FNumber.IsEnabled = false;
                        break;
                    case "A":
                        ShutterSpeed.IsEnabled = false;
                        FNumber.IsEnabled = true;
                        break;
                    case "S":
                        ShutterSpeed.IsEnabled = true;
                        FNumber.IsEnabled = false;
                        break;
                    default:
                        ShutterSpeed.IsEnabled = false;
                        FNumber.IsEnabled = false;
                        break;
                }
                Device.Properties[Conts.CONST_PROP_ExposureMode].set_Value(val);
            }
        }

        private void WhiteBalance_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_WhiteBalance].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void ShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_Exposure_Time].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void IsoNumber_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_ISO_Number].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }

        private void FNumber_ValueChanged(object sender, string key, long val)
        {
            lock (Locker)
            {
                try
                {
                    Device.Properties[Conts.CONST_PROP_F_Number].set_Value(val);
                }
                catch (Exception)
                {
                }
            }
        }


        public override void StartLiveView()
        {
            //throw new NotImplementedException();
        }

        public override void StopLiveView()
        {
            //throw new NotImplementedException();
        }

        public override LiveViewData GetLiveViewImage()
        {
            //throw new NotImplementedException();
            return new LiveViewData();
        }

        public override void AutoFocus()
        {
            //throw new NotImplementedException();
        }

        public override int Focus(int step)
        {
            return 0;
        }

        public override void Focus(int x, int y)
        {
            //throw new NotImplementedException();
        }

        public override void CapturePhotoNoAf()
        {
            lock (Locker)
            {
                CapturePhoto();
            }
        }

        public override void CapturePhoto()
        {
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                Device.ExecuteCommand(Conts.wiaCommandTakePicture);
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

        public override void Close()
        {
            if (Device != null)
                Marshal.ReleaseComObject(Device);
            Device = null;
            HaveLiveView = false;
            if (DeviceName != null)
                DeviceManager.OnEvent -= DeviceManager_OnEvent;
        }

        public override void ReadDeviceProperties(uint o)
        {
            HaveLiveView = true;
        }

        public override void TransferFile(object o, string filename)
        {
            ImageFile deviceEventArgs = ((object[])(o))[0] as ImageFile;
            if (deviceEventArgs != null)
            {
                deviceEventArgs.SaveFile(filename);
            }
        }

        //public override event PhotoCapturedEventHandler PhotoCaptured;

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            string id = (string)((object[]) (deviceObject.Handle))[1];
            for (int j = 1; j <= Device.Items.Count; j++)
            {
                if (Device.Items[j].ItemID == id)
                {
                    Device.Items.Remove(j);
                    break;
                }

            }
            return true;
        }

        #endregion

        public override string ToStringCameraData()
        {
            StringBuilder c = new StringBuilder(base.ToString() + "\n\tType..................WIA");

            if (Device != null)
                c.AppendFormat("\n\tDevice ID.............{0}", Device.DeviceID);

            return c.ToString();

        }
    }
}
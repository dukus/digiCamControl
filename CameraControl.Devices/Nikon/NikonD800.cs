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
using CameraControl.Devices.Classes;
using PortableDeviceLib;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD800 : NikonBase
    {

        public NikonD800()
        {
            _isoTable = new Dictionary<uint, string>()
            {
                {0x0032, "Lo 1.0"},
                {0x0040, "Lo 0.7"},
                {0x0048, "Lo 0.5"},
                {0x0050, "Lo 0.3"},
                {0x0064, "100"},
                {0x007d, "125"},
                {0x008c, "140"},
                {0x00A0, "160"},
                {0x00C8, "200"},
                {0x00FA, "250"},
                {0x0118, "280"},
                {0x0140, "320"},
                {0x0190, "400"},
                {0x01F4, "500"},
                {0x0230, "560"},
                {0x0280, "640"},
                {0x0320, "800"},
                {0x03E8, "1000"},
                {0x04E2, "1250"},
                {0x0640, "1600"},
                {0x07D0, "2000"},
                {0x0898, "2200"},
                {0x09C4, "2500"},
                {0x0C80, "3200"},
                {0x0FA0, "4000"},
                {0x1194, "4500"},
                {0x1388, "5000"},
                {0x1900, "6400"},
                {0x1f40, "Hi 0.3"},
                {0x2328, "Hi 0.5"},
                {0x2710, "Hi 0.7"},
                {0x3200, "Hi 1"},
                {0x6400, "Hi 2"},
            };

            _autoIsoTable = new Dictionary<byte, string>()
            {
                {0, "200"},
                {1, "250"},
                {2, "280"},
                {3, "320"},
                {4, "400"},
                {5, "500"},
                {6, "560"},
                {7, "640"},
                {8, "800"},
                {9, "1000"},
                {10, "1100"},
                {11, "1250"},
                {12, "1600"},
                {13, "2000"},
                {14, "2200"},
                {15, "2500"},
                {16, "3200"},
                {17, "4000"},
                {18, "4500"},
                {19, "5000"},
                {20, "6400"},
                {21, "Hi 0.3"},
                {22, "Hi 0.5"},
                {23, "Hi 0.7"},
                {24, "Hi 1"},
                {25, "Hi 2"},
            };

            _shutterTable = new Dictionary<uint, string>
            {
                {0x00011f40, "1/8000"},
                {0x00011900, "1/6400"},
                {0x00011770, "1/6000"},
                {0x00011388, "1/5000"},
                {0x00010FA0, "1/4000"},
                {0x00010C80, "1/3200"},
                {0x000109C4, "1/2500"},
                {0x000107d0, "1/2000"},
                {0x00010640, "1/1600"},
                {0x000104e2, "1/1250"},
                {0x000103e8, "1/1000"},
                {0x00010320, "1/800"},
                {0x000102ee, "1/750"},
                {0x00010280, "1/640"},
                {0x000101f4, "1/500"},
                {0x00010190, "1/400"},
                {0x0001015e, "1/350"},
                {0x00010140, "1/320"},
                {0x000100fa, "1/250"},
                {0x000100c8, "1/200"},
                {0x000100b4, "1/180"},
                {0x000100a0, "1/160"},
                {0x0001007d, "1/125"},
                {0x00010064, "1/100"},
                {0x0001005a, "1/90"},
                {0x00010050, "1/80"},
                {0x0001003c, "1/60"},
                {0x00010032, "1/50"},
                {0x0001002d, "1/45"},
                {0x00010028, "1/40"},
                {0x0001001e, "1/30"},
                {0x00010019, "1/25"},
                {0x00010014, "1/20"},
                {0x0001000f, "1/15"},
                {0x0001000d, "1/13"},
                {0x0001000a, "1/10"},
                {0x00010008, "1/8"},
                {0x00010006, "1/6"},
                {0x00010005, "1/5"},
                {0x00010004, "1/4"},
                {0x00010003, "1/3"},
                {0x000a0019, "1/2.5"},
                {0x00010002, "1/2"},
                {0x000a0010, "1/1.6"},
                {0x000a000f, "1/1.5"},
                {0x000a000d, "1/1.3"},
                {0x00010001, "1s"},
                {0x000d000a, "1.3s"},
                {0x000f000a, "1.5s"},
                {0x0010000a, "1.6s"},
                {0x00020001, "2s"},
                {0x0019000a, "2.5s"},
                {0x00030001, "3s"},
                {0x00040001, "4s"},
                {0x00050001, "5s"},
                {0x00060001, "6s"},
                {0x00080001, "8s"},
                {0x000a0001, "10s"},
                {0x000d0001, "13s"},
                {0x000f0001, "15s"},
                {0x00140001, "20s"},
                {0x00190001, "25s"},
                {0x001e0001, "30s"},
                {0xFFFFFFFF, "Bulb"},
                {0xFFFFFFFD, "Time"},
                {0xFFFFFFFE, "Flash Sync"},
            };
        }

        public const int CONST_CMD_TerminateCapture = 0x920C;

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            bool res = base.Init(deviceDescriptor);
            Capabilities.Clear();
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.Bulb);
            Capabilities.Add(CapabilityEnum.RecordMovie);
            Capabilities.Add(CapabilityEnum.CaptureInRam);
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            return res;
        }

        public override void StartBulbMode()
        {
            DeviceReady();
            ExecuteWithNoData(CONST_CMD_EndLiveView);
            DeviceReady();
            ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 1);
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) 0x0001),
                        CONST_PROP_ExposureProgramMode);
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(0xFFFFFFFF),
                        CONST_PROP_ExposureTime);

            ErrorCodes.GetException(CaptureInSdRam
                                        ? ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia, 0xFFFFFFFF,
                                                            0x0001)
                                        : ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia, 0xFFFFFFFF,
                                                            0x0000));
        }

        public override void EndBulbMode()
        {
            lock (Locker)
            {
                DeviceReady();
                ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_TerminateCapture, 0, 0));
                DeviceReady();
                StillImageDevice.ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 0);
            }
        }

        public override void LockCamera()
        {
            StillImageDevice.ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 1);
        }

        public override void UnLockCamera()
        {
            StillImageDevice.ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 0);
        }

        public override void StartRecordMovie()
        {
            SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, CONST_PROP_ApplicationMode);
            base.StartRecordMovie();
        }

        public override void StopRecordMovie()
        {
            base.StopRecordMovie();
            SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0}, CONST_PROP_ApplicationMode);
        }

        protected override void GetAdditionalLiveViewData(LiveViewData viewData, byte[] result)
        {
            viewData.LiveViewImageWidth = ToInt16(result, 8);
            viewData.LiveViewImageHeight = ToInt16(result, 10);

            viewData.ImageWidth = ToInt16(result, 12);
            viewData.ImageHeight = ToInt16(result, 14);

            viewData.FocusFrameXSize = ToInt16(result, 16);
            viewData.FocusFrameYSize = ToInt16(result, 18);

            viewData.FocusX = ToInt16(result, 20);
            viewData.FocusY = ToInt16(result, 22);

            viewData.Focused = result[40] != 1;
            viewData.MovieIsRecording = result[60] == 1;
            viewData.MovieTimeRemain = ToDeciaml(result, 56);

            if (result[29] == 1)
                viewData.Rotation = -90;
            if (result[29] == 2)
                viewData.Rotation = 90;

            viewData.HaveLevelAngleData = true;
            viewData.LevelAngleRolling = ToInt16(result, 44);
            viewData.LevelAnglePitching = ToInt16(result, 48);
            viewData.LevelAngleYawing = ToInt16(result, 52);

            viewData.PeakSoundL = (int)(result[344] / 14.0 * 100);
            viewData.PeakSoundR = (int)(result[345] / 14.0 * 100);
            viewData.SoundL = (int)(result[346] / 14.0 * 100);
            viewData.SoundR = (int)(result[347] / 14.0 * 100);
            viewData.HaveSoundData = true;
        }

        protected override PropertyValue<long> InitExposureDelay()
        {

            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Exposure delay mode",
                IsEnabled = true,
                Code = 0xD06A
            };
            res.AddValues("3 sec", 0);
            res.AddValues("2 sec", 1);
            res.AddValues("1 sec", 2);
            res.AddValues("OFF", 3);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)val }, res.Code);
            return res;
        }

        protected override void InitShutterSpeed()
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
            UInt32 localValue = Convert.ToUInt32(val & 0x00000000FFFFFFFF);

            if (Mode != null && (Mode.Value == "M" || Mode.Value == "S"))
                if (key == "Bulb")
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(0xFFFFFFFF),
                        CONST_PROP_ShutterSpeed);
                else
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(localValue),
                        CONST_PROP_ShutterSpeed);
        }

        private void MovieShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            UInt32 localValue = Convert.ToUInt32(val & 0x00000000FFFFFFFF);

            if (Mode != null && (Mode.Value == "M" || Mode.Value == "S"))
                if (key == "Bulb")
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(0xFFFFFFFF),
                        CONST_PROP_MovieShutterSpeed);
                else
                    SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(localValue),
                        CONST_PROP_MovieShutterSpeed);
        }

        protected override void ReInitShutterSpeed()
        {
            lock (Locker)
            {
                DeviceReady();
                try
                {
                    byte datasize = 4;
                    var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                     CONST_PROP_ShutterSpeed);
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
                    if (result.Data == null || result.Data.Length == 0)
                        return;
                    MovieShutterSpeed.Clear();
                    int type = BitConverter.ToInt16(result.Data, 2);
                    byte formFlag = result.Data[(2 * datasize) + 5];
                    UInt32 defval = BitConverter.ToUInt32(result.Data, datasize + 5);
                    for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                    {
                        UInt32 val = BitConverter.ToUInt32(result.Data, ((2 * datasize) + 6 + 2) + i);
                        MovieShutterSpeed.AddValues("1/" + (val - 0x10000), val);
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
                    //case CONST_PROP_ExposureIndexEx:
                    //    NormalIsoNumber.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                    //                                                        CONST_PROP_ExposureIndexEx), false);
                    //    break;
                    case CONST_PROP_MovieExposureIndex:
                        MovieFNumber.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                            CONST_PROP_MovieExposureIndex), false);
                        break;
                    //case CONST_PROP_ExposureTime:
                    //    NormalShutterSpeed.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                    //                                                           CONST_PROP_ExposureTime), false);
                    //    break;
                    case CONST_PROP_ShutterSpeed:
                        NormalShutterSpeed.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                               CONST_PROP_ShutterSpeed), false);
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
                            if (response.Data != null && response.Data.Length > 0)
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
    }
}
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
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD800 : NikonBase
    {

        public NikonD800()
        {
            _autoIsoTable = new Dictionary<byte, string>()
                                                           {
                                                               {0, "200"},
                                                               {1, "250"},
                                                             //  {2, "280"},
                                                               {3, "320"},
                                                               {4, "400"},
                                                               {5, "500"},
                                                             //  {6, "560"},
                                                               {7, "640"},
                                                               {8, "800"},
                                                               {9, "1000"},
                                                            //   {10, "1100"},
                                                               {11, "1250"},
                                                               {12, "1600"},
                                                               {13, "2000"},
                                                             //  {14, "2200"},
                                                               {15, "2500"},
                                                               {16, "3200"},
                                                               {17, "4000"},
                                                               {18, "4500"},
                                                               {19, "5000"},
                                                               {20, "6400"},
                                                               {21, "Hi 0.3"},
                                                          //     {22, "Hi 0.5"},
                                                               {23, "Hi 0.7"},
                                                               {24, "Hi 1"},
                                                               {25, "Hi 2"},
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
    }
}
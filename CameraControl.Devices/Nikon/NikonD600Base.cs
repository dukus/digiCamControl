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
using System.Threading;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD600Base : NikonD800
    {
        public override LiveViewData GetLiveViewImage()
        {
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
                        return null;
                    GetAdditionalLiveViewData(viewData, result.Data);
                    viewData.ImageDataPosition = headerSize;
                    viewData.ImageData = result.Data;
                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            return viewData;
        }

        protected override void GetAdditionalLiveViewData(LiveViewData viewData, byte[] result)
        {
            viewData.LiveViewImageWidth = ToInt16(result, 8);
            viewData.LiveViewImageHeight = ToInt16(result, 10);

            viewData.ImageWidth = ToInt16(result, 12);
            viewData.ImageHeight = ToInt16(result, 14);

            viewData.FocusFrameXSize = ToInt16(result, 24);
            viewData.FocusFrameYSize = ToInt16(result, 26);

            viewData.FocusX = ToInt16(result, 28);
            viewData.FocusY = ToInt16(result, 30);

            viewData.Focused = result[48] != 1;
            viewData.MovieIsRecording = result[68] == 1;
            viewData.MovieTimeRemain = ToDeciaml(result, 64);

            if (result[37] == 1)
                viewData.Rotation = -90;
            if (result[37] == 2)
                viewData.Rotation = 90;

            viewData.HaveLevelAngleData = true;
            viewData.LevelAngleRolling = ToInt16(result, 52);
            viewData.LevelAnglePitching = ToInt16(result, 56);
            viewData.LevelAngleYawing = ToInt16(result, 60);
            viewData.PeakSoundL = (int)(result[352] / 14.0 * 100);
            viewData.PeakSoundR = (int)(result[353] / 14.0 * 100);
            viewData.SoundL = (int)(result[354] / 14.0 * 100);
            viewData.SoundR = (int)(result[355] / 14.0 * 100);
            viewData.HaveSoundData = true;
        }


        protected override PropertyValue<long> InitStillCaptureMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Still Capture Mode",
                                              IsEnabled = true,
                                              Code = 0x5013,
                                              SubType = typeof (UInt16)
                                          };
            res.AddValues("Single shot (single-frame shooting)", 0x0001);
            res.AddValues("Continuous high-speed shooting (CH)", 0x0002);
            res.AddValues("Continuous low-speed shooting (CL)", 0x8010);
            res.AddValues("Self-timer", 0x8011);
            res.AddValues("Mirror-up", 0x8012);
            res.AddValues("Quiet shooting", 0x8016);
            res.AddValues("Remote control", 0x8017);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
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
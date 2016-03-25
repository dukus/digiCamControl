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

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD7100 : NikonD600Base
    {
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
            viewData.PeakSoundL = (int)(result[352] / 14.0 * 100);
            viewData.PeakSoundR = (int)(result[353] / 14.0 * 100);
            viewData.SoundL = (int)(result[354] / 14.0 * 100);
            viewData.SoundR = (int)(result[355] / 14.0 * 100);
            viewData.HaveSoundData = true;
        }

        protected override void InitFNumber()
        {
            base.InitFNumber();
            MovieFNumber.IsEnabled = false;
        }

        protected override PropertyValue<long> CaptureAreaCrop()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Capture area crop",
                IsEnabled = true,
                Code = 0xD030,
                SubType = typeof(sbyte)
            };
            res.AddValues("DX", 0);
            res.AddValues("1.3x", 1);
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }
    }
}
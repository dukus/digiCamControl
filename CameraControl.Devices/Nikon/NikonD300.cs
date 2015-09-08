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
using CameraControl.Devices.Classes;
using PortableDeviceLib;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD300 : NikonD3X
    {
        public const int CONST_PROP_LiveViewMode = 0xD1A0;

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            var res = base.Init(deviceDescriptor);
            Capabilities.Remove(CapabilityEnum.CaptureNoAf);
            return res;
        }

        public override void StartLiveView()
        {
            if (!CaptureInSdRam)
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, CONST_PROP_RecordingMedia);
            SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, CONST_PROP_LiveViewMode);
            base.StartLiveView();
        }

        public override void StopLiveView()
        {
            base.StopLiveView();
            if (!CaptureInSdRam)
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0}, CONST_PROP_RecordingMedia);
            DeviceReady();
        }

        public override void CapturePhotoNoAf()
        {
            lock (Locker)
            {
                MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus );
                ErrorCodes.GetException(response.ErrorCode);
                // test if live view is on 
                if (response.Data != null && response.Data.Length > 0 && response.Data[0] > 0)
                {
                    if (CaptureInSdRam)
                    {
                        DeviceReady();
                        ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram,
                                                                                   0xFFFFFFFF));
                        return;
                    }
                    StopLiveView();
                }

                DeviceReady();
                byte oldval = 0;
                var val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect );
                if (val.Data != null && val.Data.Length > 0)
                    oldval = val.Data[0];
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 4}, CONST_PROP_AFModeSelect);
                DeviceReady();
                ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCapture));
                if (val.Data != null && val.Data.Length > 0)
                    SetProperty(CONST_CMD_SetDevicePropValue, new[] {oldval}, CONST_PROP_AFModeSelect);
            }
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
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }

        protected override void InitFocusMode()
        {
            base.InitFocusMode();
            FocusMode.IsEnabled = false;
        }
    }
}
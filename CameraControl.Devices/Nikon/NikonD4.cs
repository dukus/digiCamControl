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

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD4 : NikonBase
    {
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
            StillImageDevice.ExecuteWithNoData(CONST_CMD_ChangeCameraMode, 1);
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) 0x0001),
                        CONST_PROP_ExposureProgramMode);
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt32) 0xFFFFFFFF),
                        CONST_PROP_ExposureTime);

            ErrorCodes.GetException(CaptureInSdRam
                                        ? StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia,
                                                                             0xFFFFFFFF,
                                                                             0x0001)
                                        : StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia,
                                                                             0xFFFFFFFF,
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

        protected override PropertyValue<long> InitExposureDelay()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {Name = "Exposure delay mode", IsEnabled = true, Code = 0xD06A};
            res.AddValues("3 sec", 0);
            res.AddValues("2 sec", 1);
            res.AddValues("One sec", 1);
            res.AddValues("OFF", 1);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) val}, res.Code);
            return res;
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
    }
}
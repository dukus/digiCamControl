using System;
using CameraControl.Devices.Classes;
using PortableDeviceLib;

namespace CameraControl.Devices.Nikon
{
    public class NikonD300 : NikonD3X
    {
        public const int CONST_PROP_LiveViewMode = 0xD1A0;

        public override void StartLiveView()
        {
            if (!CaptureInSdRam)
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, CONST_PROP_RecordingMedia, -1);
            SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 1}, CONST_PROP_LiveViewMode, -1);
            base.StartLiveView();
        }

        public override void StopLiveView()
        {
            base.StopLiveView();
            if (!CaptureInSdRam)
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0}, CONST_PROP_RecordingMedia, -1);
            DeviceReady();
        }

        public override void CapturePhotoNoAf()
        {
            lock (Locker)
            {
                MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus, -1);
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
                byte[] val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect, -1);
                if (val != null && val.Length > 0)
                    oldval = val[0];
                SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 4}, CONST_PROP_AFModeSelect, -1);
                DeviceReady();
                ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCapture));
                if (val != null && val.Length > 0)
                    SetProperty(CONST_CMD_SetDevicePropValue, new[] {oldval}, CONST_PROP_AFModeSelect, -1);
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
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code, -1);
            return res;
        }
    }
}

using System;
using CameraControl.Devices.Classes;

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
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16)0x0001),
                        CONST_PROP_ExposureProgramMode, -1);
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt32)0xFFFFFFFF),
                        CONST_PROP_ExposureTime, -1);

            ErrorCodes.GetException(CaptureInSdRam
                                      ? StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia, 0xFFFFFFFF,
                                                                            0x0001)
                                      : StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInMedia, 0xFFFFFFFF,
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
            PropertyValue<long> res = new PropertyValue<long>() { Name = "Exposure delay mode", IsEnabled = true, Code = 0xD06A };
            res.AddValues("3 sec", 0);
            res.AddValues("2 sec", 1);
            res.AddValues("One sec", 1);
            res.AddValues("OFF", 1);
            res.ValueChanged += (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)val }, res.Code, -1);
            return res;
        }

        public override void StartRecordMovie()
        {
            SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_ApplicationMode, -1);
            base.StartRecordMovie();
        }

        public override void StopRecordMovie()
        {
            base.StopRecordMovie();
            SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_ApplicationMode, -1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;
using PortableDeviceLib;

namespace CameraControl.Devices.Nikon
{
    public class NikonL830 : BaseMTPCamera
    {
        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            base.Init(deviceDescriptor);
            Properties.Add(InitFocalLength());
            ExposureCompensation = new PropertyValue<long> {Available = false};
            ExposureMeteringMode = new PropertyValue<long> {Available = false};
            FNumber = new PropertyValue<long> {Available = false};
            IsoNumber = new PropertyValue<long> {Available = false};
            CompressionSetting = new PropertyValue<long> {Available = false};
            Mode = new PropertyValue<long>() {Available = false};
            ShutterSpeed = new PropertyValue<long>() {Available = false};
            WhiteBalance = new PropertyValue<long>() {Available = false};
            FocusMode = new PropertyValue<long>() {Available = false};

            StillImageDevice imageDevice = StillImageDevice as StillImageDevice;
            if (imageDevice != null)
                imageDevice.DeviceEvent += StillImageDevice_DeviceEvent;
            return true;
        }

        protected virtual PropertyValue<long> InitFocalLength()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Focal Length",
                IsEnabled = true,
                Code = 0x5008,
                SubType = typeof(UInt32)
            };
            res.AddValues("3500", 0x0DAC);
            res.AddValues("4600", 0x11F8);
            res.AddValues("5300", 0x14B4);
            res.AddValues("6100", 0x17D4);
            res.AddValues("7300", 0x1C84);
            res.AddValues("8600", 0x2198);
            res.AddValues("10500", 0x2904);
            res.SetValue(StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                            res.Code), false);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val), res.Code);
            return res;
        }

        private void StillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_OBJECT_ADDED)
            {
                var id = e.EventType.DeviceObject.ID;
                PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                {
                    WiaImageItem = null,
                    CameraDevice = this,
                    FileName = e.EventType.DeviceObject.Name,
                    Handle = e.EventType.DeviceObject.ID
                };
                OnPhotoCapture(this, args);
            }
        }
    }
}

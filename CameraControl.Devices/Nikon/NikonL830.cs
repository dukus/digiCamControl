using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
    public class NikonL830 : BaseMTPCamera
    {
        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            base.Init(deviceDescriptor);
            AdvancedProperties.Add(InitFocalLength());

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
    }
}

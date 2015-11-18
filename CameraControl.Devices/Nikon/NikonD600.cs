using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
    public class NikonD600 : NikonD600Base
    {
        protected override PropertyValue<long> InitExposureDelay()
        {

            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Exposure delay mode",
                IsEnabled = true,
                Code = 0xD06A
            };
            res.AddValues("3 sec", 3);
            res.AddValues("2 sec", 2);
            res.AddValues("1 sec", 1);
            res.AddValues("OFF", 0);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) val}, res.Code);
            return res;
        }
    }
}

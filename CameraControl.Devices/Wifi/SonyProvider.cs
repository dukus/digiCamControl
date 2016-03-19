using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Wifi
{
    public class SonyProvider : IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }
        public DeviceDescriptor Connect(string address)
        {
            throw new NotImplementedException();
        }

        public SonyProvider()
        {
            Name = "Sony";
            DefaultIp = "<Auto>";
        }
    }
}

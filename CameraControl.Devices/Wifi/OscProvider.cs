using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Others;

namespace CameraControl.Devices.Wifi
{
    public class OscProvider: IWifiDeviceProvider
    {
        public DeviceDescriptor Connect(string address)
        {

            DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "OscCamera" };
            var cameraDevice = new OscCamera();
            descriptor.CameraDevice = cameraDevice;
            //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
            cameraDevice.Init($"http://{address}");
            return descriptor;
        }

        public string Name { get; set; }
        public string DefaultIp { get; set; }

        public OscProvider()
        {
            Name = "OSC Protocol";
            DefaultIp = "192.168.1.1";
        }
    }
}

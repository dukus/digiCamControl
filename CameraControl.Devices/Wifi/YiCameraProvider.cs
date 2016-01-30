using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Custom;
using CameraControl.Devices.Others;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.TransferProtocol.PtpIp;

namespace CameraControl.Devices.Wifi
{
    public class YiCameraProvider : IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }
        public DeviceDescriptor Connect(string address)
        {
            int port = 7878;
            string ip = address;
            if (address.Contains(":"))
            {
                ip = address.Split(':')[0];
                int.TryParse(address.Split(':')[1], out port);
            }
            YiCameraProtocol protocol = new YiCameraProtocol();
            protocol.Connect(ip, port);

            DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "YiCamera" };
            var cameraDevice = new YiCamera();
            descriptor.StillImageDevice = protocol;
            descriptor.CameraDevice = cameraDevice;
            //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
            cameraDevice.Init(descriptor);
            return descriptor;
        }

        public YiCameraProvider()
        {
            Name = "Xiaomi Yi action camera";
            DefaultIp = "192.168.42.1";
        }
        
    }
}

using System;
using CameraControl.Devices.Classes;
using PanonoTest;

namespace CameraControl.Devices.Wifi
{
    public class PanonoProvider:IWifiDeviceProvider
    {
        public DeviceDescriptor Connect(string address)
        {
            PanonoDiscovery cameraDiscover = new PanonoDiscovery();

            if (cameraDiscover.UDPSocketSetup())
            {
                if (cameraDiscover.MSearch())
                {
                    var camera = new PanonoCamera();
                    camera.Init(cameraDiscover.EndPoint);
                    camera.DeviceName = "Panono";
                    DeviceDescriptor descriptor = new DeviceDescriptor
                    {
                        WpdId = "PanonoCamera",
                        CameraDevice = camera
                    };
                    //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                    return descriptor;
                }
            }
            throw new Exception("No camera was found !");
        }

        public string Name { get; set; }
        public string DefaultIp { get; set; }

        public PanonoProvider()
        {
            Name = "Panono";
            DefaultIp = "<Auto>";
        }
    }
}

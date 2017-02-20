using System;
using CameraControl.Devices.Classes;
using PanonoTest;

namespace CameraControl.Devices.Wifi
{
    public class PanonoProvider:IWifiDeviceProvider
    {
        public DeviceDescriptor Connect(string address)
        {
            if (string.IsNullOrEmpty(address) || address == DefaultIp)
            {
                PanonoDiscovery cameraDiscover = new PanonoDiscovery();

                if (cameraDiscover.UDPSocketSetup())
                {
                    return ConnectCamera(cameraDiscover.EndPoint);
                }
                throw new Exception("No camera was found !");
            }
            else
            {
                return ConnectCamera(address);
            }
        }

        public DeviceDescriptor ConnectCamera(string endpoint)
        {
            var camera = new PanonoCamera();
            camera.Init(endpoint);
            Log.Debug("Camera found at " + endpoint);
            camera.DeviceName = "Panono";
            camera.Manufacturer = "Panono";
            DeviceDescriptor descriptor = new DeviceDescriptor
            {
                WpdId = "PanonoCamera",
                CameraDevice = camera
            };
            //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
            return descriptor;
            //ws://192.168.80.80:42345/
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

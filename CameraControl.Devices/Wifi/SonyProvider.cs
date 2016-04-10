using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Sony;
using SonyCameraCommunication;

namespace CameraControl.Devices.Wifi
{
    public class SonyProvider : IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }
        public DeviceDescriptor Connect(string address)
        {
            CameraDiscovery cameraDiscover = new CameraDiscovery();

            if (cameraDiscover.UDPSocketSetup())
            {
                if (cameraDiscover.MSearch())
                {

                    var cameraResp = cameraDiscover.DeviceDescription();
                    var info = cameraDiscover.AnalyzeDescription(cameraResp);
                    var camera = new SonyWifiCamera();
                    camera.Init(info.Endpoints["camera"]);
                    camera.DeviceName = info.FriendlyName;
                    camera.SerialNumber = info.UDN;
                    DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "SonyWifiCamera" };
                    descriptor.CameraDevice = camera;
                    //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                    return descriptor;
                }
            }
            throw new Exception("No camera was found !");
        }

        public SonyProvider()
        {
            Name = "Sony";
            DefaultIp = "<Auto>";
        }
    }
}

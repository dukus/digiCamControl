using CameraControl.Devices.Classes;
using CameraControl.Devices.GoPro;
using CameraControl.Devices.Others;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Wifi
{
    public class GoProProvider : IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }

        public DeviceDescriptor Connect(string address)
        {
           return Connect(address, null);
        }
        public DeviceDescriptor Connect(string address, GoProBluetoothDevice bluetoothDevice)
        {
            DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "GoProCamera" };
            //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
            using (var client = new WebClient())
            {
                var json = JObject.Parse(client.DownloadString("http://" + address + "/gp/gpControl"));

                Log.Debug($"Model/Firmware: {json["info"]["model_name"]} / {json["info"]["firmware_version"]}");
                var j = json["info"]["firmware_version"].Value<string>().Substring(0, 3);
                switch (json["info"]["firmware_version"].Value<string>().Substring(0, 3))
                {
                    // camera models with Open GoPro support (GoPro 9,10,11 )
                    case "H22":
                    case "H21":
                    case "HD9":
                        {
                            var cameraDevice = new GoPro9Camera();
                            descriptor.CameraDevice = cameraDevice;
                            cameraDevice.Init(address, json, bluetoothDevice);
                        }
                        break;
                    default:
                        {
                            var cameraDevice = new GoProBaseCamera();
                            descriptor.CameraDevice = cameraDevice;
                            cameraDevice.Init(address, json, bluetoothDevice);
                        }
                        break;
                }
            }
            return descriptor;

        }

        public GoProProvider()
        {
            Name = "GoPro Camera";
            DefaultIp = "10.5.5.9";
        }


    }
}

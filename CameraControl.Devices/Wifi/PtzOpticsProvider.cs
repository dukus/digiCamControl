using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Others;


namespace CameraControl.Devices.Wifi
{
    
    public class PtzOpticsProvider : IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }
        public DeviceDescriptor Connect(string address)
        {
            
            var data = GetData("http://" + address + "/cgi-bin/param.cgi?get_device_conf");
            if(!data.ContainsKey("serial_num"))
                throw new Exception("Invalid connection data");
            var camera = new PtzOpticsCamera(address);
            camera.SerialNumber = data["serial_num"];
            camera.Manufacturer = "PTZOptics";
            camera.DeviceName = data["devname"];
            DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "PTZOptics" };
            descriptor.CameraDevice = camera;
            //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
            return descriptor;
        }

        public PtzOpticsProvider()
        {
            Name = "PTZOptics";
            DefaultIp = "192.168.1.1";
        }


        private Dictionary<string, string> GetData(string url)
        {
            var res=new Dictionary<string,string>();
            WebClient Client = new WebClient();
            var data=Client.DownloadString(url);
            foreach (var lines in data.Split('\n'))
            {
                if (lines?.Contains("=") == true)
                {
                    res.Add(lines.Split('=')[0].Trim(), lines.Split('=')[1].Replace("\"",""));
                }
            }
            return res;
        }
    }
}

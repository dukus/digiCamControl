using System;
using CameraControl.Devices.Classes;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.TransferProtocol.DDServer;

namespace CameraControl.Devices.Wifi
{
    public class DDServerProvider:IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }

        public DeviceDescriptor Connect(string address)
        {
            int port = 4757;
            string ip = address;
            if (address.Contains(":"))
            {
                ip = address.Split(':')[0];
                int.TryParse(address.Split(':')[1], out port);
            }
                DdClient client = new DdClient();
                if (!client.Open(ip, port))
                    throw new Exception("No server was found!");
                var devices = client.GetDevices();
                if (devices.Count == 0)
                    throw new Exception("No connected device was found!");

                client.Connect(devices[0]);
                DdServerProtocol protocol = new DdServerProtocol(client);

                if (CameraDeviceManager.GetNativeDriver(protocol.Model) != null)
                {
                    DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "ddserver" };
                    var cameraDevice = (ICameraDevice)Activator.CreateInstance(CameraDeviceManager.GetNativeDriver(protocol.Model));
                    descriptor.StillImageDevice = protocol;

                    //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                    cameraDevice.Init(descriptor);
                    descriptor.CameraDevice = cameraDevice;
                    return descriptor;
                }
                else
                {
                    throw new Exception("Not Supported device " + protocol.Model);
                }
        }

        public DDServerProvider()
        {
            Name = "DSLRDASHBOARDSERVER (Nikon only)";
            DefaultIp = "192.168.1.1";
        }
    }
}

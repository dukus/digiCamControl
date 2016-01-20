using System;
using CameraControl.Devices.Classes;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.TransferProtocol.PtpIp;

namespace CameraControl.Devices.Wifi
{
    public class PtpIpProvider:IWifiDeviceProvider
    {
        public string Name { get; set; }
        public string DefaultIp { get; set; }

        public DeviceDescriptor Connect(string address)
        {
            string ip = address;
            if (address.Contains(":"))
            {
                ip = address.Split(':')[0];
                int port;
                int.TryParse(address.Split(':')[1], out port);
            }
            PtpIpClient client = new PtpIpClient();
            if (!client.Open(ip, 15740))
                throw new Exception("No server was found!");
            PtpIpProtocol protocol = new PtpIpProtocol(client);
            protocol.ExecuteWithNoData(0x1002, 1);

            if (CameraDeviceManager.GetNativeDriver(protocol.Model) != null)
            {
                DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = "ptpip"};
                var cameraDevice = (ICameraDevice)Activator.CreateInstance(CameraDeviceManager.GetNativeDriver(protocol.Model));
                descriptor.StillImageDevice = protocol;
                descriptor.CameraDevice = cameraDevice;
                //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                cameraDevice.Init(descriptor);
                return descriptor;
            }
            else
            {
                throw new Exception("Not Supported device " + protocol.Model);
            }
        }

        public PtpIpProvider()
        {
            Name = "WU 1a/1b,Nikon camera";
            DefaultIp = "192.168.1.1";
        }
    }
}

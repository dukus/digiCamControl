using CameraControl.Devices.Classes;

namespace CameraControl.Devices
{
    public interface IWifiDeviceProvider
    {
        string Name { get; set; }
        string DefaultIp { get; set; }
        DeviceDescriptor Connect(string address);
    }
}

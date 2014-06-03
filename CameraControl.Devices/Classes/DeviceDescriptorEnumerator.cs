using System.Collections.Generic;
using System.Linq;
using Canon.Eos.Framework;

namespace CameraControl.Devices.Classes
{
    public class DeviceDescriptorEnumerator
    {
        public List<DeviceDescriptor> Devices { get; set; }

        public DeviceDescriptorEnumerator()
        {
            Devices = new List<DeviceDescriptor>();
        }

        public DeviceDescriptor GetBySerialNumber(string serial)
        {
            return Devices.FirstOrDefault(deviceDescriptor => deviceDescriptor.SerialNumber == serial);
        }

        public DeviceDescriptor GetByWiaId(string id)
        {
            return Devices.FirstOrDefault(deviceDescriptor => deviceDescriptor.WiaId == id);
        }

        public DeviceDescriptor GetByWpdId(string id)
        {
            return Devices.FirstOrDefault(deviceDescriptor => deviceDescriptor.WpdId == id);
        }

        public DeviceDescriptor GetByEosCamera(EosCamera id)
        {
            return Devices.FirstOrDefault(deviceDescriptor => deviceDescriptor.EosCamera == id);
        }

        public void RemoveDisconnected()
        {
            List<DeviceDescriptor> removedDevices = Devices.Where(deviceDescriptor => !deviceDescriptor.CameraDevice.IsConnected).ToList();
            foreach (DeviceDescriptor deviceDescriptor in removedDevices)
            {
                Devices.Remove(deviceDescriptor);
            }
        }

        public void Add(DeviceDescriptor descriptor)
        {
            Devices.Add(descriptor);
        }

        public void Remove(DeviceDescriptor descriptor)
        {
            Devices.Remove(descriptor);
        }

    }
}

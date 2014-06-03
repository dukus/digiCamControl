#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System.Collections.Generic;
using System.Linq;
using Canon.Eos.Framework;

#endregion

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
            List<DeviceDescriptor> removedDevices =
                Devices.Where(deviceDescriptor => !deviceDescriptor.CameraDevice.IsConnected).ToList();
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
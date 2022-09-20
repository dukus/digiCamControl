using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace CameraControl.Devices.GoPro
{
    public class GDeviceInformation
    {
        public GDeviceInformation(DeviceInformation inDeviceInformation, bool inPresent, bool inConnected)
        {
            DeviceInfo = inDeviceInformation;
            IsPresent = inPresent;
            IsConnected = inConnected;
        }
        public DeviceInformation DeviceInfo { get; set; } = null;
        public bool IsPresent { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public bool IsVisible { get { return IsPresent || IsConnected; } }
        public string DisplayName => DeviceInfo.Name ?? "";

        private GDeviceInformation() { }
    }
}

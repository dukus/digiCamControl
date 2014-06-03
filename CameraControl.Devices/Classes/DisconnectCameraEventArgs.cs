using System;
using Canon.Eos.Framework;
using PortableDeviceLib;

namespace CameraControl.Devices.Classes
{
    public delegate void CameraDisconnectedEventHandler(object sender, DisconnectCameraEventArgs eventArgs);

    public class DisconnectCameraEventArgs : EventArgs
    {
        public string WiaId { get; set; }
        public StillImageDevice StillImageDevice { get; set; }
        public EosCamera EosCamera { get; set; }
    }
}

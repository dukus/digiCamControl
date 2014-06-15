using System;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    internal class InvalidContainerException : Exception
    {
        public InvalidContainerException(string msg) : base(msg)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol
{
    public class MtpProtocol : StillImageDevice, ITransferProtocol
    {
        public MtpProtocol(string deviceId)
            : base(deviceId)
        {
            
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Others
{
    public class FileTransferDevice: FakeCameraDevice
    {

        public FileTransferDevice():base()
        {
            DeviceName = "FileTransfer";
        }

        public override void TransferFile(object o, string filename)
        {
            File.WriteAllBytes(filename,(byte[])o);
        }
    }
}

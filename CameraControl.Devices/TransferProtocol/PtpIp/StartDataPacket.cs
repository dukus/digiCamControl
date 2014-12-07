using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class StartDataPacket:BaseCmd
    {
        
        public int Size { get; set; }
        
        public override void Read(Stream s)
        {
            this.TransactionID = (uint)(readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            this.Size = (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));

            int i = (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
        }
    }
}

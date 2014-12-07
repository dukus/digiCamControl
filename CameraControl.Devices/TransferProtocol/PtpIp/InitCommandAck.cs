using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    internal class InitCommandAck : BaseCmd
    {
        public int SessionId { get; set; }

        public override void Read(Stream s)
        {
            
            byte[] b = new byte[Header.Length];
            SessionId = (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            s.Read(b, 0, (int) (Header.Length - (8 + 4)));
        }
    }
}

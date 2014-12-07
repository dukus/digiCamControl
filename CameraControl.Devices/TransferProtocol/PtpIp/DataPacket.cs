using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class DataPacket : BaseCmd
    {
        public byte[] Data { get; set; }
        public override void Read(Stream s)
        {
            TransactionID = (uint)(readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            Data = new byte[Header.Length - 8 - 4];
            s.Read(Data, 0, (int) (Header.Length - 8 - 4));
        }
    }
}

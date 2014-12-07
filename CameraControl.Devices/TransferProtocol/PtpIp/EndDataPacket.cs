using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class EndDataPacket:BaseCmd
    {
        
        public byte[] Data { get; set; }

        public override void Read(Stream s)
        {
            this.TransactionID = (uint)(readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            Data = new byte[Header.Length - 8 - 4];
            int numBytes = 0;
            while (numBytes != Header.Length-8-4)
            {
                numBytes += s.Read(Data, numBytes, (int)(Header.Length - 8 - 4 - numBytes));// payload.Read(Payload, numBytes, Header.PayloadLength - numBytes);
                //if (callback != null)
                //    callback(Header.PayloadLength, numBytes);
            }
            
        }
    }
}

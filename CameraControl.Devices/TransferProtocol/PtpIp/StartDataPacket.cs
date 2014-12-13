using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class StartDataPacket:BaseCmd
    {
        
        public int Size { get; set; }
        
        public byte[] Data { get; set; }

        
        public override void Read(Stream s)
        {
            this.TransactionID = (uint)(readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            this.Size = (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));

            int i = (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
        }

        public override void Write(Stream s)
        {
            Header.Type = (uint) PtpIpContainerType.Start_Data_Packet;
            Header.Length = (uint) (8 + 4 + 8 );
            Header.Write(s);
            
            WriteInt(TransactionID, s);
            WriteInt((uint) Data.Length, s);
            WriteInt(0, s);
         
            WriteInt((uint) (Data.Length+12), s);
            WriteInt(12, s);
            WriteInt(TransactionID, s);
            s.Write(Data, 0, Data.Length);

        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class CmdResponse:BaseCmd
    {
        public int Code { get; set; }
        public uint TransactionID { get; set; }
        public uint[] Parameters { get; set; }

        public override void Read(Stream s)
        {
            this.Code = readByte(s) | (readByte(s) << 8);
            this.TransactionID = (uint) (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            
            if (Header.Length > 14)
            {
                List<uint> vals = new List<uint>();
                for (int i = 0; i < Header.Length - (8 + 2 + 4)/4; i++)
                {
                    vals.Add((uint) (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24)));
                }
                Parameters = vals.ToArray();
            }
        }
    }
}

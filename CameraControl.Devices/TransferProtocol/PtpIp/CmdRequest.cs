using System.Drawing.Design;
using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class CmdRequest : BaseCmd
    {
        public uint Code { get; set; }
        public uint TransactionID { get; set; }
        public uint[] Parameters { get; set; }
        public uint DataType { get; set; }

        public CmdRequest(uint code, uint datatype = 1)
        {
            TransactionID = PtpIpProtocol.TransactionId++;
            Header = new PtpIpHeader();
            Header.Type = (uint) PtpIpContainerType.Cmd_Request;
            Code = code;
            DataType = datatype;
        }

        public override void Write(Stream s)
        {
            Header.Length = (uint) (8 + 4 + 2 + 4 + (Parameters != null ? (Parameters.Length*4) : 0));
            Header.Write(s);
            // 4 byte unknown. set to 1 currently.
            //s.Write(new byte[] {1, 0, 0, 0}, 0, 4);
            WriteInt(DataType,s);
            s.WriteByte((byte) (0xff & this.Code));
            s.WriteByte((byte) (0xff & (this.Code >> 8)));

            WriteInt(TransactionID, s);

            if (Parameters != null)
            {
                for (int i = 0; i < this.Parameters.Length; i++)
                {
                    WriteInt(Parameters[i], s);
                }
            }
        }

        public override void Read(Stream s)
        {
            Header.Read(s);
            this.Code = (uint) (readByte(s) | (readByte(s) << 8));
            this.TransactionID = (uint) (readByte(s) | (readByte(s) << 8) | (readByte(s) << 16) | (readByte(s) << 24));
            byte[] b = new byte[Header.Length];
            s.Read(b, 0, (int) (Header.Length - (8 + 2 + 4)));
        }
    }
}

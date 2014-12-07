using System.IO;
using CameraControl.Devices.TransferProtocol.DDServer;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class BaseCmd : IPtpIpCommand
    {
        public PtpIpHeader Header { get; set; }
        public uint TransactionID { get; set; }

        public BaseCmd()
        {
            Header = new PtpIpHeader();
        }
        
        public virtual void Write(Stream s)
        {

        }

        public virtual void Read(Stream s)
        {
            
        }

        public void WriteInt(uint val, Stream s)
        {
            s.WriteByte((byte)(0xff & val));
            s.WriteByte((byte)(0xff & (val >> 8)));
            s.WriteByte((byte)(0xff & (val >> 16)));
            s.WriteByte((byte)(0xff & (val >> 24)));
        }

        public int readByte(Stream s)
        {
            int result = s.ReadByte();
            if (result == -1)
                throw new InvalidContainerException("Unexpected end of stream");
            return result;
        }
    }
}

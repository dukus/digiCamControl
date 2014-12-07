using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class PtpIpHeader
    {
        public uint Length { get; set; }

        public uint Type { get; set; }


        public void Write(Stream s)
        {
            s.WriteByte((byte)(0xff & this.Length ));
            s.WriteByte((byte)(0xff & (this.Length  >> 8)));
            s.WriteByte((byte)(0xff & (this.Length  >> 16)));
            s.WriteByte((byte)(0xff & (this.Length  >> 24)));

            s.WriteByte((byte)(0xff & this.Type ));
            s.WriteByte((byte)(0xff & (this.Type  >> 8)));
            s.WriteByte((byte)(0xff & (this.Type  >> 16)));
            s.WriteByte((byte)(0xff & (this.Type  >> 24)));
        }

        public void Read(Stream s)
        {
            Length = (uint) (s.ReadByte() | (s.ReadByte() << 8) | (s.ReadByte() << 16) | (s.ReadByte() << 24));
            Type = (uint)(s.ReadByte() | (s.ReadByte() << 8) | (s.ReadByte() << 16) | (s.ReadByte() << 24));
        }
    }
}

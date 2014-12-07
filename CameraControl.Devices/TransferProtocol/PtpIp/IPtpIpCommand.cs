using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public interface IPtpIpCommand
    {
        PtpIpHeader Header { get; set; }
        void Write(Stream s);
        void Read(Stream s);
    }
}
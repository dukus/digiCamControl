using System.IO;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class InitEventRequest:BaseCmd
    {
        public int SessionId { get; set; }

        public InitEventRequest(int sessionId)
        {
            Header.Type = (uint) PtpIpContainerType.Init_Event_Request;
            Header.Length = 8 + 4;
            SessionId = sessionId;
        }

        public override void Write(Stream s)
        {
            Header.Write(s);
            WriteInt((uint) SessionId, s);
        }


    }
}

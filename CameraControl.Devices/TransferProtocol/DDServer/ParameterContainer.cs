using System.IO;
using ddserverTest;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public class ParameterContainer : Container
    {
        public uint[] Parameters;

        public ParameterContainer()
        {
        }

        public ParameterContainer(uint[] parameters)
        {
            Header = new ContainerHeader();
            Parameters = parameters;
            Header.PayloadLength = payloadLength;
        }

        protected int payloadLength
        {
            get { return (Parameters == null) ? 0 : Parameters.Length*4; }
        }

        public override void WritePayload(Stream s)
        {
            if (payloadLength > 0)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    s.WriteByte((byte) (0xff & Parameters[i]));
                    s.WriteByte((byte) (0xff & (Parameters[i] >> 8)));
                    s.WriteByte((byte) (0xff & (Parameters[i] >> 16)));
                    s.WriteByte((byte) (0xff & (Parameters[i] >> 24)));
                }
            }
        }
    }
}
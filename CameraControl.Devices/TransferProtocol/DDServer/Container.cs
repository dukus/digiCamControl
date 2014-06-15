using System.IO;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public abstract class Container
    {
        public ContainerHeader Header;

        protected Container()
        {
        }

        protected Container(ContainerHeader header)
        {
            Header = header;
        }

        /// <summary>
        /// Write all bytes of this container to stream.
        /// </summary>
        /// <param name="s"></param>
        public void Write(Stream s)
        {
            Header.Write(s);
            WritePayload(s);
        }

        public abstract void WritePayload(Stream s);
    }
}
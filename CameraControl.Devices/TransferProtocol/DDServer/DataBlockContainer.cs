using System.IO;
using ddserverTest;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public class DataBlockContainer : Container
    {
        public byte[] Payload;

        public DataBlockContainer(ContainerHeader header, Stream payload)
        {
            Header = header;
            Payload = new byte[Header.PayloadLength];
            payload.Read(Payload, 0, Header.PayloadLength);
        }

        public DataBlockContainer(int commandCode, byte[] data)
        {
            Header = new ContainerHeader();
            Header.Code = commandCode;
            Header.ContainerType = ContainerType.DataBlock;
            Header.PayloadLength = data.Length;
            Payload = data;
        }

        public override void WritePayload(Stream s)
        {
            s.Write(Payload, 0, Payload.Length);
        }
    }
}
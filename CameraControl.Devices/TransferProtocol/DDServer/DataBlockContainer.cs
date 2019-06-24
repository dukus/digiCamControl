using System;
using System.IO;
using System.Net.Sockets;
using PortableDeviceLib;
using ddserverTest;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public class DataBlockContainer : Container
    {
        public byte[] Payload;

        public DataBlockContainer(ContainerHeader header, Stream payload, StillImageDevice.TransferCallback callback)
        {
            Header = header;
            Payload = new byte[Header.PayloadLength];
//            int readnum = payload.Read(Payload, 0, Header.PayloadLength);
            int numBytes = 0;
            while (numBytes != Header.PayloadLength)
            {
                numBytes += payload.Read(Payload, numBytes, Header.PayloadLength - numBytes);
                if (callback != null)
                    callback((uint)Header.PayloadLength, (uint)numBytes);
            }
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
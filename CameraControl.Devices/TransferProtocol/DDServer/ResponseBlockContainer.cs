using System;
using System.IO;
using CameraControl.Devices.TransferProtocol.DDServer;

namespace ddserverTest
{
    public class ResponseBlockContainer : ParameterContainer
    {
        /// <summary>
        /// This constructor is used mainly for testing, because response blocks are
        /// usually read from stream
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="parameters"></param>
        public ResponseBlockContainer(int responseCode, params uint[] parameters)
            : base(parameters)
        {
            Header.Code = responseCode;
            Header.ContainerType = ContainerType.ResponseBlock;
        }

        /// <summary>
        /// Read response block from stream
        /// </summary>
        /// <param name="header">the container header</param>
        /// <param name="payload">data after header</param>
        public ResponseBlockContainer(ContainerHeader header, Stream payload)
        {
            if (payload == null)
                throw new ArgumentNullException("payload");

            Header = header;
            readParameters(payload);
        }

        protected void readParameters(Stream stream)
        {
            //PIMA 15740:2000: Response datasets may have at most five parameters
            //-> this operation doesn't use too much memory
            int pLength = Header.PayloadLength;
            if (pLength < 0)
                throw new InvalidContainerException("Invalid container length");
            if ((pLength%4) != 0)
                throw new InvalidContainerException("Response payload length must by multiplier of 4");
            if (pLength > 20)
                throw new InvalidContainerException("Response datasets may have at most five parameters");

            int i = 0;
            Parameters = new uint[pLength/4];
            while (i < Parameters.Length)
            {
                Parameters[i++] =
                    (uint)
                    (stream.ReadByte() | (stream.ReadByte() << 8) | (stream.ReadByte() << 16) |
                     (stream.ReadByte() << 24));
            }
        }
    }
}
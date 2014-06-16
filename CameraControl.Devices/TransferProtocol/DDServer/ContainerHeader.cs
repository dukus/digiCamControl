using System;
using System.IO;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public class ContainerHeader
    {
        /// <summary>
        /// Header length in bytes
        /// </summary>
        public const int HeaderLength = 12;

        /// <summary>
        /// USB Still Image Capture Device Definition: This field contains the PIMA 15740 OperationCode, ResponseCode, or EventCode.
        /// </summary>
        public int Code;

        /// <summary>
        /// This field describes the type of the container
        /// </summary>
        public ContainerType ContainerType = ContainerType.Undefined;

        /// <summary>
        /// Container Length (including payload length)
        /// </summary>
        public int Length;

        /// <summary>
        /// USB Still Image Capture Device Definition: This is a host generated number that associates all phases of an PIMA15740 operation
        /// </summary>
        public int TransactionID;

        public ContainerHeader()
        {
        }

        public ContainerHeader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Length = readByte(stream) | (readByte(stream) << 8) | (readByte(stream) << 16) | (readByte(stream) << 24);
            ContainerType = (ContainerType) readByte(stream);
            //byte 5 skipped
            readByte(stream);
            Code = readByte(stream) | (readByte(stream) << 8);
            TransactionID = readByte(stream) | (readByte(stream) << 8) | (readByte(stream) << 16) |
                            (readByte(stream) << 24);
        }

        public int PayloadLength
        {
            get { return Length - HeaderLength; }
            set { Length = HeaderLength + value; }
        }

        private static int readByte(Stream s)
        {
            int result = s.ReadByte();
            if (result == -1)
                throw new InvalidContainerException("Unexpected end of stream");
            return result;
        }

        public void Write(Stream s)
        {
            s.WriteByte((byte) (0xff & Length));
            s.WriteByte((byte) (0xff & (Length >> 8)));
            s.WriteByte((byte) (0xff & (Length >> 16)));
            s.WriteByte((byte) (0xff & (Length >> 24)));
            s.WriteByte((byte) (0xff & (int) ContainerType));
            s.WriteByte(0);
            s.WriteByte((byte) (0xff & Code));
            s.WriteByte((byte) (0xff & (Code >> 8)));
            s.WriteByte((byte) (0xff & TransactionID));
            s.WriteByte((byte) (0xff & (TransactionID >> 8)));
            s.WriteByte((byte) (0xff & (TransactionID >> 16)));
            s.WriteByte((byte) (0xff & (TransactionID >> 24)));
        }
    }
}
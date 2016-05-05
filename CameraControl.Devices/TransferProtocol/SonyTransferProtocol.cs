using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol
{
    public class SonyTransferProtocol : ITransferProtocol
    {
        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public string SerialNumber { get; private set; }
        public bool IsConnected { get; set; }
        public string DeviceId { get; private set; }


        public MTPDataResponse ExecuteReadBigData(uint code, Stream stream, StillImageDevice.TransferCallback callback, params uint[] parameters)
        {
            throw new NotImplementedException();
        }

        public MTPDataResponse ExecuteReadData(uint code, params uint[] parameters)
        {
            throw new NotImplementedException();
        }

        public uint ExecuteWithNoData(uint code, params uint[] parameters)
        {
            throw new NotImplementedException();
        }

        public uint ExecuteWriteData(uint code, byte[] data, params uint[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}

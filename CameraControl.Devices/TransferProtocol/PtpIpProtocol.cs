using System;
using System.IO;
using System.Text;
using CameraControl.Devices.TransferProtocol.PtpIp;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol
{
    public class PtpIpProtocol: ITransferProtocol
    {
        public static uint TransactionId { get; set; }

        private object _locker = new object();

        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public string SerialNumber { get; private set; }
        public bool IsConnected { get; set; }
        public string DeviceId { get; private set; }

        private PtpIpClient _client;

        public PtpIpProtocol(PtpIpClient client)
        {
            TransactionId = 1;
            _client = client;
            LoadDeviceInfo();
            IsConnected = true;
        }

        private void LoadDeviceInfo()
        {
            var res = ExecuteReadData(0x1001);
            int index = 2 + 4 + 2;
            int vendorDescCount = res.Data[index];
            index += vendorDescCount * 2;
            index += 3;
            int comandsCount = res.Data[index];
            index += 2;
            // load commands
            for (int i = 0; i < comandsCount; i++)
            {
                index += 2;
            }
            index += 2;
            int eventcount = res.Data[index];
            index += 2;
            // load events
            for (int i = 0; i < eventcount; i++)
            {
                index += 2;
            }
            index += 2;
            int propertycount = res.Data[index];
            index += 2;
            // load properties codes
            for (int i = 0; i < propertycount; i++)
            {
                index += 2;
            }
            index += 2;
            int formatscount = res.Data[index];
            index += 2;
            // load properties codes
            for (int i = 0; i < formatscount; i++)
            {
                index += 2;
            }
            index += 2;
            int imageformatscount = res.Data[index];
            index += 2;
            // load properties codes
            for (int i = 0; i < imageformatscount; i++)
            {
                index += 2;
            }
            index += 2;
            int strlen1 = res.Data[index] * 2;
            index += 1;
            Manufacturer = Encoding.Unicode.GetString(res.Data, index, strlen1 - 2);
            index += strlen1;
            int strlen2 = res.Data[index] * 2;
            index += 1;
            Model = Encoding.Unicode.GetString(res.Data, index, strlen2 - 2);
            if (Model.Contains("\0"))
                Model = Model.Substring(0, Model.IndexOf('\0'));
            index += strlen2;
            int strlen3 = res.Data[index] * 2;
            index += 1;
            index += strlen3;
            int strlen4 = res.Data[index] * 2;
            index += 1;
            SerialNumber = Encoding.Unicode.GetString(res.Data, index, strlen4 - 2);
        }

        public MTPDataResponse ExecuteReadBigData(uint code,Stream stream, StillImageDevice.TransferCallback callback, params uint[] parameters)
        {
            lock (_locker)
            {
                var cmd = new CmdRequest(code) { Parameters = parameters };
                _client.Write(cmd);

                var res1 = _client.Read();


                var response = res1 as CmdResponse;
                if (response != null)
                {
                    return new MTPDataResponse() { ErrorCode = (uint)response.Code };
                }

                var response1 = res1 as StartDataPacket;
                var res2 = _client.Read(callback, stream);
                var res3 = (EndDataPacket)res2;

                var res4 = _client.Read();
                return new MTPDataResponse() { Data = res3.Data };
            }

        }

        public MTPDataResponse ExecuteReadData(uint code, params uint[] parameters)
        {
            lock (_locker)
            {
                var cmd = new CmdRequest(code) {Parameters = parameters};
                _client.Write(cmd);

                var res1 = _client.Read();


                var response = res1 as CmdResponse;
                if (response != null)
                {
                    return new MTPDataResponse() {ErrorCode = (uint) response.Code};
                }

                var response1 = res1 as StartDataPacket;
                var res2 = _client.Read();
                var res3 = (EndDataPacket) res2;

                var res4 = _client.Read();
                return new MTPDataResponse() {Data = res3.Data};
            }
        }


        public uint ExecuteWithNoData(uint code, params uint[] parameters)
        {
            lock (_locker)
            {
                _client.Write(new CmdRequest(code) {Parameters = parameters});

                var res1 = _client.Read();


                var response = res1 as CmdResponse;
                if (response != null)
                {
                    return (uint) response.Code;
                }
                return 0;
            }
        }

        public uint ExecuteWriteData(uint code, byte[] data, params uint[] parameters)
        {
            //6->
            //9->
            //7<-
            lock (_locker)
            {
                var cmd = new CmdRequest(code,2) {Parameters = parameters};
                _client.Write(cmd);
                _client.Write(new StartDataPacket() {Data = data, TransactionID = cmd.TransactionID});

                var res1 = _client.Read();

                var response = res1 as CmdResponse;
                if (response != null)
                {
                    return (uint) response.Code;
                }
            }
            return 0;
        }

        public void Disconnect()
        {
            //throw new NotImplementedException();
        }
    }
}

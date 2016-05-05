using System;
using System.IO;
using System.Text;
using CameraControl.Devices.TransferProtocol.DDServer;
using PortableDeviceLib;
using ddserverTest;

namespace CameraControl.Devices.TransferProtocol
{
    public class DdServerProtocol : ITransferProtocol
    {
        private DdClient _client;
        private uint _sessionId;
        private readonly object _syncRoot = new object();


        #region Implementation of ITransferProtocol
        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public string SerialNumber { get; private set; }
        public bool IsConnected { get; set; }
        public string DeviceId { get; private set; }


        public MTPDataResponse ExecuteReadBigData(uint code,Stream stream ,StillImageDevice.TransferCallback callback, params uint[] parameters)
        {
            lock (_syncRoot)
            {
                ReconnectIfNeeded();
                DataBlockContainer data;
                var res = new MTPDataResponse();
                _client.Write(new CommandBlockContainer((int)code, parameters));
                int len = _client.ReadInt();
                Container resp = _client.ReadContainer(callback);
                if (resp.Header.Length >= len - 4)
                {
                    return new MTPDataResponse() { ErrorCode = (uint)resp.Header.Code };
                }

                data = (DataBlockContainer)resp;
                resp = _client.ReadContainer();
                return new MTPDataResponse() { Data = data.Payload, ErrorCode = (uint)data.Header.Code };
            }

        }

        public MTPDataResponse ExecuteReadData(uint code, params uint[] parameters)
        {
            lock (_syncRoot)
            {
                ReconnectIfNeeded();
                DataBlockContainer data;
                var res = new MTPDataResponse();
                _client.Write(new CommandBlockContainer((int) code, parameters));
                int len = _client.ReadInt();
                Container resp = _client.ReadContainer();
                if (resp.Header.Length >= len - 4)
                {
                    return new MTPDataResponse() {ErrorCode = (uint) resp.Header.Code};
                }

                data = (DataBlockContainer) resp;
                resp = _client.ReadContainer();
                return new MTPDataResponse() {Data = data.Payload, ErrorCode = (uint) data.Header.Code};
            }
        }

        public uint ExecuteWithNoData(uint code, params uint[] parameters)
        {
            lock (_syncRoot)
            {
                ReconnectIfNeeded();
                _client.Write(new CommandBlockContainer((int) code, parameters));
                int len = _client.ReadInt();
                Container resp = _client.ReadContainer();
                if (resp.Header.Length >= len - 4)
                {
                    return (uint) resp.Header.Code;
                }

                resp = _client.ReadContainer();
                return (uint) resp.Header.Code;
            }
        }

        public uint ExecuteWriteData(uint code, byte[] data, params uint[] parameters)
        {
            lock (_syncRoot)
            {
                ReconnectIfNeeded();
                _client.Write(new CommandBlockContainer((int)code, parameters), new DataBlockContainer((int)code, data));
                int len = _client.ReadInt();
                Container resp = _client.ReadContainer();
                if (resp.Header.Length >= len - 4)
                {
                    return (uint)resp.Header.Code;
                }

                resp = _client.ReadContainer();
                return (uint)resp.Header.Code;
            }
        }

        public void Disconnect()
        {

        }

        #endregion

        public DdServerProtocol(DdClient client)
        {
            Init(client);
        }

        public void Init(DdClient client)
        {
            _client = client;
            LoadDeviceInfo();
            OpenSession();
        }

        public void OpenSession()
        {
            _sessionId++;
            ExecuteWithNoData(0x1002, _sessionId);
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
            int strlen1 = res.Data[index]*2;
            index += 1;
            Manufacturer = Encoding.Unicode.GetString(res.Data, index, strlen1-2);
            index += strlen1;
            int strlen2 = res.Data[index] * 2;
            index += 1;
            Model = Encoding.Unicode.GetString(res.Data, index, strlen2-2);
            index += strlen2;
            int strlen3 = res.Data[index] * 2;
            index += 1;
            index += strlen3;
            int strlen4 = res.Data[index] * 2;
            index += 1;
            SerialNumber = Encoding.Unicode.GetString(res.Data, index, strlen4-2);
        }

        private void ReconnectIfNeeded()
        {
            if(!_client.IsConnected())
            {
                _client.Reconnect();
                Init(_client);
            }
        }
    }
}

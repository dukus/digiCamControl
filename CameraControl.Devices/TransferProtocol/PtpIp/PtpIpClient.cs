using System;
using System.IO;
using System.Net.Sockets;
using ddserverTest;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public class PtpIpClient
    {
        private Stream _inerStream;
        private TcpClient _client;

        private Stream _eventinerStream;
        private TcpClient _eventclient;

        private object _commandLock = new object();

        public bool Open(string ip, int port)
        {
            try
            {
                var header = new PtpIpHeader();

                _client = new TcpClient();
                //_client.SendTimeout = 0;
                //_client.ReceiveTimeout = 0;
                _client.Connect(ip, port);
                _client.ReceiveTimeout = 300000;
                _client.SendTimeout = 300000;
                _inerStream = _client.GetStream();

                Write(new InitCommandRequest());
                InitCommandAck res = (InitCommandAck)Read();

                _eventclient = new TcpClient();
                _eventclient.Connect(ip, port);

                _eventinerStream = _eventclient.GetStream();

                WriteEvent(new InitEventRequest(res.SessionId));
                header.Read(_eventinerStream);

                header.Length = 8;
                header.Type = (uint)PtpIpContainerType.Ping;
                WriteEvent(header);

                header.Read(_eventinerStream);

            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
            return true;
        }

        public int ReadInt()
        {
            return _inerStream.ReadByte() | (_inerStream.ReadByte() << 8) | (_inerStream.ReadByte() << 16) | (_inerStream.ReadByte() << 24);
        }


        private byte[] ReadPacket()
        {
            int length = ReadInt();
            byte[] buff = new byte[length - 4];
            _inerStream.Read(buff, 0, length - 4);
            return buff;
        }

        public void Write(IPtpIpCommand container)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                container.Write(ms);
                _inerStream.Write(ms.ToArray(), 0, (int)ms.Length);
                //if(container.Header.Length!=ms.Length)
                //    throw new Exception("Wrong length");
                _inerStream.Flush();
            }
        }

        public IPtpIpCommand Read(StillImageDevice.TransferCallback callback = null, Stream stream = null)
        {
            var header = new PtpIpHeader();
            header.Read(_inerStream);
            switch ((PtpIpContainerType)header.Type)
            {
                case PtpIpContainerType.Init_Command_Request:
                    break;
                case PtpIpContainerType.Init_Command_Ack:
                    var initcommand = new InitCommandAck() { Header = header };
                    initcommand.Read(_inerStream);
                    return initcommand;
                case PtpIpContainerType.Init_Event_Request:
                    break;
                case PtpIpContainerType.Init_Event_Ack:
                    break;
                case PtpIpContainerType.Init_Fail:
                    break;
                case PtpIpContainerType.Cmd_Request:
                    break;
                case PtpIpContainerType.Cmd_Response:
                    var cmdresp = new CmdResponse() { Header = header };
                    cmdresp.Read(_inerStream);
                    return cmdresp;
                case PtpIpContainerType.Event:
                    break;
                case PtpIpContainerType.Start_Data_Packet:
                    var stardatares = new StartDataPacket() { Header = header };
                    stardatares.Read(_inerStream);
                    return stardatares;
                case PtpIpContainerType.Data_Packet:
                    var data = new DataPacket() { Header = header };
                    data.Read(_inerStream);
                    return data;
                case PtpIpContainerType.Cancel_Transaction:
                    break;
                case PtpIpContainerType.End_Data_Packet:
                    var enddata = new EndDataPacket() { Header = header };
                    if (stream != null)
                        enddata.Read(_inerStream, callback, stream);
                    else
                        enddata.Read(_inerStream, callback);
                    return enddata;
                case PtpIpContainerType.Ping:
                    break;
                case PtpIpContainerType.Pong:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        public void Write(PtpIpHeader header)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                header.Write(ms);
                _inerStream.Write(ms.ToArray(), 0, (int)ms.Length);
                _inerStream.Flush();
            }
        }

        public void WriteEvent(IPtpIpCommand container)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                container.Write(ms);
                _eventinerStream.Write(ms.ToArray(), 0, (int)ms.Length);
                if (container.Header.Length != ms.Length)
                    throw new Exception("Wrong length");
            }
        }

        public void WriteEvent(PtpIpHeader container)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                container.Write(ms);
                _eventinerStream.Write(ms.ToArray(), 0, (int)ms.Length);
            }
        }

        //public virtual Container ReadContainer()
        //{
        //    Container result = getContainer(false);
        //    return result;
        //}


    }
}

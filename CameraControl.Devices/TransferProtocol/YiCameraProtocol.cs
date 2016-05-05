using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol
{
    public class YiCameraProtocol : ITransferProtocol
    {
        private object _locker = new object();
        public event DataReceiverdHandler DataReceiverd;

        public delegate void DataReceiverdHandler(object sender, string data);

        public string Token { get; set; }

        // My Attributes
        private Socket m_sock;						// Server connection
        private byte[] m_byBuff = new byte[256];	// Recieved data buffer
        private int bracker = 0;
        private string data = "";

        public string Ip { get; set; }

        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public string SerialNumber { get; private set; }
        public bool IsConnected { get; set; }
        public string DeviceId { get; private set; }

        public YiCameraProtocol()
        {
            Manufacturer = "Xiaomi";
            Model = "Yi action camera";
        }

        public void Connect(string address, int port)
        {
            Ip = address;
            // Close the socket if it is still open
            if (m_sock != null && m_sock.Connected)
            {
                m_sock.Shutdown(SocketShutdown.Both);
                Thread.Sleep(10);
                m_sock.Close();
            }

            // Create the socket object
            m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress host = IPAddress.Parse(address);
            IPEndPoint hostep = new IPEndPoint(host, port);

            // Connect to server non-Blocking method
            m_sock.Blocking = false;
            AsyncCallback onconnect = new AsyncCallback(OnConnect);
            m_sock.BeginConnect(hostep, onconnect, m_sock);
            var t = DateTime.Now;
            while (!m_sock.Connected)
            {
                Thread.Sleep(50);
                if ((DateTime.Now - t).TotalMilliseconds > 3000)
                    throw new Exception("Connection timeout.");
            }
            // get token
            SendCommand(257);
            t = DateTime.Now;
            while (string.IsNullOrEmpty(Token))
            {
                Thread.Sleep(50);
                if ((DateTime.Now - t).TotalMilliseconds > 3000)
                    throw new Exception("Unable to get token.");

            }
//            SendCommand(3);
        }

        public void SendCommand(int command)
        {
            m_sock.Send(
                    Encoding.UTF8.GetBytes(String.Format("{{\"msg_id\":{0},\"token\":{1}}}", command, Token ?? "0")));
        }

        public void SendCommand(int command, string param)
        {
                var data = String.Format("{{\"msg_id\":{0},\"param\":\"{1}\",\"token\":{2}}}", command, param,
                    Token ?? "0");
                m_sock.Send(Encoding.UTF8.GetBytes(data));
        }

        public void SendValue(string param, string value)
        {
                var data = String.Format("{{\"type\":\"{0}\",\"msg_id\":2,\"param\":\"{1}\",\"token\":{2}}}", param,
                    value,
                    Token ?? "0");

                m_sock.Send(Encoding.UTF8.GetBytes(data));
        }

        public void OnConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we were sucessfull
            try
            {
                //sock.EndConnect( ar );
                if (sock.Connected)
                    SetupRecieveCallback(sock);
                //else
                //    MessageBox.Show(this, "Unable to connect to remote machine", "Connect Failed!");
            }
            catch (Exception ex)
            {
                Log.Error("OnRecievedData", ex);   
            }
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Wrote the data to the List
                    data += Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);

                    // make sure the json string is valid
                    if (data.ToCharArray().Where((x) => x == '{').Count() ==
                        data.ToCharArray().Where((x) => x == '}').Count())
                    {
                        Console.WriteLine(data + "**************");
                        // check if multile json strings
                        if (data.Contains("}{"))
                        {
                            data = data.Replace("}{", "}|{");
                            var datas = data.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in datas)
                            {
                                ProcessData(s);                            
                            }
                        }
                        else
                        {
                            ProcessData(data);                            
                        }
                        data = "";
                    }
                    // If the connection is still usable restablish the callback
                    SetupRecieveCallback(sock);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Client {0}, disconnected",
                                       sock.RemoteEndPoint);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("OnRecievedData", ex);
            }
        }

        private void ProcessData(string data)
        {
            try
            {
                dynamic resp = JsonConvert.DeserializeObject(data);
                switch ((string) resp.msg_id)
                {
                    case "257": // token
                        Token = resp.param;
                        break;
                    default:
                        OnDataReceiverd(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("SetupRecieveCallback", ex);
            }
        }
        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        public void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock);
            }
            catch (Exception ex)
            {
                Log.Error("SetupRecieveCallback", ex);
            }
        }


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
            
        }

        protected virtual void OnDataReceiverd(string data)
        {
            var handler = DataReceiverd;
            if (handler != null) handler(this, data);
        }
    }
}

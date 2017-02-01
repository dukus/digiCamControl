using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace CameraControl.Devices.Wifi
{
    public class PanonoDiscovery
    {
        

        public string EndPoint { get; set; }

        IPEndPoint multicastEndpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

        Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        public static string response;

        public bool UDPSocketSetup()
        {
            string udpStatus;
            bool binded = false;
            try
            {
                udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //udpSocket.Bind(localEndPoint);
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                    new MulticastOption(multicastEndpoint.Address, IPAddress.Any));
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties ip_properties = adapter.GetIPProperties();
                    if (!adapter.GetIPProperties().MulticastAddresses.Any())
                        continue; // most of VPN adapters will be skipped
                    if (!adapter.SupportsMulticast)
                        continue; // multicast is meaningless for this type of connection
                    if (OperationalStatus.Up != adapter.OperationalStatus)
                        continue; // this adapter is off or not connected
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                        continue;

                    IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
                    if (null == p)
                        continue; // IPv4 is not configured on this adapter

                    foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {

                            udpSocket.Bind(new IPEndPoint(ip.Address, 1900));
                            binded = true;
                            break;
                        }
                    }

                    // now we have adapter index as p.Index, let put it to socket option
                    udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
                        (int)IPAddress.HostToNetworkOrder(p.Index));
                }

                if (!binded)
                    throw new Exception("Camea not connected !");
                return true;
            }
            catch (Exception exc)
            {
                Log.Debug("Camera discovery fail", exc);
                return false;
            }
        }

        public bool MSearch()
        {
            string searchString = "M-SEARCH * HTTP/1.1\r\n" +
                                  "MX: 10\r\n" +
                                  "HOST: 239.255.255.250:1900\r\n" +
                                  "MAN: \"ssdp:discover\"\r\n" +
                                  "NT: panono:ball-camera\r\n" +
                                  "\r\n";


            byte[] receiveBuffer = new byte[64000];

            int receivedBytes = 0;
            DateTime starTime = DateTime.Now;

            while (true)
            {
                try
                {

                    for (int i = 0; i < 5; i++)
                    {
                        if (udpSocket.Available > 0)
                        {

                            receivedBytes = udpSocket.Receive(receiveBuffer, SocketFlags.None);
                            response = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);
                            var lines = response.Split('\n');
                            foreach (string line in lines)
                            {
                                if (line.StartsWith("LOCATION"))
                                {
                                    udpSocket.Close();
                                    var data = line.Split(' ');
                                    EndPoint = data[1];
                                    Log.Debug("Camera found at " + EndPoint);
                                    return true;
                                }
                            }
                        }
                        Thread.Sleep(150);
                    }
                    udpSocket.SendTo(Encoding.UTF8.GetBytes(searchString), SocketFlags.None, multicastEndpoint);
                }
                catch (Exception exc)
                {
                    Log.Debug("Camera connection fail", exc);
                    return false;
                }
                if ((DateTime.Now - starTime).TotalSeconds > 1000)
                {
                    return false;
                }
                Thread.Sleep(150);
            }
        }

    }
}

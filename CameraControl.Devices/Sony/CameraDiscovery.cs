using Kazyx.DeviceDiscovery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SonyCameraCommunication
{
    public class CameraDiscovery
    {

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("128.0.0.1"), 60000);
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
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastEndpoint.Address, IPAddress.Any));
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
                            udpSocket.Bind(new IPEndPoint(ip.Address, 60000));
                            binded = true;
                            break;
                        }
                    }

                    // now we have adapter index as p.Index, let put it to socket option
                    udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(p.Index));
                }
                
                if(!binded)
                    throw new Exception("Camea not connected !");
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public bool MSearch()
        {
            string searchString = "M-SEARCH * HTTP/1.1\r\nHOST:239.255.255.250:1900\r\nMAN:\"ssdp:discover\"\r\nMX:1\r\nST:urn:schemas-sony-com:service:ScalarWebAPI:1\r\n\r\n";

           
            byte[] receiveBuffer = new byte[64000];

            int receivedBytes = 0;
            DateTime starTime = DateTime.Now;

            while (true)
            {
                try
                {
                    udpSocket.SendTo(Encoding.UTF8.GetBytes(searchString), SocketFlags.None, multicastEndpoint);
                    if (udpSocket.Available > 0)
                    {
                        receivedBytes = udpSocket.Receive(receiveBuffer, SocketFlags.None);

                        if (receivedBytes > 0)
                        {
                            response = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);
                            return true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    return false;
                }
                if ((DateTime.Now - starTime).TotalSeconds > 4000)
                {
                    return false;
                }
                Thread.Sleep(150);
            }
        }

        public string DeviceDescription()
        {
            string[] responseStrings = response.Split('\n');
            string cameraIP = "";
            foreach (string resp in responseStrings)
            {
                if (resp.StartsWith("LOCATION: "))
                {
                    cameraIP = resp.Substring(resp.LastIndexOf(" "));
                }
            }
            HttpWebRequest descriptionReq = (HttpWebRequest)WebRequest.Create(cameraIP);
            descriptionReq.Method = "GET";
            WebResponse descriptionResp = descriptionReq.GetResponse();
            Stream descriptionStream = descriptionResp.GetResponseStream();
            StreamReader descriptionRead = new StreamReader(descriptionStream);
            string readDescription = descriptionRead.ReadToEnd();
            return readDescription;
        }

        private const string upnp_ns = "{urn:schemas-upnp-org:device-1-0}";
        private const string sony_ns = "{urn:schemas-sony-com:av}";

        public SonyCameraDeviceInfo AnalyzeDescription(string response)
        {
            //Log(response);
            var endpoints = new Dictionary<string, string>();

            var xml = XDocument.Parse(response);
            var device = xml.Root.Element(upnp_ns + "device");
            if (device == null)
            {
                return null;
            }
            var f_name = device.Element(upnp_ns + "friendlyName").Value;
            var m_name = device.Element(upnp_ns + "modelName").Value;
            var udn = device.Element(upnp_ns + "UDN").Value;
            var info = device.Element(sony_ns + "X_ScalarWebAPI_DeviceInfo");
            if (info == null)
            {
                return null;
            }
            var list = info.Element(sony_ns + "X_ScalarWebAPI_ServiceList");

            foreach (var service in list.Elements())
            {
                var name = service.Element(sony_ns + "X_ScalarWebAPI_ServiceType").Value;
                var url = service.Element(sony_ns + "X_ScalarWebAPI_ActionList_URL").Value;
                if (name == null || url == null)
                    continue;

                string endpoint;
                if (url.EndsWith("/"))
                    endpoint = url + name;
                else
                    endpoint = url + "/" + name;

                endpoints.Add(name, endpoint);
            }

            if (endpoints.Count == 0)
            {
                return null;
            }

            return new SonyCameraDeviceInfo(udn, m_name, f_name, endpoints);
        }
    }
}

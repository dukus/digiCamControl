using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Rssdp;
using Rssdp.Infrastructure;

namespace CameraControl.Devices.Wifi
{
    public class PanonoDiscovery
    {
        

        public string EndPoint { get; set; }

        public bool UDPSocketSetup()
        {
            try
            {
                SsdpDeviceLocator _DeviceLocator = null;
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

                            _DeviceLocator =
                                new SsdpDeviceLocator(new SsdpCommunicationsServer(new SocketFactory(ip.Address.ToString())));
                            break;
                        }
                    }


                }

                if (_DeviceLocator == null)
                    throw new Exception("Camea not connected !");

                // (Optional) Set the filter so we only see notifications for devices we care about 
                // (can be any search target value i.e device type, uuid value etc - any value that appears in the 
                // DiscoverdSsdpDevice.NotificationType property or that is used with the searchTarget parameter of the Search method).
                _DeviceLocator.NotificationFilter = "ssdp:all";

                // Connect our event handler so we process devices as they are found
                //_DeviceLocator.DeviceAvailable += deviceLocator_DeviceAvailable;

                // Enable listening for notifications (optional)
                _DeviceLocator.StartListeningForNotifications();

                // Perform a search so we don't have to wait for devices to broadcast notifications 
                // again to get any results right away (notifications are broadcast periodically).
                var s = _DeviceLocator.SearchAsync(new TimeSpan(0, 0, 15)).Result.ToList();
                if (s.Any())
                {
                    foreach (var device in s)
                    {
                        if (device.NotificationType == "panono:ball-camera")
                        {
                            EndPoint = device.DescriptionLocation.ToString();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception exc)
            {
                Log.Debug("Camera discovery fail", exc);
                return false;
            }
        }

    }
}

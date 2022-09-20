using CameraControl.Devices.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace CameraControl.Devices.GoPro
{
    public class GoProBluetoothHelper : INotifyPropertyChanged
    {
        DeviceWatcher mDeviceWatcher = null;
        private string message;
        private readonly Dictionary<string, DeviceInformation> mAllDevices = new Dictionary<string, DeviceInformation>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Message
        {
            get => message;
            set
            {
                message = value;
                Log.Debug("Bluetooth helper message :" + message);
                OnPropertyChanged(nameof(Message));
            }
        }

        public AsyncObservableCollection<GDeviceInformation> Devices { get; set; }

        public GoProBluetoothHelper()
        {
            Devices = new AsyncObservableCollection<GDeviceInformation>();
        }

        public void Initialize()
        {
            string BLESelector = "System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"";
            DeviceInformationKind deviceInformationKind = DeviceInformationKind.AssociationEndpoint;
            string[] requiredProperties = { "System.Devices.Aep.Bluetooth.Le.IsConnectable", "System.Devices.Aep.IsConnected" };

            mDeviceWatcher = DeviceInformation.CreateWatcher(BLESelector, requiredProperties, deviceInformationKind);
            mDeviceWatcher.Added += MDeviceWatcher_Added; ;
            mDeviceWatcher.Updated += MDeviceWatcher_Updated; ;
            mDeviceWatcher.Removed += MDeviceWatcher_Removed; ;
            mDeviceWatcher.EnumerationCompleted += MDeviceWatcher_EnumerationCompleted; ;
            mDeviceWatcher.Stopped += MDeviceWatcher_Stopped; ;

            Message = "Scanning for devices...";
            mDeviceWatcher.Start();
        }

        public void Rescan()
        {
            if (mDeviceWatcher != null)
            {
                mDeviceWatcher.Stop();
                Thread.Sleep(1000);
                if (mDeviceWatcher.Status == DeviceWatcherStatus.Stopped)
                    mDeviceWatcher.Start();
            }
        }

        public void Pair(GDeviceInformation lDevice)
        {
            if (lDevice != null)
            {
                Message = "Pairing started";

                var mBLED = BluetoothLEDevice.FromIdAsync(lDevice.DeviceInfo.Id).AsTask().Result;
                mBLED.DeviceInformation.Pairing.Custom.PairingRequested += Custom_PairingRequested;
                if (mBLED.DeviceInformation.Pairing.CanPair)
                {
                    DevicePairingProtectionLevel dppl = mBLED.DeviceInformation.Pairing.ProtectionLevel;
                    DevicePairingResult dpr = mBLED.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, dppl).AsTask().Result;

                    Message = "Pairing result = " + dpr.Status.ToString();
                }
                else
                {
                    Message = "Pairing failed";
                }
            }
            else
            {
                Message = "Select a device";
            }
        }

        private void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            Message = "Pairing request...";
            args.Accept();
        }

        private void MDeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            Message = "Scan Stoped";
        }

        private void MDeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Message = "Scan Complete";
        }

        private void MDeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].DeviceInfo.Id == args.Id)
                {
                    Devices.RemoveAt(i);
                    break;
                }
            }
        }

        private void MDeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {

            bool isPresent = false, isConnected = false, found = false;
            bool isPaired = false;

            if (args.Properties.ContainsKey("System.Devices.Aep.Bluetooth.Le.IsConnectable"))
            {
                isPresent = (bool)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
            }
            if (args.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
            {
                isConnected = (bool)args.Properties["System.Devices.Aep.IsConnected"];
            }
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].DeviceInfo.Id == args.Id)
                {
                    found = true;
                    Devices[i].DeviceInfo.Update(args);
                    Devices[i].IsPresent = isPresent;
                    Devices[i].IsConnected = isConnected;
                    break;
                }
            }
            if (!found && (isPresent || isConnected))
            {
                if (mAllDevices.ContainsKey(args.Id))
                {
                    mAllDevices[args.Id].Update(args);
                    Devices.Add(new GDeviceInformation(mAllDevices[args.Id], isPresent, isConnected));
                }
            }
        }

        private void MDeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            bool isPresent = false;
            bool isConnected = false;
            if (args.Properties.ContainsKey("System.Devices.Aep.Bluetooth.Le.IsConnectable"))
            {
                isPresent = (bool)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
            }
            if (args.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
            {
                isConnected = (bool)args.Properties["System.Devices.Aep.IsConnected"];
            }
            // Filter for GoPro devices only 
            // Skip GoPro Cam, this show up only when camera isn't paired 
            if (args.Name != "" && args.Name.Contains("GoPro") && args.Name!= "GoPro Cam")
            {
                bool found = false;
                if (!mAllDevices.ContainsKey(args.Id))
                {
                    mAllDevices.Add(args.Id, args);
                }
                for (int i = 0; i < Devices.Count; i++)
                {
                    if (Devices[i].DeviceInfo.Id == args.Id)
                    {
                        found = true;
                        Devices[i].DeviceInfo = args;
                        Devices[i].IsPresent = isPresent;
                        Devices[i].IsConnected = isConnected;
                        break;
                    }
                }
                if (!found && (isPresent || isConnected))
                {
                    Devices.Add(new GDeviceInformation(args, isPresent, isConnected));
                }
            }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
//https://stackoverflow.com/questions/65295888/microsoft-windows-sdk-contracts-and-must-use-packagereference

namespace CameraControl.Devices.GoPro
{
    public class GoProBluetoothDevice
    {
        public static readonly byte[] BLE_CMD_WIFI_ON = { 0x03, 0x17, 0x01, 0x01 };
        public static readonly byte[] BLE_CMD_WIFI_OFF = { 0x03, 0x17, 0x01, 0x00 };
        public static readonly byte[] BLE_CMD_CAPTURE_ON = { 0x03, 0x01, 0x01, 0x01 };
        public static readonly byte[] BLE_CMD_CAPTURE_OFF = { 0x03, 0x01, 0x01, 0x00 };

        private BluetoothLEDevice _bluetoothLEDevice;
        public GattCharacteristic mNotifyCmds = null;
        public GattCharacteristic mSendCmds = null;
        public GattCharacteristic mSetSettings = null;
        public GattCharacteristic mNotifySettings = null;
        public GattCharacteristic mSendQueries = null;
        public GattCharacteristic mNotifyQueryResp = null;

        public string ApName { get; set; }
        public string ApPass { get; set; }
        public bool WifiConnected { get; set; }

        public GoProBluetoothDevice(BluetoothLEDevice device)
        {
            _bluetoothLEDevice = device;
            GattDeviceServicesResult result = device.GetGattServicesAsync().AsTask().Result;
            device.ConnectionStatusChanged += Device_ConnectionStatusChanged;

            if (result.Status == GattCommunicationStatus.Success)
            {
                IReadOnlyList<GattDeviceService> services = result.Services;
                foreach (GattDeviceService gatt in services)
                {
                    GattCharacteristicsResult res = gatt.GetCharacteristicsAsync().AsTask().Result;
                    if (res.Status == GattCommunicationStatus.Success)
                    {
                        IReadOnlyList<GattCharacteristic> characteristics = res.Characteristics;
                        foreach (GattCharacteristic characteristic in characteristics)
                        {
                            GattCharacteristicProperties properties = characteristic.CharacteristicProperties;
                            if (properties.HasFlag(GattCharacteristicProperties.Read))
                            {
                                // This characteristic supports reading from it.
                            }
                            if (properties.HasFlag(GattCharacteristicProperties.Write))
                            {
                                // This characteristic supports writing to it.
                            }
                            if (properties.HasFlag(GattCharacteristicProperties.Notify))
                            {
                                // This characteristic supports subscribing to notifications.
                            }
                            if (characteristic.Uuid.ToString() == "b5f90002-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                ApName = ReadGattCharacteristicAsString(characteristic);
                                Log.Debug("AP name found " + ApName);
                            }
                            if (characteristic.Uuid.ToString() == "b5f90003-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                ApPass = ReadGattCharacteristicAsString(characteristic);
                                Log.Debug("AP pass found ");
                            }
                            if (characteristic.Uuid.ToString() == "b5f90072-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mSendCmds = characteristic;                               
                                Log.Debug("AP send command found ");
                            }
                            if (characteristic.Uuid.ToString() == "b5f90073-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mNotifyCmds = characteristic;
                                
                                GattCommunicationStatus status = mNotifyCmds.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify).AsTask().Result;
                                if (status == GattCommunicationStatus.Success)
                                {
                                    mNotifyCmds.ValueChanged += MNotifyCmds_ValueChanged;
                                }
                                else
                                {
                                    //failure
                                    Log.Debug("Failed to attach notify cmd " + status);
                                    throw new Exception("Failed to attach notify cmd " + status);
                                }
                            }
                            if (characteristic.Uuid.ToString() == "b5f90074-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mSetSettings = characteristic;
                            }
                            if (characteristic.Uuid.ToString() == "b5f90075-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mNotifySettings = characteristic;
                                GattCommunicationStatus status = mNotifySettings.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify).AsTask().Result;
                                if (status == GattCommunicationStatus.Success)
                                {
                                    mNotifySettings.ValueChanged += MNotifySettings_ValueChanged;
                                }
                                else
                                {
                                    Log.Debug("Failed to attach notify settings " + status);
                                    throw new Exception("Failed to attach notify settings " + status);
                                }
                            }
                            if (characteristic.Uuid.ToString() == "b5f90076-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mSendQueries = characteristic;
                            }
                            if (characteristic.Uuid.ToString() == "b5f90077-aa8d-11e3-9046-0002a5d5c51b")
                            {
                                mNotifyQueryResp = characteristic;
                                GattCommunicationStatus status = mNotifyQueryResp.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify).AsTask().Result;
                                if (status == GattCommunicationStatus.Success)
                                {
                                    mNotifyQueryResp.ValueChanged += MNotifyQueryResp_ValueChanged;
                                    if (mSendQueries != null)
                                    {
                                        //Register for settings and status updates
                                        DataWriter mm = new DataWriter();
                                        mm.WriteBytes(new byte[] { 1, 0x52 });
                                        GattCommunicationStatus gat = mSendQueries.WriteValueAsync(mm.DetachBuffer()).AsTask().Result;
                                        mm = new DataWriter();
                                        mm.WriteBytes(new byte[] { 1, 0x53 });
                                        gat = mSendQueries.WriteValueAsync(mm.DetachBuffer()).AsTask().Result;
                                    }
                                    else
                                    {
                                        throw new Exception("Send queries was null!" + status);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Failed to attach notify query " + status);
                                }
                            }
                        }
                    }
                }
                SetThirdPartySource();
            }
            else if (result.Status == GattCommunicationStatus.Unreachable)
            {
                //couldn't find camera
                throw new Exception("Connection failed");
            }
        }

        public void ExecuteCommand(byte[] arg)
        {
            DataWriter mm = new DataWriter();
            mm.WriteBytes(arg);
            GattCommunicationStatus res = GattCommunicationStatus.Unreachable;
            res = mSendCmds.WriteValueAsync(mm.DetachBuffer()).AsTask().Result;
            if (res != GattCommunicationStatus.Success)
            {
                throw new Exception("Failed to execute command: " + res.ToString());
            }
        }

        private string ReadGattCharacteristicAsString(GattCharacteristic characteristic)
        {
            GattReadResult val = characteristic.ReadValueAsync().AsTask().Result;
            if (val.Status == GattCommunicationStatus.Success)
            {
                DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(val.Value);
                return dataReader.ReadString(val.Value.Length);
            }
            return null;
        }

        private async void SetThirdPartySource()
        {
            DataWriter mm = new DataWriter();
            mm.WriteBytes(new byte[] { 0x01, 0x50 });
            GattCommunicationStatus res = GattCommunicationStatus.Unreachable;

            if (mSendCmds != null)
            {
                res = await mSendCmds.WriteValueAsync(mm.DetachBuffer());
            }
            if (res != GattCommunicationStatus.Success && mSendCmds != null)
            {
                //StatusOutput("Failed to set command source: " + res.ToString());
            }
        }

        public async Task ConnectWifiAsync(int timeOut = 25000)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                WiFiAdapter firstAdapter = null;
                var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                if (result.Count >= 1)
                {
                    firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                    await firstAdapter.ScanAsync();
                    while (true)
                    {
                        if (sw.ElapsedMilliseconds > timeOut)
                        {
                            Log.Debug("Wifi conection timeout");
                            break;
                        }
                        var connectionProfile = await firstAdapter.NetworkAdapter.GetConnectedProfileAsync();

                        Log.Debug(connectionProfile?.ProfileName ?? "No AP connected to WiFi adapter ");

                        WiFiAvailableNetwork selectedAP = null;
                        var wifis = firstAdapter.NetworkReport.AvailableNetworks.OrderByDescending(ap => ap.NetworkRssiInDecibelMilliwatts);
                        foreach (var item in wifis)
                        {
                            Log.Debug($"Found AP: {item.Ssid}");

                            if (ApName == item.Ssid)
                            {
                                selectedAP = item;
                                // return if the accespoint is already connected 

                                if (connectionProfile?.ProfileName == item.Ssid)
                                {
                                    WifiConnected = true;
                                    Log.Debug("Wifi already is connected");
                                    return;
                                }
                            }
                        }
                        if (selectedAP != null)
                        {
                            var credential = new PasswordCredential();
                            credential.Password = ApPass;

                            Log.Debug("Wifi conection started ....");
                            await firstAdapter.ConnectAsync(selectedAP, WiFiReconnectionKind.Manual, credential);
                            OnConnectedComplete(true);
                        }
                        else
                        {
                            Log.Debug($"AP not found `{ApName}` ");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No WIFI adapter available");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Wifi connection error", ex);
            }

        }

        private void OnConnectedComplete(bool success)
        {
            Log.Debug(success ? "Wifi connection succesed" : "Wifi connection failed");
            WifiConnected = success;
        }

        private readonly List<byte> mBufQ = new List<byte>();
        private int mExpectedLengthQ = 0;
        private void MNotifyQueryResp_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] myBytes = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(myBytes);
            int newLength = ReadBytesIntoBuffer(myBytes, mBufQ);
            if (newLength > 0)
                mExpectedLengthQ = newLength;

            if (mExpectedLengthQ == mBufQ.Count)
            {
                if ((mBufQ[0] == 0x53 || mBufQ[0] == 0x93) && mBufQ[1] == 0)
                {
                    //status messages
                    for (int k = 0; k < mBufQ.Count;)
                    {
                        if (mBufQ[k] == 10)
                        {
                            //Encoding = mBufQ[k + 2] > 0;
                        }
                        if (mBufQ[k] == 70)
                        {
                            //BatteryLevel = mBufQ[k + 2];
                        }
                        if (mBufQ[k] == 69)
                        {
                            if (mBufQ[k + 2] == 1)
                            {
                                Log.Debug("Camera AP wifi enabled, starting to connect pc wifi to it");
                                // Don't call this here as it gets fired twice when connecting to bluetooth
                                //ConnectWifiAsync();
                            }
                        }
                        k += 2 + mBufQ[k + 1];
                    }
                }
                else
                {
                    //Unhandled Query Message
                }
                mBufQ.Clear();
                mExpectedLengthQ = 0;
            }
        }

        private void MNotifySettings_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

        }

        private void MNotifyCmds_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

        }

        private void Device_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {

        }

        private int ReadBytesIntoBuffer(byte[] bytes, List<byte> mBuf)
        {
            int returnLength = -1;
            int startbyte = 1;
            int theseBytes = bytes.Length;
            if ((bytes[0] & 32) > 0)
            {
                //extended 13 bit header
                startbyte = 2;
                int len = (bytes[0] & 0xF) << 8 | bytes[1];
                returnLength = len;
            }
            else if ((bytes[0] & 64) > 0)
            {
                //extended 16 bit header
                startbyte = 3;
                int len = bytes[1] << 8 | bytes[2];
                returnLength = len;
            }
            else if ((bytes[0] & 128) > 0)
            {
                //its a continuation packet
            }
            else
            {
                //8 bit header
                returnLength = bytes[0];
            }
            for (int k = startbyte; k < theseBytes; k++)
                mBuf.Add(bytes[k]);

            return returnLength;
        }
    }
}

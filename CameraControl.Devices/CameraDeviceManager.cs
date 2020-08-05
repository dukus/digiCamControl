#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using CameraControl.Devices.Canon;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Custom;
using CameraControl.Devices.Nikon;
using CameraControl.Devices.Others;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.TransferProtocol.DDServer;
using CameraControl.Devices.TransferProtocol.PtpIp;
using CameraControl.Devices.Wifi;
using Canon.Eos.Framework;
using PortableDeviceLib;
using WIA;
using Accord.Video.DirectShow;

#endregion

namespace CameraControl.Devices
{
    public class CameraDeviceManager : BaseFieldClass
    {
        private const string AppName = "CameraControl";
        private const int AppMajorVersionNumber = 1;
        private const int AppMinorVersionNumber = 0;
        private bool _connectionInProgress = false;
        private DeviceDescriptorEnumerator _deviceEnumerator;
        private EosFramework _framework;
        private object _locker = new object();
        private List<DeviceDescription> _deviceDescriptions = new List<DeviceDescription>();

        public ConcurrentDictionary<ICameraDevice, byte[]> LiveViewImage;
        public ConcurrentDictionary<ICameraDevice, string> LastCapturedImage;

        /// <summary>
        /// Gets or sets a value indicating whether use experimental drivers.
        /// Experimental drivers isn't tested and may not implement all camera possibilities 
        /// </summary>
        /// <value>
        /// <c>true</c> if [use experimental drivers]; otherwise, <c>false</c>.
        /// </value>
        public bool UseExperimentalDrivers { get; set; }

        public bool LoadWiaDevices { get; set; }

        /// <summary>
        /// Gets or sets the natively supported model dictionary.
        /// This property is used to find the right driver for connected camera, 
        /// if model not found in this dictionary generic wia driver will used.
        /// </summary>
        /// <value>
        /// The device class.
        /// </value>
        public static Dictionary<string, Type> DeviceClass { get; set; }
        public Dictionary<string, Type> CustomDeviceClass { get; set; }

        public List<IWifiDeviceProvider> WifiDeviceProviders { get; set; }

        private ICameraDevice _selectedCameraDevice;

        /// <summary>
        /// Gets or sets the default selected camera device. When new camera connected this property set automatically to new device
        /// </summary>
        /// <value>
        /// The selected camera device.
        /// </value>
        public ICameraDevice SelectedCameraDevice
        {
            get { return _selectedCameraDevice; }
            set
            {
                ICameraDevice device = _selectedCameraDevice;
                _selectedCameraDevice = value ?? (ConnectedDevices.Count > 0
                    ? ConnectedDevices[0]
                    : new NotConnectedCameraDevice());

                CameraSelected?.Invoke(device, _selectedCameraDevice);

                NotifyPropertyChanged("SelectedCameraDevice");
            }
        }

        /// <summary>
        /// If enabled the application will detect the connected webcams
        /// </summary>
        /// <value>
        ///   <c>true</c> if [detect webcams]; otherwise, <c>false</c>.
        /// </value>
        public bool DetectWebcams { get; set; }


        public bool StartInNewThread { get; set; }

        private AsyncObservableCollection<ICameraDevice> _connectedDevices;

        public AsyncObservableCollection<ICameraDevice> ConnectedDevices
        {
            get { return _connectedDevices; }
            set
            {
                _connectedDevices = value;
                NotifyPropertyChanged("ConnectedDevices");
            }
        }

        private void PopulateDeviceClass()
        {
            DeviceClass = new Dictionary<string, Type>
            {
                {"D200", typeof(NikonD40)},
                {"D3", typeof(NikonD90)},
                {"D3S", typeof(NikonD90)},
                {"D3X", typeof(NikonD3X)},
                {"D300", typeof(NikonD300)},
                {"D300s", typeof(NikonD300)},
                {"D300S", typeof(NikonD300)},
                {"D3200", typeof(NikonD3200)},
                {"D3300", typeof(NikonD600Base)},
                {"D3400", typeof(NikonD600Base)},
                {"D3500", typeof(NikonD600Base)},
                {"D4", typeof(NikonD4)},
                {"D4s", typeof(NikonD4)},
                {"D4S", typeof(NikonD4)},
                {"D40", typeof(NikonD40)},
                {"D40X", typeof(NikonD40)},
                {"D5", typeof(NikonD5)},
                {"D50", typeof(NikonD40)},
                {"D500", typeof(NikonD500)},
                {"D5600", typeof(NikonD5200)},
                {"D5500", typeof(NikonD5200)},
                {"D5300", typeof(NikonD5200)},
                {"D5200", typeof(NikonD5200)},
                {"D5100", typeof(NikonD5100)},
                {"D5000", typeof(NikonD90)},
                {"D60", typeof(NikonD60)},
                {"D610", typeof(NikonD600Base)},
                {"D600", typeof(NikonD600)},
                {"D70", typeof(NikonD40)},
                {"D70s", typeof(NikonD40)},
                {"D700", typeof(NikonD700)},
                {"D750", typeof(NikonD750)},
                {"D7000", typeof(NikonD7000)},
                {"D7100", typeof(NikonD7100)},
                {"D7200", typeof(NikonD7100)},
                {"D7500", typeof(NikonD7500)},
                {"D80", typeof(NikonD80)},
                {"D800", typeof(NikonD800)},
                {"D800E", typeof(NikonD800)},
                {"D800e", typeof(NikonD800)},
                {"D810", typeof(NikonD810)},
                {"D810A", typeof(NikonD810)},
                {"D810a", typeof(NikonD810)},
                {"D850", typeof(NikonD850)},
                {"D90", typeof(NikonD90)},
                {"V1", typeof(NikonD5100)},
                {"V2", typeof(NikonD5100)},
                {"V3", typeof(NikonD600Base)},
                {"J3", typeof(NikonD600Base)},
                {"J4", typeof(NikonD600Base)},
                {"J5", typeof(NikonD600Base)},
                {"Df", typeof(NikonD600Base)},
                {"L830", typeof(NikonL830)},
                {"L840", typeof(NikonL830)},
                {"Z 50", typeof(NikonZ6)},
                {"Z 6", typeof(NikonZ6)},
                {"Z 7", typeof(NikonZ7)},
                //{"Canon EOS 5D Mark II", typeof (CanonSDKBase)},
                {"MTP Sim", typeof(BaseMTPCamera)},
                //{"D.*", typeof (NikonBase)},
                // for mtp simulator
                //{"Test Camera ", typeof (NikonBase)},
            };
            //if(UseExperimentalDrivers)
            //{
            //  DeviceClass.Add("Canon EOS.*", typeof(CanonSDKBase));
            //}
            WifiDeviceProviders.Add(new DDServerProvider());
            WifiDeviceProviders.Add(new PtpIpProvider());
            WifiDeviceProviders.Add(new YiCameraProvider());
            WifiDeviceProviders.Add(new SonyProvider());
            WifiDeviceProviders.Add(new PanonoProvider());
            WifiDeviceProviders.Add(new OscProvider());
            WifiDeviceProviders.Add(new PtzOpticsProvider());
            foreach (var type in CustomDeviceClass)
            {
                DeviceClass.Add(type.Key, type.Value);
            }
        }

        public CameraDeviceManager(string datafolder=null)
        {
            UseExperimentalDrivers = true;
            LoadWiaDevices = true;
            StartInNewThread = false;
            DetectWebcams = true;
            CustomDeviceClass = new Dictionary<string, Type>();
            SelectedCameraDevice = new NotConnectedCameraDevice();
            ConnectedDevices = new AsyncObservableCollection<ICameraDevice>();
            _deviceEnumerator = new DeviceDescriptorEnumerator();
            LiveViewImage = new ConcurrentDictionary<ICameraDevice, byte[]>();
            LastCapturedImage = new ConcurrentDictionary<ICameraDevice, string>();
            WifiDeviceProviders = new List<IWifiDeviceProvider>();


            // prevent program crash in something wrong with wia
            try
            {
                WiaDeviceManager = new DeviceManager();
                WiaDeviceManager.RegisterEvent(Conts.wiaEventDeviceConnected, "*");
                WiaDeviceManager.RegisterEvent(Conts.wiaEventDeviceDisconnected, "*");
                WiaDeviceManager.OnEvent += DeviceManager_OnEvent;
                Log.Error("Wia initialized");
            }
            catch (Exception exception)
            {
                Log.Error("Error initialize WIA", exception);
            }
            if (datafolder != null && Directory.Exists(datafolder))
            {
                try
                {
                    var files = Directory.GetFiles(datafolder, "*.xml");
                    foreach (var file in files)
                    {
                        var device = DeviceDescription.Load(file);
                        if (device != null)
                            _deviceDescriptions.Add(device);
                    }
                }
                catch (Exception)
                {
                    Log.Error("Error loading custom data");
                }
            }
        }

        private void InitCanon()
        {
            try
            {
                if (_framework == null)
                {
                    _framework = new EosFramework();
                    _framework.CameraAdded += _framework_CameraAdded;
                }
                AddCanonCameras();
            }
            catch (Exception exception)
            {
                Log.Error("Unable init canon driver", exception);
                /* Give specific guidance if the error is a missing DLL */
                if ((exception.InnerException != null) && (exception.InnerException.Message != null) && (exception.InnerException.Message.Contains("EDSDK.dll")))
                {
                    Console.WriteLine("\n**CRITICAL ERROR**\n\nCanon EOS camera library, EDSDK.dll is missing\nInstall it after downloading from Canon's site\n");
                }
            }
        }

        private void _framework_CameraAdded(object sender, EventArgs e)
        {
            AddCanonCameras();
        }

        public IEnumerable<EosCamera> GetEosCameras()
        {
            using (EosCameraCollection cameras = _framework.GetCameraCollection())
                return cameras.ToArray();
        }

        private void AddWebcameras()
        {
            try
            {
                List<string> monikers = new List<string>();
                var loaclWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (Accord.Video.DirectShow.FilterInfo localcamera in loaclWebCamsCollection)
                {
                    monikers.Add(localcamera.MonikerString);
                    bool added = false;
                    foreach (ICameraDevice device in ConnectedDevices)
                    {
                        WebCameraDevice webCamera = device as WebCameraDevice;
                        if (webCamera != null)
                        {
                            if (webCamera.PortName == localcamera.MonikerString)
                            {
                                added = true;
                            }
                        }
                    }

                    if (added)
                        continue;

                    WebCameraDevice camera = new WebCameraDevice();
                    camera.Init(localcamera.MonikerString);
                    camera.DeviceName = localcamera.Name;
                    camera.SerialNumber = localcamera.MonikerString;

                    ConnectedDevices.Add(camera);

                    SelectedCameraDevice = camera;

                    camera.PhotoCaptured += cameraDevice_PhotoCaptured;
                    camera.CameraDisconnected += cameraDevice_CameraDisconnected;

                    CameraConnected?.Invoke(camera);
                }
                //List<WebCameraDevice> devicesToDisconnect = ConnectedDevices.OfType<WebCameraDevice>()
                //    .Where(webCamera => !monikers.Contains(webCamera.PortName))
                //    .ToList();
                //foreach (var webCamera in devicesToDisconnect)
                //{
                //    cameraDevice_CameraDisconnected(webCamera, new DisconnectCameraEventArgs() { });
                //}
            }
            catch (Exception ex)
            {
                Log.Error("Unable to connect to a webcamera", ex);
            }

        }

        private void AddCanonCameras()
        {
            lock (_locker)
            {
                foreach (EosCamera eosCamera in GetEosCameras())
                {
                    bool shouldbeadded =
                        ConnectedDevices.OfType<CanonSDKBase>().All(camera => camera.PortName != eosCamera.PortName);

                    if (shouldbeadded)
                    {
                        Log.Debug("New canon camera found !");
                        CanonSDKBase camera = new CanonSDKBase();
                        Log.Debug("Pas 1");
                        DeviceDescriptor descriptor = new DeviceDescriptor {EosCamera = eosCamera};
                        descriptor.CameraDevice = camera;
                        Log.Debug("Pas 2");
                        camera.Init(eosCamera);
                        Log.Debug("Pas 3");
                        ConnectedDevices.Add(camera);
                        Log.Debug("Pas 4");
                        _deviceEnumerator.Add(descriptor);
                        Log.Debug("Pas 5");
                        NewCameraConnected(camera);
                        Log.Debug("New canon camera found done!");
                    }
                }
                //Thread.Sleep(2500);
            }
        }

        private ICameraDevice GetWiaIDevice(IDeviceInfo devInfo)
        {
            // if camera already is connected do nothing
            if (_deviceEnumerator.GetByWiaId(devInfo.DeviceID) != null)
                return _deviceEnumerator.GetByWiaId(devInfo.DeviceID).CameraDevice;
            _deviceEnumerator.RemoveDisconnected();
            DeviceDescriptor descriptor = new DeviceDescriptor {WiaDeviceInfo = devInfo, WiaId = devInfo.DeviceID};

            ICameraDevice cameraDevice = new WiaCameraDevice();
            bool isConnected = cameraDevice.Init(descriptor);

            descriptor.CameraDevice = cameraDevice;
            _deviceEnumerator.Add(descriptor);
            ConnectedDevices.Add(cameraDevice);

            if (isConnected)
            {
                NewCameraConnected(cameraDevice);
            }
            //ServiceProvider.DeviceManager.SelectedCameraDevice.ReadDeviceProperties(0);

            return SelectedCameraDevice;
        }

        private void cameraDevice_CameraDisconnected(object sender, DisconnectCameraEventArgs e)
        {
            if (e.StillImageDevice != null)
            {
                DisconnectCamera(e.StillImageDevice);
            }
            if (e.EosCamera != null)
            {
                DisconnectCamera(e.EosCamera);
            }
            OnCameraDisconnected((ICameraDevice) sender);
        }

        /// <summary>
        /// Raise CameraDisconnected event.
        /// </summary>
        /// <param name="cameraDevice">The camera device.</param>
        public void OnCameraDisconnected(ICameraDevice cameraDevice)
        {
            if (CameraDisconnected != null)
                CameraDisconnected(cameraDevice);
        }

        private void cameraDevice_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            if (PhotoCaptured != null)
                PhotoCaptured(sender, eventArgs);
        }

        public void OnPhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            if (PhotoCaptured != null)
                PhotoCaptured(sender, eventArgs);
        }

        private void ConnectDevices()
        {
            if (_connectionInProgress)
                return;
            _connectionInProgress = true;
            if (PortableDeviceCollection.Instance == null)
            {
                PortableDeviceCollection.CreateInstance(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
                PortableDeviceCollection.Instance.AutoConnectToPortableDevice = false;
            }
            _deviceEnumerator.RemoveDisconnected();

            Log.Debug("Connection device start" );
            try
            {
                var devices = PortableDeviceCollection.Instance.Devices;
                foreach (PortableDevice portableDevice in devices)
                {
                    Log.Debug("Connection device " + portableDevice.DeviceId);
                    //TODO: avoid to load some mass storage in my computer need to find a general solution
                    if (!portableDevice.DeviceId.StartsWith("\\\\?\\usb") &&
                        !portableDevice.DeviceId.StartsWith("\\\\?\\comp"))
                        continue;
                    // ignore some Canon cameras
                    if (!SupportedCanonCamera(portableDevice.DeviceId))
                        continue;
                    portableDevice.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);

                    if (_deviceEnumerator.GetByWpdId(portableDevice.DeviceId) == null &&
                        GetNativeDriver(portableDevice.Model) != null)
                    {
                        ICameraDevice cameraDevice;
                        DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = portableDevice.DeviceId};
                        cameraDevice = (ICameraDevice) Activator.CreateInstance(GetNativeDriver(portableDevice.Model));
                        MtpProtocol device = new MtpProtocol(descriptor.WpdId);
                        device.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);

                        descriptor.StillImageDevice = device;

                        cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                        cameraDevice.Init(descriptor);

                        if (string.IsNullOrWhiteSpace(cameraDevice.SerialNumber))
                            cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);

                        ConnectedDevices.Add(cameraDevice);
                        NewCameraConnected(cameraDevice);

                        descriptor.CameraDevice = cameraDevice;
                        _deviceEnumerator.Add(descriptor);
                    }

                    if (_deviceEnumerator.GetByWpdId(portableDevice.DeviceId) == null &&
                        GetNativeDriver(portableDevice.Model) == null)
                    {
                        var description = getDeviceDescription(portableDevice.Model);
                        if (description != null)
                        {
                            CustomDevice cameraDevice = new CustomDevice();
                            DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = portableDevice.DeviceId};
                            MtpProtocol device = new MtpProtocol(descriptor.WpdId);
                            device.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);

                            descriptor.StillImageDevice = device;

                            cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                            cameraDevice.Init(descriptor, description);

                            ConnectedDevices.Add(cameraDevice);
                            NewCameraConnected(cameraDevice);

                            descriptor.CameraDevice = cameraDevice;
                            _deviceEnumerator.Add(descriptor);
                            break;
                        }

                    }

                }

            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect to cameras ", exception);
            }

            _connectionInProgress = false;
        }

        private DeviceDescription getDeviceDescription(string model)
        {
            return _deviceDescriptions.FirstOrDefault(description => description.Model == model);
        }

        public void AddDevice(DeviceDescriptor descriptor)
        {
            _deviceEnumerator.RemoveDisconnected();
            ConnectedDevices.Add(descriptor.CameraDevice);
            NewCameraConnected(descriptor.CameraDevice);
            _deviceEnumerator.Add(descriptor);
        }

        public void ConnectToServer(string s, int type)
        {
            if(string.IsNullOrEmpty(s))
                return;
            
            if (type == 0)
            {
                int port = 15740;
                string ip = s;
                if (s.Contains(":"))
                {
                    ip = s.Split(':')[0];
                    int.TryParse(s.Split(':')[1], out port);
                }
                ConnectDevicesPtpIp(ip, port);
            }

            if (type == 1)
            {
                int port = 4757;
                string ip = s;
                if (s.Contains(":"))
                {
                    ip = s.Split(':')[0];
                    int.TryParse(s.Split(':')[1], out port);
                }
                ConnectDevicesDDServer(ip, port);
            }
        }

        public void ConnectDevicesDDServer(string ip, int port)
        {
            if (_connectionInProgress)
                return;
            try
            {
                _connectionInProgress = true;
                _deviceEnumerator.RemoveDisconnected();
                DdClient client = new DdClient();
                if (!client.Open(ip, port))
                    throw new Exception("No server was found!");
                var devices = client.GetDevices();
                if (devices.Count == 0)
                    throw new Exception("No connected device was found!");

                client.Connect(devices[0]);
                DdServerProtocol protocol = new DdServerProtocol(client);

                if (GetNativeDriver(protocol.Model) != null)
                {
                    ICameraDevice cameraDevice;
                    DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = "ddserver"};
                    cameraDevice = (ICameraDevice) Activator.CreateInstance(GetNativeDriver(protocol.Model));
                    descriptor.StillImageDevice = protocol;

                    //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                    cameraDevice.Init(descriptor);
                    ConnectedDevices.Add(cameraDevice);
                    NewCameraConnected(cameraDevice);

                    descriptor.CameraDevice = cameraDevice;
                    _deviceEnumerator.Add(descriptor);
                }
                else
                {
                    throw new Exception("Not Supported device " + protocol.Model);
                }
            }
            finally
            {
                _connectionInProgress = false;
            }
        }

        public void ConnectDevicesPtpIp(string ip, int port)
        {
            if (_connectionInProgress)
                return;
            try
            {
                _connectionInProgress = true;
                _deviceEnumerator.RemoveDisconnected();

                PtpIpClient client = new PtpIpClient();
                if (!client.Open(ip, 15740))
                    throw new Exception("No server was found!");
                PtpIpProtocol protocol = new PtpIpProtocol(client);
                protocol.ExecuteWithNoData(0x1002, 1);

                if (GetNativeDriver(protocol.Model) != null)
                {
                    ICameraDevice cameraDevice;
                    DeviceDescriptor descriptor = new DeviceDescriptor { WpdId = "ptpip" };
                    cameraDevice = (ICameraDevice)Activator.CreateInstance(GetNativeDriver(protocol.Model));
                    descriptor.StillImageDevice = protocol;

                    //cameraDevice.SerialNumber = StaticHelper.GetSerial(portableDevice.DeviceId);
                    cameraDevice.Init(descriptor);
                    ConnectedDevices.Add(cameraDevice);
                    NewCameraConnected(cameraDevice);

                    descriptor.CameraDevice = cameraDevice;
                    _deviceEnumerator.Add(descriptor);
                }
                else
                {
                    throw new Exception("Not Supported device " + protocol.Model);
                }
            }
            finally
            {
                _connectionInProgress = false;
            }
        }

        private bool SupportedCanonCamera(string id)
        {
            // isn't canon the manufacturer 
            if (!id.Contains("vid_04a9"))
                return true;
            return false;
        }


        private void NewCameraConnected(ICameraDevice cameraDevice)
        {
            const string usbPrefix = "\\\\?\\usb";

            StaticHelper.Instance.SystemMessage = "New Camera is connected ! Driver :" + cameraDevice.DeviceName;
            Log.Debug("===========Camera is connected==============");

            if (cameraDevice.PortName != null && cameraDevice.PortName.Substring(0, usbPrefix.Length).Equals(usbPrefix))
            {
                string vid = "";
                string pid = "";
                char[] delimiterChars = { '#', '&', '_' };

                string[] words = cameraDevice.PortName.Split(delimiterChars);

                for (int i = 1; i < words.Length - 1; i++)
                {
                    if (words[i].Equals("pid"))
                        pid = words[i + 1];
                    else if (words[i].Equals("vid"))
                        vid = words[i + 1];
                }

                if (!vid.Equals("") && !pid.Equals(""))
                    Log.Debug("USB : VID=" + vid + ", PID=" + pid);
            }

            Log.Debug("Driver :" + cameraDevice.GetType().Name);
            Log.Debug("Name :" + cameraDevice.DeviceName);
            Log.Debug("Manufacturer :" + cameraDevice.Manufacturer);
            if (CameraConnected != null)
                CameraConnected(cameraDevice);
            SelectedCameraDevice = cameraDevice;
            cameraDevice.PhotoCaptured += cameraDevice_PhotoCaptured;
            cameraDevice.CameraDisconnected += cameraDevice_CameraDisconnected;
        }

        /// <summary>
        /// Gets the native driver based on camera model.
        /// </summary>
        /// <param name="model">The model name.</param>
        /// <returns>If the model not supported return null else the driver type</returns>
        public static Type GetNativeDriver(string model)
        {
            if (String.IsNullOrEmpty(model))
                return null;
            // first check if driver exist with same driver name
            if (DeviceClass.ContainsKey(model))
                return DeviceClass[model];
            return null;
            //// in driver not found will check with regex name
            //return (from keyValuePair in DeviceClass
            //        let regex = new Regex(keyValuePair.Key)
            //        where regex.IsMatch(model)
            //        select keyValuePair.Value).FirstOrDefault();
        }

        [HandleProcessCorruptedStateExceptions]
        public void DisconnectCamera(ICameraDevice cameraDevice)
        {

            cameraDevice.PhotoCaptured -= cameraDevice_PhotoCaptured;
            cameraDevice.CameraDisconnected -= cameraDevice_CameraDisconnected;
            ConnectedDevices.Remove(cameraDevice);
            StaticHelper.Instance.SystemMessage = "Camera disconnected :" + cameraDevice.DeviceName;
            Log.Debug("===========Camera disconnected==============");
            Log.Debug("Name :" + cameraDevice.DeviceName);

            cameraDevice.Close();
            OnCameraDisconnected(cameraDevice);
        }

        private void DisconnectCamera(string wiaId)
        {
            DeviceDescriptor descriptor = _deviceEnumerator.GetByWiaId(wiaId);
            if (descriptor != null)
            {
                descriptor.CameraDevice.PhotoCaptured -= cameraDevice_PhotoCaptured;
                descriptor.CameraDevice.CameraDisconnected -= cameraDevice_CameraDisconnected;
                ConnectedDevices.Remove(descriptor.CameraDevice);
                StaticHelper.Instance.SystemMessage = "Camera disconnected :" + descriptor.CameraDevice.DeviceName;
                Log.Debug("===========Camera disconnected==============");
                Log.Debug("Name :" + descriptor.CameraDevice.DeviceName);

                _deviceEnumerator.Remove(descriptor);
                descriptor.CameraDevice.Close();
                var wiaCameraDevice = descriptor.CameraDevice as WiaCameraDevice;
                if (wiaCameraDevice != null)
                {
                    OnCameraDisconnected(wiaCameraDevice);
                }
                if (PortableDeviceCollection.Instance != null)
                    PortableDeviceCollection.Instance.RefreshDevices();
            }
        }

        private void DisconnectCamera(ITransferProtocol device)
        {
            DeviceDescriptor descriptor = _deviceEnumerator.GetByWpdId(device.DeviceId);
            if (descriptor != null)
            {
                descriptor.CameraDevice.PhotoCaptured -= cameraDevice_PhotoCaptured;
                descriptor.CameraDevice.CameraDisconnected -= cameraDevice_CameraDisconnected;
                StaticHelper.Instance.SystemMessage = "Camera disconnected :" + descriptor.CameraDevice.DeviceName;
                Log.Debug("===========Camera disconnected==============");
                Log.Debug("Name :" + descriptor.CameraDevice.DeviceName);
                PortableDeviceCollection.Instance.RemoveDevice(device.DeviceId);
                device.IsConnected = false;
                ConnectedDevices.Remove(descriptor.CameraDevice);
                descriptor.CameraDevice.Close();
                _deviceEnumerator.Remove(descriptor);
                _deviceEnumerator.RemoveDisconnected();
            }
            RemoveDisconnected();
        }

        private void DisconnectCamera(EosCamera device)
        {
            DeviceDescriptor descriptor = _deviceEnumerator.GetByEosCamera(device);
            if (descriptor != null)
            {
                descriptor.CameraDevice.PhotoCaptured -= cameraDevice_PhotoCaptured;
                descriptor.CameraDevice.CameraDisconnected -= cameraDevice_CameraDisconnected;
                StaticHelper.Instance.SystemMessage = "Camera disconnected :" + descriptor.CameraDevice.DeviceName;
                Log.Debug("===========Camera disconnected==============");
                Log.Debug("Name :" + descriptor.CameraDevice.DeviceName);
                ConnectedDevices.Remove(descriptor.CameraDevice);
                descriptor.CameraDevice.Close();
                _deviceEnumerator.Remove(descriptor);
                _deviceEnumerator.RemoveDisconnected();
            }
            RemoveDisconnected();
        }

        private void RemoveDisconnected()
        {
            List<ICameraDevice> removedCameras = ConnectedDevices.Where(device => !device.IsConnected).ToList();
            foreach (ICameraDevice device in removedCameras)
            {
                ConnectedDevices.Remove(device);
            }
        }

        private DeviceManager WiaDeviceManager { get; set; }

        /// <summary>
        /// Gets or sets a value for disabling native drivers.
        /// </summary>
        /// <value>
        /// <c>true</c> all devices are loaded like WIA devices <c>false</c> If native driver are available for connected model the will be loaded that driver else will be loaded WIA driver.
        /// </value>
        public bool DisableNativeDrivers { get; set; }


        private void DeviceManager_OnEvent(string eventId, string deviceId, string itemId)
        {
            //if (!LoadWiaDevices)
            //    return;

            if (eventId == Conts.wiaEventDeviceConnected)
            {
                if (StartInNewThread)
                {
                    Thread _thread = new Thread(() => ConnectToCamera(true));
                    _thread.SetApartmentState(ApartmentState.MTA);
                    _thread.Start();
                }
                else
                {
                    ConnectToCamera(true);
                }
            }
            else if (eventId == Conts.wiaEventDeviceDisconnected)
            {
                DisconnectCamera(deviceId);
            }
        }


        public bool ConnectToCamera()
        {
            if (UseExperimentalDrivers)
                InitCanon();
            return ConnectToCamera(true);
        }

        public void AddFakeCamera()
        {
            FakeCameraDevice device = new FakeCameraDevice();
            ConnectedDevices.Add(device);
            NewCameraConnected(device);
        }

        /// <summary>
        /// Populate the ConnectedDevices list with connected cameras. This method will be called automatically every time new devices will be connected 
        /// Except Canon cameras that is handled by framework event handler
        /// </summary>
        /// <returns></returns>
        public bool ConnectToCamera(bool retry)
        {
            if (DeviceClass == null || DeviceClass.Count == 0)
                PopulateDeviceClass();

            if(DetectWebcams)
                AddWebcameras();

            if (!DisableNativeDrivers)
            {
                ConnectDevices();
            }
            else
            {
                Log.Debug("Native drivers are disabled !!!!");
            }
            // if canon camera is connected don't use wia driver
            if (UseExperimentalDrivers && _framework != null && _framework.GetCameraCollection().Count > 0)
                return true;

            if (!LoadWiaDevices)
                return true;

            bool ret = false;
            bool noDriversDetected = ConnectedDevices.Count == 0;
            int retries = 0;
            foreach (IDeviceInfo devInfo in new DeviceManager().DeviceInfos)
            {
                // Look for CameraDeviceType devices
                string model = devInfo.Properties["Name"].get_Value();
                // skip canon cameras 
                //if (!string.IsNullOrEmpty(model) && model.Contains("Canon"))
                //    continue;
                if (getDeviceDescription(model) != null)
                    continue;

                var nativeDriver = GetNativeDriver(model);
                ret = nativeDriver != null;

                if ((devInfo.Type == WiaDeviceType.CameraDeviceType || devInfo.Type == WiaDeviceType.VideoDeviceType)
                    && (nativeDriver == null || DisableNativeDrivers || noDriversDetected) && retries < 3)
                {
                    do
                    {
                        Log.Debug("Wia Camera Found: " + model);
                        try
                        {
                            GetWiaIDevice(devInfo);
                            retries = 4;
                            ret = true;
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Unable to connect to the camera", exception);
                            retries++;
                            if (retries < 3)
                            {
                                Log.Debug("Retrying");
                                StaticHelper.Instance.SystemMessage = "Unable to connect to the camera. Retrying";
                            }
                            else
                            {
                                StaticHelper.Instance.SystemMessage =
                                    "Unable to connect to the camera. Please reconnect your camera !";
                            }
                            Thread.Sleep(1000);
                        }
                    } while (retries < 3);
                }
            }
            return ret;
        }

        public void CloseAll()
        {
            foreach (
                ICameraDevice connectedDevice in ConnectedDevices.Where(connectedDevice => connectedDevice.IsConnected))
            {
                connectedDevice.Close();
            }
        }

        public event PhotoCapturedEventHandler PhotoCaptured;

        public delegate void CameraConnectedEventHandler(ICameraDevice cameraDevice);

        /// <summary>
        /// Occurs when a new camera is connected.
        /// </summary>
        public event CameraConnectedEventHandler CameraConnected;

        /// <summary>
        /// Occurs when a camera disconnected.
        /// </summary>
        public event CameraConnectedEventHandler CameraDisconnected;

        public delegate void CameraSelectedEventHandler(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice);

        /// <summary>
        /// Occurs when SelectedCameraDevice property changed.
        /// </summary>
        public event CameraSelectedEventHandler CameraSelected;


        public void SelectNextCamera()
        {
            if (ConnectedDevices.Count == 0)
                return;
            int idx = 0;
            for (int i = 0; i < ConnectedDevices.Count; i++)
            {
                var device = ConnectedDevices[i];
                if (device == SelectedCameraDevice)
                {
                    idx = i;
                    break;
                }
            }
            idx++;
            if (idx < ConnectedDevices.Count)
                SelectedCameraDevice = ConnectedDevices[idx];
        }

        public void SelectPrevCamera()
        {
            if (ConnectedDevices.Count == 0)
                return;
            int idx = 0;
            for (int i = 0; i < ConnectedDevices.Count; i++)
            {
                var device = ConnectedDevices[i];
                if (device == SelectedCameraDevice)
                {
                    idx = i;
                    break;
                }
            }
            idx--;
            if (idx >= 0)
                SelectedCameraDevice = ConnectedDevices[idx];
        }
    }
}
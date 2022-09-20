using CameraControl.Devices.Classes;
using CameraControl.Devices.GoPro;
using Newtonsoft.Json.Linq;
using PortableDeviceLib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CameraControl.Devices.Others
{
    public class GoProBaseCamera: BaseCameraDevice
    {
        private GoProBluetoothDevice _bluetoothDevice;
        protected bool _useOpenGoPro = false;
        protected Timer _timer = new System.Timers.Timer(2000);
        protected Timer _keepAlivetimer = new System.Timers.Timer(2500);
        private object _locker = new object();
        public string Address { get; set; }

        public PropertyValue<long> Fps { get; set; }

        public GoProBaseCamera()
        {
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.LiveViewStream);
            Capabilities.Add(CapabilityEnum.RecordMovie);
            LiveViewImageZoomRatio = new PropertyValue<long>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";
            Manufacturer = "GoPro";
            
        }

        public virtual void Init(string address, JObject json, GoProBluetoothDevice bluetoothDevice)
        {
            _bluetoothDevice = bluetoothDevice;
            Address = "http://" + address;
            DeviceName = json["info"]["model_name"].Value<string>();
            SerialNumber = json["info"]["serial_number"].Value<string>();
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();
            _keepAlivetimer.Elapsed += _keepAlivetimer_Elapsed;
            _keepAlivetimer.AutoReset = true;
            _keepAlivetimer.Start();
            IsConnected = true;
            //-------------------
            IsoNumber = new PropertyValue<long> { Available = false };
            
            //-------------------
            Mode = new PropertyValue<long> { Tag = "144", Available = true, IsEnabled = true };
            Mode.AddValues("Video", 0);
            Mode.AddValues("Photo", 1);
            Mode.AddValues("MultiShotMode", 2);
            Mode.ValueChanged += Mode_ValueChanged;
            Mode.ReloadValues();
            CompressionSetting = new PropertyValue<long> {Tag="2", Available = true, IsEnabled = true };
            CompressionSetting.AddValues("5K", 24);
            CompressionSetting.AddValues("4K", 1);
            CompressionSetting.AddValues("4K 4:3", 18);
            CompressionSetting.AddValues("2.7K", 4);
            CompressionSetting.AddValues("2.7K 4:3", 6);
            CompressionSetting.AddValues("1440", 7);
            CompressionSetting.AddValues("1080", 9);
            CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
            CompressionSetting.ReloadValues();
            Fps = new PropertyValue<long> { Tag = "3", Available = true, IsEnabled = true, Name = "FPS" };
            Fps.AddValues("240 - 8X", 0);
            Fps.AddValues("200 - 8X", 13);
            Fps.AddValues("120 - 4X", 1);
            Fps.AddValues("60  - 2X", 5);
            Fps.AddValues("30  - 1X", 8);
            Fps.AddValues("24  - 1X", 10);
            Fps.ValueChanged += Fps_ValueChanged;
            Fps.ReloadValues();
            AdvancedProperties.Add(Fps);
        }

        private void Fps_ValueChanged(object sender, string key, long val)
        {
            GetJson("gp/gpControl/setting/3/" + val.ToString());
        }

        private void CompressionSetting_ValueChanged(object sender, string key, long val)
        {
            GetJson("gp/gpControl/setting/2/" + val.ToString());
        }

        private void Mode_ValueChanged(object sender, string key, long val)
        {
            try
            {
                GetJson("gp/gpControl/command/sub_mode?mode=" + val.ToString() + "&sub_mode=0" );
            }
            catch (Exception ex)
            {

                
            }
        }


        private void _keepAlivetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _keepAlivetimer.Stop();
                var keep_alive_payload = "_GPHD_:1:0:2:0.000000\n";
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                EndPoint endPoint = new IPEndPoint(IPAddress.Parse(Address.Replace("http://", "")), 8554);
                s.SendTo(Encoding.ASCII.GetBytes(keep_alive_payload), endPoint);
                if(_useOpenGoPro)
                    GetJson("gopro/camera/keep_alive");
            }
            catch (Exception ex)
            {

                
            }
            _keepAlivetimer.Start();
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //_timer.Stop();
            Task.Factory.StartNew(GetEvent);
        }

        public override string GetLiveViewStream()
        {
            return String.Format("udp://10.5.5.9:8554");
        }
        public override void StartLiveView()
        {
            GetJson("gp/gpControl/execute?p1=gpStream&c1=stop");
            var res = GetJson("gp/gpControl/execute?p1=gpStream&c1=restart");
            // handle if there any error

        }

        public override void StopLiveView()
        {
            GetJson("gp/gpControl/execute?p1=gpStream&c1=stop");
        }

        public override void CapturePhoto()
        {
            StopLiveView();
            IsBusy = true;
            if (Mode.Value != "Photo")
            {
                Mode.SetValue("Photo");
                Thread.Sleep(1000);
            }
            if (_bluetoothDevice != null)
            {
                _bluetoothDevice.ExecuteCommand(GoProBluetoothDevice.BLE_CMD_CAPTURE_ON);
            }
            else
            {
                var json = GetJson(_useOpenGoPro ? "/gopro/camera/shutter/start" : "gp/gpControl/command/shutter?p=1");
                
            }
            Thread.Sleep(500);
            while (IsBusy)
            {
                Thread.Sleep(50);
                GetEvent();
                Console.WriteLine("====");
            }
            var filename = GetLastMedia();
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                WiaImageItem = null,
                EventArgs = new PortableDeviceEventArgs(),
                CameraDevice = this,
                FileName = filename,
                Handle = filename
            };
            OnPhotoCapture(this, args);
        }

        public override void StartRecordMovie()
        {
            StopLiveView();
            IsBusy = true;
            if (Mode.Value != "Video")
            {
                Mode.SetValue("Video");
                Thread.Sleep(1000);
            }
            if (_bluetoothDevice != null)
            {
                _bluetoothDevice.ExecuteCommand(GoProBluetoothDevice.BLE_CMD_CAPTURE_ON);
            }
            else
            {
                var json = GetJson(_useOpenGoPro ? "/gopro/camera/shutter/start" : "gp/gpControl/command/shutter?p=1");
            }
        }

        public override void StopRecordMovie()
        {

            if (_bluetoothDevice != null)
            {
                _bluetoothDevice.ExecuteCommand(GoProBluetoothDevice.BLE_CMD_CAPTURE_OFF);
            }
            else
            {
                var json = GetJson(_useOpenGoPro ? "/gopro/camera/shutter/stop" : "gp/gpControl/command/shutter?p=0");
            }
            Thread.Sleep(500);
            while (IsBusy)
            {
                Thread.Sleep(50);
                GetEvent();
                Console.WriteLine("====");
            }
            var filename = GetLastMedia();
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                WiaImageItem = null,
                EventArgs = new PortableDeviceEventArgs(),
                CameraDevice = this,
                FileName = filename,
                Handle = filename
            };
            OnPhotoCapture(this, args);
        }
        public override void TransferFile(object o, string filename)
        {
            _keepAlivetimer.Stop();
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(String.Format("{0}/videos/DCIM/{1}",Address,(string)o), filename, this);
            _keepAlivetimer.Start();
        }
        public override void TransferFileThumb(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(String.Format(_useOpenGoPro ? "{0}/gopro/media/screennail?path={1}" : "{0}/gp/gpMediaMetadata?p={1}", Address, (string)o), filename, this);
        }
        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            _keepAlivetimer.Stop();
            AsyncObservableCollection<DeviceObject> res = new AsyncObservableCollection<DeviceObject>();
            var json = GetJson(_useOpenGoPro ? "gopro/media/list" : "gp/gpMediaList");
            foreach (var item in json["media"])
            {
                var folder = (string)item["d"];
                foreach (var fileItem in item["fs"])
                {
                    var file = (string)fileItem["n"];
                    res.Add(new DeviceObject()
                    {
                        FileName = file,
                        Handle = string.Format("{1}/{2}", Address, folder, file),
                        ThumbData = !loadThumbs ? null : (GetData(string.Format(_useOpenGoPro ? "/gopro/media/thumbnail?path={0}/{1}" : "/gp/gpMediaMetadata?p={0}/{1}", folder, file)))
                    }); 
                }
            }
            _keepAlivetimer.Start();
            return res;

        }
        private string GetLastMedia()
        {
            try
            {
                var folder = "";
                var file = "";
                var json = GetJson(_useOpenGoPro ? "gopro/media/list" : "gp/gpMediaList");
                foreach (var item in json["media"])
                {
                    folder = (string)item["d"];
                    foreach(var fileItem in item["fs"])
                    {
                        file = (string)fileItem["n"];
                    }
                }
                return string.Format("{0}/videos/DCIM/{1}/{2}",Address, folder, file);

            }
            catch (Exception ex)
            {
                Log.Error("Error to get last media", ex);
            }
            return "";
        }

        public void GetEvent()
        {
            if (Monitor.TryEnter(Locker, 5000))
            {
                try
                {
                    var json = GetJson(_useOpenGoPro ? "gopro/camera/state" : "gp/gpControl/status");
                    if (json.ContainsKey("status"))
                    {
                        foreach (JProperty item in json["status"])
                        {
                            if (item.Name == "70")
                                Battery = ((int)item.Value);
                            if (item.Name == "8")
                                IsBusy = ((int)item.Value) != 0;
                            if (item.Name == "43" && !_useOpenGoPro)
                            {
                                Mode.SetValue(((int)item.Value), false);
                            }
                            if (item.Name == "96" && _useOpenGoPro)
                            {
                                if (Mode.NumericValue != (long)item.Value)
                                {
                                    Mode.SetValue(((int)item.Value), false);
                                    ReloadSubModes();
                                }
                            }
                        }
                    }
                    if (json.ContainsKey("settings"))
                    {
                        foreach (JProperty item in json["settings"])
                        {
                            if (Mode != null && Mode.Tag == item.Name && !_useOpenGoPro)
                            {
                                if (Mode.NumericValue != (long)item.Value)
                                {
                                    Mode.SetValue((long)item.Value, false);
                                    ReloadSubModes();
                                }
                            }
                            if (CompressionSetting != null && CompressionSetting.Tag == item.Name)
                                CompressionSetting.SetValue((long)item.Value, false);
                            
                            if (ShutterSpeed != null && ShutterSpeed.Tag == item.Name)
                                ShutterSpeed.SetValue((long)item.Value, false);
                            
                            if (WhiteBalance != null && WhiteBalance.Tag == item.Name)
                                WhiteBalance.SetValue((long)item.Value, false);

                            if (ExposureCompensation != null && WhiteBalance.Tag == item.Name)
                                ExposureCompensation.SetValue((long)item.Value, false);

                            foreach (var property in AdvancedProperties)
                            {
                                if (property.Tag == item.Name)
                                {
                                    if (!property.NumericValues.Contains((long)item.Value))
                                    {
                                        property.Available = false;
                                    }
                                    else
                                    {
                                        property.SetValue((long)item.Value, false);
                                        property.Available = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("GoPro get event error ", ex);

                }
                finally
                {
                    Monitor.Exit(Locker);
                }
                //_timer.Start();
            }
        }
         
        public virtual void ReloadSubModes()
        {

        }
        public JObject GetJson(string endpoint)
        {
            if (Monitor.TryEnter(Locker, 5000))
            {
                try
                {
                    Console.WriteLine(endpoint);
                    string res = "";
                    if (!endpoint.StartsWith("/"))
                        endpoint = "/" + endpoint;
                    using (var client = new WebClient())
                    {
                        res = client.DownloadString(Address + endpoint);
                    }
                    return JObject.Parse(res);
                }
                catch (Exception ex)
                {
                    return new JObject();

                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            else
            {
                return new JObject();
            }

        }
        public byte[] GetData(string endpoint)
        {
            if (Monitor.TryEnter(Locker, 5000))
            {
                try
                {
                    if (!endpoint.StartsWith("/"))
                        endpoint = "/" + endpoint;
                    using (var client = new WebClient())
                    {
                        return client.DownloadData(Address + endpoint);
                    }
                }
                catch (Exception ex)
                {
                    return null;

                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            else
            {
                return null;
            }

        }

    }
}

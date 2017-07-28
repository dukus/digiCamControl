using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using CameraControl.Devices.TransferProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableDeviceLib;

namespace CameraControl.Devices.Others
{
    public class YiCamera : BaseCameraDevice
    {
        public YiCameraProtocol Protocol { get; set; }
        public Dictionary<string, string> CurrentValues { get; set; }

        private static AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private static AutoResetEvent _listingEvent = new AutoResetEvent(false);
        private string _lastData = "";
        private bool _timelapse_running = false;



        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            CurrentValues = new Dictionary<string, string>();
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.LiveViewStream);
            Capabilities.Add(CapabilityEnum.RecordMovie);
            Protocol = deviceDescriptor.StillImageDevice as YiCameraProtocol;
            Protocol.DataReceiverd += Protocol_DataReceiverd;

            DeviceName = Protocol.Model;
            Manufacturer = Protocol.Manufacturer;
            IsConnected = true;
            CompressionSetting = new PropertyValue<long> { Tag = "photo_quality" };
            
            Mode = new PropertyValue<long> { Tag = "capture_mode" };
            Mode.AddValues("Single", 0);
            Mode.AddValues("Burst", 1);
            Mode.AddValues("Delayed", 2);
            //Mode.AddValues("TimeLapse", 3);
            Mode.ReloadValues();

            ExposureMeteringMode = new PropertyValue<long>() { Tag = "meter_mode" };
            
            LiveViewImageZoomRatio = new PropertyValue<long>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";
            SendCommand(3);
            Thread.Sleep(500);

            SerialNumber = GetValue("serial_number");
            SetProperty(Mode.Tag, GetValue(Mode.Tag));


            var thread = new Thread(LoadProperties) { Priority = ThreadPriority.Lowest };
            thread.Start();
            return true;
        }

        private void LoadProperties()
        {
            try
            {
                IsoNumber = new PropertyValue<long> {Available = false};
                FNumber = new PropertyValue<long> {Available = false};
                ExposureCompensation = new PropertyValue<long> {Available = false};
                FocusMode = new PropertyValue<long> {Available = false};
                ShutterSpeed = new PropertyValue<long> {Available = false};
                WhiteBalance = new PropertyValue<long> {Available = false};

                Properties.Add(AddNames("photo_size", "Photo size"));
                Properties.Add(AddNames("precise_selftime", "Capture delay"));
                Properties.Add(AddNames("burst_capture_number", "Burst capture number"));
                Properties.Add(AddNames("auto_low_light", "Auto low light"));
                
                AdvancedProperties.Add(AddNames("video_resolution", "Video resolution"));
                AdvancedProperties.Add(AddNames("led_mode", "Led mode"));
                AdvancedProperties.Add(AddNames("auto_power_off", "Auto power off"));
                AdvancedProperties.Add(AddNames("loop_record", "Loop record"));
                AdvancedProperties.Add(AddNames("warp_enable", "Lens correction"));
                AdvancedProperties.Add(AddNames("buzzer_ring", "Find device"));

                CompressionSetting.ValueChanged +=
                    (sender, key, val) => { Protocol.SendValue(CompressionSetting.Tag, key); };
                SendCommand(9, CompressionSetting.Tag);

                Mode.ValueChanged += Mode_ValueChanged;
                SendCommand(9, Mode.Tag);

                ExposureMeteringMode.ValueChanged +=
                    (sender, key, val) => { Protocol.SendValue(ExposureMeteringMode.Tag, key); };
                SendCommand(9, ExposureMeteringMode.Tag);

                foreach (var property in Properties)
                {
                    SendCommand(9, property.Tag);
                }
                foreach (var property in AdvancedProperties)
                {
                    SendCommand(9, property.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to load data", ex);
            }
        }

        void Mode_ValueChanged(object sender, string key, long val)
        {
            switch (val)
            {
                case 0:
                    Protocol.SendValue("capture_mode", "precise quality");
                    break;
                case 1:
                    Protocol.SendValue("capture_mode", "burst quality");
                    break;
                case 2:
                    Protocol.SendValue("capture_mode", "precise self quality");
                    break;
                case 3:
                    Protocol.SendValue("capture_mode", "precise quality cont.");
                    break;
            }
        }

        public void SendCommand(int command)
        {
            _resetEvent.Reset();
            Protocol.SendCommand(command);
            _resetEvent.WaitOne(500);
        }

        public void SendCommand(int command, string param)
        {
            _resetEvent.Reset();
            Protocol.SendCommand(command, param);
            _resetEvent.WaitOne(1000);
        }

        private PropertyValue<long> AddNames(string keyname, string  name)
        {
            var res = new PropertyValue<long>
            {
                Tag = keyname,
                Name = name
            };
            res.ValueChanged += (sender, key, val) => { Protocol.SendValue(res.Tag, key); };
            return res;
        }

        private string GetValue(string key)
        {
            return CurrentValues.ContainsKey(key) ? CurrentValues[key] : "";
        }

        void Protocol_DataReceiverd(object sender, string data)
        {
            try
            {
                if (data.Contains("msg_id"))
                {
                    dynamic resp = JsonConvert.DeserializeObject(data);
                    switch ((string)resp.msg_id)
                    {
                        case "3": // allproperties
                            CurrentValues.Clear();
                            foreach (JObject o in resp.param)
                            {
                                var k = o.ToObject<Dictionary<string, string>>();
                                CurrentValues.Add(k.First().Key, k.First().Value);
                            }
                            break;
                        case "7":
                            switch ((string)resp.type)
                            {
                                case "burst_complete":
                                case "photo_taken":
                                    string filename = Path.GetFileName(((string)resp.param).Replace('/', '\\'));
                                        PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                                                                          {
                                                                              WiaImageItem = null,
                                                                              EventArgs =new PortableDeviceEventArgs(),
                                                                              CameraDevice = this,
                                                                              FileName = filename,
                                                                              Handle = filename
                                                                          };
                                        OnPhotoCapture(this, args);
                                    break;
                                default:
                                    SetProperty((string)resp.type, (string)resp.param);
                                    break;
                            }
                            break;
                        case "9":
                            SetValues(data);
                            break;
                    }
                    _lastData = data;
                    if (data.Contains("listing"))
                        _listingEvent.Set();
                }
            }
            catch (Exception)
            {

            }
            _resetEvent.Set();
        }

        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            var res = new AsyncObservableCollection<DeviceObject>();
            SendCommand(1283, "\\/var\\/www\\/DCIM\\/100MEDIA/");
            _listingEvent.Reset();
            SendCommand(1282, " -D -S");
            _listingEvent.WaitOne(2500);
            dynamic resp = JsonConvert.DeserializeObject(_lastData);
            WebClient client = new WebClient();
            foreach (JObject o in resp.listing)
            {
                var k = o.ToObject<Dictionary<string, string>>();
                string v = k.First().Value;
                var file = new DeviceObject();
                file.FileName = k.First().Key;
                if (file.FileName.ToLower().Contains("thm"))
                    continue;
                file.Handle = file.FileName;
                try
                {
                    if (loadThumbs)
                    {
                        if (file.FileName.Contains(".jpg"))
                            file.ThumbData =
                                client.DownloadData(string.Format("http://{0}/DCIM/100MEDIA/{1}?type=thumb", Protocol.Ip,
                                    file.FileName));
                        if (file.FileName.Contains(".mp4"))
                            file.ThumbData =
                                client.DownloadData(string.Format("http://{0}/DCIM/100MEDIA/{1}?type=thumb", Protocol.Ip,
                                    file.FileName.Replace("mp4", "THM")));
                    }
                    if (v.Contains("|"))
                        file.FileDate = DateTime.ParseExact(v.Split('|')[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {

                }
                res.Add(file);
            }
            return res;
        }

        public override void FormatStorage(object storageId)
        {
            var o = GetObjects(storageId, false);
            foreach (var deviceObject in o)
            {
                DeleteObject(deviceObject);
            }
        }

        public override void CapturePhotoNoAf()
        {
            CapturePhoto();
        }

        public override void CapturePhoto()
        {
            IsBusy = true;
            if (Mode.NumericValue == 3)
                _timelapse_running = !_timelapse_running;
            else
                _timelapse_running = false;
            Mode.IsEnabled = !_timelapse_running;
            Protocol.SendCommand(_timelapse_running ? 770 : 769);
        }


        public override void StartRecordMovie()
        {
            Protocol.SendCommand(513);
        }

        public override void StopRecordMovie()
        {
            Protocol.SendCommand(514);
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            SendCommand(1283, "\\/var\\/www\\/DCIM\\/100MEDIA/");
            SendCommand(1281, (string) deviceObject.Handle);
            return true;
        }

        public override void TransferFile(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(String.Format("http://{0}/DCIM/100MEDIA/{1}", Protocol.Ip, o), filename, this);
        }

        public override void TransferFileThumb(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(String.Format("http://{0}/DCIM/100MEDIA/{1}?type=screen", Protocol.Ip, o), filename,
                this);
        }

        private void SetProperty(string prop, string val)
        {
            switch (prop)
            {
                case "battery":
                    int i;
                    if (int.TryParse(val, out i))
                        Battery = i;
                    break;
                case "photo_quality":
                    CompressionSetting.Value = val;
                    break;
                case "capture_mode":
                    switch (val)
                    {
                        case "precise quality":
                            Mode.SetValue(0);
                            break;
                        case "burst quality":
                            Mode.SetValue(1);
                            break;
                        case "precise self quality":
                            Mode.SetValue(2);
                            break;
                        case "precise quality cont.":
                            Mode.SetValue(3);
                            break;
                    }
                    break;
                case "meter_mode":
                    ExposureMeteringMode.Value = val;
                    break;
                default:
                    foreach (var property in Properties)
                    {
                        if (property.Tag == prop)
                            property.Value = val;
                    }
                    foreach (var property in AdvancedProperties)
                    {
                        if (property.Tag == prop)
                            property.Value = val;
                    }
                    break;
            }
        }

        private void SetValues(string data)
        {
            List<string> values=new List<string>();
            dynamic a = JsonConvert.DeserializeObject(data);
            foreach (string o in a.options)
            {
                values.Add(o);
            }
            string param = (string) a.param;
            switch (param)
            {
                case "photo_quality":
                    CompressionSetting.Clear();
                    for (int i = 0; i < values.Count; i++)
                    {
                        CompressionSetting.AddValues(values[i], i);
                    }
                    CompressionSetting.IsEnabled = a.permission == "settable";
                    CompressionSetting.ReloadValues();
                    CompressionSetting.Value = GetValue(param);
                    break;
                case "meter_mode":
                    ExposureMeteringMode.Clear();
                    for (int i = 0; i < values.Count; i++)
                    {
                        ExposureMeteringMode.AddValues(values[i], i);
                    }
                    ExposureMeteringMode.ReloadValues();
                    ExposureMeteringMode.IsEnabled = a.permission == "settable";
                    ExposureMeteringMode.Value = GetValue(param);
                    break;
                default:
                    foreach (var property in Properties)
                    {
                        if (property.Tag == param)
                        {
                            property.Clear();
                            for (int i = 0; i < values.Count; i++)
                            {
                                property.AddValues(values[i], i);
                            }
                            property.IsEnabled = a.permission == "settable";
                            property.ReloadValues();
                            property.Value = GetValue(param);                            
                        }
                    }
                    foreach (var property in AdvancedProperties)
                    {
                        if (property.Tag == param)
                        {
                            property.Clear();
                            for (int i = 0; i < values.Count; i++)
                            {
                                property.AddValues(values[i], i);
                            }
                            property.IsEnabled = a.permission == "settable";
                            property.ReloadValues();
                            property.Value = GetValue(param);
                        }
                    }
                    break;
            }

        }


        public override string GetLiveViewStream()
        {
            return String.Format("rtsp://{0}/live",Protocol.Ip);
        }

        public override void StartLiveView()
        {
            Protocol.SendCommand(259, "none_force");
        }

        public override string ToString()
        {

            StringBuilder c = new StringBuilder(base.ToString() + "\n\tType..................YiA");
            return c.ToString();
        }
    }
}

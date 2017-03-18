using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using Timer = System.Timers.Timer;

namespace PanonoTest
{
    public class PanonoCamera : BaseCameraDevice
    {
        private WebSocket _ws;
        private static int request_id = 0;
        private Timer _timer = new Timer(500);


        public bool Init(string endpoint)
        {
            
            _ws = new WebSocket(endpoint);
            _ws.WaitTime = new TimeSpan(0, 0, 4);
            _ws.OnMessage += Ws_OnMessage;
            _ws.OnClose += _ws_OnClose;
            _ws.OnError += _ws_OnError;
            _ws.Connect();
            LiveViewImageZoomRatio = new PropertyValue<long>();
            Auth();
            IsConnected = true;
            IsoNumber = new PropertyValue<long> {Available = true};
            FNumber = new PropertyValue<long> {Available = false};
            ExposureCompensation = new PropertyValue<long> {Available = false};
            FocusMode = new PropertyValue<long> {Available = false};
            ShutterSpeed = new PropertyValue<long> {Available = false};
            WhiteBalance = new PropertyValue<long> {Available = false};
            InitIso();
            InitMode();
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
           // _timer.Start();
            return true;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //using (WebClient client = new WebClient())
                //{
                //    var s = client.DownloadString("http://" + _ws.Url.Host + "/update_image");
                //}
            }
            catch (WebException ex)
            {
                if ((uint)ex.HResult != 0x80131515)
                    Log.Error("Ping error ", ex);
            }
        }

        private void _ws_OnError(object sender, ErrorEventArgs e)
        {
            StaticHelper.Instance.SystemMessage = e.Message;
        }

        private void _ws_OnClose(object sender, CloseEventArgs e)
        {
            Close();
            OnCameraDisconnected(this, new DisconnectCameraEventArgs {});
        }

        public override void Close()
        {
            _timer.Stop();
            try
            {
                _ws.OnMessage -= Ws_OnMessage;
                _ws.OnClose -= _ws_OnClose;
                _ws.OnError -= _ws_OnError;
                _ws.Close();
            }
            catch (Exception e)
            {
                Log.Error("Camera close error", e);
            }
        }

        private void InitIso()
        {
            IsoNumber.Tag = "ISO";
            IsoNumber.AddValues("50", 50);
            IsoNumber.AddValues("100", 100);
            IsoNumber.AddValues("200", 200);
            IsoNumber.AddValues("400", 400);
            IsoNumber.AddValues("800", 800);
            IsoNumber.AddValues("1600", 1600);
            IsoNumber.ReloadValues();
            IsoNumber.ValueChanged += IsoNumber_ValueChanged;
            ExecuteMethod("get_options", IsoNumber.Tag);
        }

        private void InitMode()
        {
            Mode = new PropertyValue<long> {Tag = "ImageType"};
            Mode.AddValues("Default", 0);
            Mode.AddValues("HDR", 1);
            Mode.ReloadValues();
            Mode.ValueChanged += Mode_ValueChanged;
            ExecuteMethod("get_options", Mode.Tag);
        }


        private void SetProperty(string key, string value)
        {
            _ws.Send("{ \"id\":3,\"method\":\"set_options\",\"params\":{ \"" + key + "\":\"" + value +
                     "\"},\"jsonrpc\":\"2.0\"}");
        }

        private void Mode_ValueChanged(object sender, string key, long val)
        {
            SetProperty(Mode.Tag, key);
        }

        private void IsoNumber_ValueChanged(object sender, string key, long val)
        {
            SetProperty(IsoNumber.Tag, key);
        }

        public void Auth()
        {
            ExecuteMethod("auth", "{\"device\":\"test\",\"force\":\"test\"}");
        }

        public override void CapturePhoto()
        {
            IsBusy = true;
            ExecuteMethod("capture");
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            return true;
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Data);
                if (json["result"] != null)
                {
                    var values = json["result"].ToObject<Dictionary<string, object>>();
                    if (values.ContainsKey("serial_number"))
                        SerialNumber = (string)values["serial_number"];
                    if (values.ContainsKey(IsoNumber.Tag))
                        IsoNumber.SetValue((string)values[IsoNumber.Tag], false);

                    if (values.ContainsKey(Mode.Tag))
                        Mode.SetValue((string)values[Mode.Tag], false);
                }
                if ((string)json["method"] == "status_update")
                {
                    if (json["params"]["battery_value"] != null)
                    {
                        Battery = json["params"]["battery_value"].Value<int>();
                    }
                    if (json["params"]["capture_available"] != null)
                    {
                        IsBusy = json["params"]["capture_available"].Value<bool>();
                    }
                }
                if ((string)json["method"] == "upf_infos_update")
                {
                    if (json["params"] != null)
                    {
                        var values = json["params"]["upf_infos"][0].ToObject<Dictionary<string, object>>();
                        if (values.ContainsKey("upf_status") && (string)values["upf_status"] == "ready")
                        {
                            string url = (string)values["upf_url"];
                            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                            {
                                WiaImageItem = null,
                                EventArgs = null,
                                CameraDevice = this,
                                FileName = url.Replace('/', '\\'),
                                Handle = new List<object>() { url, (long)values["upf_size"] }
                            };
                            OnPhotoCapture(this, args);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public override void TransferFile(object o, string filename)
        {
            _timer.Stop();
            Thread.Sleep(100);
            TransferProgress = 0;
            var param = o as List<object>;
            HttpHelper.DownLoadFileByWebRequest((string) param[0], filename, this, (long) param[1]);
            _timer.Start();
        }

        public override void TransferFileThumb(object o, string filename)
        {
            _timer.Stop();
            Thread.Sleep(100);
            TransferProgress = 0;
            var param = o as List<object>;
            HttpHelper.DownLoadFileByWebRequest(((string)param[0]).Replace("panoramas", "previews"), filename,
                this);
            _timer.Start();
        }

        private void ExecuteMethod(string method, params object[] prms)
        {
            _ws.Send(CreateJson(method, prms));
        }


        private int GetId()
        {
            var id = Interlocked.Increment(ref request_id);
            if (request_id > 1000000000)
            {
                request_id = 0;
            }
            return id;
        }

        private string CreateJson(string name, params object[] prms)
        {
            return CreateJson(name, "2.0", prms);
        }

        private  string CreateJson(string name, string version, params object[] prms)
        {
            var param = new JArray();
            JObject json;
            if (prms != null && prms.Length > 0)
            {
                foreach (var p in prms)
                {
                    param.Add(p);
                }
                json = new JObject(
                    new JProperty("id", GetId()),
                    new JProperty("method", name),
                    new JProperty("params", param),
                    new JProperty("jsonrpc", version));
            }
            else
            {
                json = new JObject(
                    new JProperty("id", GetId()),
                    new JProperty("method", name),
                    new JProperty("jsonrpc", version));
            }


            return json.ToString(Formatting.None);
        }
    }
}

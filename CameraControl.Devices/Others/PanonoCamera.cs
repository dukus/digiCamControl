using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace PanonoTest
{
    public class PanonoCamera: BaseCameraDevice
    {
        private WebSocket _ws;
        private static int request_id = 0;

        public bool Init(string endpoint)
        {
            _ws = new WebSocket(endpoint);
            _ws.OnMessage += Ws_OnMessage;
            _ws.Connect();
            LiveViewImageZoomRatio = new PropertyValue<int>();
            Auth();
            IsConnected = true;
            return true;
        }

        public void Auth()
        {
            ExecuteMethod("auth", "{\"device\":\"test\",\"force\":\"test\"}");
        }

        public override void CapturePhoto()
        {
            ExecuteMethod("capture");
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            var json = JObject.Parse(e.Data);
            if (json["result"] !=null)
            {
                var values = json["result"].ToObject<Dictionary<string, object>>();
                if (values.ContainsKey("serial_number"))
                    SerialNumber = (string)values["serial_number"];

            }
            if ((string)json["method"]== "status_update")
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
            if ((string) json["method"] == "upf_infos_update")
            {
                if (json["params"] != null)
                {
                    var values = json["params"]["upf_infos"][0].ToObject<Dictionary<string, object>>();
                    if (values.ContainsKey("upf_status") && (string) values["upf_status"] == "ready")
                    {
                        string url = (string) values["upf_url"];
                        PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                        {
                            WiaImageItem = null,
                            EventArgs = null,
                            CameraDevice = this,
                            FileName = url.Replace('/', '\\'),
                            Handle = new List<object>() {url, (long)values["upf_size"] }
                        };
                        OnPhotoCapture(this, args);
                    }
                }
            }
        }

        public override void TransferFile(object o, string filename)
        {
            TransferProgress = 0;
            var param = o as List<object>;
            HttpHelper.DownLoadFileByWebRequest((string) param[0], filename, this, (long) param[1]);
        }

        public override void TransferFileThumb(object o, string filename)
        {
            TransferProgress = 0;
            var param = o as List<object>;
            HttpHelper.DownLoadFileByWebRequest(((string)param[0]).Replace("panoramas", "previews"), filename,
                this);
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

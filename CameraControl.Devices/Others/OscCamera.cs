using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CameraControl.Devices.Others
{
    public class OscCamera : BaseCameraDevice
    {
        private string ExecuteEndPoint = "/osc/commands/execute";
        private string InfoEndPoint = "/osc/info";
        private string StatusEndPoint = "/osc/commands/status";


        public string Address { get; set; }
        public string SessionId { get; set; }

        public void Init(string address)
        {

            LiveViewImageZoomRatio = new PropertyValue<long>();
            IsConnected = true;
            IsoNumber = new PropertyValue<long> {Available = true};
            FNumber = new PropertyValue<long> {Available = false};
            ExposureCompensation = new PropertyValue<long> {Available = true};
            FocusMode = new PropertyValue<long> {Available = false};
            ShutterSpeed = new PropertyValue<long> {Available = false};
            WhiteBalance = new PropertyValue<long> {Available = false};
            Mode = new PropertyValue<long>() {Available = true};
            SessionId = null;
            Address = address;
            GetInfo();
            GetSessionId(GetExecute(CreateJson("camera.startSession")));
            //SetProperty("clientVersion", 2);
            InitIso();
            InitExposureCompensation();
            InitMode();
        }


        private void InitIso()
        {
            try
            {
                IsoNumber = new PropertyValue<long> { Available = true, IsEnabled = true};
                string property = "iso";
                var response = GetProperty(property);
//                response =
                //"{\"name\":\"camera.getOptions\",\"state\":\"done\",\"results\":{\"options\":{\"iso\":100,\"isoSupport\":[100,125,160,200,250,320,400,500,640,800,1000,1250,1600]}}}";
                var json = Initialize(response);
                var val = json["results"]["options"][property].Value<string>();
                var vals = json["results"]["options"][property + "Support"].Values<int>().ToList();
                foreach (int i in vals)
                {
                    IsoNumber.AddValues(i == 0 ? "Auto" : i.ToString(), i);
                }
                if (val == "0")
                {
                    if (!vals.Contains(0))
                        IsoNumber.AddValues("Auto", 0);
                    IsoNumber.Value = "Auto";
                    IsoNumber.IsEnabled = false;
                }
                else
                    IsoNumber.Value = val;
                IsoNumber.ReloadValues();
                IsoNumber.ValueChanged += (o, s, i) => SetProperty(property, i);
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to get ISO", ex);
                throw;
            }
        }

        private void InitExposureCompensation()
        {
            try
            {
                string property = "exposureCompensation";
                var response = GetProperty(property);
//                response =
//                    "{\"name\":\"camera.getOptions\",\"state\":\"done\",\"results\":{\"options\":{\"exposureCompensation\":0.0,\"exposureCompensationSupport\":[-2.0,-1.7,-1.3,-1.0,-0.7,-0.3,0.0,0.3,0.7,1.0,1.3,1.7,2.0]}}}";
                var json = Initialize(response);
                var val = json["results"]["options"][property].Value<string>();
                var vals = json["results"]["options"][property + "Support"].Values<string>();
                foreach (string s in vals)
                {
                    ExposureCompensation.AddValues(s, 0);
                }
                ExposureCompensation.Value = val;
                ExposureCompensation.ReloadValues();
                ExposureCompensation.ValueChanged += (o, s, i) => SetProperty(property, double.Parse(s,CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to get exposureCompensation", ex);

            }

        }

        private void InitMode()
        {
            try
            {
                string property = "exposureProgram";
                var response = GetProperty(property);
                var json = Initialize(response);
                var val = json["results"]["options"][property].Value<uint>();
                //var vals = json["results"]["options"][property + "Support"].Values<string>();
               // Mode.AddValues("Manual program", 1);
                Mode.AddValues("Auto", 2);
               // Mode.AddValues("Shutter priority program", 4);
                Mode.AddValues("ISO priority program", 9);

                Mode.ReloadValues();
                Mode.ValueChanged += (o, s, i) =>
                {
                    SetProperty(property, i);
                    InitIso();
                };
                Mode.SetValue(val); 
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to get exposureProgram", ex);

            }

        }

        private string GetProperty(string name)
        {
            var param = new JArray();
            param.Add(name);
            param.Add(name + "Support");
            var s = CreateJson("camera.getOptions", new JProperty("optionNames", param));
            return GetExecute(s);
        }

        public void SetProperty(string name, object val)
        {
            try
            {
                var p = new JObject();
                p.Add(new JProperty(name, val));
                var s = CreateJson("camera.setOptions", new JProperty("options", p));
              //  Log.Debug(s);
                var res = GetExecute(s);
                //Log.Debug(res);
                Initialize(res);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to set property " + name+" "+val.ToString(), ex);
            }
        }

        private void GetInfo()
        {
            var response = Get(InfoEndPoint);
            var json = Initialize(response);
            Manufacturer = json["manufacturer"].Value<string>();
            DeviceName = json["model"].Value<string>();
            SerialNumber = json["serialNumber"].Value<string>();
        }

        private void GetSessionId(string response)
        {
            //response = "{\"name\":\"camera.startSession\",\"state\":\"done\",\"results\":{ \"sessionId\":\"SID_0001\",\"timeout\":180}}";
            var json = Initialize(response);
            SessionId = json["results"]["sessionId"].Value<string>();
        }

        public override void CapturePhoto()
        {
            var json = Initialize(GetExecute(CreateJson("camera.takePicture")));
            string id = json["id"].Value<string>();
            string url = null;
            do
            {
                var jsonStatus = Initialize(GetStatus(id));
                if (jsonStatus["state"].Value<string>() == "done")
                {
                    url = jsonStatus["results"]["fileUri"].Value<string>();
                }
                else
                {
                    Thread.Sleep(150);
                }
            } while (string.IsNullOrEmpty(url));
            Log.Debug("Url to process " + url);
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                WiaImageItem = null,
                EventArgs = null,
                CameraDevice = this,
                FileName = url.Replace('/', '\\'),
                Handle = url
            };
            OnPhotoCapture(this, args);
        }

        public override void TransferFile(object o, string filename)
        {
            string url = ((string) o);
            if (!url.StartsWith("http"))
            {
                url = Address + "/" + url;
            }
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(url, filename, this);
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            try
            {
                string url = ((string)deviceObject.Handle);
                var array = new JArray {url};
                var s = CreateJson("camera.delete", new JProperty("fileUrls", array));
                GetExecute(s);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to set property ", ex);
            }
            return true;
        }

        internal static JObject Initialize(string response)
        {
            var json = JObject.Parse(response);
            if (json == null)
            {
                //throw new RemoteApiException(StatusCode.IllegalResponse);
            }
            if (json["error"] != null)
            {
                throw new DeviceException(json["error"]["code"].Value<string>(), json["error"]["message"].Value<string>());
                //throw new RemoteApiException((StatusCode)json["error"].Value<int>(0));
            }
            return json;
        }


        private string CreateJson(string name, params object[] prms)
        {
            var param = new JObject();
            if (!string.IsNullOrEmpty(SessionId))
            {
                param.Add(new JProperty("sessionId", SessionId));
            }

            if (prms != null)
            {
                foreach (var p in prms)
                {
                    param.Add(p);
                }
            }
            var json = new JObject(
                new JProperty("name", name),
                new JProperty("parameters", param));

            return json.ToString(Formatting.Indented);
        }

        private string GetUrl(string command)
        {
            return Address + command;
        }

        public string GetStatus(string id)
        {
            return Get(StatusEndPoint, new JObject(new JProperty("id", id)).ToString(Formatting.None));
        }

        public string GetExecute( string postData = null)
        {
            return Get(ExecuteEndPoint, postData);
        }

        public string Get(string command, string postData=null)
        {
            try
            {
                if (string.IsNullOrEmpty(postData))
                {
                    using (var client = new WebClient())
                    {
                        return client.DownloadString(GetUrl(command));
                    }
                }
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(GetUrl(command));
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Create POST data and convert it to a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/json;charset=utf-8";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                Stream dataStream = null;

                // Get the request stream.
                dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                var res = reader.ReadToEnd();
                // Display the content.
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
                return res;
            }
            catch (WebException exx)
            {
                using (WebResponse response = exx.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse != null)
                    {
                        using (Stream data = httpResponse.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            return "";
        }
    }
}

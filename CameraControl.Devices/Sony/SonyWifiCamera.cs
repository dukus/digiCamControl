using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CameraControl.Devices.Classes;
using PortableDeviceLib;
using Timer = System.Timers.Timer;

namespace CameraControl.Devices.Sony
{
    public class SonyWifiCamera : BaseCameraDevice
    {
        private static int request_id = 0;
        public string EndPoint { get; set; }
        private readonly HttpClient mClient = new HttpClient();
        List<string> AvailableMethods;
        private Timer _timer = new Timer(100);

        public SonyWifiCamera()
        {

        }

        public bool Init(string endpoint)
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            IsBusy = true;
            EndPoint = endpoint;
            AvailableMethods = GetMethodTypes();
            ExecuteMethod("startRecMode");
            IsConnected = true;
            Task.Factory.StartNew(InitProps);
            return true;
        }

        private void InitProps()
        {
            Thread.Sleep(3500);
            InitIso();
            InitShutterSpeed();
            InitFNumber();
            InitFocusMode();
            IsBusy = false;
            _timer.Start();
        }

        private void InitIso()
        {
            IsoNumber=new PropertyValue<int>();
            var prop = IsoNumber;
            var cap = AsCapability<string>(Post(CreateJson("getAvailableIsoSpeedRate")));
            SetCapability(prop, cap);
            prop.ValueChanged += (sender, key, val) => CheckError(Post(CreateJson("setIsoSpeedRate", "1.0", key)));
        }

        private void InitFNumber()
        {
            FNumber = new PropertyValue<int>();
            var prop = FNumber;
            var cap = AsCapability<string>(Post(CreateJson("getAvailableFNumber")));
            SetCapability(prop, cap);
            prop.ValueChanged += (sender, key, val) => CheckError(Post(CreateJson("setFNumber", "1.0", key)));
        }

        private void InitShutterSpeed()
        {
            ShutterSpeed = new PropertyValue<long>();
            var prop = ShutterSpeed;

            var cap = AsCapability<string>(Post(CreateJson("getAvailableShutterSpeed")));

            for (var index = 0; index < cap.Candidates.Count; index++)
            {
                string val = cap.Candidates[index];
                prop.AddValues(val, index);
            }

            if (cap.Candidates.Count == 0 && !string.IsNullOrEmpty(cap.Current))
            {
                prop.AddValues(cap.Current, 0);
                prop.IsEnabled = false;
            }
            prop.Value = cap.Current;
            prop.ReloadValues();
            prop.ValueChanged += (sender, key, val) => CheckError(Post(CreateJson("setShutterSpeed", "1.0", key)));
        }

        private void InitFocusMode()
        {
            var vals = AsPrimitiveList<string>(Post(CreateJson("getSupportedFocusMode")));
            FocusMode = new PropertyValue<long>();
            int i = 0;
            foreach (string val in vals)
            {
                FocusMode.AddValues(val, i++);
            }
            FocusMode.Value = AsPrimitive<string>(Post(CreateJson("getFocusMode")));
            FocusMode.ReloadValues();
            FocusMode.ValueChanged += (sender, key, val) => CheckError(Post(CreateJson("setFocusMode", "1.0", key)));
        }


        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            Task.Factory.StartNew(GetEvent);
        }

        private void GetEvent()
        {
            try
            {
                var jString = Post(CreateJson("getEvent","1.0",false));
                var json = Initialize(jString);
                var jResult = json["result"] as JArray;

                if (jResult == null) return;
                
                var elem = jResult[32];
                if (elem.HasValues)
                {
                    SetCapability(ShutterSpeed, new Capability<string>
                    {
                        Current = elem.Value<string>("currentShutterSpeed"),
                        Candidates = elem["shutterSpeedCandidates"].Values<string>().ToList()
                    });
                }
            }
            catch (Exception)
            {
                
            }
            _timer.Start();
        }

        public override void CapturePhoto()
        {
            var url = AsPrimitiveList<string>(Post(CreateJson("actTakePicture")))[0];
            if (url.Contains("?"))
                url = url.Split('?')[0];
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                WiaImageItem = null,
                EventArgs = new PortableDeviceEventArgs(),
                CameraDevice = this,
                FileName = url.Replace('/', '\\'),
                Handle = url
            };
            OnPhotoCapture(this, args);
        }

        public override void TransferFile(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(((string)o) + "?size=Origin", filename, this);
        }

        public override void TransferFileThumb(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(((string)o) + "?size=Scn", filename,
                this);
        }

        private static void SetCapability<T>(PropertyValue<T> prop, Capability<string> cap)
        {
            foreach (string val in cap.Candidates)
            {
                prop.AddValues(val, default(T));
            }

            if (cap.Candidates.Count == 0 && !string.IsNullOrEmpty(cap.Current))
            {
                prop.AddValues(cap.Current, default(T));
                prop.IsEnabled = false;
            }
            prop.Value = cap.Current;
            prop.ReloadValues();
        }
        
        private void ExecuteMethod(string method, params object[] prms)
        {
            var res = Post(CreateJson(method, prms));
        }

        private List<string> GetMethodTypes()
        {
            var methodTypes = new List<string>();
            var res = Post(CreateJson("getMethodTypes", "1.0", ""));
            var json = Initialize(res);

            foreach (var token in json["results"])
            {
                methodTypes.Add(token.Value<string>(0));
            }
            return methodTypes;
        }

        private static string CreateJson(string name, params object[] prms)
        {
            return CreateJson(name, "1.0", prms);
        }

        private static string CreateJson(string name, string version, params object[] prms)
        {
            var param = new JArray();
            if (prms != null)
            {
                foreach (var p in prms)
                {
                    param.Add(p);
                }
            }
            var json = new JObject(
                new JProperty("method", name),
                new JProperty("version", version),
                new JProperty("id", GetID()),
                new JProperty("params", param));

            return json.ToString(Formatting.None);
        }

        private static int GetID()
        {
            var id = Interlocked.Increment(ref request_id);
            if (request_id > 1000000000)
            {
                request_id = 0;
            }
            return id;
        }

        internal string Post(string body)
        {
            if (EndPoint == null || body == null)
            {
                throw new ArgumentNullException();
            }

            var content = new StringContent(body);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            try
            {
                Task<HttpResponseMessage> task;

                var response = mClient.PostAsync(EndPoint, content).Result;
                return Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);

                //if (response.IsSuccessStatusCode)
                //{
                //    // ReadAsString fails in case of charset=utf-8
                //    return Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);
                //}
                //else
                //{
                //    //Debug.WriteLine("Http Status Error: " + response.StatusCode);
                //    //throw new RemoteApiException((int)response.StatusCode);
                //}
            }
            //catch (RemoteApiException e)
            //{
            //    Debug.WriteLine("Request error: " + e.Message);
            //    Debug.WriteLine("Request error: " + e.StackTrace);
            //    throw e;
            //}
            catch (Exception e)
            {
                throw e;
                //if (e is TaskCanceledException || e is OperationCanceledException)
                //{
                //    Debug.WriteLine("Request cancelled: " + e.StackTrace);
                //    throw new RemoteApiException(StatusCode.Cancelled);
                //}
                //else
                //{
                //    Debug.WriteLine("HttpPost Exception: " + e.StackTrace);
                //    throw new RemoteApiException(StatusCode.NetworkError);
                //}
            }
        }

        internal static void CheckError(string jString)
        {
            var json = Initialize(jString);
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
                //throw new RemoteApiException((StatusCode)json["error"].Value<int>(0));
            }
            return json;
        }

        /// <summary>
        /// For response which has a single Array consists of a single type.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="jString"></param>
        internal static List<T> AsPrimitiveList<T>(string jString)
        {
            var json = Initialize(jString);

            return json["result"][0].Values<T>().ToList();
        }

        internal static T AsPrimitive<T>(string jString)
        {
            var json = Initialize(jString);

            return json["result"].Value<T>(0);
        }

        internal static Capability<T> AsCapability<T>(string jString)
        {
            var json = Initialize(jString);

            return new Capability<T>
            {
                Current = json["result"].Value<T>(0),
                Candidates = json["result"][1].Values<T>().ToList()
            };
        }
    }
}

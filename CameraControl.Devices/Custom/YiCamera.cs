using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using CameraControl.Devices.TransferProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableDeviceLib;
using PortableDeviceLib.Model;

namespace CameraControl.Devices.Custom
{
    public class YiCamera : BaseCameraDevice
    {
        public YiCameraProtocol Protocol { get; set; }


        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.LiveViewStream);
            Protocol = deviceDescriptor.StillImageDevice as YiCameraProtocol;
            DeviceName = Protocol.Model;
            Manufacturer = Protocol.Manufacturer;
            IsConnected = true;
            CompressionSetting = new PropertyValue<int> { Tag = "photo_quality" };
            Mode = new PropertyValue<uint> { Tag = "system_mode" };
            ExposureMeteringMode = new PropertyValue<int>() { Tag = "meter_mode" };
            
            LiveViewImageZoomRatio = new PropertyValue<int>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";

            Protocol.DataReceiverd += Protocol_DataReceiverd;
            var thread = new Thread(LoadProperties) { Priority = ThreadPriority.Lowest };
            thread.Start();
            return true;
        }

        private void LoadProperties()
        {
            IsoNumber = new PropertyValue<int> {Available = false};
            FNumber = new PropertyValue<int> {Available = false};
            ExposureCompensation = new PropertyValue<int> { Available = false };
            FocusMode = new PropertyValue<long> { Available = false };
            ShutterSpeed = new PropertyValue<long> { Available = false };
            WhiteBalance = new PropertyValue<long> { Available = false };

            Properties.Add(AddNames("photo_size", "Photo size"));
            Properties.Add(AddNames("video_resolution", "Video resolution"));

            SerialNumber = GetValue("serial_number");
            
            CompressionSetting.ValueChanged += (sender, key, val) => { Protocol.SendValue(CompressionSetting.Tag, key); };
            Protocol.SendCommand(9, CompressionSetting.Tag);
            Thread.Sleep(250);

            Mode.ValueChanged += (sender, key, val) => { Protocol.SendValue(Mode.Tag, key); };
            Protocol.SendCommand(9, Mode.Tag);
            Thread.Sleep(250);

            ExposureMeteringMode.ValueChanged += (sender, key, val) => { Protocol.SendValue(ExposureMeteringMode.Tag, key); };
            Protocol.SendCommand(9, ExposureMeteringMode.Tag);
            Thread.Sleep(250);

            foreach (var property in Properties)
            {
                Protocol.SendCommand(9, property.Tag);
                Thread.Sleep(250);
            }
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
            return Protocol.CurrentValues.ContainsKey(key) ? Protocol.CurrentValues[key] : "";
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
                        case "7":
                            switch ((string)resp.type)
                            {
                                case "photo_taken":
                                    string filename = System.IO.Path.GetFileName(((string)resp.param).Replace('/', '\\'));
                                    Log.Debug("File name" + filename);
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
                }
            }
            catch (Exception)
            {

            }
        }

        public override void CapturePhoto()
        {
            Protocol.SendCommand(769);
        }

        public override void TransferFile(object o, string filename)
        {
            TransferProgress = 0;
            DownLoadFileByWebRequest(String.Format("http://{0}/DCIM/100MEDIA/{1}", Protocol.Ip, o), filename);
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
                case "system_mode":
                    Mode.Value = val;
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
                case "system_mode":
                    Mode.Clear();
                    for (uint i = 0; i < values.Count; i++)
                    {
                        Mode.AddValues(values[(int) i], i);
                    }
                    Mode.ReloadValues();
                    Mode.IsEnabled = a.permission == "settable";
                    Mode.Value = GetValue(param);
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
                    break;
            }

        }

        private void DownLoadFileByWebRequest(string urlAddress, string filePath)
        {
            try
            {
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                request = (HttpWebRequest)WebRequest.Create(urlAddress);
                request.Timeout = 30000;  //8000 Not work 
                response = (HttpWebResponse)request.GetResponse();
                Stream s = response.GetResponseStream();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                FileStream os = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[102400];
                int c = 0;
                while ((c = s.Read(buff, 0, 102400)) > 0)
                {
                    os.Write(buff, 0, c);
                    os.Flush();
                    TransferProgress += 1;
                }
                os.Close();
                s.Close();
                TransferProgress = 100;
            }
            catch
            {
                return;
            }
            finally
            {
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
    }
}

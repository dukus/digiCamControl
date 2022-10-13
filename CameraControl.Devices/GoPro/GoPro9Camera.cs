using CameraControl.Devices.Classes;
using CameraControl.Devices.GoPro;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Others
{
    public class GoPro9Camera: GoProBaseCamera
    {
        private Dictionary<int, string> _subModes = new Dictionary<int, string>();
        private Dictionary<int, IEnumerable<int>> _subModesGroups = new Dictionary<int, IEnumerable<int>>();
        public PropertyValue<long> SubMode { get; set; }

        override public void Init(string address, JObject json, GoProBluetoothDevice bluetoothDevice)
        {
            _useOpenGoPro = true;

            //Mode.AddValues("Night Photo", 18);
            //Mode.AddValues("Burst Photo", 19);
            //Mode.AddValues("Time Lapse Video", 13);
            //Mode.AddValues("Time Lapse Photo", 20);
            //Mode.AddValues("Night Lapse Photo", 21);
            //Mode.AddValues("Time Warp Video", 24);
            //Mode.AddValues("Live Burst", 25);
            //Mode.AddValues("Night Lapse Video", 26);
            //Mode.AddValues("Slo-Mo", 27);
            //Mode.AddValues("TimeLapse", 3);
            base.BaseInit(address, json, bluetoothDevice);

            foreach (var item in json["modes"])
            {
                _subModes.Add((int)item["id"], (string)item["display_name"]);
            }
            foreach (var item in json["ui_mode_groups"])
            {
                _subModesGroups.Add((int)item["id"], item["modes"].Values<int>());
            }

            Mode = new PropertyValue<long> { Tag = "-1", Available = true, IsEnabled = true };
            Mode.AddValues("Video", 1000);
            Mode.AddValues("Photo", 1001);
            Mode.AddValues("Timelapse", 1002);
            Mode.ValueChanged += Mode_ValueChanged;
            Mode.ReloadValues();
            
            SubMode = new PropertyValue<long> { Tag = "144", Available = true, IsEnabled = false };
            SubMode.ValueChanged += Property_ValueChanged;
            AdvancedProperties.Insert(0, SubMode);
            
            ShutterSpeed = new PropertyValue<long> { Tag = "145", Available = true, IsEnabled = true };
            ShutterSpeed.ValueChanged += Property_ValueChanged;
            LoadJsonValues(ShutterSpeed, json);

            WhiteBalance = new PropertyValue<long> { Tag = "115", Available = true, IsEnabled = true };
            WhiteBalance.ValueChanged += Property_ValueChanged;
            LoadJsonValues(WhiteBalance, json);

            ExposureCompensation = new PropertyValue<long> { Tag = "118", Available = true, IsEnabled = true };
            ExposureCompensation.ValueChanged += Property_ValueChanged;
            LoadJsonValues(ExposureCompensation, json);

            
            AdvancedProperties.Add(LoadJsonValues("121", json));
            AdvancedProperties.Add(LoadJsonValues("122", json));
            AdvancedProperties.Add(LoadJsonValues("123", json));
            AdvancedProperties.Add(LoadJsonValues("59", json));
            AdvancedProperties.Add(LoadJsonValues("87", json));
            AdvancedProperties.Add(LoadJsonValues("88", json));
            AdvancedProperties.Add(LoadJsonValues("147", json));
            GetEvent();
        }

        private PropertyValue<long> LoadJsonValues(string tag, JObject json)
        {
            PropertyValue<long> prop = new PropertyValue<long> { Tag = tag, IsEnabled = true, Available = true };
            prop.ValueChanged += Property_ValueChanged;
            LoadJsonValues(prop, json);
            return prop;        
        }

        private void LoadJsonValues(PropertyValue<long> prop, JObject json)
        {
            prop.Clear();
            foreach (var item in json["settings"])
            {
                if (((int)item["id"]).ToString() == prop.Tag)
                {
                    prop.Name = (string)item["display_name"];
                    foreach (var val in item["options"])
                    {
                        prop.AddValues((string)val["display_name"], (long)val["id"]);
                    }
                }
            }
            prop.ReloadValues();
        }

        private void Property_ValueChanged(object sender, string key, long val)
        {
            GetJson(String.Format("/gopro/camera/setting?setting={0}&option={1}" ,((PropertyValue<long>)sender).Tag, val));
        }

        private void Mode_ValueChanged(object sender, string key, long val)
        {
            GetJson("/gopro/camera/presets/set_group?id=" + val.ToString());
        }
        public override void ReloadSubModes()
        {
            if (_subModesGroups.ContainsKey((int)Mode.NumericValue))
            {
                SubMode.Clear();
                foreach (int item in _subModesGroups[(int)Mode.NumericValue])
                {
                    SubMode.AddValues(_subModes[item], (long)item);
                }
                SubMode.ReloadValues();
            }
        }
        public override void StartLiveView()
        {
            GetJson("/gopro/camera/stream/stop");
            var res = GetJson("/gopro/camera/stream/start");
            //var res = GetJson("gp/gpControl/execute?p1=gpStream&c1=restart");
            // handle if there any error

        }

        public override void StopLiveView()
        {
            var res = GetJson("/gopro/camera/stream/stop");
        }


    }
}

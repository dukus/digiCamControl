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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Serialization;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class CustomConfig : BaseFieldClass
    {
        public string Name { get; set; }
        public AsyncObservableCollection<ValuePair> ConfigData { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public object AttachedObject { get; set; }

        private string _driverName;

        public string DriverName
        {
            get { return _driverName; }
            set
            {
                _driverName = value;
                IExternalDevice source = ServiceProvider.ExternalDeviceManager.Get(_driverName);
                if (source != null)
                {
                    Config = source.GetConfig(this);
                    SourceEnum = source.DeviceType;
                }
                NotifyPropertyChanged("DriverName");
            }
        }

        public SourceEnum SourceEnum { get; set; }
        private UserControl _config;

        [XmlIgnore]
        [JsonIgnore]
        public UserControl Config
        {
            get
            {
                IExternalDevice source = ServiceProvider.ExternalDeviceManager.Get(_driverName);
                if (source != null)
                {
                    _config = source.GetConfig(this);
                }
                return _config;
            }
            set
            {
                _config = value;
                NotifyPropertyChanged("Config");
            }
        }

        public string Get(string name)
        {
            return (from valuePair in ConfigData where valuePair.Name == name select valuePair.Value).FirstOrDefault();
        }

        public void Set(string name, string value)
        {
            foreach (ValuePair valuePair in ConfigData)
            {
                if (valuePair.Name == name)
                {
                    valuePair.Value = value;
                    return;
                }
            }
            ConfigData.Add(new ValuePair() {Name = name, Value = value});
        }

        public CustomConfig()
        {
            ConfigData = new AsyncObservableCollection<ValuePair>();
        }
    }
}
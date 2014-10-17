using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class AutoExportPluginConfig:BaseFieldClass
    {
        private bool _isError;
        private string _error;
        private bool _isRedy;

        [XmlAttribute]
        public string Type { get; set; }
        public ValuePairEnumerator ConfigData { get; set; }
        [XmlAttribute]
        public bool IsEnabled { get; set; }
        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                NotifyPropertyChanged("IsError");
            }
        }

        [XmlIgnore]
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                NotifyPropertyChanged("Error");
            }
        }


        [XmlIgnore]
        public bool IsRedy
        {
            get { return _isRedy; }
            set
            {
                _isRedy = value;
                NotifyPropertyChanged("IsRedy");
            }
        }


        public string DisplayName
        {
            get { return string.IsNullOrEmpty(Name) ? Type : Name; }
        }

        public AutoExportPluginConfig(IAutoExportPlugin plugin)
        {
            Type = plugin.Name;
            IsEnabled = true;
            IsError = false;
            IsRedy = true;
            ConfigData = new ValuePairEnumerator();
        }

        public AutoExportPluginConfig()
        {
            ConfigData = new ValuePairEnumerator();
            IsError = false;
            IsRedy = true;
        }
    }
}

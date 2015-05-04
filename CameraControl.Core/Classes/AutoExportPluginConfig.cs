using System.Collections.ObjectModel;
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
        private ObservableCollection<ValuePairEnumerator> _configDataCollection;
        private bool _isEnabled;
        private string _name;

        [XmlAttribute]
        public string Type { get; set; }
        public ValuePairEnumerator ConfigData { get; set; }

        public ObservableCollection<ValuePairEnumerator> ConfigDataCollection
        {
            get
            {
                if (_configDataCollection.Count == 0)
                {
                    // for compatibility for older versions
                    var pl = ConfigData["TransformPlugin"];
                    if (!string.IsNullOrEmpty(pl))
                    {
                        ConfigDataCollection.Add(ConfigData);
                    }
                }
                return _configDataCollection;
            }
            set
            {
                _configDataCollection = value;
            }
        }

        [XmlAttribute]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged("IsEnabled");
            }
        }

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("DisplayName");
            }
        }

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
            ConfigDataCollection = new ObservableCollection<ValuePairEnumerator>();
        }

        public AutoExportPluginConfig()
        {
            ConfigData = new ValuePairEnumerator();
            ConfigDataCollection = new ObservableCollection<ValuePairEnumerator>();
            IsError = false;
            IsRedy = true;
        }
    }
}

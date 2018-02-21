using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

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
        private ObservableCollection<PluginCondition> _conditions;
        private bool _runAfterTransfer;

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

        public ObservableCollection<PluginCondition> Conditions
        {
            get { return _conditions; }
            set
            {
                _conditions = value;
                NotifyPropertyChanged("Conditions");
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
        public bool RunAfterTransfer
        {
            get { return _runAfterTransfer; }
            set
            {
                _runAfterTransfer = value;
                NotifyPropertyChanged("RunAfterTransfer");
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
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
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
            Conditions = new AsyncObservableCollection<PluginCondition>();
            RunAfterTransfer = true;
        }

        public AutoExportPluginConfig()
        {
            ConfigData = new ValuePairEnumerator();
            ConfigDataCollection = new ObservableCollection<ValuePairEnumerator>();
            Conditions = new AsyncObservableCollection<PluginCondition>();
            IsError = false;
            IsRedy = true;
            RunAfterTransfer = true;
        }

        public bool Evaluate(ICameraDevice device)
        {
            if (Conditions.Count == 0)
                return true;
            var res = Conditions[0].Evaluate(device);
            if (Conditions.Count == 1)
                return res;
            for (int i = 1; i < Conditions.Count; i++)
            {
                var cond = Conditions[i];
                if (cond.Operator == "AND")
                {
                    res = res && cond.Evaluate(device);
                }
                else
                {
                    res = res || cond.Evaluate(device);
                }
            }
            return res;
        }
    }
}

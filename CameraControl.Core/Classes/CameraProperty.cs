using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class CameraProperty : BaseFieldClass
    {
        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged("SerialNumber");
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                NotifyPropertyChanged("DeviceName");
            }
        }

        private string _profileNmae;
        public string PhotoSessionName
        {
            get { return _profileNmae; }
            set
            {
                _profileNmae = value;
                NotifyPropertyChanged("PhotoSessionName");
            }
        }

        private string _defaultPresetName;

        public string DefaultPresetName
        {
            get { return _defaultPresetName; }
            set
            {
                _defaultPresetName = value;
                NotifyPropertyChanged("PhotoSessionName");
            }
        }

        [XmlIgnore]
        public PhotoSession PhotoSession { get; set; }

        private bool _noDownload;
        public bool NoDownload
        {
            get { return _noDownload; }
            set
            {
                _noDownload = value;
                NotifyPropertyChanged("NoDownload");
            }
        }

        private int _counterInc;
        public int CounterInc
        {
            get
            {
                if (_counterInc < 1)
                    _counterInc = 1;
                return _counterInc;
            }
            set
            {
                _counterInc = value;
                NotifyPropertyChanged("CounterInc");
            }
        }

        private bool _captureInSdRam;
        public bool CaptureInSdRam
        {
            get { return _captureInSdRam; }
            set
            {
                _captureInSdRam = value;
                NotifyPropertyChanged("CaptureInSdRam");
            }
        }

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                NotifyPropertyChanged("Counter");
            }
        }

        private bool _useExternalShutter;
        public bool UseExternalShutter
        {
            get { return _useExternalShutter; }
            set
            {
                _useExternalShutter = value;
                NotifyPropertyChanged("UseExternalShutter");
            }
        }

        private CustomConfig _selectedConfig;
        [XmlIgnore]
        public CustomConfig SelectedConfig
        {
            get
            {
                foreach (CustomConfig config in ServiceProvider.ExternalDeviceManager.ExternalShutters)
                {
                    if (config.Name == SelectedConfigName)
                        _selectedConfig = config;
                }
                return _selectedConfig;
            }
            set
            {
                _selectedConfig = value;
                SelectedConfigName = _selectedConfig == null ? "" : value.Name;
                NotifyPropertyChanged("SelectedConfig");
            }
        }

        private string _selectedConfigName;
        public string SelectedConfigName
        {
            get { return _selectedConfigName; }
            set
            {
                _selectedConfigName = value;
                NotifyPropertyChanged("SelectedConfigName");
            }
        }

        private LiveviewSettings _liveviewSettings;

        public LiveviewSettings LiveviewSettings
        {
            get { return _liveviewSettings; }
            set
            {
                _liveviewSettings = value;
                NotifyPropertyChanged("LiveviewSettings");
            }
        }

        public CameraProperty()
        {
            NoDownload = false;
            CaptureInSdRam = true;
            Counter = 0;
            LiveviewSettings = new LiveviewSettings();
        }

    }
}

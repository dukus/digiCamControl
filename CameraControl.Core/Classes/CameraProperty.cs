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

using System.Net.NetworkInformation;
using System.Windows;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

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
        [JsonIgnore]
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
        [JsonIgnore]
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
        private int _sortOrder;
        private WindowCommandItem _keyTrigger;
        private int _delay;
        private bool _liveViewInSecMonitor;
        private bool _saveLiveViewWindow;

        public LiveviewSettings LiveviewSettings
        {
            get { return _liveviewSettings; }
            set
            {
                _liveviewSettings = value;
                NotifyPropertyChanged("LiveviewSettings");
            }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value;
                NotifyPropertyChanged("SortOrder");
            }
        }

        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                NotifyPropertyChanged("Delay");
            }
        }

        public WindowCommandItem KeyTrigger
        {
            get { return _keyTrigger; }
            set
            {
                _keyTrigger = value;
                NotifyPropertyChanged("KeyTrigger");
            }
        }


        public bool SaveLiveViewWindow
        {
            get { return _saveLiveViewWindow; }
            set
            {
                _saveLiveViewWindow = value;
                NotifyPropertyChanged("SaveLiveViewWindow");
            }
        }

        public Rect WindowRect { get; set; }

        public CameraProperty()
        {
            NoDownload = false;
            CaptureInSdRam = true;
            Counter = 0;
            LiveviewSettings = new LiveviewSettings();
            KeyTrigger = new WindowCommandItem();
            SaveLiveViewWindow = true;
            WindowRect = new Rect();
        }
    }
}
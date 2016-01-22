using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ChromakeyViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        public RelayCommand BrowseCommand { get; set; }


        public string BackgroundColor
        {
            get { return _config["BackgroundColor"] ?? "Green"; }
            set
            {
                _config["BackgroundColor"] = value;
                RaisePropertyChanged(() => BackgroundColor);
            }
        }

        public int Hue 
        {
            get { return GetInt(_config["Hue"]); }
            set { _config["Hue"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Saturnation
        {
            get { return GetInt(_config["Saturnation"]); }
            set { _config["Saturnation"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Brigthness 
        {
            get { return GetInt(_config["Brigthness"]); }
            set { _config["Brigthness"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string BackgroundFile
        {
            get { return _config["BackgroundFile"]; }
            set
            {
                _config["BackgroundFile"] = value;
                RaisePropertyChanged(() => BackgroundFile);
            }
        }

        public bool UnsharpMask
        {
            get { return _config["UnsharpMask"] == "True"; }
            set { _config["UnsharpMask"] = value.ToString(); }
        }

        public ChromakeyViewModel()
        {
            
        }

        public ChromakeyViewModel(ValuePairEnumerator config)
        {
            _config = config;
            // first run set default values
            if (Hue == 0 && Saturnation == 0 && Brigthness == 0)
            {
                Hue = 40;
                Saturnation = 70;
                Brigthness = 50;
            }
            BrowseCommand = new RelayCommand(Browse);
        }

        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

        private void Browse()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                BackgroundFile = dialog.FileName;
            }
        }
    }
}

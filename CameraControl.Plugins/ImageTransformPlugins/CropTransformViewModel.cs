using System;
using System.Globalization;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class CropTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public CropTransformViewModel()
        {
            WidthProcent = 5;
            HeightProcent = 5;
        }

        public CropTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
        }

        public int Left
        {
            get { return GetInt(_config["Left"]); }
            set { _config["Left"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Top
        {
            get { return GetInt(_config["Top"]); }
            set { _config["Top"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Width
        {
            get { return GetInt(_config["Width"]); }
            set { _config["Width"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Height
        {
            get { return GetInt(_config["Height"]); }
            set { _config["Height"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public bool FromLiveView
        {
            get { return _config["FromLiveView"] == "True"; }
            set { _config["FromLiveView"] = value.ToString(); }
        }

        public bool CropMargins
        {
            get { return _config["CropMargins"] == "True"; }
            set
            {
                _config["CropMargins"] = value.ToString();
                RaisePropertyChanged(() => CropMargins);
            }
        }

        public bool LiveViewCrop
        {
            get { return _config["LiveViewCrop"] == "True"; }
            set
            {
                _config["LiveViewCrop"] = value.ToString();
                RaisePropertyChanged(() => CropMargins);
            }
        }

        public int WidthProcent
        {
            get { return GetInt(_config["WidthProcent"]); }
            set { _config["WidthProcent"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int HeightProcent
        {
            get { return GetInt(_config["HeightProcent"]); }
            set { _config["HeightProcent"] = value.ToString(CultureInfo.InvariantCulture); }
        }


        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s,CultureInfo.InvariantCulture);
        }

    }
}

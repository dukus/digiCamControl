using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class CropTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public CropTransformViewModel()
        {

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

        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s,CultureInfo.InvariantCulture);
        }
    }
}

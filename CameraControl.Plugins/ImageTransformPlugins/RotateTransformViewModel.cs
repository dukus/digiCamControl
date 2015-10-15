using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class RotateTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public RotateTransformViewModel()
        {
            
        }

        public RotateTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
        }

        public int Angle
        {
            get { return GetInt(_config["Angle"]); }
            set { _config["Angle"] = value.ToString(CultureInfo.InvariantCulture); }
        }


        public bool AutoRotate
        {
            get { return _config["AutoRotate"] == "True"; }
            set { _config["AutoRotate"] = value.ToString(); }
        }

        public bool FlipHorizontal
        {
            get { return _config["FlipHorizontal"] == "True"; }
            set { _config["FlipHorizontal"] = value.ToString(); }
        }

        public bool FlipVertical
        {
            get { return _config["FlipVertical"] == "True"; }
            set { _config["FlipVertical"] = value.ToString(); }
        }

        public bool ManualRotate
        {
            get { return _config["ManualRotate"] == "True"; }
            set { _config["ManualRotate"] = value.ToString(); }
        }
        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class EnhanceViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public int Brightness
        {
            get { return GetInt(_config["Brightness"]); }
            set { _config["Brightness"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Contrast
        {
            get { return GetInt(_config["Contrast"]); }
            set { _config["Contrast"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public EnhanceViewModel()
        {
            
        }

        public EnhanceViewModel(ValuePairEnumerator config)
        {
            _config = config;
        }

        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }
    }
}

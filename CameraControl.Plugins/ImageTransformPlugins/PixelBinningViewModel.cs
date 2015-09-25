using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class PixelBinningViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public int SelectedMode
        {
            get { return GetInt(_config["SelectedMode"]); }
            set { _config["SelectedMode"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public PixelBinningViewModel()
        {
            
        }

        public PixelBinningViewModel(ValuePairEnumerator config)
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

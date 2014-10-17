using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ResizeTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public ResizeTransformViewModel()
        {
            
        }

        public ResizeTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
        }

        public int Width
        {
            get { return GetInt(_config["Width"]); }
            set { _config["Width"] = value.ToString(); }
        }

        public int Height
        {
            get { return GetInt(_config["Height"]); }
            set { _config["Height"] = value.ToString(); }
        }

        public bool KeepAspect
        {
            get { return _config["KeepAspect"]=="True"; }
            set { _config["KeepAspect"] = value.ToString(); }
        }


        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s);
        }
    }
}

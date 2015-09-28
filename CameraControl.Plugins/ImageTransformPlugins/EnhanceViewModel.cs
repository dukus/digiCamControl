using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class EnhanceViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();

        public RelayCommand ResetCommand { get; set; }

        public int Brightness
        {
            get { return GetInt(_config["Brightness"]); }
            set
            {
                _config["Brightness"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => Brightness);
            }
        }

        public int Contrast
        {
            get { return GetInt(_config["Contrast"]); }
            set
            {
                _config["Contrast"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => Contrast);
            }
        }

        public int Sharpen
        {
            get { return GetInt(_config["Sharpen"]); }
            set
            {
                _config["Sharpen"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => Sharpen);
            }
        }

        public double SContrast
        {
            get { return Getdouble(_config["SContrast"]); }
            set
            {
                _config["SContrast"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => SContrast);
            }
        }

        public bool Normalize
        {
            get { return _config["Normalize"] == "True"; }
            set
            {
                _config["Normalize"] = value.ToString();
                RaisePropertyChanged(() => Normalize);
            }
        }

        public bool AutoGamma
        {
            get { return _config["AutoGamma"] == "True"; }
            set
            {
                _config["AutoGamma"] = value.ToString();
                RaisePropertyChanged(() => AutoGamma);
            }
        }

        public bool Edge
        {
            get { return _config["Edge"] == "True"; }
            set
            {
                _config["Edge"] = value.ToString();
                RaisePropertyChanged(() => Edge);
            }
        }

        public EnhanceViewModel()
        {
            
        }

        public EnhanceViewModel(ValuePairEnumerator config)
        {
            _config = config;
            ResetCommand = new RelayCommand(Reset);
        }

        private void Reset()
        {
            Brightness = 0;
            Contrast = 0;
            SContrast = 0;
            Sharpen = 0;
            Normalize = false;
            Edge = false;

        }

        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

        private double Getdouble(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToDouble(s, CultureInfo.InvariantCulture);
        }
    }
}

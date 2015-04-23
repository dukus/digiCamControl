using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class OverlayTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        private ObservableCollection<string> _fonts;

        public RelayCommand BrowseCommand { get; set; }

        public OverlayTransformViewModel()
        {
            
        }

        public OverlayTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
            BrowseCommand = new RelayCommand(Browse);
        }

        public bool A11
        {
            get { return _config["A11"] == "True"; }
            set
            {
                _config["A11"] = value.ToString();
                RaisePropertyChanged(()=>A11);
                if (!A11) return;
                A12 = false;
                A13 = false;
                A21 = false;
                A22 = false;
                A23 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A12
        {
            get { return _config["A12"] == "True"; }
            set
            {
                _config["A12"] = value.ToString();
                RaisePropertyChanged(() => A12);
                if (!A12) return;
                A11 = false;
                A13 = false;
                A21 = false;
                A22 = false;
                A23 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A13
        {
            get { return _config["A13"] == "True"; }
            set
            {
                _config["A13"] = value.ToString();
                RaisePropertyChanged(() => A13);
                if (!A13) return;
                A11 = false;
                A12 = false;
                A21 = false;
                A22 = false;
                A23 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A21
        {
            get { return _config["A21"] == "True"; }
            set
            {
                _config["A21"] = value.ToString();
                RaisePropertyChanged(() => A21);
                if (!A21) return;
                A11 = false;
                A12 = false;
                A13 = false;
                A22 = false;
                A23 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A22
        {
            get { return _config["A22"] == "True"; }
            set
            {
                _config["A22"] = value.ToString();
                RaisePropertyChanged(() => A22);
                if (!A22) return;
                A11 = false;
                A12 = false;
                A13 = false;
                A21 = false;
                A23 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A23
        {
            get { return _config["A23"] == "True"; }
            set
            {
                _config["A23"] = value.ToString();
                RaisePropertyChanged(() => A23);
                if (!A23) return;
                A11 = false;
                A12 = false;
                A13 = false;
                A21 = false;
                A22 = false;
                A31 = false;
                A32 = false;
                A33 = false;
            }
        }

        public bool A31
        {
            get { return _config["A31"] == "True"; }
            set
            {
                _config["A31"] = value.ToString();
                RaisePropertyChanged(() => A31);
                if (A31)
                {
                    A11 = false;
                    A12 = false;
                    A13 = false;
                    A21 = false;
                    A22 = false;
                    A23 = false;
                    A32 = false;
                    A33 = false;
                }
            }
        }

        public bool A32
        {
            get { return _config["A32"] == "True"; }
            set
            {
                _config["A32"] = value.ToString();
                RaisePropertyChanged(() => A32);
                if (A32)
                {
                    A11 = false;
                    A12 = false;
                    A13 = false;
                    A21 = false;
                    A22 = false;
                    A23 = false;
                    A31 = false;
                    A33 = false;
                }
            }
        }

        public bool A33
        {
            get { return _config["A33"] == "True"; }
            set
            {
                _config["A33"] = value.ToString();
                RaisePropertyChanged(() => A33);
                if (A33)
                {
                    A11 = false;
                    A12 = false;
                    A13 = false;
                    A21 = false;
                    A22 = false;
                    A23 = false;
                    A31 = false;
                    A32 = false;
                }
            }
        }

        public string Text
        {
            get { return _config["Text"]; }
            set { _config["Text"] = value; }
        }

        public int FontSize
        {
            get { return GetInt(_config["FontSize"]); }
            set { _config["FontSize"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int Transparency
        {
            get
            {
                var val = GetInt(_config["Transparency"]);
                if (val == 0)
                    val = 100;
                return val;
            }
            set { _config["Transparency"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string Font
        {
            get { return _config["Font"]??"Arial"; }
            set
            {
                _config["Font"] = value;
                RaisePropertyChanged(()=>Font);
            }
        }

        public string FontColor
        {
            get { return _config["FontColor"] ?? "Black"; }
            set
            {
                _config["FontColor"] = value;
                RaisePropertyChanged(() => FontColor);
            }
        }

        public int Margins
        {
            get { return GetInt(_config["Margins"]); }
            set { _config["Margins"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string OverlayFile
        {
            get { return _config["OverlayFile"]; }
            set
            {
                _config["OverlayFile"] = value;
                RaisePropertyChanged(() => OverlayFile);
            }
        }

        public bool StrechOverlay
        {
            get { return _config["StrechOverlay"] == "True"; }
            set { _config["StrechOverlay"] = value.ToString(); }
        }

        public ObservableCollection<string> Fonts
        {
            get
            {
                if (_fonts == null)
                {
                    _fonts = new ObservableCollection<string>();
                    foreach (var aFontFamily in System.Windows.Media.Fonts.SystemFontFamilies)
                    {
                        var ltypFace = new Typeface(aFontFamily, FontStyles.Normal, FontWeights.Normal,
                            FontStretches.Normal);

                        try
                        {
                            GlyphTypeface lglyphTypeFace;
                            if (ltypFace.TryGetGlyphTypeface(out lglyphTypeFace) && !string.IsNullOrEmpty(aFontFamily.Source))
                            {
                                _fonts.Add(aFontFamily.Source);
                            }
                        }
                        catch
                        {
                        }
                    }   
                }
                return _fonts;
            }
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
                OverlayFile = dialog.FileName;
            }
        }

    }
}

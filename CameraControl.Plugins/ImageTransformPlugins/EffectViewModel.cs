using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class EffectViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        private EffectType _selectedEffect;

        public ObservableCollection<EffectType> Effects { get; set; }

        public EffectType SelectedEffect
        {
            get { return _selectedEffect; }
            set
            {
                if (_selectedEffect.Name != value.Name)
                {
                    Param1 = value.Default1;
                    Param2 = value.Default2;
                }
                _selectedEffect = value;
                RaisePropertyChanged(() => SelectedEffect);
            }
        }

        public int SelectedMode
        {
            get { return GetInt(_config["SelectedMode"]); }
            set
            {
                _config["SelectedMode"] = value.ToString(CultureInfo.InvariantCulture);
                SelectedEffect = Effects[value];
                RaisePropertyChanged(() => SelectedMode);
            }
        }

        public int Param1
        {
            get { return GetInt(_config["Param1"]); }
            set
            {
                _config["Param1"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => Param1);
            }
        }

        public int Param2
        {
            get { return GetInt(_config["Param2"]); }
            set
            {
                _config["Param2"] = value.ToString(CultureInfo.InvariantCulture);
                RaisePropertyChanged(() => Param2);
            }
        }


        public EffectViewModel()
        {
            
        }

        public EffectViewModel(ValuePairEnumerator config)
        {
            _config = config;
            Effects = new ObservableCollection<EffectType>();
            Effects.Add(new EffectType()
            {
                Name = "Sepia",
                Name1 = "Threshold",
                Min1 = 0,
                Max1 = 99,
                Default1 = 80,
                Param1Visible = true
            });
            Effects.Add(new EffectType()
            {
                Name = "Oil Paint",
                Name1 = "Radius",
                Min1 = 3,
                Max1 = 35,
                Default1 = 15,
                Param1Visible = true
            });
            Effects.Add(new EffectType()
            {
                Name = "Sketch",
                Name1 = "Sigma",
                Min1 = 1,
                Max1 = 99,
                Default1 = 20,
                Param1Visible = false,
                Name2 = "Angle",
                Min2 = 0,
                Max2 = 359,
                Default2 = 135,
                Param2Visible = false

            });
            Effects.Add(new EffectType()
            {
                Name = "Charcoal",
            });
            Effects.Add(new EffectType()
            {
                Name = "Solarize",
                Name1 = "Threshold of the intensity",
                Min1 = 0,
                Max1 = 99,
                Default1 = 10,
                Param1Visible = false
            });
            Effects.Add(new EffectType()
            {
                Name = "Swirl",
                Name1 = "Degrees",
                Min1 = 0,
                Max1 = 359,
                Default1 = 45,
                Param1Visible = true
            });
            Effects.Add(new EffectType()
            {
                Name = "Wave",
                Name1 = "Amplitude",
                Min1 = 1,
                Max1 = 99,
                Default1 = 30,
                Param1Visible = true,
                Name2 = "Wave length",
                Min2 = 0,
                Max2 = 365,
                Default2 = 135,
                Param2Visible = true

            });
            Effects.Add(new EffectType()
            {
                Name = "BlueShift",
            });
            Effects.Add(new EffectType()
            {
                Name = "Radial Blur",
                Name1 = "Degrees",
                Min1 = 0,
                Max1 = 100,
                Default1 = 10,
                Param1Visible = true
            });
            Effects.Add(new EffectType()
            {
                Name = "Raise",
                Name1 = "Degrees",
                Min1 = -200,
                Max1 = +200,
                Default1 = 20,
                Param1Visible = true
            });
            _selectedEffect = Effects[SelectedMode];
        }

        private int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }
    }
}

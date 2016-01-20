using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ChromakeyViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        
        public ChromakeyViewModel()
        {
            
        }

        public ChromakeyViewModel(ValuePairEnumerator config)
        {
            _config = config;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class TransformPluginItem: ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        private string _name;

        public ValuePairEnumerator Config
        {
            get { return _config; }
            set { _config = value; }
        }

        public TransformPluginItem(ValuePairEnumerator config)
        {
            _config = config;
        }

        public string Name
        {
            get { return _config["TransformPlugin"]; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Xml
{
    public class XmlDataManager : BaseFieldClass
    {
        public const string PM_MODE = "Mode";
        public const string PM_ISO = "Iso";
        public const string PM_Shutter_Speed = "Shutter speed";
        public const string PM_Aperture = "Aperture";
        public const string PM_WhiteBalance = "White balance";
        public const string PM_ExposureComp = "Exposure Comp";
        public const string PM_Compression = "Compression";
        public const string PM_MeteringMode = "Metering mode";
        public const string PM_FocusMode = "Focus mode";
        public const string PM_BATERY = "Battery";
        public const string PM_Advanced = "Advanced properties";

        private AsyncObservableCollection<string> _propertyMapping;
        public AsyncObservableCollection<string> PropertyMapping
        {
            get { return _propertyMapping; }
            set
            {
                _propertyMapping = value;
                NotifyPropertyChanged("PropertyMapping");
            }
        }

        public XmlDataManager()
        {
            PropertyMapping = new AsyncObservableCollection<string>
                                  {
                                      PM_MODE,
                                      PM_ISO,
                                      PM_Shutter_Speed,
                                      PM_Aperture,
                                      PM_WhiteBalance,
                                      PM_ExposureComp,
                                      PM_Compression,
                                      PM_MeteringMode,
                                      PM_FocusMode,
                                      PM_BATERY,
                                      PM_Advanced
                                  };
        }
    }
}

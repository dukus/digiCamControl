#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

#endregion

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
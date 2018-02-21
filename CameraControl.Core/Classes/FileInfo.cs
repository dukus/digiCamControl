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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class FileInfo : BaseFieldClass
    {
        private int _width;
        private int _height;
        private int[] _histogramRed;
        private int[] _histogramBlue;
        private int[] _histogramGreen;
        private int[] _histogramLuminance;
        public string InfoLabel { get; set; }
        public int Orientation { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public PointCollection LuminanceHistogramPoints
        {
            get { return BitmapLoader.ConvertToPointCollection(HistogramLuminance);  }
        }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsFocused
        {
            get
            {
                return FocusPoints != null && FocusPoints.Count > 0;
                
            } 
        }


        public bool IsOverExposed
        {
            get
            {
                return HistogramLuminance != null && HistogramLuminance.Length == 256 && HistogramLuminance[254] > 5 &&
                       HistogramLuminance[255] > 5;
            }
        }

        public bool IsUnderExposed
        {
            get
            {
                return HistogramLuminance != null && HistogramLuminance.Length == 256 && HistogramLuminance[0] > 5 &&
                       HistogramLuminance[1] > 5;
            }
        }

        public int Width
        {
            get
            {
                if (_width == 0)
                    _width = 4000;
                return _width;
            }
            set
            {
                _width = value;
                NotifyPropertyChanged("Width");
            }
        }

        public int Height
        {
            get
            {
                if (_height == 0)
                    _height = 3000;
                return _height;
            }
            set
            {
                _height = value;
                NotifyPropertyChanged("Height");
            }
        }


        public ValuePairEnumerator ExifTags { get; set; }
        public List<Rect> FocusPoints { get; set; }

        public int[] HistogramRed
        {
            get { return _histogramRed; }
            set
            {
                _histogramRed = value;
                NotifyPropertyChanged("HistogramRed");
            }
        }

        public int[] HistogramBlue
        {
            get { return _histogramBlue; }
            set
            {
                _histogramBlue = value;
                NotifyPropertyChanged("HistogramBlue");
            }
        }

        public int[] HistogramGreen
        {
            get { return _histogramGreen; }
            set
            {
                _histogramGreen = value;
                NotifyPropertyChanged("HistogramGreen");
            }
        }

        public int[] HistogramLuminance
        {
            get { return _histogramLuminance; }
            set
            {
                _histogramLuminance = value;
                NotifyPropertyChanged("HistogramLuminance");
                NotifyPropertyChanged("LuminanceHistogramPoints");
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsLoading { get; set; }

        public FileInfo()
        {
            ExifTags = new ValuePairEnumerator();
            FocusPoints = new List<Rect>();
        }

        public void ValidateValues()
        {
            foreach (var exifTag in ExifTags.Items )
            {
                exifTag.Value = exifTag.Value.Replace('\0', ' ');
                exifTag.Value = CleanInvalidXmlChars(exifTag.Value);
            }
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x07\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]";
            return Regex.Replace(text, re, "");
        }

        public void SetSize(int w, int h)
        {
            _width = w;
            _height = h;
        }

    }
}
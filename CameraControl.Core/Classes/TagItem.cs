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

using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public class TagItem : BaseFieldClass
    {
        private string _displayValue;

        public string DisplayValue
        {
            get { return _displayValue; }
            set
            {
                _displayValue = value;
                NotifyPropertyChanged("DisplayValue");
            }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        private bool _tag1Checked;

        public bool Tag1Checked
        {
            get { return _tag1Checked; }
            set
            {
                _tag1Checked = value;
                NotifyPropertyChanged("Tag1Checked");
            }
        }

        private bool _tag2Checked;

        public bool Tag2Checked
        {
            get { return _tag2Checked; }
            set
            {
                _tag2Checked = value;
                NotifyPropertyChanged("Tag2Checked");
            }
        }

        private bool _tag3Checked;

        public bool Tag3Checked
        {
            get { return _tag3Checked; }
            set
            {
                _tag3Checked = value;
                NotifyPropertyChanged("Tag3Checked");
            }
        }

        private bool _tag4Checked;
        private string _color;

        public bool Tag4Checked
        {
            get { return _tag4Checked; }
            set
            {
                _tag4Checked = value;
                NotifyPropertyChanged("Tag4Checked");
            }
        }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyPropertyChanged("Color");
            }
        }

        public TagItem()
        {
            Value = "";
            DisplayValue = "";
            Tag1Checked = false;
            Tag2Checked = false;
            Tag3Checked = false;
            Tag4Checked = false;
            Color = "White";
        }

        public override string ToString()
        {
            return DisplayValue + " - " + Value;
        }
    }
}
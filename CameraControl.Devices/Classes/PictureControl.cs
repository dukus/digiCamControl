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

#endregion

namespace CameraControl.Devices.Classes
{
    public class PictureControl
    {
        public byte ItemNumber { get; set; }
        public bool IsLoaded { get; set; }
        public bool Monocrome { get; set; }
        public byte CustomFlag { get; set; }
        public string RegistrationName { get; set; }
        public byte QuickAdjustFlag { get; set; }
        public sbyte QuickAdjust { get; set; }
        public sbyte Saturation { get; set; }
        public sbyte Hue { get; set; }
        public sbyte Sharpening { get; set; }
        public sbyte Contrast { get; set; }
        public sbyte Brightness { get; set; }
        public byte CustomCurveFlag { get; set; }
        public byte[] CustomCurveData { get; set; }
        //color
        public byte FilterEffects { get; set; }
        public byte Toning { get; set; }
        public byte ToningDensity { get; set; }
    }
}
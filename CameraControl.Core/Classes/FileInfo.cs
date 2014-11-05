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

#endregion

namespace CameraControl.Core.Classes
{
    public class FileInfo
    {
        public string InfoLabel { get; set; }
        public int Orientation { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ValuePairEnumerator ExifTags { get; set; }
        public List<Rect> FocusPoints { get; set; }
        public int[] HistogramRed { get; set; }
        public int[] HistogramBlue { get; set; }
        public int[] HistogramGreen { get; set; }
        public int[] HistogramLuminance { get; set; }

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
            string re = @"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]";
            return Regex.Replace(text, re, "");
        }
    }
}
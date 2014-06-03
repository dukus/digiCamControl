using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
    }
}

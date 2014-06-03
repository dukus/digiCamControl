using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

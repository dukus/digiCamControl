using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Nikon
{
    public class NikonD500: NikonD600Base
    {
        protected override void InitCompressionSetting()
        {
            _csTable = new Dictionary<int, string>()
            {
                {0, "JPEG (BASIC)"},
                {1, "JPEG (BASIC*)"},
                {2, "JPEG (NORMAL)"},
                {3, "JPEG (NORMAL*)"},
                {4, "JPEG (FINE)"},
                {5, "JPEG (FINE*)"},
                {6, "TIFF (RGB)"},
                {7, "RAW"},
                {8, "RAW + JPEG (BASIC)"},
                {9, "RAW + JPEG (BASIC*)"},
                {10, "RAW + JPEG (NORMAL)"},
                {11, "RAW + JPEG (NORMAL*)"},
                {12, "RAW + JPEG (FINE)"},
                {13, "RAW + JPEG (FINE*)"}
            };
            base.InitCompressionSetting();
        }
    }
}

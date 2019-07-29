using CameraControl.Devices.Classes;
using PortableDeviceLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Nikon
{
    public class NikonD750 : NikonD600
    {
        public NikonD750()
        {
            _isoTable = new Dictionary<uint, string>()
            {
                {0x0032, "Lo 1.0"},
                {0x0040, "Lo 0.7"},
                {0x0048, "Lo 0.5"},
                {0x0050, "Lo 0.3"},
                {0x0064, "100"},
                {0x007d, "125"},
                {0x008c, "140"},
                {0x00A0, "160"},
                {0x00C8, "200"},
                {0x00FA, "250"},
                {0x0118, "280"},
                {0x0140, "320"},
                {0x0190, "400"},
                {0x01F4, "500"},
                {0x0230, "560"},
                {0x0280, "640"},
                {0x0320, "800"},
                {0x03E8, "1000"},
                {0x04E2, "1250"},
                {0x0640, "1600"},
                {0x07D0, "2000"},
                {0x0898, "2200"},
                {0x09C4, "2500"},
                {0x0C80, "3200"},
                {0x0FA0, "4000"},
                {0x1194, "4500"},
                {0x1388, "5000"},
                {0x1900, "6400"},
                {0x1F40, "8000"},
                {0x2710, "10000"},
                {0x3200, "12800"},
                {0x3E80, "Hi 0.3"},
                {0x4650, "Hi 0.5"},
                {0x4E20, "Hi 0.7"},
                {0x6400, "Hi 1"},
                {0xC800, "Hi 2"},
            };

            _autoIsoTable = new Dictionary<byte, string>()
            {
                {0, "200"},
                {1, "250"},
                {2, "280"},
                {3, "320"},
                {4, "400"},
                {5, "500"},
                {6, "560"},
                {7, "640"},
                {8, "800"},
                {9, "1000"},
                {10, "1100"},
                {11, "1250"},
                {12, "1600"},
                {13, "2000"},
                {14, "2200"},
                {15, "2500"},
                {16, "3200"},
                {17, "4000"},
                {18, "4500"},
                {19, "5000"},
                {20, "6400"},
                {21, "8000"},
                {22, "9000"},
                {23, "10000"},
                {24, "12800"},
                {25, "Hi 0.3"},
                {26, "Hi 0.5"},
                {27, "Hi 0.7"},
                {28, "Hi 1"},
                {29, "Hi 2"},
            };
        }

        protected override PropertyValue<long> InitStillCaptureMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
            {
                Name = "Still Capture Mode",
                IsEnabled = true,
                Code = 0x5013,
                SubType = typeof(UInt16)
            };
            res.AddValues("Single frame (S)", 0x0001);
            res.AddValues("Continuous low speed (CL)", 0x8010);
            res.AddValues("Continuous high speed (CH)", 0x0002);
            res.AddValues("Quiet shutter-release (Q)", 0x8016);
            res.AddValues("Quiet continuous shutter-release (QC)", 0x8018);
            res.AddValues("Self-timer", 0x8011);
            res.AddValues("Mirror up", 0x8012);
            res.ReloadValues();
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code);
            return res;
        }
    }
}

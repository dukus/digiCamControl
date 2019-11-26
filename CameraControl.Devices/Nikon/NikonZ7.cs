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
    public class NikonZ7 : NikonD500
    {
        public NikonZ7()
        {
            _isoTable = new Dictionary<uint, string>()
            {
                {0x0020, "Lo 1.0"},
                {0x0028, "Lo 0.7"},
                {0x002d, "Lo 0.5"},
                {0x0032, "Lo 0.3"},
                {0x0040, "64"},
                {0x0048, "72"},
                {0x0050, "80"},
                {0x0064, "100"},
                {0x007D, "125"},
                {0x00A0, "160"},
                {0x00C8, "200"},
                {0x00FA, "250"},
                {0x0140, "320"},
                {0x0190, "400"},
                {0x01F4, "500"},
                {0x0280, "640"},
                {0x0320, "800"},
                {0x03E8, "1000"},
                {0x04E2, "1250"},
                {0x0640, "1600"},
                {0x07D0, "2000"},
                {0x09C4, "2500"},
                {0x0C80, "3200"},
                {0x0FA0, "4000"},
                {0x1388, "5000"},
                {0x1900, "6400"},
                {0x1F40, "8000"},
                {0x2710, "10000"},
                {0x3200, "12800"},
                {0x3e80, "16000"},
                {0x4e20, "20000"},
                {0x6400, "25600"},
                {0x7d00, "Hi 0.3"},
                {0x9c40, "Hi 0.7"},
                {0xC800, "Hi 1.0"},
                {0x19000, "Hi 2.0"},
            };

            _autoIsoTable = new Dictionary<byte, string>()
            {
                {0, "100"},
                {1, "125"},
                {2, "140"},
                {3, "160"},
                {4, "200"},
                {5, "250"},
                {6, "280"},
                {7, "320"},
                {8, "400"},
                {9, "500"},
                {10, "560"},
                {11, "640"},
                {12, "800"},
                {13, "1000"},
                {14, "1100"},
                {15, "1250"},
                {16, "1600"},
                {17, "2000"},
                {18, "2200"},
                {19, "2500"},
                {20, "3200"},
                {21, "4000"},
                {22, "4500"},
                {23, "5000"},
                {24, "6400"},
                {25, "8000"},
                {26, "9000"},
                {27, "10000"},
                {28, "12800"},
                {29, "16000"},
                {30, "18000"},
                {31, "20000"},
                {32, "25600"},
                {33, "Hi 0.3"},
                {34, "Hi 0.5"},
                {35, "Hi 0.7"},
                {36, "Hi 1"},
                {37, "Hi 2"},
            };
        }
        protected virtual void InitFNumber()
        {
            NormalFNumber = new PropertyValue<long> { IsEnabled = true, Name = "FNumber" };
            NormalFNumber.ValueChanged += NormalFNumber_ValueChanged;
            NormalFNumber.SubType = typeof(UInt16);
            MovieFNumber = new PropertyValue<long> { IsEnabled = true, Name = "FNumber" };
            MovieFNumber.ValueChanged += MovieFNumber_ValueChanged;
            MovieFNumber.SubType = typeof(UInt16);
            ReInitFNumber(false);
        }

        private void MovieFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16)val),
                    CONST_PROP_MovieFnumber);
        }

        private void NormalFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((ushort)val),
                    CONST_PROP_Fnumber);
        }
    }
}

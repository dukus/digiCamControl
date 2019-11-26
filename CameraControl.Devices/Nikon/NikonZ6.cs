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
    public class NikonZ6 : NikonD500
    {
        public NikonZ6()
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
                {0x3e80, "16000"},
                {0x4650, "18000"},
                {0x4e20, "20000"},
                {0x6400, "25600"},
                {0x7d00, "32000"},
                {0x8ca0, "36000"},
                {0x9c40, "40000"},
                {0xC800, "51200"},
                {0xFA00, "Hi 0.3"},
                {0x11940, "Hi 0.5"},
                {0x13d30, "Hi 0.7"},
                {0x19000, "Hi 1"},
                {0x32000, "Hi 2"},
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
                {25, "16000"},
                {26, "18000"},
                {27, "20000"},
                {28, "25600"},
                {29, "32000"},
                {30, "36000"},
                {31, "40000"},
                {32, "51200"},
                {33, "Hi 0.3"},
                {34, "Hi 0.5"},
                {35, "Hi 0.7"},
                {36, "Hi 1"},
                {37, "Hi 2"},
            };

        }

        protected virtual void InitFNumber()
        {
            NormalFNumber = new PropertyValue<long> {IsEnabled = true, Name = "FNumber"};
            NormalFNumber.ValueChanged += NormalFNumber_ValueChanged;
            NormalFNumber.SubType = typeof(UInt16);
            MovieFNumber = new PropertyValue<long> {IsEnabled = true, Name = "FNumber"};
            MovieFNumber.ValueChanged += MovieFNumber_ValueChanged;
            MovieFNumber.SubType = typeof(UInt16);
            ReInitFNumber(false);
        }

        private void MovieFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) val),
                    CONST_PROP_MovieFnumber);
        }

        private void NormalFNumber_ValueChanged(object sender, string key, long val)
        {
            if (Mode != null && (Mode.Value == "A" || Mode.Value == "M"))
                SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((UInt16) val),
                    CONST_PROP_Fnumber);
        }

    }

}

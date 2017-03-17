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
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD60 : NikonBase
    {
        protected new Dictionary<int, string> _csTable = new Dictionary<int, string>()
                                                             {
                                                                 {0, "JPEG (BASIC)"},
                                                                 {1, "JPEG (NORMAL)"},
                                                                 {2, "JPEG (FINE)"},
                                                                 {3, "RAW"},
                                                                 {4, "RAW + JPEG (BASIC)"},
                                                             };


        protected override void InitCompressionSetting()
        {
            try
            {
                byte datasize = 1;
                CompressionSetting = new PropertyValue<long>();
                CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                 CONST_PROP_CompressionSetting);
                byte defval = result.Data[datasize + 5];
                for (int i = 0; i < result.Data.Length - ((2 * datasize) + 6 + 2); i += datasize)
                {
                    byte val = result.Data[((2 * datasize) + 6 + 2) + i];
                    CompressionSetting.AddValues(_csTable.ContainsKey(val) ? _csTable[val] : val.ToString(), val);
                }
                CompressionSetting.ReloadValues();
                CompressionSetting.SetValue(defval);
            }
            catch (Exception)
            {
            }
        }

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            bool res = base.Init(deviceDescriptor);
            Capabilities.Clear();
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            Capabilities.Add(CapabilityEnum.CaptureInRam);
            HaveLiveView = false;
            CaptureInSdRam = false;
            PropertyChanged -= NikonBase_PropertyChanged;
            return res;
        }

        public override void ReadDeviceProperties(uint prop)
        {
            base.ReadDeviceProperties(prop);
            HaveLiveView = false;
        }
    }
}
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
    public class NikonD80 : NikonBase
    {
        protected new Dictionary<int, string> _csTable = new Dictionary<int, string>()
                                                             {
                                                                 {0, "JPEG (BASIC)"},
                                                                 {1, "JPEG (NORMAL)"},
                                                                 {2, "JPEG (FINE)"},
                                                                 {3, "RAW"},
                                                                 {4, "RAW + JPEG (BASIC)"},
                                                                 {5, "RAW + JPEG (NORMAL)"},
                                                                 {6, "RAW + JPEG (FINE)"}
                                                             };

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            bool res = base.Init(deviceDescriptor);
            Capabilities.Clear();
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            HaveLiveView = false;
            CaptureInSdRam = false;
            return res;
        }

        protected override void InitCompressionSetting()
        {
            try
            {
                byte datasize = 1;
                CompressionSetting = new PropertyValue<long>();
                CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
                var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc,
                                                                 CONST_PROP_CompressionSetting);
                int type = BitConverter.ToInt16(result.Data, 2);
                byte formFlag = result.Data[(2 * datasize) + 5];
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

        public override void ReadDeviceProperties(uint prop)
        {
            base.ReadDeviceProperties(prop);
            HaveLiveView = false;
        }

        /// <summary>
        /// Take picture with no autofocus
        /// If live view runnig the live view is stoped after done restarted
        /// </summary>
        public override void CapturePhotoNoAf()
        {
            lock (Locker)
            {
                byte oldval = 0;
                var val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect);
                if (val.Data != null && val.Data.Length > 0)
                    oldval = val.Data[0];

                ErrorCodes.GetException(StillImageDevice.ExecuteWriteData(CONST_CMD_SetDevicePropValue, new[] {(byte) 4},
                                                                          CONST_PROP_AFModeSelect));
                ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCapture));
                if (val.Data != null && val.Data.Length > 0)
                    ErrorCodes.GetException(StillImageDevice.ExecuteWriteData(CONST_CMD_SetDevicePropValue,
                                                                              new[] {oldval},
                                                                              CONST_PROP_AFModeSelect));
            }
        }
    }
}
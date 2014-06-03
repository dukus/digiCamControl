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

using System.Runtime.InteropServices;

#endregion

namespace CameraControl.Devices.Classes
{
    public class Conts
    {
        [MarshalAs(UnmanagedType.LPStr)] public const string wiaEventDeviceConnected =
            "{A28BBADE-64B6-11D2-A231-00C04FA31809}";

        [MarshalAs(UnmanagedType.LPStr)] public const string wiaEventDeviceDisconnected =
            "{143E4E83-6497-11D2-A231-00C04FA31809}";

        [MarshalAs(UnmanagedType.LPStr)] public const string wiaEventItemCreated =
            "{4C8F4EF5-E14F-11D2-B326-00C04F68CE61}";

        [MarshalAs(UnmanagedType.LPStr)] public const string wiaEventItemDeleted =
            "{1D22A559-E14F-11D2-B326-00C04F68CE61}";

        [MarshalAs(UnmanagedType.LPStr)] public const string wiaCommandTakePicture =
            "{AF933CAC-ACAD-11D2-A093-00C04F72DC3C}";

        public const string CONST_PROP_F_Number = "F Number";
        public const string CONST_PROP_ISO_Number = "Exposure Index";
        public const string CONST_PROP_Exposure_Time = "Exposure Time";
        public const string CONST_PROP_WhiteBalance = "White Balance";
        public const string CONST_PROP_ExposureMode = "Exposure Mode";
        public const string CONST_PROP_ExposureCompensation = "Exposure Compensation";
        public const string CONST_PROP_BatteryStatus = "Battery Status";
        public const string CONST_PROP_CompressionSetting = "Compression Setting";
        public const string CONST_PROP_ExposureMeteringMode = "Exposure Metering Mode";
        public const string CONST_PROP_FocusMode = "Focus Mode";
    }
}
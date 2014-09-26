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
using Canon.Eos.Framework.Internal.SDK;

#endregion

namespace CameraControl.Devices.Classes
{
    public class ErrorCodes
    {
        public const uint WIA_ERROR_GENERAL_ERROR = 0x80210001;
        public const uint WIA_ERROR_OFFLINE = 0x80210005;
        public const uint WIA_ERROR_BUSY = 0x80210006;
        public const uint WIA_ERROR_DEVICE_COMMUNICATION = 0x8021000A;
        public const uint WIA_ERROR_INVALID_COMMAND = 0x8021000B;
        public const uint WIA_ERROR_INCORRECT_HARDWARE_SETTING = 0x8021000C;
        public const uint WIA_ERROR_DEVICE_LOCKED = 0x8021000D;
        public const uint WIA_ERROR_EXCEPTION_IN_DRIVER = 0x8021000E;
        public const uint WIA_ERROR_INVALID_DRIVER_RESPONSE = 0x8021000F;
        public const uint WIA_S_NO_DEVICE_AVAILABLE = 0x80210015;
        public const uint WIA_ERROR_UNABLE_TO_FOCUS = 0x80004005;

        public const uint MTP_OK = 0x2001;
        public const uint MTP_General_Error = 0x2002;
        public const uint MTP_Device_Busy = 0x2019;
        public const uint MTP_Set_Property_Not_Support = 0xA005;
        public const uint MTP_Store_Not_Available = 0x2013;
        public const uint MTP_Out_of_Focus = 0xA002;
        public const uint MTP_Shutter_Speed_Bulb = 0xA008;
        public const uint MTP_Store_Full = 0x200C;
        public const uint MTP_Store_Read_Only = 0x200E;
        public const uint MTP_Parameter_Not_Supported = 0x2006;
        public const uint MTP_Not_LiveView = 0xA00B;
        public const uint MTP_Invalid_Parameter = 0x201D;
        public const uint MTP_Invalid_Status = 0xA004;
        public const uint MTP_Operation_Not_Supported = 0x2005;
        public const uint MTP_Access_Denied = 0x200F;


        public const uint E_WPD_DEVICE_IS_HUNG = 0x802A0006;
        public const uint ERROR_BUSY = 0x800700AA;

        public static void GetException(int code)
        {
            GetException((uint) code);
        }

        public static void GetCanonException(uint code)
        {
            if (code != 0 && code != Edsdk.EDS_ERR_OK)
            {
                switch (code)
                {
                    default:
                        throw new DeviceException("Canon error code: " + code.ToString("X"), code);  
                }
            }
        }

        public static void GetException(uint code)
        {
            if (code != 0 && code != MTP_OK)
            {
                switch (code)
                {
                    case MTP_Device_Busy:
                        throw new DeviceException("Device MTP error: Device is busy", code);
                    case MTP_General_Error:
                        throw new DeviceException("General error. No focus ?", code);
                    case MTP_Store_Not_Available:
                        throw new DeviceException("The card cannot be accessed", code);
                    case MTP_Shutter_Speed_Bulb:
                        throw new DeviceException("Bulb mode isn't supported", code);
                    case MTP_Out_of_Focus:
                        throw new DeviceException("Unable to focus.", code);
                    case MTP_Store_Full:
                        throw new DeviceException("Storage is full.", code);
                    case MTP_Store_Read_Only:
                        throw new DeviceException("Storage is read only.", code);
                    case MTP_Not_LiveView:
                        throw new DeviceException("Not in live view.", code);
                    case MTP_Invalid_Parameter:
                        throw new DeviceException("Invalid parameter. Coding error !", code);
                    case MTP_Parameter_Not_Supported:
                        throw new DeviceException("Parameter Not Supported. Coding error !", code);
                    case MTP_Invalid_Status:
                        throw new DeviceException("Invalid status.", code);
                    case MTP_Access_Denied:
                        throw new DeviceException("Access denied.", code);
                    case MTP_Set_Property_Not_Support:
                        throw new DeviceException("Set Property Not Support.", code);
                    case E_WPD_DEVICE_IS_HUNG:
                        throw new DeviceException("E_WPD_DEVICE_IS_HUNG", code);
                    case ERROR_BUSY:
                        throw new DeviceException("MTP device busy", code);
                    case MTP_Operation_Not_Supported:
                        throw new DeviceException("Operation Not Supported", code);
                    default:
                        throw new DeviceException("Device MTP error code: " + code.ToString("X"), code);
                }
            }
        }

        public static void GetException(COMException exception)
        {
            switch ((uint) exception.ErrorCode)
            {
                case WIA_ERROR_BUSY:
                    throw new DeviceException("Device is busy. Error code :WIA_ERROR_BUSY", exception,
                                              (uint) exception.ErrorCode);
                case WIA_ERROR_DEVICE_COMMUNICATION:
                    throw new DeviceException("Device communication error. Error code :WIA_ERROR_DEVICE_COMMUNICATION",
                                              exception, (uint) exception.ErrorCode);
                case WIA_ERROR_DEVICE_LOCKED:
                    throw new DeviceException("Device is locked. Error code :WIA_ERROR_DEVICE_LOCKED", exception,
                                              (uint) exception.ErrorCode);
                case WIA_ERROR_EXCEPTION_IN_DRIVER:
                    throw new DeviceException("Exception in driver. Error code :WIA_ERROR_EXCEPTION_IN_DRIVER",
                                              exception, (uint) exception.ErrorCode);
                case WIA_ERROR_GENERAL_ERROR:
                    throw new DeviceException("General error. Error code :WIA_ERROR_GENERAL_ERROR", exception,
                                              (uint) exception.ErrorCode);
                case WIA_ERROR_INCORRECT_HARDWARE_SETTING:
                    throw new DeviceException(
                        "Incorrect hardware error. Error code :WIA_ERROR_INCORRECT_HARDWARE_SETTING", exception,
                        (uint) exception.ErrorCode);
                case WIA_ERROR_INVALID_COMMAND:
                    throw new DeviceException("Invalid command. Error code :WIA_ERROR_INVALID_COMMAND", exception,
                                              (uint) exception.ErrorCode);
                case WIA_ERROR_INVALID_DRIVER_RESPONSE:
                    throw new DeviceException("Invalid driver response. Error code :WIA_ERROR_INVALID_DRIVER_RESPONSE",
                                              exception, (uint) exception.ErrorCode);
                case WIA_ERROR_OFFLINE:
                    throw new DeviceException("Device is offline. Error code :WIA_ERROR_OFFLINE", exception,
                                              WIA_ERROR_OFFLINE, (uint) exception.ErrorCode);
                case WIA_ERROR_UNABLE_TO_FOCUS:
                    throw new DeviceException("Unable to focus. Error code :WIA_ERROR_UNABLE_TO_FOCUS", exception,
                                              WIA_ERROR_UNABLE_TO_FOCUS, (uint) exception.ErrorCode);
                default:
                    throw new DeviceException("Unknown error. Error code:" + (uint) exception.ErrorCode, exception,
                                              (uint) exception.ErrorCode);
            }
        }
    }
}
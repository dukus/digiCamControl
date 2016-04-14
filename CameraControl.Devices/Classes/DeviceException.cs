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
using System.Runtime.Serialization;

#endregion

namespace CameraControl.Devices.Classes
{
    [Serializable]
    public class DeviceException : Exception
    {
        public uint ErrorCode { get; set; }

        public DeviceException(string message)
            : base(message)
        {
        }

        public DeviceException(string message, uint errorcode)
            : base(message)
        {
            ErrorCode = errorcode;
        }

        public DeviceException(string message, int errorcode)
            : base(message)
        {
            ErrorCode = (uint) errorcode;
        }

        public DeviceException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public DeviceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DeviceException(string message, Exception innerException, uint code)
            : base(message, innerException)
        {
            ErrorCode = code;
        }


        public DeviceException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected DeviceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public DeviceException()
            : base()
        {
        }
    }
}
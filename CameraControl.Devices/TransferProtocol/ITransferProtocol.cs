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

using System.IO;
using PortableDeviceLib;

namespace CameraControl.Devices.TransferProtocol
{
    public interface ITransferProtocol
    {
        string Model { get; }
        string Manufacturer { get; }
        string SerialNumber { get; }
        bool IsConnected { get; set; }
        string DeviceId { get; }


        MTPDataResponse ExecuteReadBigData(uint code, Stream stream, StillImageDevice.TransferCallback callback,
            params uint[] parameters);

        MTPDataResponse ExecuteReadData(uint code, params uint[] parameters);
        uint ExecuteWithNoData(uint code, params uint[] parameters);
        uint ExecuteWriteData(uint code, byte[] data, params uint[] parameters);

        void Disconnect();

    }
}
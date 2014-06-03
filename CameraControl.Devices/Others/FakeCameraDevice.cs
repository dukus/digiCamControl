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

using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Others
{
    public class FakeCameraDevice : BaseCameraDevice
    {
        #region Implementation of ICameraDevice

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            return true;
        }

        public override void CapturePhoto()
        {
        }

        public override void CapturePhotoNoAf()
        {
        }

        #endregion

        public FakeCameraDevice()
        {
            HaveLiveView = false;
            IsBusy = false;
            DeviceName = "Fake camera";
            SerialNumber = "00000000";
            IsConnected = true;
            HaveLiveView = false;
            ExposureStatus = 1;
            ExposureCompensation = new PropertyValue<int>() {IsEnabled = false};
            Mode = new PropertyValue<uint> {IsEnabled = false};
            FNumber = new PropertyValue<int> {IsEnabled = false};
            ShutterSpeed = new PropertyValue<long> {IsEnabled = false};
            WhiteBalance = new PropertyValue<long> {IsEnabled = false};
            FocusMode = new PropertyValue<long> {IsEnabled = false};
            CompressionSetting = new PropertyValue<int> {IsEnabled = false};
            IsoNumber = new PropertyValue<int> {IsEnabled = false};
            ExposureMeteringMode = new PropertyValue<int> {IsEnabled = false};
            Battery = 100;
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
        }
    }
}
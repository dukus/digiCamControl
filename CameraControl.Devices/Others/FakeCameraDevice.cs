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
using System.IO;
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

        private LiveViewData _liveViewData = new LiveViewData();

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
            IsoNumber = new PropertyValue<int> {IsEnabled = true};
            ExposureMeteringMode = new PropertyValue<int> {IsEnabled = false};
            Battery = 100;
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            Capabilities.Add(CapabilityEnum.LiveView);
            LiveViewImageZoomRatio = new PropertyValue<int>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";

            IsoNumber.AddValues("100", 100);
            IsoNumber.AddValues("200", 200);
            IsoNumber.AddValues("300", 300);
            IsoNumber.Value = "100";

            var val = new PropertyValue<long>() {Name = "Test Test"};
            val.AddValues("Val 1", 1);
            val.AddValues("Val 2", 2);
            val.AddValues("Val 3", 3);
            val.Value = "Val 1";
            AdvancedProperties.Add(val);
        }

        public override void StartLiveView()
        {
            _liveViewData.ImageData = File.ReadAllBytes("logo_big.jpg");
            _liveViewData.FocusFrameXSize = 100;
            _liveViewData.FocusFrameYSize = 100;
            _liveViewData.HaveFocusData = true;
            _liveViewData.IsLiveViewRunning = true;
            _liveViewData.LiveViewImageHeight = 639;
            _liveViewData.LiveViewImageWidth = 639;
            _liveViewData.ImageWidth = 639;
            _liveViewData.ImageHeight = 639;
            _liveViewData.FocusX = 639/2;
            _liveViewData.FocusY = 639 / 2;
        }

        public override void StopLiveView()
        {
            
        }


        public override LiveViewData GetLiveViewImage()
        {
            _liveViewData.SoundL++;
            if (_liveViewData.SoundL > 99)
                _liveViewData.SoundL = 0;
             
            _liveViewData.SoundR++;
            if (_liveViewData.SoundR > 99)
                _liveViewData.SoundR = 0;

            return _liveViewData;
        }

        public override void Focus(int x, int y)
        {
            _liveViewData.FocusX = x;
            _liveViewData.FocusY = y;
        }

        public override int Focus(int step)
        {
            return step;
        }
    }
}
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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Accord;
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
            List<string> files = new List<string>(
                Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "*."+ CompressionSetting.Value));
            if (files.Count > 0)
            {
                Random rnd1 = new Random();
                var file = files[rnd1.Next(files.Count - 1)];
                PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                {
                    WiaImageItem = null,
                    EventArgs = null,
                    CameraDevice = this,
                    FileName = file,
                    Handle = file
                };
                Task.Factory.StartNew(() => OnPhotoCapture(this, args));
            }
            else
            {
                throw new Exception("No file in the MyPictures folder with extension " + CompressionSetting.Value);
            }
        }

        public override void CapturePhotoNoAf()
        {
            CapturePhoto();
        }

        public override void TransferFile(object o, Stream stream)
        {
                var b = File.ReadAllBytes((string) o);
                stream.Write(b, 0, b.Length);
        }

        public override void TransferFile(object o, string filename)
        {
            File.Copy((string) o, filename);
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
            ExposureCompensation = new PropertyValue<long>() {IsEnabled = false};
            Mode = new PropertyValue<long> {IsEnabled = false};
            FNumber = new PropertyValue<long> {IsEnabled = false};
            ShutterSpeed = new PropertyValue<long> {IsEnabled = false};
            WhiteBalance = new PropertyValue<long> {IsEnabled = false};
            FocusMode = new PropertyValue<long> {IsEnabled = false};

            CompressionSetting = new PropertyValue<long> {IsEnabled = true};
            CompressionSetting.AddValues("jpg",0);
            CompressionSetting.AddValues("cr2", 1);
            CompressionSetting.AddValues("nef", 2);
            CompressionSetting.Value = "jpg";
            CompressionSetting.ReloadValues();

            IsoNumber = new PropertyValue<long> {IsEnabled = true};
            ExposureMeteringMode = new PropertyValue<long> {IsEnabled = false};
            Battery = 100;
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.LiveViewStream);
            LiveViewImageZoomRatio = new PropertyValue<long>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";

            IsoNumber.AddValues("100", 100);
            IsoNumber.AddValues("200", 200);
            IsoNumber.AddValues("300", 300);
            IsoNumber.Value = "100";
            IsoNumber.ReloadValues();

            var val = new PropertyValue<long>() {Name = "Test Test"};
            val.AddValues("Val 1", 1);
            val.AddValues("Val 2", 2);
            val.AddValues("Val 3", 3);
            val.Value = "Val 1";
            val.ReloadValues();
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

        public override string GetLiveViewStream()
        {
            return "rtsp://freja.hiof.no:1935/rtplive/definst/hessdalen03.stream";
           // return "rtsp://173.12.1.249/1";
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

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            return true;
        }

        //public override string ToString()
        //{

        //    StringBuilder c = new StringBuilder(base.ToString() + "\n\tType..................Fake");
        //    return c.ToString();
        //}
        
    }
}
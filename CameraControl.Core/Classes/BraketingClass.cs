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
using System.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public class BracketingClass : BaseFieldClass
    {
        public int Mode { get; set; }
        public string ExpLow { get; set; }
        public string ExpHigh { get; set; }
        public int ExpCaptureCount { get; set; }

        public string FLow { get; set; }
        public string FHigh { get; set; }
        public int FCaptureCount { get; set; }

        public string IsoLow { get; set; }
        public string IsoHigh { get; set; }
        public int IsoCaptureCount { get; set; }
        //------------------------------------------------------

        public int Index;
        private string _defec = "0";
        private CameraPreset _cameraPreset = new CameraPreset();
        private ICameraDevice _cameraDevice = null;

        public event EventHandler PhotoCaptured;
        public event EventHandler IsBusyChanged;
        public event EventHandler BracketingDone;

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
                if (IsBusyChanged != null)
                    IsBusyChanged(this, new EventArgs());
            }
        }

       

        private AsyncObservableCollection<string> _exposureValues;

        public AsyncObservableCollection<string> ExposureValues
        {
            get { return _exposureValues; }
            set
            {
                _exposureValues = value;
                NotifyPropertyChanged("ExposureValues");
            }
        }

        private AsyncObservableCollection<string> _shutterValues;

        public AsyncObservableCollection<string> ShutterValues
        {
            get { return _shutterValues; }
            set
            {
                _shutterValues = value;
                NotifyPropertyChanged("ShutterValues");
            }
        }

        private AsyncObservableCollection<string> _presetValues;
        private AsyncObservableCollection<string> _apertureValues;

        public AsyncObservableCollection<string> PresetValues
        {
            get { return _presetValues; }
            set
            {
                _presetValues = value;
                NotifyPropertyChanged("PresetValues");
            }
        }

        public AsyncObservableCollection<string> ApertureValues
        {
            get { return _apertureValues; }
            set
            {
                _apertureValues = value;
                NotifyPropertyChanged("ApertureValues");
            }
        }


        public BracketingClass()
        {
            IsBusy = false;
            ExposureValues = new AsyncObservableCollection<string>();
            ShutterValues = new AsyncObservableCollection<string>();
            PresetValues = new AsyncObservableCollection<string>();
            ApertureValues = new AsyncObservableCollection<string>();
            Mode = 0;
        }

        public void TakePhoto(ICameraDevice device)
        {
            _cameraDevice = device;
            Log.Debug("Bracketing started");
            _cameraDevice.CaptureCompleted += _cameraDevice_CaptureCompleted;
            IsBusy = true;
            switch (Mode)
            {
                case 0:
                    {
                        if (ExposureValues.Count == 0)
                        {
                            Stop();
                            return;
                        }
                        Index = 0;
                        try
                        {
                            _defec = _cameraDevice.ExposureCompensation.Value;
                            Thread.Sleep(100);
                            _cameraDevice.ExposureCompensation.SetValue(ExposureValues[Index]);
                            Thread.Sleep(100);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 1:
                    {
                        if (ShutterValues.Count == 0)
                        {
                            Stop();
                            return;
                        }
                        Index = 0;
                        try
                        {
                            _defec = _cameraDevice.ShutterSpeed.Value;
                            Thread.Sleep(100);
                            _cameraDevice.ShutterSpeed.SetValue(ShutterValues[Index]);
                            Thread.Sleep(100);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 2:
                    {
                        if (PresetValues.Count == 0)
                        {
                            Stop();
                            return;
                        }
                        Index = 0;
                        try
                        {
                            _cameraPreset.Get(_cameraDevice);
                            Thread.Sleep(100);
                            CameraPreset preset = ServiceProvider.Settings.GetPreset(PresetValues[Index]);
                            if (preset != null)
                                preset.Set(_cameraDevice);
                            Thread.Sleep(100);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 3:
                    {
                        if (ApertureValues.Count == 0)
                        {
                            Stop();
                            return;
                        }
                        Index = 0;
                        try
                        {
                            _defec = _cameraDevice.FNumber.Value;
                            Thread.Sleep(100);
                            _cameraDevice.FNumber.SetValue(ApertureValues[Index]);
                            Thread.Sleep(100);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
            }
        }

        private void _cameraDevice_CaptureCompleted(object sender, EventArgs e)
        {
            if (!IsBusy)
                return;
            Thread thread = new Thread(EventNextPhoto);
            thread.Start();
        }


        private void EventNextPhoto()
        {
            while (_cameraDevice.IsBusy)
            {
                Thread.Sleep(1);
            }
            if (PhotoCaptured != null)
                PhotoCaptured(this, new EventArgs());
            switch (Mode)
            {
                case 0:
                    {
                        if (Index < ExposureValues.Count)
                        {
                            CaptureNextPhoto();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                    break;
                case 1:
                    {
                        if (Index < ShutterValues.Count)
                        {
                            CaptureNextPhoto();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                    break;
                case 2:
                    {
                        if (Index < PresetValues.Count)
                        {
                            CaptureNextPhoto();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                    break;
                case 3:
                    {
                        if (Index < ApertureValues.Count)
                        {
                            CaptureNextPhoto();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                    break;
            }
        }

        private void CaptureNextPhoto()
        {
            Log.Debug("Bracketing take next photo");
            switch (Mode)
            {
                case 0:
                    {
                        try
                        {
                            _cameraDevice.ExposureCompensation.SetValue(ExposureValues[Index]);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 1:
                    {
                        try
                        {
                            _cameraDevice.ShutterSpeed.SetValue(ShutterValues[Index]);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 2:
                    {
                        try
                        {
                            CameraPreset preset = ServiceProvider.Settings.GetPreset(PresetValues[Index]);
                            if (preset != null)
                                preset.Set(_cameraDevice);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
                case 3:
                    {
                        try
                        {
                            _cameraDevice.FNumber.SetValue(ApertureValues[Index]);
                            CameraHelper.Capture(_cameraDevice);
                            Index++;
                        }
                        catch (DeviceException exception)
                        {
                            Log.Error(exception);
                            StaticHelper.Instance.SystemMessage = exception.Message;
                        }
                    }
                    break;
            }
        }

        public void Stop()
        {
            if (_cameraDevice == null)
                return;
            Log.Debug("Bracketing stop");
            IsBusy = false;
            _cameraDevice.CaptureCompleted -= _cameraDevice_CaptureCompleted;
            Thread thread = null;
            switch (Mode)
            {
                case 0:
                    {
                        thread = new Thread(() => _cameraDevice.
                                                      ExposureCompensation.SetValue(_defec));
                    }
                    break;
                case 1:
                    {
                        thread = new Thread(() => _cameraDevice.
                                                      ShutterSpeed.SetValue(_defec));
                    }
                    break;
                case 2:
                    {
                        thread = new Thread(() => _cameraPreset.Set(_cameraDevice));
                    }
                    break;
                case 3:
                    {
                        thread = new Thread(() => _cameraDevice.
                                                      FNumber.SetValue(_defec));
                    }
                    break;
            }
            thread.Start();
            if (BracketingDone != null)
                BracketingDone(this, new EventArgs());
        }
    }
}
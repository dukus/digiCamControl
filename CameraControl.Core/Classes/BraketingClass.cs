using System;
using System.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class BraketingClass : BaseFieldClass
    {
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

        public int Mode { get; set; }

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
        public AsyncObservableCollection<string> PresetValues
        {
            get { return _presetValues; }
            set
            {
                _presetValues = value;
                NotifyPropertyChanged("PresetValues");
            }
        }


        public BraketingClass()
        {
            IsBusy = false;
            ExposureValues = new AsyncObservableCollection<string>();
            ShutterValues = new AsyncObservableCollection<string>();
            PresetValues = new AsyncObservableCollection<string>();
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
                            return;
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
                            return;
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
                            return;
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
            }
        }

        void _cameraDevice_CaptureCompleted(object sender, EventArgs e)
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
            }
            thread.Start();
            if (BracketingDone != null)
                BracketingDone(this, new EventArgs());
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Timer = System.Timers.Timer;

namespace CameraControl.ViewModel
{
    public class BracketingViewModel : ViewModelBase
    {
        private ICameraDevice _camera;
        private ObservableCollection<string> _expLowList;
        private ObservableCollection<string> _expHighList;
        private string _error;
        private string _message;
        private int _expCountMax;
        private Timer _timer = new Timer(200);
        private bool _isBusy;

        public RelayCommand StartCommand { get; set; }
        
        public BracketingClass BracketingSettings
        {
            get { return ServiceProvider.Settings.DefaultSession.Braketing; }
            
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public ICameraDevice Camera
        {
            get
            {
                if (_camera == null)
                    return ServiceProvider.DeviceManager.SelectedCameraDevice;
                return _camera;
            }
            set
            {
                _camera = value;
                RaisePropertyChanged(() => Camera);
            }
        }


        public int Mode
        {
            get { return BracketingSettings.Mode; }
            set
            {
                BracketingSettings.Mode = value;
                RaisePropertyChanged(() => Mode);
            }
        }


        public ObservableCollection<string> ExpLowList
        {
            get
            {
                return Camera.ExposureCompensation.Values;
                return _expLowList;
            }
            set { _expLowList = value; }
        }

        public ObservableCollection<string> ExpHighList
        {
            get
            {
                if (_expHighList == null)
                    return Camera.ExposureCompensation.Values;
                return _expHighList;
            }
            set
            {
                _expHighList = value;
                RaisePropertyChanged(() => ExpHighList);
            }
        }

        public string ExpLow
        {
            get { return BracketingSettings.ExpLow; }
            set
            {
                BracketingSettings.ExpLow = value;
                RaisePropertyChanged(() => ExpLow);
                try
                {
                    var i = Camera.ExposureCompensation.Values.IndexOf(ExpLow)+1;
                    if (i < Camera.ExposureCompensation.Values.Count)
                        ExpHighList = new ObservableCollection<string>(Camera.ExposureCompensation.Values.ToList()
                            .GetRange(i, Camera.ExposureCompensation.Values.Count - i));
                }
                catch (Exception)
                {
                    
                }
                SetMessage();
            }
        }

        public string ExpHigh
        {
            get { return BracketingSettings.ExpHigh; }
            set
            {
                BracketingSettings.ExpHigh = value;
                RaisePropertyChanged(() => ExpHigh);
                SetMessage();
            }
        }

        public int ExpCaptureCount
        {
            get { return BracketingSettings.ExpCaptureCount; }
            set
            {
                BracketingSettings.ExpCaptureCount = value;
                RaisePropertyChanged(() => ExpCaptureCount);
                SetMessage();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
                RaisePropertyChanged(() => IsFree);
            }
        }

        public bool IsFree
        {
            get { return !IsBusy; }
        }

        public int Counter { get; set; }
        public List<string> Values { get; set; }


        public BracketingViewModel()
        {
            _timer.Elapsed += _timer_Elapsed;
            if (!IsInDesignMode)
                SetMessage();
            StartCommand = new RelayCommand(Start);
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Camera.IsBusy)
                return;
            try
            {
                _timer.Stop();
                switch (Mode)
                {
                    case 0:
                    {
                        Camera.ExposureCompensation.Value = Values[Counter];
                        Thread.Sleep(200);
                        CameraHelper.Capture(Camera);
                        Counter++;
                        if (Counter >= Values.Count)
                        {
                            Stop();
                            return;
                        }
                    }
                        break;
                }
                _timer.Start();
            }
            catch (Exception ex)
            {
                StaticHelper.Instance.SystemMessage = ex.Message;
                Stop();
            }
            
        }

        public void Start()
        {
            Counter = 0;
            IsBusy = true;
            _timer.Start();
        }

        public void Stop()
        {
            IsBusy = false;
            _timer.Stop();
        }

        public void SetMessage()
        {
            Error = "";
            Message = "";
            switch (Mode)
            {
                case 0:
                {
                    var vals = GetValues(ExpLowList.ToList(), ExpLow, ExpHigh, ExpCaptureCount);
                    if (vals == null || vals.Count == 0)
                        return;
                    Values = vals;
                    foreach (var val in vals)
                    {
                        Message += (val +", ");
                    }
                }
                    break;
            }
        }

        public List<string> GetValues(IList<string> vals, string low, string high, int count)
        {
            var res = new List<string>();
            if (string.IsNullOrEmpty(ExpLow))
            {
                Error = TranslationStrings.LabelNoLowValueError;
                return null;
            }
            if (string.IsNullOrEmpty(ExpHigh))
            {
                Error = TranslationStrings.LabelNoHighValueError;
                return null;
            }
            var il = ExpLowList.IndexOf(ExpLow);
            var ih = ExpLowList.IndexOf(ExpHigh);
            if (il < 0 || ih < 0 || ih <= il || count < 2)
            {
                Error = TranslationStrings.LabelWrongValue;
                return null;
            }
            count = Math.Min(count, ih - il);
            var step = (ih - il) / (count - 1);
            if (step == 0)
                step = 1;

            for (int i = il; i < ih; i += step)
            {
                res.Add(vals[i]);
            }
            res.Add(vals[ih]);
            return res;
        }

    }
}

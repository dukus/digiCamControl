using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
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
        private ObservableCollection<string> _isoLowList;
        private ObservableCollection<string> _isoHighList;
        private ObservableCollection<string> _fLowList;
        private ObservableCollection<string> _fHighList;

        private string _error;
        private string _message;
        private Timer _timer = new Timer(100);
        private bool _isBusy;
        private string _curValue;

        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }

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
                RaisePropertyChanged(() => ExpVisibility);
                RaisePropertyChanged(() => FVisibility);
                RaisePropertyChanged(() => IsoVisibility);
            }
        }

        public Visibility ExpVisibility
        {
            get { return Mode == 0 ? Visibility.Visible : Visibility.Hidden; }
        }


        public ObservableCollection<string> ExpLowList
        {
            get
            {
                return Camera.ExposureCompensation.Values;
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
                    var i = Camera.ExposureCompensation.Values.IndexOf(ExpLow) + 1;
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
        #region f
        public Visibility FVisibility
        {
            get { return Mode == 1 ? Visibility.Visible : Visibility.Hidden; }
        }


        public ObservableCollection<string> FLowList
        {
            get { return _fLowList ?? (_fLowList = Camera.FNumber.Values); }
            set { _fLowList = value; }
        }

        public ObservableCollection<string> FHighList
        {
            get
            {
                if (_fHighList == null)
                    return Camera.FNumber.Values;
                return _fHighList;
            }
            set
            {
                _fHighList = value;
                RaisePropertyChanged(() => FHighList);
            }
        }

        public string FLow
        {
            get { return BracketingSettings.FLow; }
            set
            {
                BracketingSettings.FLow = value;
                RaisePropertyChanged(() => FLow);
                try
                {
                    var i = Camera.FNumber.Values.IndexOf(FLow) + 1;
                    if (i < Camera.FNumber.Values.Count)
                        FHighList = new ObservableCollection<string>(Camera.FNumber.Values.ToList()
                            .GetRange(i, Camera.FNumber.Values.Count - i));
                }
                catch (Exception)
                {

                }
                SetMessage();
            }
        }

        public string FHigh
        {
            get { return BracketingSettings.FHigh; }
            set
            {
                BracketingSettings.FHigh = value;
                RaisePropertyChanged(() => FHigh);
                SetMessage();
            }
        }

        public int FCaptureCount
        {
            get { return BracketingSettings.FCaptureCount; }
            set
            {
                BracketingSettings.FCaptureCount = value;
                RaisePropertyChanged(() => FCaptureCount);
                SetMessage();
            }
        }

        #endregion
#region iso
        public Visibility IsoVisibility
        {
            get { return Mode == 2 ? Visibility.Visible : Visibility.Hidden; }
        }


        public ObservableCollection<string> IsoLowList
        {
            get
            {
                return Camera.IsoNumber.Values;
            }
            set { _isoLowList = value; }
        }

        public ObservableCollection<string> IsoHighList
        {
            get
            {
                if (_isoHighList == null)
                    return Camera.IsoNumber.Values;
                return _isoHighList;
            }
            set
            {
                _isoHighList = value;
                RaisePropertyChanged(() => IsoHighList);
            }
        }

        public string IsoLow
        {
            get { return BracketingSettings.IsoLow; }
            set
            {
                BracketingSettings.IsoLow = value;
                RaisePropertyChanged(() => IsoLow);
                try
                {
                    var i = Camera.IsoNumber.Values.IndexOf(IsoLow) + 1;
                    if (i < Camera.IsoNumber.Values.Count)
                        IsoHighList = new ObservableCollection<string>(Camera.IsoNumber.Values.ToList()
                            .GetRange(i, Camera.IsoNumber.Values.Count - i));
                }
                catch (Exception)
                {

                }
                SetMessage();
            }
        }

        public string IsoHigh
        {
            get { return BracketingSettings.IsoHigh; }
            set
            {
                BracketingSettings.IsoHigh = value;
                RaisePropertyChanged(() => IsoHigh);
                SetMessage();
            }
        }

        public int IsoCaptureCount
        {
            get { return BracketingSettings.IsoCaptureCount; }
            set
            {
                BracketingSettings.IsoCaptureCount = value;
                RaisePropertyChanged(() => IsoCaptureCount);
                SetMessage();
            }
        }
#endregion
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

        public string CurValue
        {
            get { return _curValue; }
            set
            {
                _curValue = value;
                RaisePropertyChanged(() => CurValue);
            }
        }

        public int Counter { get; set; }
        public List<string> Values { get; set; }
        public string DefValue { get; set; }

        public BracketingViewModel()
        {
            _timer.Elapsed += _timer_Elapsed;
            if (!IsInDesignMode)
                SetMessage();
            if (IsInDesignMode)
                Mode = 1;
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Camera.IsBusy)
                return;
            try
            {
                _timer.Stop();
                Thread.Sleep(100);
                switch (Mode)
                {
                    case 0:
                        {
                            Camera.ExposureCompensation.Value = Values[Counter];
                            CurValue = Values[Counter];
                        }
                        break;
                    case 1:
                        {
                            Camera.FNumber.Value = Values[Counter];
                            CurValue = Values[Counter];
                        }
                        break;
                    case 2:
                        {
                            Camera.IsoNumber.Value = Values[Counter];
                            CurValue = Values[Counter];
                        }
                        break;
                }
                Thread.Sleep(200);
                CameraHelper.Capture(Camera);
                Counter++;
                if (Counter >= Values.Count)
                {
                    Stop();
                    return;
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
            try
            {
                Error = "";
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.NextSeries);
                switch (Mode)
                {
                    case 0:
                        DefValue = Camera.ExposureCompensation.Value;
                        break;
                    case 1:
                        if (!Camera.FNumber.IsEnabled)
                        {
                            Error = TranslationStrings.LabelWrongFNumber;
                            return;
                        }
                        DefValue = Camera.FNumber.Value;
                        break;
                    case 2:
                        if (Camera.Mode.Value != "M")
                        {
                            Error = TranslationStrings.LabelBracketingMMode;
                        }
                        DefValue = Camera.IsoNumber.Value;
                        break;
                }
                Counter = 0;
                IsBusy = true;
                _timer.Start();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                Log.Error("Unable to start bracketing ", ex);
            }
        }

        public void Stop()
        {
            _timer.Stop();
            CurValue = "";
            Thread.Sleep(200);
            switch (Mode)
            {
                case 0:
                    Camera.ExposureCompensation.Value = DefValue;
                    break;
                case 1:
                    Camera.FNumber.Value = DefValue;
                    break;
                case 2:
                    Camera.IsoNumber.Value = DefValue;
                    break;
            }
            IsBusy = false;
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
                            Message += (val + ", ");
                        }
                    }
                    break;
                case 1:
                    {
                        var vals = GetValues(FLowList.ToList(), FLow, FHigh, FCaptureCount);
                        if (vals == null || vals.Count == 0)
                            return;
                        Values = vals;
                        foreach (var val in vals)
                        {
                            Message += (val + ", ");
                        }
                    }
                    break;
                case 2:
                    {
                        var vals = GetValues(IsoLowList.ToList(), IsoLow, IsoHigh, IsoCaptureCount);
                        if (vals == null || vals.Count == 0)
                            return;
                        Values = vals;
                        foreach (var val in vals)
                        {
                            Message += (val + ", ");
                        }
                    }
                    break;
            }
        }

        public List<string> GetValues(IList<string> vals, string low, string high, int count)
        {
            var res = new List<string>();
            if (string.IsNullOrEmpty(low))
            {
                Error = TranslationStrings.LabelNoLowValueError;
                return null;
            }
            if (string.IsNullOrEmpty(high))
            {
                Error = TranslationStrings.LabelNoHighValueError;
                return null;
            }
            var il = vals.IndexOf(low);
            var ih = vals.IndexOf(high);
            if (il < 0 || ih < 0 || ih <= il || count < 2)
            {
                Error = TranslationStrings.LabelWrongValue;
                return null;
            }
            count = Math.Min(count, ih - il);
            count = Math.Max(count, 2);
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

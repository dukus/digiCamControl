using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.TclScripting;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.ViewModel
{
    public class TimelapseViewModel : ViewModelBase
    {
        private Timer _timer = new Timer(1000);
        private bool _isRunning;
        private bool _isActive;
        private int _totalCaptures;
        private DateTime _lastCaptureTime = DateTime.Now;
        private DateTime _timeLapseStartTime = DateTime.Now;
        private DateTime _firstLapseStartTime = DateTime.Now;
        private bool _fullSpeed;
        private BracketingViewModel _bracketingViewModel = null;

        public TimeLapseSettings TimeLapseSettings { get; set; }

        public bool StartNow
        {
            get { return TimeLapseSettings.StartNow; }
            set
            {
                TimeLapseSettings.StartNow = value;
                RaisePropertyChanged(() => StartNow);
                RaisePropertyChanged(() => DateVisibility);
                RaisePropertyChanged(() => TimeVisibility);
                RaisePropertyChanged(() => DayVisibility);
                RaisePropertyChanged(() => StopAtEnable);
            }
        }

        public bool StartAt
        {
            get { return TimeLapseSettings.StartAt; }
            set
            {
                TimeLapseSettings.StartAt = value;
                RaisePropertyChanged(() => StartAt);
                RaisePropertyChanged(() => DateVisibility);
                RaisePropertyChanged(() => TimeVisibility);
                RaisePropertyChanged(() => DayVisibility);
                RaisePropertyChanged(() => StopAtEnable);
            }
        }

        public bool StartIn
        {
            get { return TimeLapseSettings.StartIn; }
            set
            {
                TimeLapseSettings.StartIn = value;
                RaisePropertyChanged(() => DateVisibility);
                RaisePropertyChanged(() => TimeVisibility);
                RaisePropertyChanged(() => StartIn);
                RaisePropertyChanged(() => DayVisibility);
                RaisePropertyChanged(() => StopAtEnable);
            }
        }

        public bool StartDaily
        {
            get { return TimeLapseSettings.StartDaily; }
            set
            {
                TimeLapseSettings.StartDaily = value;
                if (StopAt)
                {
                    StopAt = false;
                    StopIn = true;
                }
                RaisePropertyChanged(() => StartDaily);
                RaisePropertyChanged(() => DateVisibility);
                RaisePropertyChanged(() => TimeVisibility);
                RaisePropertyChanged(() => DayVisibility);
                RaisePropertyChanged(() => StopAtEnable);
            }
        }

        public DateTime StartDate
        {
            get { return TimeLapseSettings.StartDate; }
            set
            {
                var d = TimeLapseSettings.StartDate;
                TimeLapseSettings.StartDate = TimeLapseSettings.StartDate = new DateTime(value.Year, value.Month, value.Day, d.Hour, d.Minute, d.Second);
                RaisePropertyChanged(() => StartDate);
            }
        }

        public TimeSpan StartTime
        {
            get { return new TimeSpan(TimeLapseSettings.StartDate.Hour, TimeLapseSettings.StartDate.Minute, TimeLapseSettings.StartDate.Second); }
            set
            {
                var d = TimeLapseSettings.StartDate;
                TimeLapseSettings.StartDate = new DateTime(d.Year, d.Month, d.Day, value.Hours, value.Minutes,
                    value.Seconds);
                RaisePropertyChanged(() => StartTime);
            }
        }

        public int StartHour
        {
            get { return TimeLapseSettings.StartDate.Hour; }
            set
            {
                var d = TimeLapseSettings.StartDate;
                TimeLapseSettings.StartDate = new DateTime(d.Year, d.Month, d.Day, value, d.Minute, d.Second);
                RaisePropertyChanged(() => StartHour);
            }
        }

        public int StartMinute
        {
            get { return TimeLapseSettings.StartDate.Minute; }
            set
            {
                var d = TimeLapseSettings.StartDate;
                TimeLapseSettings.StartDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, value, d.Second);
                RaisePropertyChanged(() => StartMinute);
            }
        }

        public int StartSecond
        {
            get { return TimeLapseSettings.StartDate.Second; }
            set
            {
                var d = TimeLapseSettings.StartDate;
                TimeLapseSettings.StartDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, value);
                RaisePropertyChanged(() => StartSecond);
            }
        }

        public bool StartDay0
        {
            get { return TimeLapseSettings.StartDay0; }
            set
            {
                TimeLapseSettings.StartDay0 = value;
                RaisePropertyChanged(() => StartDay0);
            }
        }

        public bool StartDay1
        {
            get { return TimeLapseSettings.StartDay1; }
            set
            {
                TimeLapseSettings.StartDay1 = value;
                RaisePropertyChanged(() => StartDay1);
            }
        }

        public bool StartDay2
        {
            get { return TimeLapseSettings.StartDay2; }
            set
            {
                TimeLapseSettings.StartDay2 = value;
                RaisePropertyChanged(() => StartDay2);
            }
        }

        public bool StartDay3
        {
            get { return TimeLapseSettings.StartDay3; }
            set
            {
                TimeLapseSettings.StartDay3 = value;
                RaisePropertyChanged(() => StartDay3);
            }
        }

        public bool StartDay4
        {
            get { return TimeLapseSettings.StartDay4; }
            set
            {
                TimeLapseSettings.StartDay4 = value;
                RaisePropertyChanged(() => StartDay4);
            }
        }

        public bool StartDay5
        {
            get { return TimeLapseSettings.StartDay5; }
            set
            {
                TimeLapseSettings.StartDay5 = value;
                RaisePropertyChanged(() => StartDay5);
            }
        }

        public bool StartDay6
        {
            get { return TimeLapseSettings.StartDay6; }
            set
            {
                TimeLapseSettings.StartDay6 = value;
                RaisePropertyChanged(() => StartDay6);
            }
        }

        public bool StopAtPhotos
        {
            get { return TimeLapseSettings.StopAtPhotos; }
            set
            {
                TimeLapseSettings.StopAtPhotos = value;
                RaisePropertyChanged(() => StopAtPhotos);
                RaisePropertyChanged(() => StopDateVisibility);
                RaisePropertyChanged(() => StopTimeVisibility);
                RaisePropertyChanged(() => StopCountVisibility);
            }
        }

        public bool StopIn
        {
            get { return TimeLapseSettings.StopIn; }
            set
            {
                TimeLapseSettings.StopIn = value;
                RaisePropertyChanged(() => StopIn);
                RaisePropertyChanged(() => StopDateVisibility);
                RaisePropertyChanged(() => StopTimeVisibility);
                RaisePropertyChanged(() => StopCountVisibility);
            }
        }

        public bool StopAt
        {
            get { return TimeLapseSettings.StopAt; }
            set
            {
                TimeLapseSettings.StopAt = value;
                RaisePropertyChanged(() => StopAt);
                RaisePropertyChanged(() => StopDateVisibility);
                RaisePropertyChanged(() => StopTimeVisibility);
                RaisePropertyChanged(() => StopCountVisibility);
            }
        }

        public int StopCaptureCount
        {
            get { return TimeLapseSettings.StopCaptureCount; }
            set
            {
                TimeLapseSettings.StopCaptureCount = value;
                RaisePropertyChanged(() => StopCaptureCount);
            }
        }

        public bool StopAtEnable
        {
            get
            {
                return !StartDaily;
            }
        }

        public DateTime StopDate
        {
            get { return TimeLapseSettings.StopDate; }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate = TimeLapseSettings.StopDate = new DateTime(value.Year, value.Month, value.Day, d.Hour, d.Minute, d.Second);
                RaisePropertyChanged(() => StopDate);
            }
        }

        public TimeSpan StopTime
        {
            get { return new TimeSpan(TimeLapseSettings.StopDate.Hour, TimeLapseSettings.StopDate.Minute, TimeLapseSettings.StopDate.Second); }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate = new DateTime(d.Year, d.Month, d.Day, value.Hours, value.Minutes,
                    value.Seconds);
                RaisePropertyChanged(() => StopTime);
            }
        }

        public int StopHour
        {
            get { return TimeLapseSettings.StopDate.Hour; }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate = new DateTime(d.Year, d.Month, d.Day, value, d.Minute, d.Second);
                RaisePropertyChanged(() => StopHour);
            }
        }

        public int StopMinute
        {
            get { return TimeLapseSettings.StopDate.Minute; }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, value, d.Second);
                RaisePropertyChanged(() => StopMinute);
            }
        }

        public int StopSecond
        {
            get { return TimeLapseSettings.StopDate.Second; }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, value);
                RaisePropertyChanged(() => StopSecond);
            }
        }

        public int TimeBetweenShots
        {
            get { return TimeLapseSettings.TimeBetweenShots; }
            set
            {
                TimeLapseSettings.TimeBetweenShots = value;
                RaisePropertyChanged(() => TimeBetweenShots);
            }
        }

        public bool Capture
        {
            get { return TimeLapseSettings.Capture; }
            set
            {
                TimeLapseSettings.Capture = value;
                RaisePropertyChanged(() => Capture);
                RaisePropertyChanged(() => ScriptVisibility);
                RaisePropertyChanged(() => FullSpeed);
                RaisePropertyChanged(() => NotFullSpeed);
            }
        }

        public bool CaptureAll
        {
            get { return TimeLapseSettings.CaptureAll; }
            set
            {
                TimeLapseSettings.CaptureAll = value;
                RaisePropertyChanged(() => CaptureAll);
                RaisePropertyChanged(() => ScriptVisibility);
                RaisePropertyChanged(() => FullSpeed);
                RaisePropertyChanged(() => NotFullSpeed);
            }
        }

        public bool CaptureScript
        {
            get { return TimeLapseSettings.CaptureScript; }
            set
            {
                TimeLapseSettings.CaptureScript = value;
                RaisePropertyChanged(() => CaptureScript);
                RaisePropertyChanged(() => ScriptVisibility);
                RaisePropertyChanged(() => FullSpeed);
                RaisePropertyChanged(() => NotFullSpeed);
            }
        }

        public bool Bracketing
        {
            get { return TimeLapseSettings.Bracketing; }
            set { TimeLapseSettings.Bracketing = value; }
        }

        public string ScriptFile
        {
            get { return TimeLapseSettings.ScriptFile; }
            set
            {
                TimeLapseSettings.ScriptFile = value;
                RaisePropertyChanged(() => ScriptFile);
            }
        }


        public Visibility DateVisibility
        {
            get { return StartAt ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TimeVisibility
        {
            get { return StartIn || StartAt || StartDaily ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DayVisibility
        {
            get { return StartDaily ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility StopDateVisibility
        {
            get { return StopAt ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility StopTimeVisibility
        {
            get { return StopIn || StopAt ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility StopCountVisibility
        {
            get { return StopAtPhotos ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ScriptVisibility
        {
            get { return CaptureScript ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsFree
        {
            get { return !_isRunning; }
        }

        public bool FullSpeed
        {
            get { return _fullSpeed && Capture; }
            set
            {
                _fullSpeed = value;
                RaisePropertyChanged(() => FullSpeed);
                RaisePropertyChanged(() => NotFullSpeed);
            }
        }

        public bool NotFullSpeed
        {
            get { return !FullSpeed; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                RaisePropertyChanged(() => IsRunning);
                RaisePropertyChanged(() => StartText);
                RaisePropertyChanged(() => StatusText);
                RaisePropertyChanged(() => IsFree);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
                RaisePropertyChanged(() => StatusText);
            }
        }

        public string StartText
        {
            get { return !IsRunning ? TranslationStrings.ButtonStartTimeLapse : TranslationStrings.ButtonStopTimeLapse; }
        }

        public string StatusText
        {
            get
            {
                if (!IsRunning)
                    return "Not running";
                if (!IsActive)
                {
                    if (StartIn)
                    {
                        var t = new TimeSpan(StartHour, StartMinute, StartSecond);
                        var sec = t.TotalSeconds - (DateTime.Now - _timeLapseStartTime).TotalSeconds;

                        return sec < 0
                            ? ""
                            : string.Format("The timlapse will start in {0} seconds", Math.Round(sec, 0));
                    }
                    if (StartAt)
                    {
                        return StartDate < DateTime.Now ? "" : string.Format("The timlapse will start in {0:dd\\.hh\\:mm\\:ss}", (StartDate - DateTime.Now));
                    }
                    return "Waiting for shedule";
                }
                return FullSpeed ? string.Format("Timelapse in progress.\nTotal captured photos {0}", _totalCaptures) 
                    : string.Format("Timelapse in progress.\nNext capture in {0} seconds\nTotal captured photos {1}",
                    Math.Round(TimeBetweenShots - (DateTime.Now - _lastCaptureTime).TotalSeconds, 0), _totalCaptures);
            }
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand BrowsCommand { get; set; }

        public TimelapseViewModel()
        {
            StartCommand = new RelayCommand(Start);
            BrowsCommand = new RelayCommand(Browse);
            if (IsInDesignMode)
            {
                TimeLapseSettings = new TimeLapseSettings();
                TimeLapseSettings.StartNow = false;
                TimeLapseSettings.StartDaily = true;
            }
            else
            {
                TimeLapseSettings = ServiceProvider.Settings.DefaultSession.TimeLapseSettings;
                ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            }
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
        }

        void WindowsManager_Event(string cmd, object o)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.TimeLapse_Start:
                    StartL();
                    break;
                case WindowsCmdConsts.TimeLapse_Stop:
                    StopL();
                    break;
            }
        }

        public void Browse()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Tcl Script file(*.tcl)|*.tcl|Script file(*.dccscript)|*.dccscript|All files|*.*";
            if (dlg.ShowDialog() == true)
            {
                ScriptFile = dlg.FileName;
            }
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsActive)
            {
                IsActive = CheckStart();
                if (IsActive)
                    _firstLapseStartTime = DateTime.Now;
            }
            RaisePropertyChanged(() => StatusText);
            if (!IsActive)
                return;
            if (IsActive)
            {
                if (Math.Round((DateTime.Now - _lastCaptureTime).TotalSeconds) >= TimeBetweenShots || (FullSpeed && !ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy))
                {
                    _lastCaptureTime = DateTime.Now;
                    _totalCaptures++;
                    try
                    {
                        if (Capture)
                        {
                            if (Bracketing)
                            {
                                if (_bracketingViewModel == null)
                                    _bracketingViewModel = new BracketingViewModel();
                                Task.Factory.StartNew(new Action(_bracketingViewModel.Start));
                                StaticHelper.Instance.SystemMessage = _bracketingViewModel.Error;
                            }
                            else
                            {
                            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.Capture);                                
                            }
                        }
                        if (CaptureAll)
                            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.CaptureAll);
                        if (CaptureScript)
                        {
                            if (Path.GetExtension(ScriptFile.ToLower()) == ".tcl")
                            {
                                try
                                {
                                    var manager = new TclScripManager();
                                    manager.Execute(File.ReadAllText(ScriptFile));
                                }
                                catch (Exception exception)
                                {
                                    Log.Error("Script error", exception);
                                    StaticHelper.Instance.SystemMessage = "Script error :" + exception.Message;
                                }
                            }
                            else
                            {
                                var script = ServiceProvider.ScriptManager.Load(ScriptFile);
                                script.CameraDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
                                ServiceProvider.ScriptManager.Execute(script);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Timelapse error ", exception);
                        StaticHelper.Instance.SystemMessage = "Capture error";
                    }
                }
                if (CheckStop())
                {
                    IsActive = false;
                    if (!StartDaily)
                    {
                        IsRunning = false;
                        _timer.Stop();
                    }
                }
            }
        }

        private bool CheckStart()
        {
            if (StartNow)
                return true;
            if (StartIn)
            {
                var t = new TimeSpan(StartHour, StartMinute, StartSecond);
                if ((DateTime.Now - _timeLapseStartTime).TotalSeconds > t.TotalSeconds)
                    return true;
            }
            if (StartAt)
            {
                if (StartDate < DateTime.Now)
                    return true;
            }
            if (StartDaily)
            {
                if (_timeLapseStartTime > DateTime.Now)
                    return false;
                int day = (int)DateTime.Now.DayOfWeek;
                bool shoulContinue = (StartDay0 && day == 0) || (StartDay1 && day == 1) || (StartDay2 && day == 2) ||
                                     (StartDay3 && day == 3) || (StartDay4 && day == 4) || (StartDay5 && day == 5) ||
                                     (StartDay6 && day == 6);
                var t = new TimeSpan(StartHour, StartMinute, StartSecond);
                var t1 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                if (t1 > t && shoulContinue)
                    return true;
            }
            return false;
        }

        private bool CheckStop()
        {
            var res = false;
            if (StopAtPhotos)
                res = _totalCaptures >= StopCaptureCount;
            if (StopIn)
            {
                var t = new TimeSpan(StopHour, StopMinute, StopSecond);
                var now = DateTime.Now;
                var endt = new DateTime(now.Year, now.Month, now.Day, StartHour, StartMinute, StartSecond);
                if (StartDaily)
                {
                    if ((DateTime.Now - endt).TotalSeconds > t.TotalSeconds)
                        res = true;
                }
                else
                {
                    if ((DateTime.Now - _firstLapseStartTime).TotalSeconds > t.TotalSeconds)
                        res = true;
                }
            }
            if (StopAt)
            {
                if (StopDate < DateTime.Now)
                    res = true;
            }
            if (IsActive && res && StartDaily)
            {
                _timeLapseStartTime = _timeLapseStartTime.AddDays(1);
                _totalCaptures = 0;
            }
            return res;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                StartL();
            }
            else
            {
                StopL();
            }
        }

        private void StartL()
        {
            if (IsRunning)
                return;
            if (_bracketingViewModel != null)
                _bracketingViewModel.Stop();
            _bracketingViewModel = null;
            IsRunning = true;
            _timeLapseStartTime = DateTime.Now;
            _lastCaptureTime = DateTime.Now;
            Log.Debug("Timelapse start");
            _totalCaptures = 0;
            _timer.Interval = FullSpeed ? 100 : 1000;
            _timer.Start();
        }

        private void StopL()
        {
            if (!IsRunning)
                return;
            Log.Debug("Timelapse stop");
            IsActive = false;
            IsRunning = false;
            _timer.Stop();            
        }
    }
}

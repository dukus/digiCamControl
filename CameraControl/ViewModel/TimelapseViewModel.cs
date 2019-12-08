using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.TclScripting;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using Eagle._Containers.Public;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Quartz;
using Quartz.Impl;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.ViewModel
{
    public class TimelapseViewModel : ViewModelBase
    {
        private Timer _timer = new Timer(1000);
        private DateTime _lastTime;
        private bool _isRunning;
        private bool _isActive;
        private int _totalCaptures;
        private DateTime _lastCaptureTime = DateTime.Now;
        private DateTime _timeLapseStartTime = DateTime.Now;
        private DateTime _firstLapseStartTime = DateTime.Now;
        private bool _fullSpeed;
        private BracketingViewModel _bracketingViewModel = null;
        private double _timeDiff;
        private ITrigger trigger;
        private IScheduler sched;

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
                StopAtPhotos = false;
                StopAt = false;
                StopIn = true;
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
                TimeLapseSettings.StartDate =
                    TimeLapseSettings.StartDate =
                        new DateTime(value.Year, value.Month, value.Day, d.Hour, d.Minute, d.Second);
                RaisePropertyChanged(() => StartDate);
            }
        }

        public TimeSpan StartTime
        {
            get
            {
                return new TimeSpan(TimeLapseSettings.StartDate.Hour, TimeLapseSettings.StartDate.Minute,
                    TimeLapseSettings.StartDate.Second);
            }
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
            get { return !StartDaily; }
        }

        public DateTime StopDate
        {
            get { return TimeLapseSettings.StopDate; }
            set
            {
                var d = TimeLapseSettings.StopDate;
                TimeLapseSettings.StopDate =
                    TimeLapseSettings.StopDate =
                        new DateTime(value.Year, value.Month, value.Day, d.Hour, d.Minute, d.Second);
                RaisePropertyChanged(() => StopDate);
            }
        }

        public TimeSpan StopTime
        {
            get
            {
                return new TimeSpan(TimeLapseSettings.StopDate.Hour, TimeLapseSettings.StopDate.Minute,
                    TimeLapseSettings.StopDate.Second);
            }
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
            get
            {
                return !IsRunning ? TranslationStrings.ButtonStartTimeLapse : TranslationStrings.ButtonStopTimeLapse;
            }
        }

        public string StatusText
        {
            get
            {
                if (trigger != null)
                {

                    var tempTrigger = sched.GetTrigger(trigger.Key);
                    var nextFireTimeUtc = tempTrigger?.Result.GetNextFireTimeUtc();
                    if (nextFireTimeUtc != null)
                        return "Next capture time " + nextFireTimeUtc.Value.ToLocalTime();
                    else
                    {

                    }
                }
                return "????";

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
                            : string.Format("The timlapse will start in {0} seconds",
                                TimeSpan.FromSeconds(sec).ToString(@"hh\:mm\:ss"));
                    }
                    if (StartAt)
                    {
                        return StartDate < DateTime.Now
                            ? ""
                            : string.Format("The timlapse will start in {0}",
                                (StartDate - DateTime.Now).ToString(@"hh\:mm\:ss"));
                    }
                    return "Waiting for shedule";
                }
                return FullSpeed
                    ? string.Format("Timelapse in progress.\nTotal captured photos {0}", _totalCaptures)
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
                case CmdConsts.All_Close:
                    sched?.Shutdown();
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
            var tempTrigger = sched.GetTrigger(trigger.Key);
            if (tempTrigger == null)
                StopL();
            RaisePropertyChanged(() => StatusText);
        }

        public void Start()
        {
            try
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
            catch (Exception ex)
            {
                Log.Error("Error starting timelapse", ex);
                MessageBox.Show("Error starting timelapse "+ex.Message);
            }
        }

        private void StartL()
        {
            if (IsRunning)
                return;
            try
            {

                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler, start the schedular before triggers or anything else
                if (sched == null)
                {
                    sched = schedFact.GetScheduler().Result;
                }

                // create job
                IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // create trigger

                var triggerB = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1");

                if (StartIn)
                {
                    triggerB =
                        triggerB.StartAt(DateBuilder.FutureDate(
                            (StartHour * 60 * 60) + (StartMinute * 60) + StartSecond,
                            IntervalUnit.Second));

                }

                if (StartAt)
                {
                    triggerB =
                        triggerB.StartAt(DateBuilder.DateOf(StartHour, StartMinute, StartSecond, StartDate.Day,
                            StartDate.Month, StartDate.Year));
                }
                if (StartDaily)
                {
                    ISet<DayOfWeek> days = new HashSet<DayOfWeek>();
                    if (StartDay0)
                        days.Add(DayOfWeek.Sunday);
                    if (StartDay1)
                        days.Add(DayOfWeek.Monday);
                    if (StartDay2)
                        days.Add(DayOfWeek.Tuesday);
                    if (StartDay3)
                        days.Add(DayOfWeek.Wednesday);
                    if (StartDay4)
                        days.Add(DayOfWeek.Thursday);
                    if (StartDay5)
                        days.Add(DayOfWeek.Friday);
                    if (StartDay6)
                        days.Add(DayOfWeek.Saturday);

                    triggerB =
                        triggerB.WithDailyTimeIntervalSchedule(
                            x =>
                                x.WithIntervalInSeconds(TimeBetweenShots)
                                    .WithMisfireHandlingInstructionFireAndProceed()
                                    .StartingDailyAt(new TimeOfDay(StartHour, StartMinute, StartSecond))
                                    .EndingDailyAt(new TimeOfDay(StopHour, StopMinute, StopSecond))
                                    .OnDaysOfTheWeek(new ReadOnlyCollection<DayOfWeek>(days.ToList()))
                        );
                }
                else
                {

                    if (StopAtPhotos)
                    {
                        triggerB =
                            triggerB.WithSimpleSchedule(
                                x =>
                                    x.WithIntervalInSeconds(TimeBetweenShots)
                                        .WithRepeatCount(StopCaptureCount)
                                        .WithMisfireHandlingInstructionNowWithExistingCount());
                    }
                    else if (StopIn)
                    {
                        triggerB =
                            triggerB.WithSimpleSchedule(
                                x =>
                                    x.WithIntervalInSeconds(TimeBetweenShots)
                                        .WithMisfireHandlingInstructionNowWithExistingCount()
                                        .WithRepeatCount(((StopHour * 60 * 60) + (StopMinute * 60) + StopSecond) /
                                                         TimeBetweenShots));
                    }
                    else
                    {
                        triggerB =
                            triggerB.WithSimpleSchedule(
                                x =>
                                    x.WithIntervalInSeconds(TimeBetweenShots)
                                        .WithMisfireHandlingInstructionNowWithExistingCount()
                                        .RepeatForever());
                    }

                    if (StopAt)
                    {
                        triggerB =
                            triggerB.EndAt(DateBuilder.DateOf(StopHour, StopMinute, StopSecond, StopDate.Day,
                                StopDate.Month,
                                StopDate.Year));
                    }
                }

                trigger = triggerB.Build();


                // Schedule the job using the job and trigger 
                sched.ScheduleJob(job, trigger);
                sched.Start();

                if (_bracketingViewModel != null)
                    _bracketingViewModel.Stop();
                _bracketingViewModel = null;
                IsRunning = true;
                _timeLapseStartTime = DateTime.Now;
                _lastCaptureTime = DateTime.Now;
                Log.Debug("Timelapse start");
                _totalCaptures = 0;
                _timer.Interval = 1000;
                _timer.Start();
                _lastTime = DateTime.Now;
                TimeLapseSettings.Started = true;
                ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to start timelapse " + e.Message);
                Log.Debug("Unable to start timelapse ", e);
            }
        }

        private void StopL()
        {
            if (!IsRunning)
                return;

            sched.Standby();
            sched.Clear();

            Log.Debug("Timelapse stop");
            IsActive = false;
            IsRunning = false;
            _timer.Stop();
            TimeLapseSettings.Started = false;
            ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
        }
    }

    /// <summary>
    /// SimpleJOb is just a class that implements IJOB interface. It implements just one method, Execute method
    /// </summary>
    public class SimpleJob : IJob
    {
        

    public Task Execute(IJobExecutionContext context)
    {
            if (!ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy)
            {

                try
                {
                    if (ServiceProvider.Settings.DefaultSession.TimeLapseSettings.Capture)
                    {
                        if (ServiceProvider.Settings.DefaultSession.TimeLapseSettings.Bracketing)
                        {
                            var _bracketingViewModel = new BracketingViewModel();
                            Task.Factory.StartNew(new Action(_bracketingViewModel.Start));
                            StaticHelper.Instance.SystemMessage = _bracketingViewModel.Error;
                        }
                        else
                        {
                            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.Capture);
                        }
                    }
                    if (ServiceProvider.Settings.DefaultSession.TimeLapseSettings.CaptureAll)
                        ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.CaptureAll);
                    if (ServiceProvider.Settings.DefaultSession.TimeLapseSettings.CaptureScript)
                    {
                        if (Path.GetExtension(ServiceProvider.Settings.DefaultSession.TimeLapseSettings.ScriptFile.ToLower()) == ".tcl")
                        {
                            try
                            {
                                var manager = new TclScripManager();
                                manager.Execute(File.ReadAllText(ServiceProvider.Settings.DefaultSession.TimeLapseSettings.ScriptFile));
                            }
                            catch (Exception exception)
                            {
                                Log.Error("Script error", exception);
                                StaticHelper.Instance.SystemMessage = "Script error :" + exception.Message;
                            }
                        }
                        else
                        {
                            var script = ServiceProvider.ScriptManager.Load(ServiceProvider.Settings.DefaultSession.TimeLapseSettings.ScriptFile);
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

            //throw new NotImplementedException();
                Console.WriteLine("Hello, JOb executed");
            return Task.FromResult(0);
        }
    }
}

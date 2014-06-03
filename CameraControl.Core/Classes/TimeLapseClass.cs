using System;
using System.Threading;
using System.Timers;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Timer = System.Timers.Timer;

namespace CameraControl.Core.Classes
{
    public class TimeLapseClass : BaseFieldClass
    {
        public event EventHandler TimeLapseDone;

        private Timer _timer = new Timer(1000);
        private int _timecounter = 0;

        private int _period;
        public int Period
        {
            get { return _period; }
            set
            {
                _period = value;
                _timePeriod = new DateTime().AddSeconds(Period * NumberOfPhotos);
                NotifyPropertyChanged("TimePeriod");
                NotifyPropertyChanged("Period");
                NotifyPropertyChanged("Message");
            }
        }

        private int _numberOfPhotos;
        public int NumberOfPhotos
        {
            get
            {
                if (_numberOfPhotos < 1)
                    _numberOfPhotos = 0;
                return
                  _numberOfPhotos;
            }
            set
            {
                _numberOfPhotos = value;
                // prevent argument error
                if (_numberOfPhotos > 1000000 || _numberOfPhotos < 0)
                    _numberOfPhotos = 100;
                _timePeriod = new DateTime().AddSeconds(Period * _numberOfPhotos);
                if (Fps > 0)
                {
                    _movieLength = new DateTime().AddSeconds(_numberOfPhotos / _fps);
                    NotifyPropertyChanged("MovieLength");
                }
                NotifyPropertyChanged("TimePeriod");
                NotifyPropertyChanged("NumberOfPhotos");
                NotifyPropertyChanged("Message");
            }
        }

        private int _photosTaken;

        private VideoType _videoType;
        public VideoType VideoType
        {
            get
            {
                if (_videoType == null || _videoType.Width == 0)
                {
                    if (ServiceProvider.Settings != null && ServiceProvider.Settings.VideoTypes.Count > 0)
                        _videoType = ServiceProvider.Settings.VideoTypes[0];
                }
                return _videoType;
            }
            set
            {
                _videoType = value;
                NotifyPropertyChanged("VideoType");
            }
        }

        private DateTime _timePeriod;
        public DateTime TimePeriod
        {
            get { return _timePeriod; }
            set
            {
                _timePeriod = value;
                if ((_timePeriod.Ticks / TimeSpan.TicksPerSecond) > 1)
                {
                    double dd = (_timePeriod.Ticks / TimeSpan.TicksPerSecond);
                    _numberOfPhotos = (int)((_timePeriod.Ticks / TimeSpan.TicksPerSecond) / Period);
                    NotifyPropertyChanged("TimePeriod");
                }
                NotifyPropertyChanged("NumberOfPhotos");
                NotifyPropertyChanged("Message");
            }
        }

        private DateTime _movieLength;
        public DateTime MovieLength
        {
            get { return _movieLength; }
            set
            {
                _movieLength = value;
                _numberOfPhotos = (int)((_movieLength.Ticks / TimeSpan.TicksPerSecond) * Fps);
                NotifyPropertyChanged("NumberOfPhotos");
                NotifyPropertyChanged("MovieLength");
            }
        }


        private string _outputFIleName;
        public string OutputFIleName
        {
            get
            {
                return _outputFIleName;
            }
            set
            {
                _outputFIleName = value;
                NotifyPropertyChanged("OutputFIleName");
            }
        }

        private int _fps;
        public int Fps
        {
            get { return _fps; }
            set
            {
                _fps = value;
                if (_fps > 0 && _numberOfPhotos > 0)
                {
                    _movieLength = new DateTime().AddSeconds(_numberOfPhotos / _fps);
                    NotifyPropertyChanged("MovieLength");
                }

                NotifyPropertyChanged("MovieLength");
                NotifyPropertyChanged("Fps");
            }
        }

        private bool _noAutofocus;
        public bool NoAutofocus
        {
            get { return _noAutofocus; }
            set
            {
                _noAutofocus = value;
                NotifyPropertyChanged("NoAutofocus");
            }
        }

        public string Message
        {
            get
            {
                string res =
                    string.Format("Timelapse will run {0} \n Will end at {1} \n", PhotoUtils.DateTimeToString(TimePeriod),
                                  DateTime.Now.AddSeconds(Period * (_numberOfPhotos - PhotosTaken)));
                if (!IsDisabled)
                    res += PhotosTaken == 0
                               ? string.Format("Time Lapse will start in {0} second(s)",
                                               (Period - _timecounter))
                               : string.Format(
                                   "Next Time Lapse photo will be taken in  {0} second(s). Total photos :{1}/{2}",
                                   (Period - _timecounter), PhotosTaken, NumberOfPhotos);
                return res;
            }
        }

        [XmlIgnore]
        public int PhotosTaken
        {
            get { return _photosTaken; }
            set
            {
                _photosTaken = value;
                NotifyPropertyChanged("PhotosTaken");
            }
        }

        private bool _isDisabled;

        [XmlIgnore]
        public bool IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                _isDisabled = value;
                NotifyPropertyChanged("IsDisabled");
            }
        }

        private bool _fillImage;
        public bool FillImage
        {
            get { return _fillImage; }
            set
            {
                _fillImage = value;
                NotifyPropertyChanged("FillImage");
            }
        }

        private bool _virtualMove;
        public bool VirtualMove
        {
            get { return _virtualMove; }
            set
            {
                _virtualMove = value;
                NotifyPropertyChanged("VirtualMove");
            }
        }

        private int _movePercent;
        public int MovePercent
        {
            get { return _movePercent; }
            set
            {
                _movePercent = value;
                NotifyPropertyChanged("MovePercent");
            }
        }

        private int _moveDirection;
        public int MoveDirection
        {
            get { return _moveDirection; }
            set
            {
                _moveDirection = value;
                NotifyPropertyChanged("MoveDirection");
            }
        }

        private int _moveAlignment;
        public int MoveAlignment
        {
            get { return _moveAlignment; }
            set
            {
                _moveAlignment = value;
                NotifyPropertyChanged("MoveAlignment");
            }
        }

        public int ProgresPictures
        {
            get
            {
                if (NumberOfPhotos != 0)
                {
                    return (int)((double)PhotosTaken / NumberOfPhotos * 100);
                }
                return 0;
            }
        }

        public int ProgresTime
        {
            get
            {
                if (Period != 0)
                    return (int)((double)_timecounter / Period * 100);
                return 0;
            }
        }

        public TimeLapseClass()
        {
            Period = 5;
            NumberOfPhotos = 100;
            PhotosTaken = 0;
            IsDisabled = true;
            VideoType = new VideoType("", 0, 0);
            Fps = 15;
            FillImage = false;
            VirtualMove = false;
            MovePercent = 10;
            MoveDirection = 0;
            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timecounter++;
            // prevent counter underflow 
            if (_timecounter > Period)
                _timecounter = Period;
            
            if (_timecounter >= Period && !ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy)
            {
                _timecounter = 0;
                new Thread(Capture).Start();
                PhotosTaken++;
                if (PhotosTaken >= NumberOfPhotos)
                {
                    Stop();
                }
            }
            NotifyPropertyChanged("ProgresPictures");
            NotifyPropertyChanged("ProgresTime");
            NotifyPropertyChanged("Message");
        }


        private void Capture()
        {
            try
            {
                WaitForReady(ServiceProvider.DeviceManager.SelectedCameraDevice);
                if (NoAutofocus && ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureNoAf))
                    CameraHelper.CaptureNoAf(ServiceProvider.DeviceManager.SelectedCameraDevice);
                else
                    CameraHelper.Capture(ServiceProvider.DeviceManager.SelectedCameraDevice);
                //_timer.Enabled = true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                StaticHelper.Instance.SystemMessage = exception.Message;
            }
        }

        //public TimeLapseClass Copy()
        //{
        //  TimeLapseClass timeLapseClass = new TimeLapseClass {NumberOfPhotos = this.NumberOfPhotos, Period = this.Period};
        //  return timeLapseClass;
        //}

        public void Start()
        {
            NotifyPropertyChanged("Message");
            PhotosTaken = 0;
            _timer.AutoReset = true;
            IsDisabled = false;
            ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            _timer.Start();
        }

        void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            //if (!IsDisabled)
            //  _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            IsDisabled = true;
            ServiceProvider.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;
            StaticHelper.Instance.SystemMessage = "Timelapse done";
            if (TimeLapseDone != null)
                TimeLapseDone(this, new EventArgs());
        }

        private void WaitForReady(ICameraDevice device)
        {
            while (device.IsBusy)
            {

            }
        }

    }
}

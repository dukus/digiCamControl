using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Vision.Motion;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Others;
using CameraControl.windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Color = System.Windows.Media.Color;
using HelpProvider = CameraControl.Classes.HelpProvider;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;

namespace CameraControl.ViewModel
{
    public class LiveViewViewModel : ViewModelBase
    {
        public static event EventHandler FocuseDone;

        private const int DesiredFrameRate = 20;
        private const int DesiredWebFrameRate = 5;

        private bool _operInProgress = false;
        private int _totalframes = 0;
        private DateTime _framestart;
        private int _retries = 0;
        private MotionDetector _detector;
        private DateTime _photoCapturedTime;
        private Timer _timer = new Timer(1000/DesiredFrameRate);
        private Timer _freezeTimer = new Timer();
        private Timer _focusStackingTimer = new Timer(1000);
        private Timer _restartTimer = new Timer(1000);
        private DateTime _restartTimerStartTime;
        private string _lastOverlay = string.Empty;

        private BackgroundWorker _worker = new BackgroundWorker();
        private bool _focusStackingPreview = false;
        private bool _focusIProgress = false;
        private int _focusStackinMode = 0;
        private WriteableBitmap _overlayImage = null;

        private ICameraDevice _cameraDevice;
        private CameraProperty _cameraProperty;
        private BitmapSource _bitmap;
        private int _fps;
        private bool _recording;
        private string _recButtonText;
        private int _gridType;
        private AsyncObservableCollection<string> _grids;
        private double _currentMotionIndex;
        private bool _triggerOnMotion;
        private PointCollection _luminanceHistogramPoints = null;
        private BitmapSource _preview;
        private bool _isBusy;
        private int _photoNo;
        private int _waitTime;
        private int _focusStep;
        private int _photoCount;
        private int _focusValue;
        private int _focusCounter;
        private string _counterMessage;
        private bool _freezeImage;
        private bool _lockA;
        private bool _lockB;
        private int _selectedFocusValue;
        private bool _delayedStart;
        private int _brightness;
        private bool _simpleFocus;
        private int _direction;
        private int _photoNumber;
        private bool _simpleManualFocus;
        private int _focusStepSize;
        private PointCollection _redColorHistogramPoints;
        private PointCollection _greenColorHistogramPoints;
        private PointCollection _blueColorHistogramPoints;
        private AsyncObservableCollection<ValuePair> _overlays;
        private bool _overlayActivated;
        private int _overlayScale;
        private int _overlayHorizontal;
        private int _overlayVertical;
        private bool _stayOnTop;
        private int _focusStackingTick;
        private int _overlayTransparency;
        private bool _overlayUseLastCaptured;
        private int _captureDelay;
        private int _countDown;
        private bool _countDownVisible;
        private bool _captureInProgress;
        private bool _focusProgressVisible;
        private int _focusProgressMax;
        private int _focusProgressValue;
        private int _levelAngle;
        private string _levelAngleColor;
        private decimal _movieTimeRemain;
        private bool _showLeftTab;
        private bool _noProcessing;

        public ICameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set
            {
                _cameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }

        public CameraProperty CameraProperty
        {
            get { return _cameraProperty; }
            set
            {
                _cameraProperty = value;
                RaisePropertyChanged(() => CameraProperty);
            }
        }

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public int LevelAngle
        {
            get { return _levelAngle; }
            set
            {
                _levelAngle = value;
                RaisePropertyChanged(()=>LevelAngle);
                LevelAngleColor = _levelAngle % 90 <= 1 || _levelAngle % 90 >= 89 ? "Green" : "Red";
            }
        }

        public string LevelAngleColor
        {
            get { return _levelAngleColor; }
            set
            {
                _levelAngleColor = value;
                RaisePropertyChanged(() => LevelAngleColor);
            }
        }

        public int Fps
        {
            get { return _fps; }
            set
            {
                _fps = value;
                RaisePropertyChanged(() => Fps);
            }
        }

        public string RecButtonText
        {
            get { return _recButtonText; }
            set
            {
                _recButtonText = value;
                RaisePropertyChanged(() => RecButtonText);
            }
        }

        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                RecButtonText = _recording
                    ? TranslationStrings.ButtonRecordStopMovie
                    : TranslationStrings.ButtonRecordMovie;
                RaisePropertyChanged(() => Recording);
            }
        }

        public decimal MovieTimeRemain
        {
            get { return _movieTimeRemain; }
            set
            {
                _movieTimeRemain = value;
                RaisePropertyChanged(() => MovieTimeRemain);
            }
        }


        public int GridType
        {
            get { return _gridType; }
            set
            {
                _gridType = value;
                RaisePropertyChanged(() => GridType);
            }
        }

        public AsyncObservableCollection<string> Grids
        {
            get { return _grids; }
            set
            {
                _grids = value;
                RaisePropertyChanged(() => Grids);
            }
        }

        public AsyncObservableCollection<ValuePair> Overlays
        {
            get { return _overlays; }
            set
            {
                _overlays = value;
                RaisePropertyChanged(() => Overlays);
            }
        }

        public string SelectedOverlay
        {
            get { return CameraProperty.LiveviewSettings.SelectedOverlay; }
            set
            {
                CameraProperty.LiveviewSettings.SelectedOverlay = value;
                _overlayImage = null;
                RaisePropertyChanged(() => SelectedOverlay);
            }
        }

        public bool OverlayActivated
        {
            get { return _overlayActivated; }
            set
            {
                _overlayActivated = value;
                RaisePropertyChanged(() => OverlayActivated);
            }
        }

        public int OverlayScale
        {
            get { return _overlayScale; }
            set
            {
                _overlayScale = value;
                RaisePropertyChanged(()=>OverlayScale);
            }
        }

        public int OverlayHorizontal
        {
            get { return _overlayHorizontal; }
            set
            {
                _overlayHorizontal = value;
                RaisePropertyChanged(()=>OverlayHorizontal);
            }
        }

        public int OverlayVertical
        {
            get { return _overlayVertical; }
            set
            {
                _overlayVertical = value;
                RaisePropertyChanged(()=>OverlayVertical);
            }
        }

        public int OverlayTransparency
        {
            get { return _overlayTransparency; }
            set
            {
                _overlayTransparency = value;
                RaisePropertyChanged(()=>OverlayTransparency);
            }
        }

        public bool OverlayUseLastCaptured
        {
            get { return _overlayUseLastCaptured; }
            set
            {
                _overlayUseLastCaptured = value;
                RaisePropertyChanged(() => OverlayUseLastCaptured);
            }
        }


        public bool BlackAndWhite
        {
            get { return CameraProperty.LiveviewSettings.BlackAndWhite; }
            set
            {
                CameraProperty.LiveviewSettings.BlackAndWhite = value;
                RaisePropertyChanged(() => BlackAndWhite);
            }
        }

        public bool EdgeDetection
        {
            get { return CameraProperty.LiveviewSettings.EdgeDetection; }
            set
            {
                CameraProperty.LiveviewSettings.EdgeDetection = value;
                RaisePropertyChanged(() => EdgeDetection);
            }
        }

        public int RotationIndex
        {
            get { return CameraProperty.LiveviewSettings.RotationIndex; }
            set
            {
                CameraProperty.LiveviewSettings.RotationIndex = value;
                RaisePropertyChanged(() => RotationIndex);
            }
        }

        public bool FreezeImage
        {
            get { return _freezeImage; }
            set
            {
                _freezeImage = value;
                if (_freezeImage)
                    _freezeTimer.Start();
                RaisePropertyChanged(() => FreezeImage);

            }
        }

        public int Brightness
        {
            get { return CameraProperty.LiveviewSettings.Brightness; }
            set
            {
                CameraProperty.LiveviewSettings.Brightness = value;
                RaisePropertyChanged(() => Brightness);
            }
        }

        public int FocusStackingTick
        {
            get { return _focusStackingTick; }
            set
            {
                _focusStackingTick = value;
                RaisePropertyChanged(() => FocusStackingTick);
            }
        }

        public bool SimpleFocusStacking
        {
            get { return false; }
        }

        public bool AdvancexFocusStacking
        {
            get { return !SimpleFocusStacking; }
        }

        public bool ShowHistogram { get; set; }

        #region motion detection

        public double CurrentMotionIndex
        {
            get { return _currentMotionIndex; }
            set
            {
                _currentMotionIndex = value;
                RaisePropertyChanged(() => CurrentMotionIndex);
            }
        }

        public int MotionThreshold
        {
            get { return CameraProperty.LiveviewSettings.MotionThreshold; }
            set
            {
                CameraProperty.LiveviewSettings.MotionThreshold = value;
                RaisePropertyChanged(() => MotionThreshold);
            }
        }

        public bool TriggerOnMotion
        {
            get { return _triggerOnMotion; }
            set
            {
                _triggerOnMotion = value;
                RaisePropertyChanged(() => TriggerOnMotion);
            }
        }

        public int WaitForMotionSec
        {
            get { return CameraProperty.LiveviewSettings.WaitForMotionSec; }
            set { CameraProperty.LiveviewSettings.WaitForMotionSec = value; }
        }

        public bool MotionAutofocusBeforCapture
        {
            get { return CameraProperty.LiveviewSettings.MotionAutofocusBeforCapture; }
            set { CameraProperty.LiveviewSettings.MotionAutofocusBeforCapture = value; }
        }

        public bool DetectMotion
        {
            get { return CameraProperty.LiveviewSettings.DetectMotion; }
            set { CameraProperty.LiveviewSettings.DetectMotion = value; }
        }

        public bool DetectMotionArea
        {
            get { return CameraProperty.LiveviewSettings.DetectMotionArea; }
            set
            {
                CameraProperty.LiveviewSettings.DetectMotionArea = value;
                if(_detector!=null)
                    _detector.Reset();
                if (value)
                    ShowRuler = true;
            }
        }

        #endregion

        #region histogram

        public PointCollection LuminanceHistogramPoints
        {
            get { return _luminanceHistogramPoints; }
            set
            {
                if (_luminanceHistogramPoints != value)
                {
                    _luminanceHistogramPoints = value;
                    RaisePropertyChanged(() => LuminanceHistogramPoints);
                }
            }
        }


        public PointCollection RedColorHistogramPoints
        {
            get { return _redColorHistogramPoints; }
            set
            {
                _redColorHistogramPoints = value;
                RaisePropertyChanged(()=>RedColorHistogramPoints);
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get { return _greenColorHistogramPoints; }
            set
            {
                _greenColorHistogramPoints = value;
                RaisePropertyChanged(()=>GreenColorHistogramPoints);
            }
        }

        public PointCollection BlueColorHistogramPoints
        {
            get { return _blueColorHistogramPoints; }
            set
            {
                _blueColorHistogramPoints = value;
                RaisePropertyChanged(() => BlueColorHistogramPoints);
            }
        }

        public bool HighlightOverExp
        {
            get { return CameraProperty.LiveviewSettings.HighlightOverExp; }
            set
            {
                CameraProperty.LiveviewSettings.HighlightOverExp = value;
                RaisePropertyChanged(() => HighlightOverExp);
            }
        }

        public bool HighlightUnderExp
        {
            get { return CameraProperty.LiveviewSettings.HighlightUnderExp; }
            set
            {
                CameraProperty.LiveviewSettings.HighlightUnderExp = value;
                RaisePropertyChanged(() => HighlightUnderExp);
            }
        }

        #endregion

        #region focus stacking

        public bool IsFocusStackingRunning
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsFocusStackingRunning);
                RaisePropertyChanged(() => IsFree);
                RaisePropertyChanged(() => FocusingEnabled);
            }
        }

        public bool IsFree
        {
            get { return !_isBusy && !CaptureInProgress; }
        }


        public int PhotoNo
        {
            get { return _photoNo; }
            set
            {
                _photoNo = value;
                RaisePropertyChanged(() => PhotoNo);
                if (PhotoNo > 0)
                    _focusStep =
                        Convert.ToInt32(Decimal.Round((decimal) FocusValue/PhotoNo, MidpointRounding.AwayFromZero));
                RaisePropertyChanged(() => FocusStep);
                RaisePropertyChanged(() => PhotoNo);
            }
        }

        public int PhotoNumber
        {
            get { return _photoNumber; }
            set
            {
                _photoNumber = value;
                RaisePropertyChanged(()=>PhotoCount);
            }
        }

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                _waitTime = value;
                RaisePropertyChanged(() => WaitTime);
            }
        }

        public int FocusStep
        {
            get { return _focusStep; }
            set
            {
                _focusStep = value;
                _photoNo = Convert.ToInt32(Decimal.Round((decimal) FocusValue/FocusStep, MidpointRounding.AwayFromZero));
                RaisePropertyChanged(() => FocusStep);
                RaisePropertyChanged(() => PhotoNo);
            }
        }

        public int FocusStepSize
        {
            get { return _focusStepSize; }
            set
            {
                _focusStepSize = value;
                RaisePropertyChanged(() => FocusStepSize);
            }
        }


        public int PhotoCount
        {
            get { return _photoCount; }
            set
            {
                _photoCount = value;
                RaisePropertyChanged(() => PhotoCount);
            }
        }

        public int Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                RaisePropertyChanged(() => Direction);
            }
        }


        /// <summary>
        /// Gets or sets the current focus counter.
        /// </summary>
        /// <value>
        /// The focus counter.
        /// </value>
        public int FocusCounter
        {
            get { return _focusCounter; }
            set
            {
                _selectedFocusValue = value;
                _focusCounter = value;

                RaisePropertyChanged(() => FocusCounter);
                RaisePropertyChanged(() => CounterMessage);
                RaisePropertyChanged(() => SelectedFocusValue);
            }
        }

        /// <summary>
        /// Gets or sets the maximum locked focus value.
        /// </summary>
        /// <value>
        /// The focus value.
        /// </value>
        public int FocusValue
        {
            get { return _focusValue; }
            set
            {
                _focusValue = value;
                if (FocusStep > 0)
                    PhotoNo = FocusValue/FocusStep;
                RaisePropertyChanged(() => FocusValue);
                RaisePropertyChanged(() => CounterMessage);
            }
        }

        public int SelectedFocusValue
        {
            get { return _selectedFocusValue; }
            set
            {
                var newvalue = value;
                if (_selectedFocusValue != newvalue)
                {
                    SetFocus(newvalue - _selectedFocusValue);
                    _selectedFocusValue = value;
                    RaisePropertyChanged(() => SelectedFocusValue);
                }
            }
        }


        public string CounterMessage
        {
            get
            {
                if (!LockA && !LockB)
                    return "?";
                if (LockA && !LockB)
                    return FocusCounter.ToString();
                if (LockB)
                    return FocusCounter + "/" + FocusValue;
                return _counterMessage;
            }
            set
            {
                _counterMessage = value;
                RaisePropertyChanged(() => CounterMessage);
            }
        }

        public bool LockA
        {
            get { return _lockA; }
            set
            {
                _lockA = value;
                if (_lockA && !LockB)
                {
                    FocusCounter = 0;
                    FocusValue = 0;
                    LockB = false;
                }
                if (_lockA && LockB)
                {
                    FocusValue = FocusValue - FocusCounter;
                    FocusCounter = 0;
                }
                if (!_lockA)
                {
                    LockB = false;
                }
                RaisePropertyChanged(() => LockA);
                RaisePropertyChanged(() => CounterMessage);
            }
        }

        public bool LockB
        {
            get { return _lockB; }
            set
            {
                if (FocusCounter == 0 && value)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelErrorFarPoit);
                    return;
                }
                _lockB = value;
                if (_lockB)
                {
                    FocusValue = FocusCounter;
                    PhotoCount = 5;
                }
                RaisePropertyChanged(() => LockB);
                RaisePropertyChanged(() => CounterMessage);
            }
        }

        public bool FocusingEnabled
        {
            get { return !IsFocusStackingRunning && !_focusIProgress; }
        }

        #endregion

        #region view

        public bool ShowFocusRect
        {
            get { return CameraProperty.LiveviewSettings.ShowFocusRect; }
            set
            {
                CameraProperty.LiveviewSettings.ShowFocusRect = value;
                RaisePropertyChanged(() => ShowFocusRect);
            }
        }

        public bool ShowLeftTab
        {
            get { return CameraProperty.LiveviewSettings.ShowLeftTab; }
            set
            {
                CameraProperty.LiveviewSettings.ShowLeftTab = value;
                RaisePropertyChanged(() => ShowLeftTab);
            }
        }

        public bool NoProcessing
        {
            get { return CameraProperty.LiveviewSettings.NoProcessing; }
            set
            {
                CameraProperty.LiveviewSettings.NoProcessing = value;
                RaisePropertyChanged(() => NoProcessing);
                RaisePropertyChanged(() => NoProcessingWarnColor);
            }
        }

        public string NoProcessingWarnColor
        {
            get { return CameraProperty.LiveviewSettings.NoProcessing ? "Red" : "Transparent"; }
        }


        /// <summary>
        /// Gets or sets a value indicating if restart timer is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delayed start]; otherwise, <c>false</c>.
        /// </value>
        public bool DelayedStart
        {
            get { return _delayedStart; }
            set
            {
                _delayedStart = value;
                RaisePropertyChanged(() => DelayedStart);
            }
        }

        #endregion
        public BitmapSource Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                RaisePropertyChanged(() => Preview);
            }
        }

        public bool SimpleManualFocus
        {
            get { return _simpleManualFocus; }
            set
            {
                _simpleManualFocus = value;
                RaisePropertyChanged(()=>SimpleManualFocus);
            }
        }

        public bool StayOnTop
        {
            get { return _stayOnTop; }
            set
            {
                _stayOnTop = value;
                RaisePropertyChanged(() => StayOnTop);
            }
        }

        public int CaptureDelay
        {
            get { return _captureDelay; }
            set
            {
                _captureDelay = value;
                RaisePropertyChanged(() => CaptureDelay);
            }
        }

        public int CountDown
        {
            get { return _countDown; }
            set
            {
                _countDown = value;
                RaisePropertyChanged(() => CountDown);
            }
        }

        public bool CountDownVisible
        {
            get { return _countDownVisible; }
            set
            {
                _countDownVisible = value;
                RaisePropertyChanged(()=>CountDownVisible);
            }
        }

        public bool CaptureInProgress
        {
            get { return _captureInProgress; }
            set
            {
                _captureInProgress = value;
                RaisePropertyChanged(() => CaptureInProgress);
                RaisePropertyChanged(() => IsFree);
            }
        }


        public LiveViewData LiveViewData { get; set; }

        #region ruler

        public int HorizontalMin
        {
            get { return CameraProperty.LiveviewSettings.HorizontalMin; }
            set { CameraProperty.LiveviewSettings.HorizontalMin = value; }
        }

        public int HorizontalMax
        {
            get { return  CameraProperty.LiveviewSettings.HorizontalMax; }
            set {  CameraProperty.LiveviewSettings.HorizontalMax = value; }
        }

        public int VerticalMin
        {
            get { return  CameraProperty.LiveviewSettings.VerticalMin; }
            set {  CameraProperty.LiveviewSettings.VerticalMin = value; }
        }

        public int VerticalMax
        {
            get { return  CameraProperty.LiveviewSettings.VerticalMax; }
            set {  CameraProperty.LiveviewSettings.VerticalMax = value; }
        }

        public bool ShowRuler
        {
            get { return CameraProperty.LiveviewSettings.ShowRuler; }
            set
            {
                CameraProperty.LiveviewSettings.ShowRuler = value;
                RaisePropertyChanged(() => ShowRuler);
            }
        }

        #endregion

        #region focus progress

        public bool FocusProgressVisible
        {
            get { return _focusProgressVisible; }
            set
            {
                _focusProgressVisible = value; 
                RaisePropertyChanged(()=>FocusProgressVisible);
            }
        }

        public int FocusProgressMax
        {
            get { return _focusProgressMax; }
            set
            {
                _focusProgressMax = value;
                RaisePropertyChanged(() => FocusProgressMax);
            }
        }

        public int FocusProgressValue
        {
            get { return _focusProgressValue; }
            set
            {
                _focusProgressValue = value;
                RaisePropertyChanged(() => FocusProgressValue);
            }
        }

        #endregion
        #region Commands

        public RelayCommand AutoFocusCommand { get; set; }
        public RelayCommand RecordMovieCommand { get; set; }
        public RelayCommand CaptureCommand { get; set; }
        public RelayCommand FocusPCommand { get; set; }
        public RelayCommand FocusPPCommand { get; set; }
        public RelayCommand FocusPPPCommand { get; set; }
        public RelayCommand FocusMCommand { get; set; }
        public RelayCommand FocusMMCommand { get; set; }
        public RelayCommand FocusMMMCommand { get; set; }
        public RelayCommand MoveACommand { get; set; }
        public RelayCommand MoveBCommand { get; set; }
        public RelayCommand StartFocusStackingCommand { get; set; }
        public RelayCommand PreviewFocusStackingCommand { get; set; }
        public RelayCommand StopFocusStackingCommand { get; set; }
        
        public RelayCommand StartSimpleFocusStackingCommand { get; set; }
        public RelayCommand PreviewSimpleFocusStackingCommand { get; set; }
        public RelayCommand StopSimpleFocusStackingCommand { get; set; }

        public RelayCommand StartLiveViewCommand { get; set; }
        public RelayCommand StopLiveViewCommand { get; set; }

        public RelayCommand ResetBrigthnessCommand { get; set; }
        public RelayCommand BrowseOverlayCommand { get; set; }
        public RelayCommand HelpFocusStackingCommand { get; set; }

        public RelayCommand ResetOverlayCommand { get; set; }

        public RelayCommand ZoomOutCommand { get; set; }
        public RelayCommand ZoomInCommand { get; set; }
        public RelayCommand ZoomIn100 { get; set; }
        public RelayCommand ToggleGridCommand { get; set; }
        
        #endregion

        public LiveViewViewModel()
        {
            CameraProperty = new CameraProperty();
            CameraDevice = new NotConnectedCameraDevice();
            InitCommands();
        }

        public LiveViewViewModel(ICameraDevice device)
        {
            CameraDevice = device;
            CameraProperty = device.LoadProperties();
            SimpleManualFocus = CameraDevice.GetCapability(CapabilityEnum.SimpleManualFocus);
            InitOverlay();
            InitCommands();
            if (ServiceProvider.Settings.DetectionType == 0)
            {
                _detector = new MotionDetector(
                    new TwoFramesDifferenceDetector(true),
                    new BlobCountingObjectsProcessing(
                        ServiceProvider.Settings.MotionBlockSize,
                        ServiceProvider.Settings.MotionBlockSize, true));
            }
            else
            {
                _detector = new MotionDetector(
                    new SimpleBackgroundModelingDetector(true, true),
                    new BlobCountingObjectsProcessing(
                        ServiceProvider.Settings.MotionBlockSize,
                        ServiceProvider.Settings.MotionBlockSize, true));
            }
            
            TriggerOnMotion = false;
            Init();
        }

        private void InitCommands()
        {
            AutoFocusCommand = new RelayCommand(AutoFocus);
            RecordMovieCommand = new RelayCommand(RecordMovie,
                () => CameraDevice.GetCapability(CapabilityEnum.RecordMovie));
            CaptureCommand = new RelayCommand(CaptureInThread);
            FocusMCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? -ServiceProvider.Settings.SmallFocusStepCanon : -ServiceProvider.Settings.SmalFocusStep));
            FocusMMCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? -ServiceProvider.Settings.MediumFocusStepCanon : -ServiceProvider.Settings.MediumFocusStep));
            FocusMMMCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? -ServiceProvider.Settings.LargeFocusStepCanon : -ServiceProvider.Settings.LargeFocusStep));
            FocusPCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? ServiceProvider.Settings.SmallFocusStepCanon : ServiceProvider.Settings.SmalFocusStep));
            FocusPPCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? ServiceProvider.Settings.MediumFocusStepCanon : ServiceProvider.Settings.MediumFocusStep));
            FocusPPPCommand = new RelayCommand(() => SetFocus(SimpleManualFocus ? ServiceProvider.Settings.LargeFocusStepCanon : ServiceProvider.Settings.LargeFocusStep));
            MoveACommand = new RelayCommand(() => SetFocus(-FocusCounter));
            MoveBCommand = new RelayCommand(() => SetFocus(FocusValue));
            StartFocusStackingCommand = new RelayCommand(StartFocusStacking, () => LockB);
            PreviewFocusStackingCommand = new RelayCommand(PreviewFocusStacking, () => LockB);
            StopFocusStackingCommand = new RelayCommand(StopFocusStacking);
            StartLiveViewCommand = new RelayCommand(StartLiveView);
            StopLiveViewCommand = new RelayCommand(StopLiveView);
            ResetBrigthnessCommand = new RelayCommand(() => Brightness = 0);

            StartSimpleFocusStackingCommand = new RelayCommand(StartSimpleFocusStacking);
            PreviewSimpleFocusStackingCommand = new RelayCommand(PreviewSimpleFocusStacking);
            StopSimpleFocusStackingCommand = new RelayCommand(StopFocusStacking);
            HelpFocusStackingCommand = new RelayCommand(()=> HelpProvider.Run(HelpSections.FocusStacking));

            BrowseOverlayCommand = new RelayCommand(BrowseOverlay);
            ResetOverlayCommand = new RelayCommand(ResetOverlay);
            ZoomInCommand = new RelayCommand(() => CameraDevice.LiveViewImageZoomRatio.NextValue());
            ZoomOutCommand = new RelayCommand(() => CameraDevice.LiveViewImageZoomRatio.PrevValue());
            ZoomIn100 = new RelayCommand(ToggleZoom);
            ToggleGridCommand = new RelayCommand(ToggleGrid);
            FocuseDone += LiveViewViewModel_FocuseDone;
            
        }

        private void ToggleGrid()
        {
            var i = GridType;
            i++;
            if (i >= Grids.Count)
                i = 0;
            GridType = i;
        }

        private void ToggleZoom()
        {
            try
            {
                if (CameraDevice.LiveViewImageZoomRatio == null || CameraDevice.LiveViewImageZoomRatio.Values == null ||
                    CameraDevice.LiveViewImageZoomRatio.Values.Count < 2)
                    return;
                CameraDevice.LiveViewImageZoomRatio.Value = CameraDevice.LiveViewImageZoomRatio.Value ==
                                                            CameraDevice.LiveViewImageZoomRatio.Values[0]
                    ? CameraDevice.LiveViewImageZoomRatio.Values[CameraDevice.LiveViewImageZoomRatio.Values.Count - 2]
                    : CameraDevice.LiveViewImageZoomRatio.Values[0];
            }
            catch (Exception ex)
            {
                Log.Error("Unable to set zoom", ex);
            }
        }

        private void InitOverlay()
        {
            Overlays = new AsyncObservableCollection<ValuePair>();
            Grids = new AsyncObservableCollection<string>();
            Grids.Add(TranslationStrings.LabelNone);
            Grids.Add(TranslationStrings.LabelRuleOfThirds);
            Grids.Add(TranslationStrings.LabelComboGrid);
            Grids.Add(TranslationStrings.LabelDiagonal);
            Grids.Add(TranslationStrings.LabelSplit);
            if (Directory.Exists(ServiceProvider.Settings.OverlayFolder))
            {
                string[] files = Directory.GetFiles(ServiceProvider.Settings.OverlayFolder, "*.png");
                foreach (string file in files)
                {
                    Overlays.Add(new ValuePair() {Name = Path.GetFileNameWithoutExtension(file), Value = file});
                }
            }
            OverlayTransparency = 100;
            OverlayUseLastCaptured = false;
        }

        private void Init()
        {
            WaitTime = 2;
            PhotoNo = 2;
            FocusStep = 2;
            PhotoCount = 5;
            DelayedStart = false;
            _timer.Stop();
            _timer.AutoReset = true;
            CameraDevice.CameraDisconnected += CameraDeviceCameraDisconnected;
            _photoCapturedTime = DateTime.Now;
            CameraDevice.PhotoCaptured += CameraDevicePhotoCaptured;
            StartLiveView();
            _freezeTimer.Interval = ServiceProvider.Settings.LiveViewFreezeTimeOut*1000;
            _freezeTimer.Elapsed += _freezeTimer_Elapsed;
            _timer.Elapsed += _timer_Elapsed;
            _worker.DoWork += delegate
            {
                if (!FreezeImage)
                    GetLiveImage();
            };
            ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            _focusStackingTimer.AutoReset = true;
            _focusStackingTimer.Elapsed += _focusStackingTimer_Elapsed;
            _restartTimer.AutoReset = true;
            _restartTimer.Elapsed += _restartTimer_Elapsed;
        }

        private void _restartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - _restartTimerStartTime).TotalSeconds > 2)
            {
                _restartTimer.Stop();
                StartLiveView();
                DelayedStart = false;
            }
        }

        public void UnInit()
        {
            _timer.Stop();
            _focusStackingTimer.Stop();
            _restartTimer.Stop();
            CameraDevice.PhotoCaptured -= CameraDevicePhotoCaptured; 
            Thread.Sleep(100);
            StopLiveView();
            Recording = false;
            LockA = false;
            LockB = false;
            LiveViewData = null;
        }

        private void ResetOverlay()
        {
            OverlayHorizontal = 0;
            OverlayScale = 1;
            OverlayTransparency = 100;
            OverlayVertical = 0;
        }

        public void BrowseOverlay()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Png file(*.png)|*.png|All files|*.*";
            dlg.FileName = SelectedOverlay;
            if (dlg.ShowDialog() == true)
            {
                Overlays.Add(new ValuePair()
                {
                    Name = Path.GetFileNameWithoutExtension(dlg.FileName),
                    Value = dlg.FileName
                });
                SelectedOverlay = dlg.FileName;
            }   
        }


        public void WindowsManager_Event(string cmd, object o)
        {
            ICameraDevice device = o as ICameraDevice ?? ServiceProvider.DeviceManager.SelectedCameraDevice;
            if (device != CameraDevice)
                return;

            switch (cmd)
            {
                case CmdConsts.LiveView_Capture:
                    CaptureInThread();
                    break;
                case CmdConsts.LiveView_Focus_Move_Right:
                    if (LiveViewData != null && LiveViewData.ImageData != null)
                    {
                        SetFocusPos(LiveViewData.FocusX + ServiceProvider.Settings.FocusMoveStep, LiveViewData.FocusY);
                    }
                    break;
                case CmdConsts.LiveView_Focus_Move_Left:
                    if (LiveViewData != null && LiveViewData.ImageData != null)
                    {
                        SetFocusPos(LiveViewData.FocusX - ServiceProvider.Settings.FocusMoveStep, LiveViewData.FocusY);
                    }
                    break;
                case CmdConsts.LiveView_Focus_Move_Up:
                    if (LiveViewData != null && LiveViewData.ImageData != null)
                    {
                        SetFocusPos(LiveViewData.FocusX, LiveViewData.FocusY - ServiceProvider.Settings.FocusMoveStep);
                    }
                    break;
                case CmdConsts.LiveView_Focus_Move_Down:
                    if (LiveViewData != null && LiveViewData.ImageData != null)
                    {
                        SetFocusPos(LiveViewData.FocusX, LiveViewData.FocusY + ServiceProvider.Settings.FocusMoveStep);
                    }
                    break;
                case CmdConsts.LiveView_Zoom_All:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(0);
                    break;
                case CmdConsts.LiveView_Zoom_25:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(1);
                    break;
                case CmdConsts.LiveView_Zoom_33:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(2);
                    break;
                case CmdConsts.LiveView_Zoom_50:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(3);
                    break;
                case CmdConsts.LiveView_Zoom_66:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(4);
                    break;
                case CmdConsts.LiveView_Zoom_100:
                    CameraDevice.LiveViewImageZoomRatio.SetValue(5);
                    break;
                case CmdConsts.LiveView_Focus_M:
                    FocusMCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus_P:
                    FocusPCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus_MM:
                    FocusMMCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus_PP:
                    FocusPPCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus_MMM:
                    FocusMMMCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus_PPP:
                    FocusPPPCommand.Execute(null);
                    break;
                case CmdConsts.LiveView_Focus:
                    AutoFocus();
                    break;

            }
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (!_worker.IsBusy)
            //    _worker.RunWorkerAsync();
            //ThreadPool.QueueUserWorkItem(GetLiveImage);
            Task.Factory.StartNew(GetLiveImage);
        }

        void _freezeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FreezeImage = false;
        }

        private void CameraDevicePhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            _detector.Reset();
            _photoCapturedTime = DateTime.Now;
            if (IsFocusStackingRunning)
            {
                _focusStackingTimer.Start();
            }
            _timer.Start();
            StartLiveView();
        }

        private void CameraDeviceCameraDisconnected(object sender, DisconnectCameraEventArgs eventArgs)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide, CameraDevice);
        }

        private void AutoFocus()
        {
            if (LockA || LockB)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelErrorAutoFocusLock);
                return;
            }
            string resp = CameraDevice.GetProhibitionCondition(OperationEnum.AutoFocus);
            if (string.IsNullOrEmpty(resp))
            {
                Thread thread = new Thread(AutoFocusThread);
                thread.Start();
            }
            else
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, 
                    TranslationStrings.LabelErrorUnableFocus + "\n" +
                    TranslationManager.GetTranslation(resp));
            }
        }

        private void AutoFocusThread()
        {
            try
            {
                LockB = false;
                LockA = false;
                CameraDevice.AutoFocus();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to autofocus", exception);
                StaticHelper.Instance.SystemMessage = exception.Message;
            }
        }


        public virtual void GetLiveImage(object o)
        {
            GetLiveImage();
        }

        
        public virtual void GetLiveImage()
        {
            if (_operInProgress)
            {
                Log.Error("OperInProgress");
                return;
            }

            if (DelayedStart)
            {
                Log.Error("Start is delayed");
                return;
            }

            _operInProgress = true;
            _totalframes++;
            if ((DateTime.Now - _framestart).TotalSeconds > 0)
                Fps = (int) (_totalframes/(DateTime.Now - _framestart).TotalSeconds);
            try
            {
                LiveViewData = LiveViewManager.GetLiveViewImage(CameraDevice);
            }
            catch (Exception ex)
            {
                Log.Error("Error geting lv", ex);
                _retries++;
                _operInProgress = false;
                return;
            }

            if (LiveViewData == null )
            {
                _retries++;
                _operInProgress = false;
                return;
            }

            if (!LiveViewData.IsLiveViewRunning && !IsFocusStackingRunning)
            {
                DelayedStart = true;
                _restartTimerStartTime = DateTime.Now;
                _restartTimer. Start();
                _operInProgress = false;
                return;
            }

            if (LiveViewData.ImageData == null)
            {
                Log.Error("LV image data is null !");
                _retries++;
                _operInProgress = false;
                return;
            }

            Recording = LiveViewData.MovieIsRecording;
            try
            {
                WriteableBitmap preview;
                if (LiveViewData != null && LiveViewData.ImageData != null)
                {
                    MemoryStream stream = new MemoryStream(LiveViewData.ImageData,
                        LiveViewData.
                            ImageDataPosition,
                        LiveViewData.ImageData.
                            Length -
                        LiveViewData.
                            ImageDataPosition);
                    LevelAngle = (int) LiveViewData.LevelAngleRolling;
                    MovieTimeRemain = decimal.Round(LiveViewData.MovieTimeRemain, 2);

                    if (NoProcessing)
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = stream;
                        bi.EndInit();
                        bi.Freeze();
                        Bitmap = bi;
                        ServiceProvider.DeviceManager.LiveViewImage[CameraDevice] = stream.ToArray();
                        _operInProgress = false;
                        return;
                    }

                    using (var res = new Bitmap(stream))
                    {
                        Bitmap bmp = res;
                        if (DetectMotion)
                        {
                            ProcessMotionDetection(bmp);
                        }

                        if (_totalframes%DesiredFrameRate == 0 && ShowHistogram)
                        {
                            ImageStatisticsHSL hslStatistics =
                                new ImageStatisticsHSL(bmp);
                            LuminanceHistogramPoints =
                                ConvertToPointCollection(
                                    hslStatistics.Luminance.Values);
                            ImageStatistics statistics = new ImageStatistics(bmp);
                            RedColorHistogramPoints = ConvertToPointCollection(
                                statistics.Red.Values);
                            GreenColorHistogramPoints = ConvertToPointCollection(
                                statistics.Green.Values);
                            BlueColorHistogramPoints = ConvertToPointCollection(
                                statistics.Blue.Values);
                        }

                        if (HighlightUnderExp)
                        {
                            ColorFiltering filtering = new ColorFiltering();
                            filtering.Blue = new IntRange(0, 5);
                            filtering.Red = new IntRange(0, 5);
                            filtering.Green = new IntRange(0, 5);
                            filtering.FillOutsideRange = false;
                            filtering.FillColor = new RGB(System.Drawing.Color.Blue);
                            filtering.ApplyInPlace(bmp);
                        }

                        if (HighlightOverExp)
                        {
                            ColorFiltering filtering = new ColorFiltering();
                            filtering.Blue = new IntRange(250, 255);
                            filtering.Red = new IntRange(250, 255);
                            filtering.Green = new IntRange(250, 255);
                            filtering.FillOutsideRange = false;
                            filtering.FillColor = new RGB(System.Drawing.Color.Red);
                            filtering.ApplyInPlace(bmp);
                        }

                        if (Brightness != 0)
                        {
                            BrightnessCorrection filter = new BrightnessCorrection(Brightness);
                            bmp = filter.Apply(bmp);
                        }

                        preview =
                            BitmapFactory.ConvertToPbgra32Format(
                                BitmapSourceConvert.ToBitmapSource(bmp));
                        DrawFocusPoint(preview);
                        Bitmap newbmp = bmp;
                        if (EdgeDetection)
                        {
                            var filter = new FiltersSequence(
                                Grayscale.CommonAlgorithms.BT709,
                                new HomogenityEdgeDetector()
                                );
                            newbmp = filter.Apply(bmp);
                        }

                        WriteableBitmap writeableBitmap;

                        if (BlackAndWhite)
                        {
                            Grayscale filter = new Grayscale(0.299, 0.587, 0.114);
                            writeableBitmap =
                                BitmapFactory.ConvertToPbgra32Format(
                                    BitmapSourceConvert.ToBitmapSource(
                                        filter.Apply(newbmp)));
                        }
                        else
                        {
                            writeableBitmap =
                                BitmapFactory.ConvertToPbgra32Format(
                                    BitmapSourceConvert.ToBitmapSource(newbmp));
                        }
                        DrawGrid(writeableBitmap);
                        if (RotationIndex != 0)
                        {
                            switch (RotationIndex)
                            {
                                case 1:
                                    writeableBitmap = writeableBitmap.Rotate(90);
                                    break;
                                case 2:
                                    writeableBitmap = writeableBitmap.Rotate(180);
                                    break;
                                case 3:
                                    writeableBitmap = writeableBitmap.Rotate(270);
                                    break;
                                case 4:
                                    if (LiveViewData.Rotation != 0)
                                        writeableBitmap =
                                            writeableBitmap.RotateFree(
                                                LiveViewData.Rotation, false);
                                    break;
                            }
                        }
                        if (CameraDevice.LiveViewImageZoomRatio.Value == "All")
                        {
                            preview.Freeze();
                            Preview = preview;
                            if (ShowFocusRect)
                                DrawFocusPoint(writeableBitmap);
                        }

                        writeableBitmap.Freeze();
                        Bitmap = writeableBitmap;

                        //if (_totalframes%DesiredWebFrameRate == 0)
                        ServiceProvider.DeviceManager.LiveViewImage[CameraDevice] = SaveJpeg(writeableBitmap);
                        Log.Debug("Live view draw done");
                    }
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                _retries++;
                _operInProgress = false;
            }
            finally
            {
                _operInProgress = false;
            }
            _retries = 0;
            _operInProgress = false;
        }

        public byte[] SaveJpeg(WriteableBitmap image)
        {
            var enc = new JpegBitmapEncoder();
            enc.QualityLevel = 50;
            enc.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream stm = new MemoryStream())
            {
                enc.Save(stm);
                return stm.ToArray();
            }
        }

        private void DrawAngle(WriteableBitmap writeableBitmap)
        {
            int y = writeableBitmap.PixelHeight;
        }

        private void DrawGrid(WriteableBitmap writeableBitmap)
        {
            Color color = Colors.White;
            color.A = 50;
            
            if (OverlayActivated)
            {
                if ((SelectedOverlay != null && File.Exists(SelectedOverlay) )|| OverlayUseLastCaptured)
                {
                    if (OverlayUseLastCaptured)
                    {
                        if (File.Exists(ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb) &&
                            _lastOverlay != ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb)
                        {
                            _lastOverlay = ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb;
                            _overlayImage = null;
                        }
                    }

                    if (_overlayImage == null)
                    {
                        BitmapImage bitmapSource = new BitmapImage();
                        bitmapSource.DecodePixelWidth = writeableBitmap.PixelWidth;
                        bitmapSource.BeginInit();
                        bitmapSource.UriSource = new Uri(OverlayUseLastCaptured ? _lastOverlay : SelectedOverlay);
                        bitmapSource.EndInit();
                        _overlayImage = BitmapFactory.ConvertToPbgra32Format(bitmapSource);
                        _overlayImage.Freeze();
                    }
                    int x = writeableBitmap.PixelWidth * OverlayScale / 100;
                    int y = writeableBitmap.PixelHeight * OverlayScale / 100;
                    int xx = writeableBitmap.PixelWidth * OverlayHorizontal / 100;
                    int yy = writeableBitmap.PixelWidth * OverlayVertical / 100;
                    Color transpColor = Colors.White;

                    //set color transparency for blit only the alpha chanel is used from transpColor
                    if (OverlayTransparency < 100)
                        transpColor = Color.FromArgb((byte) (0xff*OverlayTransparency/100d), 0xff, 0xff, 0xff);
                    
                    writeableBitmap.Blit(
                        new Rect(0 + (x / 2) + xx, 0 + (y / 2) + yy, writeableBitmap.PixelWidth - x,
                            writeableBitmap.PixelHeight - y),
                        _overlayImage,
                        new Rect(0, 0, _overlayImage.PixelWidth, _overlayImage.PixelHeight),transpColor,WriteableBitmapExtensions.BlendMode.Alpha);
                }
            }

            switch (GridType)
            {
                case 1:
                {
                    for (int i = 1; i < 3; i++)
                    {
                        writeableBitmap.DrawLine(0, (int) ((writeableBitmap.Height/3)*i),
                            (int) writeableBitmap.Width,
                            (int) ((writeableBitmap.Height/3)*i), color);
                        writeableBitmap.DrawLine((int) ((writeableBitmap.Width/3)*i), 0,
                            (int) ((writeableBitmap.Width/3)*i),
                            (int) writeableBitmap.Height, color);
                    }
                    writeableBitmap.SetPixel((int) (writeableBitmap.Width/2), (int) (writeableBitmap.Height/2), 128,
                        Colors.Red);
                }
                    break;
                case 2:
                {
                    for (int i = 1; i < 10; i++)
                    {
                        writeableBitmap.DrawLine(0, (int) ((writeableBitmap.Height/10)*i),
                            (int) writeableBitmap.Width,
                            (int) ((writeableBitmap.Height/10)*i), color);
                        writeableBitmap.DrawLine((int) ((writeableBitmap.Width/10)*i), 0,
                            (int) ((writeableBitmap.Width/10)*i),
                            (int) writeableBitmap.Height, color);
                    }
                    writeableBitmap.SetPixel((int) (writeableBitmap.Width/2), (int) (writeableBitmap.Height/2), 128,
                        Colors.Red);
                }
                    break;
                case 3:
                {
                    writeableBitmap.DrawLineDDA(0, 0, (int) writeableBitmap.Width,
                        (int) writeableBitmap.Height, color);

                    writeableBitmap.DrawLineDDA(0, (int) writeableBitmap.Height,
                        (int) writeableBitmap.Width, 0, color);
                    writeableBitmap.SetPixel((int) (writeableBitmap.Width/2), (int) (writeableBitmap.Height/2), 128,
                        Colors.Red);
                }
                    break;
                case 4:
                {
                    writeableBitmap.DrawLineDDA(0, (int) (writeableBitmap.Height/2), (int) writeableBitmap.Width,
                        (int) (writeableBitmap.Height/2), color);

                    writeableBitmap.DrawLineDDA((int) (writeableBitmap.Width/2), 0,
                        (int) (writeableBitmap.Width/2), (int) writeableBitmap.Height, color);
                    writeableBitmap.SetPixel((int) (writeableBitmap.Width/2), (int) (writeableBitmap.Height/2), 128,
                        Colors.Red);
                }
                    break;
                default:
                    break;
            }

            if (ShowRuler)
            {
                int x1 = writeableBitmap.PixelWidth*HorizontalMin/100;
                int x2 = writeableBitmap.PixelWidth*HorizontalMax/100;
                int y2 = writeableBitmap.PixelHeight*(100-VerticalMin)/100;
                int y1 = writeableBitmap.PixelHeight*(100-VerticalMax)/100;

                FillRectangle2(writeableBitmap, 0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, Color.FromArgb(128, 128, 128, 128));
                FillRectangleDeBlend(writeableBitmap, x1, y1, x2, y2, Color.FromArgb(128, 128, 128, 128));
                writeableBitmap.DrawRectangle( x1, y1, x2, y2, color);

            }

        }

        public static void FillRectangleDeBlend(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                unsafe
                {


                    // Use refs for faster access (really important!) speeds up a lot!
                    int w = bmp.PixelWidth;
                    int h = bmp.PixelHeight;
                    var pixels = context.Pixels;

                    // Check boundaries
                    if (x1 < 0)
                    {
                        x1 = 0;
                    }
                    if (y1 < 0)
                    {
                        y1 = 0;
                    }
                    if (x2 < 0)
                    {
                        x2 = 0;
                    }
                    if (y2 < 0)
                    {
                        y2 = 0;
                    }
                    if (x1 >= w)
                    {
                        x1 = w - 1;
                    }
                    if (y1 >= h)
                    {
                        y1 = h - 1;
                    }
                    if (x2 >= w)
                    {
                        x2 = w - 1;
                    }
                    if (y2 >= h)
                    {
                        y2 = h - 1;
                    }


                    unchecked
                    {
                        for (int y = y1; y <= y2; y++)
                        {
                            for (int i = y*w + x1; i < y*w + x2; i++)
                            {
                                byte oneOverAlpha = (byte) (255 - color.A);
                                int c = pixels[i];

                                int r = (((byte) (c >> 16) << 8)/oneOverAlpha);
                                int g = (((byte) (c >> 8) << 8)/oneOverAlpha);
                                int b = (((byte) (c >> 0) << 8)/oneOverAlpha);

                                pixels[i] = 255 << 24 | r << 16 | g << 8 | b;


                            }
                        }
                    }

                }
            }
        }

        public static void FillRectangle2(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                unsafe
                {
                    // Use refs for faster access (really important!) speeds up a lot!
                    int w = bmp.PixelWidth;
                    int h = bmp.PixelHeight;
                    var pixels = context.Pixels;

                    // Check boundaries
                    if (x1 < 0)
                    {
                        x1 = 0;
                    }
                    if (y1 < 0)
                    {
                        y1 = 0;
                    }
                    if (x2 < 0)
                    {
                        x2 = 0;
                    }
                    if (y2 < 0)
                    {
                        y2 = 0;
                    }
                    if (x1 >= w)
                    {
                        x1 = w - 1;
                    }
                    if (y1 >= h)
                    {
                        y1 = h - 1;
                    }
                    if (x2 >= w)
                    {
                        x2 = w - 1;
                    }
                    if (y2 >= h)
                    {
                        y2 = h - 1;
                    }

                    unchecked
                    {
                        for (int y = y1; y <= y2; y++)
                        {
                            for (int i = y*w + x1; i <= y*w + x2; i++)
                            {
                                byte oneOverAlpha = (byte) (255 - color.A);
                                int c = pixels[i];

                                int r = ((byte) (c >> 16)*oneOverAlpha ) >> 8;
                                int g = ((byte) (c >> 8)*oneOverAlpha ) >> 8;
                                int b = ((byte) (c >> 0)*oneOverAlpha ) >> 8;

                                pixels[i] = 255 << 24 | r << 16 | g << 8 | b;
                            }
                        }
                    }
                }
            }
        }

        private Color IncreaseColor(Color c, byte inc)
        {
            c.R += inc;
            c.G += inc;
            c.B += inc;
            return c;
        }


        private void DrawFocusPoint(WriteableBitmap bitmap)
        {
            try
            {
                if (LiveViewData == null)
                    return;
                double xt = bitmap.Width / LiveViewData.ImageWidth;
                double yt = bitmap.Height / LiveViewData.ImageHeight;

                bitmap.DrawRectangle((int) (LiveViewData.FocusX*xt - (LiveViewData.FocusFrameXSize*xt/2)),
                    (int) (LiveViewData.FocusY*yt - (LiveViewData.FocusFrameYSize*yt/2)),
                    (int) (LiveViewData.FocusX*xt + (LiveViewData.FocusFrameXSize*xt/2)),
                    (int) (LiveViewData.FocusY*yt + (LiveViewData.FocusFrameYSize*yt/2)),
                    LiveViewData.HaveFocusData ? Colors.Green : Colors.Red);
            }
            catch (Exception exception)
            {
                Log.Error("Error draw helper lines", exception);
            }
        }


        private void ProcessMotionDetection(Bitmap bmp)
        {
            try
            {
                float movement = 0;
                if (DetectMotionArea)
                {
                    int x1 = bmp.Width * HorizontalMin / 100;
                    int x2 = bmp.Width * HorizontalMax / 100;
                    int y2 = bmp.Height * (100 - VerticalMin) / 100;
                    int y1 = bmp.Height * (100 - VerticalMax) / 100;
                    using (
                        var cropbmp = new Bitmap(bmp.Clone(new Rectangle(x1, y1, (x2 - x1), (y2 - y1)), bmp.PixelFormat))
                        )
                    {
                        cropbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                        movement = _detector.ProcessFrame(cropbmp);

                        using (var currentTileGraphics = Graphics.FromImage(bmp))
                        {
                            currentTileGraphics.DrawImage(cropbmp, x1, y1);
                        }
                    }
                }
                else
                {
                    movement = _detector.ProcessFrame(bmp);    
                }
               
                CurrentMotionIndex = Math.Round(movement*100, 2);
                if (movement > ((float) MotionThreshold/100) && TriggerOnMotion &&
                    (DateTime.Now - _photoCapturedTime).TotalSeconds > WaitForMotionSec)
                {
                    if (MotionAutofocusBeforCapture)
                    {
                        BlobCountingObjectsProcessing processing =
                            _detector.MotionProcessingAlgorithm as BlobCountingObjectsProcessing;
                        if (processing != null && processing.ObjectRectangles != null &&
                            processing.ObjectRectangles.Length > 0 &&
                            LiveViewData.ImageData != null)
                        {
                            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle();
                            int surface = 0;
                            foreach (System.Drawing.Rectangle objectRectangle in processing.ObjectRectangles)
                            {
                                if (surface < objectRectangle.Width*objectRectangle.Height)
                                {
                                    surface = objectRectangle.Width*objectRectangle.Height;
                                    rectangle = objectRectangle;
                                }
                            }
                            double xt = LiveViewData.ImageWidth/(double) bmp.Width;
                            double yt = LiveViewData.ImageHeight/(double) bmp.Height;
                            int posx = (int) ((rectangle.X + (rectangle.Width/2))*xt);
                            int posy = (int) ((rectangle.Y + (rectangle.Height/2))*yt);
                            CameraDevice.Focus(posx, posy);
                        }
                        AutoFocus();
                    }
                    CameraDevice.CapturePhotoNoAf();
                    _detector.Reset();
                    _photoCapturedTime = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Motion detection error ", exception);
            }
        }

        private PointCollection ConvertToPointCollection(int[] values)
        {
            int max = values.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(values.Length - 1, max));
            points.Freeze();
            return points;
        }

        private void RecordMovie()
        {
            string resp = Recording ? "" : CameraDevice.GetProhibitionCondition(OperationEnum.RecordMovie);
            if (string.IsNullOrEmpty(resp))
            {
                var thread = new Thread(RecordMovieThread);
                thread.Start();
            }
            else
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, 
                    TranslationStrings.LabelErrorRecordMovie + "\n" +
                    TranslationManager.GetTranslation(resp));
            }
        }

        private void RecordMovieThread()
        {
            try
            {
                if (Recording)
                {
                    CameraDevice.StopRecordMovie();
                }
                else
                {
                    CameraDevice.StartRecordMovie();
                }
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Recording error", exception);
            }
        }

        private void StartLiveView()
        {
            string resp = CameraDevice.GetProhibitionCondition(OperationEnum.LiveView);
            if (string.IsNullOrEmpty(resp))
            {
                Thread thread = new Thread(StartLiveViewThread);
                thread.Start();
                thread.Join();
            }
            else
            {
                Log.Error("Error starting live view " + resp);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, 
                    TranslationStrings.LabelLiveViewError + "\n" +
                    TranslationManager.GetTranslation(resp));
            }
        }

        private void StartLiveViewThread()
        {
            try
            {
                _totalframes = 0;
                _framestart = DateTime.Now;
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview started");
                do
                {
                    try
                    {
                        LiveViewManager.StartLiveView(CameraDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(100);
                            Log.Debug("Retry live view :" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                } while (retry && retryNum < 35);
                _timer.Start();
                _operInProgress = false;
                _retries = 0;
                Log.Debug("LiveView: Liveview start done");
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to start liveview ! " + exception.Message;
            }
        }

        private void StopLiveView()
        {
            Thread thread = new Thread(StopLiveViewThread);
            thread.Start();
        }

        private void StopLiveViewThread()
        {
            try
            {
                _totalframes = 0;
                _framestart = DateTime.Now;
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview stopping");
                do
                {
                    try
                    {
                        LiveViewManager.StopLiveView(CameraDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(500);
                            Log.Debug("Retry live view stop:" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                } while (retry && retryNum < 35);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to stop liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to stop liveview ! " + exception.Message;
            }
        }

        private void SetFocus(int step)
        {
            //if (step == 0)
            //    return;

            if (_focusIProgress)
            {
                //SelectedFocusValue = FocusCounter;
                return;
            }
            _focusIProgress = true;
            RaisePropertyChanged(() => FocusingEnabled);
            try
            {
                string resp = CameraDevice.GetProhibitionCondition(OperationEnum.ManualFocus);
                if (string.IsNullOrEmpty(resp))
                {
                    Thread thread = new Thread(SetFocusThread);
                    thread.Start(step);
                    //thread.Join();
                }
                else
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, 
                                          TranslationStrings.LabelErrorUnableFocus + "\n" +
                                          TranslationManager.GetTranslation(resp));
                    _focusIProgress = false;
                    RaisePropertyChanged(() => FocusingEnabled);
                    SelectedFocusValue = FocusCounter;
                }
            }
            catch (Exception exception)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelErrorUnableFocus);
                Log.Error("Unable to focus ", exception);
                _focusIProgress = false;
                RaisePropertyChanged(() => FocusingEnabled);
                SelectedFocusValue = FocusCounter;
            }
        }

        private void SetFocusThread(object ostep)
        {
            int step = (int)ostep;
            if (step != 0)
            {
                if (LockA)
                {
                    if (FocusCounter == 0 && step < 0)
                        return;
                    if (FocusCounter + step < 0)
                        step = -FocusCounter;
                }
                if (LockB)
                {
                    if (FocusCounter + step > FocusValue)
                        step = FocusValue - FocusCounter;
                }

                try
                {
                    _timer.Stop();
                    CameraDevice.StartLiveView();
                    StaticHelper.Instance.SystemMessage = "Move focus " + step;
                    if (SimpleManualFocus)
                    {
                        FocusProgressMax = Math.Abs(step);
                        FocusProgressValue = 0;
                        FocusProgressVisible = true;

                        for (var i = 0; i < Math.Abs(step); i++)
                        {
                            FocusProgressValue ++;
                            FocusCounter += CameraDevice.Focus(step);
                            GetLiveImage();
                            Thread.Sleep(ServiceProvider.Settings.CanonFocusStepWait);
                        }
                        FocusProgressVisible = false;
                    }
                    else
                    {
                        FocusCounter += CameraDevice.Focus(step);
                    }
                }
                catch (DeviceException exception)
                {
                    Log.Error("Unable to focus", exception);
                    StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus + " " +
                                                          exception.Message;
                }
                catch (Exception exception)
                {
                    Log.Error("Unable to focus", exception);
                    StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus;
                }
            }

            _focusIProgress = false;
            _selectedFocusValue = FocusCounter;
            OnFocuseDone();
            RaisePropertyChanged(() => FocusingEnabled);
            RaisePropertyChanged(() => SelectedFocusValue);

            if (!IsFocusStackingRunning)
                _timer.Start();

        }

        private void Capture()
        {
            CaptureInProgress = true;
            Log.Debug("LiveView: Capture started");
            if (CaptureDelay > 0)
            {
                Log.Debug("LiveView: Capture delayed");
                CountDown = CaptureDelay;
                CountDownVisible = true;
                while (CountDown>0)
                {
                    Thread.Sleep(1000);
                    CountDown--;
                }
                CountDownVisible = false;
            }
            _timer.Stop();
            Thread.Sleep(300);
            try
            {
                if (CameraDevice.ShutterSpeed != null && CameraDevice.ShutterSpeed.Value == "Bulb")
                {
                    StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.MsgBulbModeNotSupported);
                    CaptureInProgress = false;
                    return;
                }
                CameraDevice.CapturePhotoNoAf();
                Log.Debug("LiveView: Capture Initialization Done");
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Unable to take picture with no af", exception);
            }
            CaptureInProgress = false;
        }

        public void SetFocusPos(Point initialPoint, double refWidth, double refHeight)
        {
            if (LiveViewData != null)
            {
                double xt = LiveViewData.ImageWidth/refWidth;
                double yt = LiveViewData.ImageHeight/refHeight;
                int posx = (int) (initialPoint.X*xt);
                int posy = (int) (initialPoint.Y*yt);
                Task.Factory.StartNew(() => SetFocusPos(posx, posy));
            }
        }

        private void SetFocusPos(int x, int y)
        {
            try
            {
                CameraDevice.Focus(x, y);
            }
            catch (Exception exception)
            {
                Log.Error("Error set focus pos :", exception);
                StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorSetFocusPos;
            }
        }

        private void StartSimpleFocusStacking()
        {
            if (LockA || LockB)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelErrorSimpleStackingFocusLock);
                return;
            }
            LockA = false;
            _focusStackinMode = 1;
            FocusStackingTick = 0;
            _focusIProgress = false;
            PhotoCount = 0;
            IsFocusStackingRunning = true;
            _focusStackingPreview = false;
            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.NextSeries);
            _focusStackingTimer.Start();
            //Thread thread = new Thread(TakePhoto);
            //thread.Start();
        }


        private void StartFocusStacking()
        {
            if (!LockA || !LockB)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelLockNearFar);
                return;
            }
            _focusStackinMode = 0;
            FocusStackingTick = 0;
            _focusIProgress = false;
            GetLiveImage();
            PhotoCount = 0;
            IsFocusStackingRunning = true;
            _focusStackingPreview = false;
            // increment defauls session series counter
            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.NextSeries);
            SetFocus(-FocusCounter);
            //_focusStackingTimer.Start();
            //Thread thread = new Thread(TakePhoto);
            //thread.Start();
        }

        private void PreviewSimpleFocusStacking()
        {
            LockA = false;
            _focusStackinMode = 1;
            PhotoCount = 0;
            IsFocusStackingRunning = true;
            _focusStackingPreview = true;
            _focusStackingTimer.Start();
        }

        private void PreviewFocusStacking()
        {
            _focusStackinMode = 0;
            //FreezeImage = true;
            GetLiveImage();
            PhotoCount = 0;
            IsFocusStackingRunning = true;
            _focusStackingPreview = true;
            SetFocus(-FocusCounter);
            //_focusStackingTimer.Start();
        }

        private void StopFocusStacking()
        {
            IsFocusStackingRunning = false;
            FocusStackingTick = 0;
            _focusStackingTimer.Stop();
            _timer.Start();
        }


        void _focusStackingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsFocusStackingRunning)
                return;
            if (_focusStackinMode == 0)
            {
                if (FocusStackingTick > WaitTime)
                {
                    _focusStackingTimer.Stop();
                    StartLiveView();
                    if (PhotoCount > 0)
                    {
                        SetFocus(FocusStep);
                    }
                }
            }
            else
            {
                if (FocusStackingTick > WaitTime)
                {
                    _focusStackingTimer.Stop();
                    StartLiveView();
                    if (PhotoCount > 0)
                    {
                        int dir = Direction == 0 ? -1 : 1;
                        switch (FocusStepSize)
                        {
                            case 0:
                                SetFocus(dir * (SimpleManualFocus ? ServiceProvider.Settings.SmallFocusStepCanon : ServiceProvider.Settings.SmalFocusStep));
                                break;
                            case 1:
                                SetFocus(dir * (SimpleManualFocus ? ServiceProvider.Settings.MediumFocusStepCanon : ServiceProvider.Settings.MediumFocusStep));
                                break;
                            case 2:
                                SetFocus(dir * (SimpleManualFocus ? ServiceProvider.Settings.LargeFocusStepCanon : ServiceProvider.Settings.LargeFocusStep));
                                break;
                        }
                    }
                    else
                    {
                        LiveViewViewModel_FocuseDone(null, null);
                    }
                }
            }
            FocusStackingTick++;
        }

        private void CaptureInThread()
        {
            var thread = new Thread(Capture);
            thread.Start();
            //thread.Join();
        }

        private void LiveViewViewModel_FocuseDone(object sender, EventArgs e)
        {
            StaticHelper.Instance.SystemMessage = "";
            if (IsFocusStackingRunning)
            {
                Recording = false;
                try
                {
                    if (!_focusStackingPreview)
                        Capture();
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    _focusStackingTimer.Start();
                    return;
                }
                if (_focusStackinMode == 0)
                {
                    PhotoCount++;
                    GetLiveImage();
                    FocusStackingTick = 0;
                    if (FocusCounter >= FocusValue)
                    {
                        IsFocusStackingRunning = false;
                    }
                }
                else
                {
                    PhotoCount++;
                    if (PhotoCount >= PhotoNumber)
                    {
                        IsFocusStackingRunning = false;
                    }
                    GetLiveImage();
                    FocusStackingTick = 0;
                }

                if (_focusStackingPreview && IsFocusStackingRunning)
                {
                    _focusStackingTimer.Start();
                }
            }

        }

        private static void OnFocuseDone()
        {
            EventHandler handler = FocuseDone;
            if (handler != null) handler(null, EventArgs.Empty);
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using Microsoft.Win32;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;

namespace CameraControl.ViewModel
{
    public class LiveViewViewModel : ViewModelBase
    {
        public static event EventHandler FocuseDone;

        private const int DesiredFrameRate = 20;

        private bool _operInProgress = false;
        private int _totalframes = 0;
        private DateTime _framestart;
        private MotionDetector _detector;
        private DateTime _photoCapturedTime = DateTime.MinValue;
        private Timer _timer = new Timer(1000 / DesiredFrameRate);
        private Timer _freezeTimer = new Timer();
        private Timer _focusStackingTimer = new Timer(1000);
        private Timer _restartTimer = new Timer(1000);
        private DateTime _restartTimerStartTime;
        private string _lastOverlay = string.Empty;

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
        private int _soundL;
        private int _soundR;
        private int _peakSoundL;
        private int _peakSoundR;
        private bool _haveSoundData;
        private bool _settingArea;
        private string _title;
        private int _rotation;
        private int _captureCount;
        private bool _autoFocusBeforCapture;


        public Rect RullerRect
        {
            get { return new Rect(HorizontalMin, VerticalMin, HorizontalMax, VerticalMax); }
        }

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
                RaisePropertyChanged(() => LevelAngle);
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

        public int SoundL
        {
            get { return _soundL; }
            set
            {
                _soundL = value;
                RaisePropertyChanged(()=>SoundL);
            }
        }

        public int SoundR
        {
            get { return _soundR; }
            set
            {
                _soundR = value;
                RaisePropertyChanged(() => SoundR);
            }
        }

        public int PeakSoundL
        {
            get { return _peakSoundL; }
            set
            {
                _peakSoundL = value;
                RaisePropertyChanged(() => PeakSoundL);
            }
        }

        public int PeakSoundR
        {
            get { return _peakSoundR; }
            set
            {
                _peakSoundR = value;
                RaisePropertyChanged(() => PeakSoundR);
            }
        }

        public bool HaveSoundData
        {
            get { return _haveSoundData; }
            set
            {
                _haveSoundData = value;
                RaisePropertyChanged(() => HaveSoundData);
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
                RaisePropertyChanged(() => OverlayScale);
            }
        }

        public int OverlayHorizontal
        {
            get { return _overlayHorizontal; }
            set
            {
                _overlayHorizontal = value;
                RaisePropertyChanged(() => OverlayHorizontal);
            }
        }

        public int OverlayVertical
        {
            get { return _overlayVertical; }
            set
            {
                _overlayVertical = value;
                RaisePropertyChanged(() => OverlayVertical);
            }
        }

        public int OverlayTransparency
        {
            get { return _overlayTransparency; }
            set
            {
                _overlayTransparency = value;
                RaisePropertyChanged(() => OverlayTransparency);
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
                {
                    _freezeTimer.Stop();
                    _freezeTimer.Start();
                }
                RaisePropertyChanged(() => FreezeImage);

            }
        }

        public int Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                RaisePropertyChanged(() => Rotation);
            }
        }

        public int PreviewTime
        {
            get { return CameraProperty.LiveviewSettings.PreviewTime; }
            set
            {
                CameraProperty.LiveviewSettings.PreviewTime = value;
                RaisePropertyChanged(() => PreviewTime);
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

        public int CropRatio
        {
            get { return CameraProperty.LiveviewSettings.CropRatio; }
            set { CameraProperty.LiveviewSettings.CropRatio = value; }
        }

        public int CropOffsetX { get; set; }
        
        public int CropOffsetY { get; set; }
        
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
        public bool CaptureCancelRequested { get; set; }

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
            set
            {
                CameraProperty.LiveviewSettings.DetectMotion = value;
                RaisePropertyChanged(()=>DetectMotion);
            }
        }

        public bool DetectMotionArea
        {
            get { return CameraProperty.LiveviewSettings.DetectMotionArea; }
            set
            {
                CameraProperty.LiveviewSettings.DetectMotionArea = value;
                if (_detector != null)
                    _detector.Reset();
                if (value)
                    ShowRuler = true;
            }
        }

        public bool FlipImage
        {
            get { return CameraProperty.LiveviewSettings.FlipImage; }
            set { CameraProperty.LiveviewSettings.FlipImage = value; }
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
                RaisePropertyChanged(() => RedColorHistogramPoints);
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get { return _greenColorHistogramPoints; }
            set
            {
                _greenColorHistogramPoints = value;
                RaisePropertyChanged(() => GreenColorHistogramPoints);
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
                        Convert.ToInt32(Decimal.Round((decimal)FocusValue / PhotoNo, MidpointRounding.AwayFromZero));
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
                RaisePropertyChanged(() => PhotoCount);
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
                _photoNo = Convert.ToInt32(Decimal.Round((decimal)FocusValue / FocusStep, MidpointRounding.AwayFromZero));
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
                    PhotoNo = FocusValue / FocusStep;
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
                if (LockB )
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
                //if (_lockA && LockB)
                //{
                //    FocusValue = FocusValue - FocusCounter;
                //    FocusCounter = 0;
                //}
                //if (!_lockA)
                //{
                //    LockB = false;
                //}
                RaisePropertyChanged(() => LockA);
                RaisePropertyChanged(() => CounterMessage);
            }
        }

        public bool LockB
        {
            get { return _lockB; }
            set
            {
                //if (FocusCounter == 0 && value)
                //{
                //    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.LabelErrorFarPoit);
                //    return;
                //}
                _lockB = value;
                if (_lockB && LockA)
                {
                    FocusValue = FocusCounter;
                    PhotoCount = 5;
                }
                if (!_lockA && LockB)
                {
                    FocusCounter = 0;
                    FocusValue = 0;
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
                RaisePropertyChanged(() => SimpleManualFocus);
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

        public int CaptureCount
        {
            get { return _captureCount; }
            set
            {
                _captureCount = value;
                RaisePropertyChanged(() => CaptureCount);
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

        public bool AutoFocusBeforCapture
        {
            get { return _autoFocusBeforCapture; }
            set
            {
                _autoFocusBeforCapture = value;
                RaisePropertyChanged(() => AutoFocusBeforCapture);
            }
        }

        public bool CountDownVisible
        {
            get { return _countDownVisible; }
            set
            {
                _countDownVisible = value;
                RaisePropertyChanged(() => CountDownVisible);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
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
            set
            {
                CameraProperty.LiveviewSettings.HorizontalMin = value;
                RaisePropertyChanged(() => RullerRect);
            }
        }

        public int HorizontalMax
        {
            get { return CameraProperty.LiveviewSettings.HorizontalMax; }
            set
            {
                CameraProperty.LiveviewSettings.HorizontalMax = value;
                RaisePropertyChanged(() => RullerRect);
            }
        }

        public int VerticalMin
        {
            get { return CameraProperty.LiveviewSettings.VerticalMin; }
            set
            {
                CameraProperty.LiveviewSettings.VerticalMin = value;
                RaisePropertyChanged(() => RullerRect);
            }
        }

        public int VerticalMax
        {
            get { return CameraProperty.LiveviewSettings.VerticalMax; }
            set
            {
                CameraProperty.LiveviewSettings.VerticalMax = value;
                RaisePropertyChanged(() => RullerRect);
            }
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

        public bool SettingArea
        {
            get { return _settingArea; }
            set
            {
                _settingArea = value;
                RaisePropertyChanged(() => SettingArea);
                RaisePropertyChanged(() => NoSettingArea);
            }
        }

        public bool NoSettingArea
        {
            get { return !SettingArea; }
        }

        #endregion

        #region focus progress

        public bool FocusProgressVisible
        {
            get { return _focusProgressVisible; }
            set
            {
                _focusProgressVisible = value;
                RaisePropertyChanged(() => FocusProgressVisible);
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
        public RelayCommand CancelCaptureCommand { get; set; }

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

        public RelayCommand SetAreaCommand { get; set; }
        public RelayCommand DoneSetAreaCommand { get; set; }

        public RelayCommand LockCurrentNearCommand { get; set; }
        public RelayCommand LockCurrentFarCommand { get; set; }

        #endregion

        public bool IsActive { get; set; }

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
            Title = TranslationStrings.LiveViewWindowTitle + " - " + CameraProperty.DeviceName;
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
            ShowHistogram = true;
            Init();
            ServiceProvider.WindowsManager.Event += WindowsManagerEvent;
        }

        private void WindowsManagerEvent(string cmd, object o)
        {
            ICameraDevice device = o as ICameraDevice ?? ServiceProvider.DeviceManager.SelectedCameraDevice;
            if (device != CameraDevice)
                return;
            if (cmd.StartsWith(CmdConsts.LiveView_ManualFocus))
            {
                try
                {
                    string step = cmd.Substring(CmdConsts.LiveView_ManualFocus.Length);
                    SetFocus(Convert.ToInt32(step));
                    while (_focusIProgress)
                    {
                        Thread.Sleep(5);
                    }
                }
                catch (Exception)
                {
                }
            }
            switch (cmd)
            {
                case WindowsCmdConsts.LiveViewWnd_StartMotionDetection:
                    DetectMotion = true;
                    TriggerOnMotion = true;
                    break;
                case WindowsCmdConsts.LiveViewWnd_StopMotionDetection:
                    DetectMotion = false;
                    break;
                case WindowsCmdConsts.LiveViewWnd_Maximize:
                    ShowLeftTab = false;
                    break;
                case WindowsCmdConsts.LiveViewWnd_StartRecord:
                    RecordMovie();
                    break;
                case WindowsCmdConsts.LiveViewWnd_StopRecord:
                    StopRecordMovie();
                    break;

            }
        }

        private void InitCommands()
        {
            AutoFocusCommand = new RelayCommand(AutoFocus);
            RecordMovieCommand = new RelayCommand(delegate
            {
                if (Recording)
                    StopRecordMovie();
                else
                    RecordMovie();
            },
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
            LockCurrentNearCommand = new RelayCommand(() =>
            {
                if (LockB)
                {
                    FocusValue = FocusValue - FocusCounter;
                    FocusCounter = 0;
                }
                LockA = true;
            });

            LockCurrentFarCommand = new RelayCommand(() =>
            {
                if (LockB || LockA)
                {
                    FocusValue = FocusCounter;
                }
                LockB = true;
            });

            StartFocusStackingCommand = new RelayCommand(StartFocusStacking, () => LockB);
            PreviewFocusStackingCommand = new RelayCommand(PreviewFocusStacking, () => LockB);
            StopFocusStackingCommand = new RelayCommand(StopFocusStacking);
            StartLiveViewCommand = new RelayCommand(StartLiveView);
            StopLiveViewCommand = new RelayCommand(StopLiveView);
            ResetBrigthnessCommand = new RelayCommand(() => Brightness = 0);

            StartSimpleFocusStackingCommand = new RelayCommand(StartSimpleFocusStacking);
            PreviewSimpleFocusStackingCommand = new RelayCommand(PreviewSimpleFocusStacking);
            StopSimpleFocusStackingCommand = new RelayCommand(StopFocusStacking);
            HelpFocusStackingCommand = new RelayCommand(() => HelpProvider.Run(HelpSections.FocusStacking));

            BrowseOverlayCommand = new RelayCommand(BrowseOverlay);
            ResetOverlayCommand = new RelayCommand(ResetOverlay);
            ZoomInCommand = new RelayCommand(() => CameraDevice.LiveViewImageZoomRatio.NextValue());
            ZoomOutCommand = new RelayCommand(() => CameraDevice.LiveViewImageZoomRatio.PrevValue());
            ZoomIn100 = new RelayCommand(ToggleZoom);
            ToggleGridCommand = new RelayCommand(ToggleGrid);
            FocuseDone += LiveViewViewModel_FocuseDone;

            SetAreaCommand = new RelayCommand(() => SettingArea = true);
            DoneSetAreaCommand = new RelayCommand(() => SettingArea = false);

            CancelCaptureCommand = new RelayCommand(() => CaptureCancelRequested = true);

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
                if (CameraDevice.LiveViewImageZoomRatio == null || CameraDevice.LiveViewImageZoomRatio.Values == null)
                    return;
                // for canon cameras 
                if (CameraDevice.LiveViewImageZoomRatio.Values.Count == 2)
                {
                    CameraDevice.LiveViewImageZoomRatio.Value = CameraDevice.LiveViewImageZoomRatio.Value ==
                                                                CameraDevice.LiveViewImageZoomRatio.Values[0]
                        ? CameraDevice.LiveViewImageZoomRatio.Values[1]
                        : CameraDevice.LiveViewImageZoomRatio.Values[0];                    
                }
                else
                {
                    CameraDevice.LiveViewImageZoomRatio.Value = CameraDevice.LiveViewImageZoomRatio.Value ==
                                                                CameraDevice.LiveViewImageZoomRatio.Values[0]
                        ? CameraDevice.LiveViewImageZoomRatio.Values[CameraDevice.LiveViewImageZoomRatio.Values.Count - 2]
                        : CameraDevice.LiveViewImageZoomRatio.Values[0];
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to set zoom", ex);
            }
        }

        private void InitOverlay()
        {
            Overlays = new AsyncObservableCollection<ValuePair>();
            Grids = new AsyncObservableCollection<string>
            {
                TranslationStrings.LabelNone,
                TranslationStrings.LabelRuleOfThirds,
                TranslationStrings.LabelComboGrid,
                TranslationStrings.LabelDiagonal,
                TranslationStrings.LabelSplit
            };
            if (Directory.Exists(ServiceProvider.Settings.OverlayFolder))
            {
                string[] files = Directory.GetFiles(ServiceProvider.Settings.OverlayFolder, "*.png");
                foreach (string file in files)
                {
                    Overlays.Add(new ValuePair { Name = Path.GetFileNameWithoutExtension(file), Value = file });
                }
            }
            OverlayTransparency = 100;
            OverlayUseLastCaptured = false;
        }

        private void Init()
        {
            IsActive = true;
            WaitTime = 2;
            PhotoNo = 2;
            FocusStep = 2;
            PhotoCount = 5;
            CaptureCount = 1;
            DelayedStart = false;

            _timer.Stop();
            _timer.AutoReset = true;
            CameraDevice.CameraDisconnected += CameraDeviceCameraDisconnected;
            _photoCapturedTime = DateTime.MinValue;
            CameraDevice.PhotoCaptured += CameraDevicePhotoCaptured;
            StartLiveView();
            _freezeTimer.Interval = ServiceProvider.Settings.LiveViewFreezeTimeOut * 1000;
            _freezeTimer.Elapsed += _freezeTimer_Elapsed;
            _timer.Elapsed += _timer_Elapsed;
            //ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            _focusStackingTimer.AutoReset = true;
            _focusStackingTimer.Elapsed += _focusStackingTimer_Elapsed;
            _restartTimer.AutoReset = true;
            _restartTimer.Elapsed += _restartTimer_Elapsed;
        }

        private void _restartTimer_Elapsed(object sender, ElapsedEventArgs e)
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
            IsActive = false;
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
                SetOverlay(dlg.FileName);
            }
        }

        public void SetOverlay(string file)
        {
            Overlays.Add(new ValuePair
            {
                Name = Path.GetFileNameWithoutExtension(file),
                Value = file
            });
            SelectedOverlay = file;
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

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if (!_worker.IsBusy)
            //    _worker.RunWorkerAsync();
            //ThreadPool.QueueUserWorkItem(GetLiveImage);
            Task.Factory.StartNew(GetLiveImage);
        }

        void _freezeTimer_Elapsed(object sender, ElapsedEventArgs e)
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
               // Log.Error("OperInProgress");
                return;
            }

            if (DelayedStart)
            {
                //Log.Error("Start is delayed");
                return;
            }

            if (FreezeImage)
                return;

            _operInProgress = true;
            _totalframes++;
            if ((DateTime.Now - _framestart).TotalSeconds > 0)
                Fps = (int)(_totalframes / (DateTime.Now - _framestart).TotalSeconds);
            try
            {
                LiveViewData = LiveViewManager.GetLiveViewImage(CameraDevice);
            }
            catch (Exception ex)
            {
                Log.Error("Error geting lv", ex);
                _operInProgress = false;
                return;
            }

            if (LiveViewData == null)
            {
                _operInProgress = false;
                return;
            }

            if (!LiveViewData.IsLiveViewRunning && !IsFocusStackingRunning)
            {
                DelayedStart = true;
                _restartTimerStartTime = DateTime.Now;
                _restartTimer.Start();
                _operInProgress = false;
                return;
            }

            if (LiveViewData.ImageData == null)
            {
               // Log.Error("LV image data is null !");
                _operInProgress = false;
                return;
            }

            Recording = LiveViewData.MovieIsRecording;
            try
            {
                if (LiveViewData != null && LiveViewData.ImageData != null)
                {
                    MemoryStream stream = new MemoryStream(LiveViewData.ImageData,
                        LiveViewData.
                            ImageDataPosition,
                        LiveViewData.ImageData.
                            Length -
                        LiveViewData.
                            ImageDataPosition);
                    LevelAngle = (int)LiveViewData.LevelAngleRolling;
                    SoundL = LiveViewData.SoundL;
                    SoundR = LiveViewData.SoundR;
                    PeakSoundL = LiveViewData.PeakSoundL;
                    PeakSoundR = LiveViewData.PeakSoundR;
                    HaveSoundData = LiveViewData.HaveSoundData;
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
                        if (PreviewTime > 0 && (DateTime.Now - _photoCapturedTime).TotalSeconds <= PreviewTime)
                        {
                            var bitmap = ServiceProvider.Settings.SelectedBitmap.DisplayImage.Clone();
                            //var dw = (double)bmp.Width / bitmap.PixelWidth;
                            //bitmap = bitmap.Resize((int)(bitmap.PixelWidth * dw), (int)(bitmap.PixelHeight * dw),
                            //    WriteableBitmapExtensions.Interpolation.NearestNeighbor);
                            // flip image only if the prview not fliped 
                            if (FlipImage && !ServiceProvider.Settings.FlipPreview)
                                bitmap = bitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
                            bitmap.Freeze();
                            ServiceProvider.DeviceManager.LiveViewImage[CameraDevice] = SaveJpeg(bitmap);
                            Bitmap = bitmap;
                            return;
                        }
                        if (DetectMotion)
                        {
                            ProcessMotionDetection(bmp);
                        }

                        if (_totalframes % DesiredFrameRate == 0 && ShowHistogram)
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
                            filtering.FillColor = new RGB(Color.Blue);
                            filtering.ApplyInPlace(bmp);
                        }

                        if (HighlightOverExp)
                        {
                            ColorFiltering filtering = new ColorFiltering();
                            filtering.Blue = new IntRange(250, 255);
                            filtering.Red = new IntRange(250, 255);
                            filtering.Green = new IntRange(250, 255);
                            filtering.FillOutsideRange = false;
                            filtering.FillColor = new RGB(Color.Red);
                            filtering.ApplyInPlace(bmp);
                        }

                        var preview = BitmapFactory.ConvertToPbgra32Format(
                            BitmapSourceConvert.ToBitmapSource(bmp));
                        DrawFocusPoint(preview, true);

                        if (Brightness != 0)
                        {
                            BrightnessCorrection filter = new BrightnessCorrection(Brightness);
                            bmp = filter.Apply(bmp);
                        }


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
                        switch (RotationIndex)
                        {
                            case 0:
                                Rotation = 0;
                                break;
                            case 1:
                                Rotation = 90;
                                break;
                            case 2:
                                Rotation = 180;
                                break;
                            case 3:
                                Rotation = 270;
                                break;
                            case 4:
                                Rotation = LiveViewData.Rotation;
                                break;
                        }

                        if (CameraDevice.LiveViewImageZoomRatio.Value == "All")
                        {
                            preview.Freeze();
                            Preview = preview;
                            if (ShowFocusRect)
                                DrawFocusPoint(writeableBitmap);
                        }
                        
                        if (FlipImage)
                        {
                            writeableBitmap = writeableBitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
                        }
                        if (CropRatio > 0)
                        {
                            CropOffsetX = (int) ((writeableBitmap.PixelWidth/2.0)*CropRatio/100);
                            CropOffsetY = (int) ((writeableBitmap.PixelHeight/2.0)*CropRatio/100);
                            writeableBitmap = writeableBitmap.Crop(CropOffsetX, CropOffsetY,
                                writeableBitmap.PixelWidth - (2*CropOffsetX),
                                writeableBitmap.PixelHeight - (2*CropOffsetY));
                        }
                        writeableBitmap.Freeze();
                        Bitmap = writeableBitmap;

                        //if (_totalframes%DesiredWebFrameRate == 0)
                        ServiceProvider.DeviceManager.LiveViewImage[CameraDevice] = SaveJpeg(writeableBitmap);
                    }
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                _operInProgress = false;
            }
            finally
            {
                _operInProgress = false;
            }
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

        private void DrawGrid(WriteableBitmap writeableBitmap)
        {
            System.Windows.Media.Color color = Colors.White;
            color.A = 50;

            if (OverlayActivated)
            {
                if ((SelectedOverlay != null && File.Exists(SelectedOverlay)) || OverlayUseLastCaptured)
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
                    int x = writeableBitmap.PixelWidth*OverlayScale/100;
                    int y = writeableBitmap.PixelHeight*OverlayScale/100;
                    int xx = writeableBitmap.PixelWidth*OverlayHorizontal/100;
                    int yy = writeableBitmap.PixelWidth*OverlayVertical/100;
                    System.Windows.Media.Color transpColor = Colors.White;

                    //set color transparency for blit only the alpha chanel is used from transpColor
                    if (OverlayTransparency < 100)
                        transpColor = System.Windows.Media.Color.FromArgb((byte) (0xff*OverlayTransparency/100d), 0xff, 0xff, 0xff);
                    writeableBitmap.Blit(
                        new Rect(0 + (x / 2) + xx, 0 + (y / 2) + yy, writeableBitmap.PixelWidth - x,
                            writeableBitmap.PixelHeight - y),
                        _overlayImage,
                        new Rect(0, 0, _overlayImage.PixelWidth, _overlayImage.PixelHeight), transpColor,
                        WriteableBitmapExtensions.BlendMode.Alpha);
                }
            }

            switch (GridType)
            {
                case 1:
                    {
                        for (int i = 1; i < 3; i++)
                        {
                            writeableBitmap.DrawLine(0, (int)((writeableBitmap.Height / 3) * i),
                                (int)writeableBitmap.Width,
                                (int)((writeableBitmap.Height / 3) * i), color);
                            writeableBitmap.DrawLine((int)((writeableBitmap.Width / 3) * i), 0,
                                (int)((writeableBitmap.Width / 3) * i),
                                (int)writeableBitmap.Height, color);
                        }
                        writeableBitmap.SetPixel((int)(writeableBitmap.Width / 2), (int)(writeableBitmap.Height / 2), 128,
                            Colors.Red);
                    }
                    break;
                case 2:
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            writeableBitmap.DrawLine(0, (int)((writeableBitmap.Height / 10) * i),
                                (int)writeableBitmap.Width,
                                (int)((writeableBitmap.Height / 10) * i), color);
                            writeableBitmap.DrawLine((int)((writeableBitmap.Width / 10) * i), 0,
                                (int)((writeableBitmap.Width / 10) * i),
                                (int)writeableBitmap.Height, color);
                        }
                        writeableBitmap.SetPixel((int)(writeableBitmap.Width / 2), (int)(writeableBitmap.Height / 2), 128,
                            Colors.Red);
                    }
                    break;
                case 3:
                    {
                        writeableBitmap.DrawLineDDA(0, 0, (int)writeableBitmap.Width,
                            (int)writeableBitmap.Height, color);

                        writeableBitmap.DrawLineDDA(0, (int)writeableBitmap.Height,
                            (int)writeableBitmap.Width, 0, color);
                        writeableBitmap.SetPixel((int)(writeableBitmap.Width / 2), (int)(writeableBitmap.Height / 2), 128,
                            Colors.Red);
                    }
                    break;
                case 4:
                    {
                        writeableBitmap.DrawLineDDA(0, (int)(writeableBitmap.Height / 2), (int)writeableBitmap.Width,
                            (int)(writeableBitmap.Height / 2), color);

                        writeableBitmap.DrawLineDDA((int)(writeableBitmap.Width / 2), 0,
                            (int)(writeableBitmap.Width / 2), (int)writeableBitmap.Height, color);
                        writeableBitmap.SetPixel((int)(writeableBitmap.Width / 2), (int)(writeableBitmap.Height / 2), 128,
                            Colors.Red);
                    }
                    break;
                default:
                    break;
            }

            if (ShowRuler && NoSettingArea)
            {
                int x1 = writeableBitmap.PixelWidth * (HorizontalMin) / 1000;
                int x2 = writeableBitmap.PixelWidth * (HorizontalMin+HorizontalMax) / 1000;
                int y2 = writeableBitmap.PixelHeight * (VerticalMin+VerticalMax) / 1000;
                int y1 = writeableBitmap.PixelHeight * VerticalMin / 1000;

                writeableBitmap.FillRectangle2(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, System.Windows.Media.Color.FromArgb(128, 128, 128, 128));
                writeableBitmap.FillRectangleDeBlend(x1, y1, x2, y2, System.Windows.Media.Color.FromArgb(128, 128, 128, 128));
                writeableBitmap.DrawRectangle(x1, y1, x2, y2, color);

            }

        }

        private void DrawFocusPoint(WriteableBitmap bitmap, bool fill=false)
        {
            try
            {
                if (LiveViewData == null)
                    return;
                double xt = bitmap.Width / LiveViewData.ImageWidth;
                double yt = bitmap.Height / LiveViewData.ImageHeight;

                if (fill)
                {
                    bitmap.FillRectangle2((int) (LiveViewData.FocusX*xt - (LiveViewData.FocusFrameXSize*xt/2)),
                        (int) (LiveViewData.FocusY*yt - (LiveViewData.FocusFrameYSize*yt/2)),
                        (int) (LiveViewData.FocusX*xt + (LiveViewData.FocusFrameXSize*xt/2)),
                        (int) (LiveViewData.FocusY*yt + (LiveViewData.FocusFrameYSize*yt/2)),
                        LiveViewData.HaveFocusData
                            ? System.Windows.Media.Color.FromArgb(0x60, 0, 0xFF, 0)
                            : System.Windows.Media.Color.FromArgb(0x60, 0xFF, 0, 0));
                }
                else
                {
                    bitmap.DrawRectangle((int)(LiveViewData.FocusX * xt - (LiveViewData.FocusFrameXSize * xt / 2)),
                        (int)(LiveViewData.FocusY * yt - (LiveViewData.FocusFrameYSize * yt / 2)),
                        (int)(LiveViewData.FocusX * xt + (LiveViewData.FocusFrameXSize * xt / 2)),
                        (int)(LiveViewData.FocusY * yt + (LiveViewData.FocusFrameYSize * yt / 2)),
                        LiveViewData.HaveFocusData ? Colors.Green : Colors.Red);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error draw helper lines", exception);
            }
        }


        private void ProcessMotionDetection(Bitmap bmp)
        {
            if (CountDownVisible)
                return;
            try
            {
                float movement = 0;
                if (DetectMotionArea)
                {
                    int x1 = bmp.Width * HorizontalMin / 1000;
                    int x2 = bmp.Width * (HorizontalMin+ HorizontalMax) / 1000;
                    int y2 = bmp.Height * (VerticalMin + VerticalMax) / 1000;
                    int y1 = bmp.Height * VerticalMin / 1000;
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

                CurrentMotionIndex = Math.Round(movement * 100, 2);
                if (movement > ((float) MotionThreshold/100) && TriggerOnMotion &&
                    (DateTime.Now - _photoCapturedTime).TotalSeconds > WaitForMotionSec &&
                    _totalframes > 10)
                {
                    if (MotionAutofocusBeforCapture)
                    {
                        var processing = _detector.MotionProcessingAlgorithm as BlobCountingObjectsProcessing;
                        if (processing != null && processing.ObjectRectangles != null &&
                            processing.ObjectRectangles.Length > 0 &&
                            LiveViewData.ImageData != null)
                        {
                            var rectangle = new Rectangle();
                            int surface = 0;
                            foreach (Rectangle objectRectangle in processing.ObjectRectangles)
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
                        AutoFocusThread();
                    }
                    CaptureInThread();
                    _detector.Reset();
                    _photoCapturedTime = DateTime.Now;
                    _totalframes = 0;
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
                CameraDevice.StartRecordMovie();
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Recording error", exception);
            }
        }

        private void StopRecordMovie()
        {
            var thread = new Thread(StopRecordMovieThread);
            thread.Start();
        }

        private void StopRecordMovieThread()
        {
            try
            {
                CameraDevice.StopRecordMovie();
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Recording error", exception);
            }
        }


        private void StartLiveView()
        {
            if(!IsActive)
                return;
            
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
                // in nikon case no show error message
                // if the images not transferd yet from SDRam
                if (resp != "LabelImageInRAM" && resp != "LabelCommandProcesingError")
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
            if(!CameraDevice.IsChecked)
                return;
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
                    {
                        _focusIProgress = false;
                        _selectedFocusValue = FocusCounter;
                        OnFocuseDone();
                        RaisePropertyChanged(() => FocusingEnabled);
                        RaisePropertyChanged(() => SelectedFocusValue);
                        return;
                    }
                    if (FocusCounter + step < 0)
                        step = -FocusCounter;
                }
                if (LockB)
                {
                    if (FocusCounter + step > FocusValue)
                        step = FocusValue - FocusCounter;
                }

                var focusStep = 0;
                _timer.Stop();

                var retryCount = 10;
                do
                {
                    try
                    {
                        CameraDevice.StartLiveView();
                        if (SimpleManualFocus)
                        {
                            FocusProgressMax = Math.Abs(step);
                            FocusProgressValue = 0;
                            FocusProgressVisible = true;

                            for (var i = 0; i < Math.Abs(step); i++)
                            {
                                FocusProgressValue++;
                                focusStep += CameraDevice.Focus(step);
                                GetLiveImage();
                                Thread.Sleep(ServiceProvider.Settings.CanonFocusStepWait);
                            }
                            FocusProgressVisible = false;
                        }
                        else
                        {
                            focusStep += CameraDevice.Focus(step);
                        }

                        FocusCounter += focusStep;
                        if (!LockA && LockB && FocusCounter < 0)
                        {
                            FocusValue += (FocusCounter * -1);
                            FocusCounter = 0;
                        }
                        StaticHelper.Instance.SystemMessage = "Move focus " + step;
                        break;
                    }
                    catch (DeviceException exception)
                    {
                        Log.Debug("Unable to focus", exception);
                        StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus + " " +
                                                              exception.Message;
                        retryCount--;
                    }
                    catch (Exception exception)
                    {
                        Log.Debug("Unable to focus", exception);
                        StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus;
                        retryCount--;
                    }
                    Thread.Sleep(50);
                } while (retryCount > 0);
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
            if (CameraDevice.ShutterSpeed != null && CameraDevice.ShutterSpeed.Value == "Bulb")
            {
                StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Message, TranslationStrings.MsgBulbModeNotSupported);
                CaptureInProgress = false;
                return;
            }
            CaptureInProgress = true;
            CaptureCancelRequested = false;
            if (CaptureCount == 0)
                CaptureCount = 1;
            for (int i = 0; i < CaptureCount; i++)
            {
                Log.Debug("LiveView: Capture started");
                if (CaptureDelay > 0)
                {
                    Log.Debug("LiveView: Capture delayed");
                    CountDown = CaptureDelay;
                    CountDownVisible = true;
                    while (CountDown > 0)
                    {
                        if (CaptureCancelRequested)
                            break;
                        Thread.Sleep(1000);
                        CountDown--;
                    }
                    CountDownVisible = false;
                }
               
                if (AutoFocusBeforCapture)
                    AutoFocusThread();

                if (CaptureCancelRequested)
                    break;

                _timer.Stop();
                Thread.Sleep(300);
                try
                {
                    CameraDevice.CapturePhotoNoAf();
                    Log.Debug("LiveView: Capture Initialization Done");
                }
                catch (Exception exception)
                {
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    Log.Error("Unable to take picture with no af", exception);
                    break;
                }

                // if multiple capture set wait also preview time
                if (CaptureCount > 1)
                {
                    CameraDevice.WaitForCamera(2000);
                    for (int j = 0; j < PreviewTime; j++)
                    {
                        Thread.Sleep(1000);
                        if (CaptureCancelRequested)
                            break;
                    }
                }
            }
            CaptureInProgress = false;
        }

        public void SetFocusPos(Point initialPoint, double refWidth, double refHeight)
        {
            if (LiveViewData != null)
            {
                //CropOffsetX = (writeableBitmap.PixelWidth / 2) * CropRatio / 100;
                double offsetX = (((refWidth*100)/(100 - CropRatio)) - refWidth)/2;
                double offsety = (((refHeight * 100) / (100 - CropRatio)) - refHeight) / 2; ; ;
                double xt = (LiveViewData.ImageWidth )/(refWidth+(offsetX*2));
                double yt = (LiveViewData.ImageHeight ) /( refHeight+(offsety*2));
                int posx = (int)((offsetX+initialPoint.X) * xt);
                if (FlipImage)
                    posx = (int)(((refWidth) - initialPoint.X + offsetX) * xt);
                int posy = (int)((initialPoint.Y+offsety) * yt);
                Task.Factory.StartNew(() => SetFocusPos(posx, posy));
            }
        }

        public virtual void SetFocusPos(int x, int y)
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


        void _focusStackingTimer_Elapsed(object sender, ElapsedEventArgs e)
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

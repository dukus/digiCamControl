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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Vision.Motion;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for LiveViewWnd.xaml
    /// </summary>
    public partial class LiveViewWnd : MetroWindow, IWindow, INotifyPropertyChanged
    {
        private const int DesiredFrameRate = 20;

        private int _retries = 0;
        private ICameraDevice selectedPortableDevice;
        private Rectangle _focusrect = new Rectangle();
        private BackgroundWorker _worker = new BackgroundWorker();
        private bool _preview = false;
        private int _totalframes = 0;
        private DateTime _framestart;
        private MotionDetector _detector;
        private DateTime _photoCapturedTime;
        private DateTime _focusMoveTime = DateTime.Now;
        private bool _focusIProgress = false;


        public LiveViewData LiveViewData { get; set; }

        private PointCollection luminanceHistogramPoints = null;

        public PointCollection LuminanceHistogramPoints
        {
            get { return this.luminanceHistogramPoints; }
            set
            {
                if (this.luminanceHistogramPoints != value)
                {
                    this.luminanceHistogramPoints = value;
                    NotifyPropertyChanged("LuminanceHistogramPoints");
                }
            }
        }

        private CameraProperty _cameraProperty;

        public CameraProperty CameraProperty
        {
            get { return _cameraProperty; }
            set
            {
                _cameraProperty = value;
                NotifyPropertyChanged("CameraProperty");
            }
        }

        private int _fps;

        public int Fps
        {
            get { return _fps; }
            set
            {
                _fps = value;
                NotifyPropertyChanged("Fps");
            }
        }


        private bool _blackAndWhite;

        public bool BlackAndWhite
        {
            get { return _blackAndWhite; }
            set
            {
                _blackAndWhite = value;
                NotifyPropertyChanged("BlackAndWhite");
            }
        }

        private bool _showFocusRect;

        public bool ShowFocusRect
        {
            get { return _showFocusRect; }
            set
            {
                _showFocusRect = value;
                NotifyPropertyChanged("ShowFocusRect");
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
                NotifyPropertyChanged("IsFree");
            }
        }

        public bool IsFree
        {
            get { return !_isBusy; }
        }

        private int _photoNo;

        public int PhotoNo
        {
            get { return _photoNo; }
            set
            {
                _photoNo = value;
                NotifyPropertyChanged("PhotoNo");
                if (PhotoNo > 0)
                    _focusStep =
                        Convert.ToInt32(Decimal.Round((decimal) FocusValue/PhotoNo, MidpointRounding.AwayFromZero));
                NotifyPropertyChanged("FocusStep");
            }
        }

        private int _waitTime;

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                _waitTime = value;
                NotifyPropertyChanged("WaitTime");
            }
        }

        private int _focusStep;

        public int FocusStep
        {
            get { return _focusStep; }
            set
            {
                _focusStep = value;
                NotifyPropertyChanged("FocusStep");
                PhotoNo = Convert.ToInt32(Decimal.Round((decimal) FocusValue/FocusStep, MidpointRounding.AwayFromZero));
            }
        }


        private int _photoCount;

        public int PhotoCount
        {
            get { return _photoCount; }
            set
            {
                _photoCount = value;
                NotifyPropertyChanged("PhotoCount");
            }
        }

        private string _counterMessage;

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
                NotifyPropertyChanged("CounterMessage");
            }
        }

        private int _focusCounter;

        public int FocusCounter
        {
            get { return _focusCounter; }
            set
            {
                _focusCounter = value;
                NotifyPropertyChanged("FocusCounter");
                NotifyPropertyChanged("CounterMessage");
            }
        }

        private int _focusValue;

        public int FocusValue
        {
            get { return _focusValue; }
            set
            {
                _focusValue = value;
                PhotoNo = FocusValue/FocusStep;
                NotifyPropertyChanged("FocusValue");
                NotifyPropertyChanged("CounterMessage");
            }
        }

        private bool _lockA;

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
                NotifyPropertyChanged("LockA");
                NotifyPropertyChanged("CounterMessage");
            }
        }

        private bool _lockB;

        public bool LockB
        {
            get { return _lockB; }
            set
            {
                _lockB = value;
                if (_lockB)
                {
                    FocusValue = FocusCounter;
                    focus_slider.Value = FocusCounter;
                }
                NotifyPropertyChanged("LockB");
                NotifyPropertyChanged("CounterMessage");
            }
        }

        private bool _freezeImage;

        public bool FreezeImage
        {
            get { return _freezeImage; }
            set
            {
                _freezeImage = value;
                if (_freezeImage)
                    _freezeTimer.Start();
                NotifyPropertyChanged("FreezeImage");
            }
        }

        private bool _recording;

        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                if (_recording)
                {
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         btn_record.Content = "Stop record movie";
                                                         lbl_rec.Visibility = Visibility.Visible;
                                                     }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         btn_record.Content = "Start record movie";
                                                         lbl_rec.Visibility = Visibility.Hidden;
                                                     }));
                }
                NotifyPropertyChanged("Recording");
            }
        }

        private int _gridType;

        public int GridType
        {
            get { return _gridType; }
            set
            {
                _gridType = value;
                NotifyPropertyChanged("GridType");
            }
        }

        private bool _edgeDetection;

        public bool EdgeDetection
        {
            get { return _edgeDetection; }
            set
            {
                _edgeDetection = value;
                NotifyPropertyChanged("EdgeDetection");
            }
        }


        private Timer _timer = new Timer(1000/DesiredFrameRate);
        private Timer _freezeTimer = new Timer();

        private bool oper_in_progress = false;

        public ICameraDevice SelectedPortableDevice
        {
            get { return this.selectedPortableDevice; }
            set
            {
                if (this.selectedPortableDevice != value)
                {
                    this.selectedPortableDevice = value;
                    NotifyPropertyChanged("SelectedPortableDevice");
                }
            }
        }

        private bool _highlightUnderExp;

        public bool HighlightUnderExp
        {
            get { return _highlightUnderExp; }
            set
            {
                _highlightUnderExp = value;
                NotifyPropertyChanged("HighlightUnderExp");
            }
        }

        private bool _highlightOverExp;

        public bool HighlightOverExp
        {
            get { return _highlightOverExp; }
            set
            {
                _highlightOverExp = value;
                NotifyPropertyChanged("HighlightOverExp");
            }
        }

        public LiveViewWnd()
        {
            SelectedPortableDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
            Init();
            LockA = false;
            FocusStep = 75;
            FreezeImage = false;
            Recording = false;
            ShowFocusRect = true;
            chk_top.IsChecked = false;
            cmb_rotation.SelectedIndex = 4;
        }

        private void SelectedBitmap_BitmapLoaded(object sender)
        {
            if (ServiceProvider.Settings.PreviewLiveViewImage && IsVisible)
            {
                FreezeImage = true;
                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     ServiceProvider.Settings.SelectedBitmap.DisplayImage.Freeze();
                                                     image1.Source =
                                                         ServiceProvider.Settings.SelectedBitmap.DisplayImage;
                                                 }));
            }
        }

        public LiveViewWnd(ICameraDevice device)
        {
            SelectedPortableDevice = device;
            Init();
        }

        public void Init()
        {
            WaitTime = 2;
            PhotoNo = 2;
            FocusStep = 2;
            InitializeComponent();
            _timer.Stop();
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _freezeTimer.Elapsed += _freezeTimer_Elapsed;
            _focusrect.Stroke = new SolidColorBrush(Colors.Green);
            canvas.Children.Add(_focusrect);
            _worker.DoWork += delegate
                                  {
                                      if (!FreezeImage)
                                          GetLiveImage();
                                  };
            if (Directory.Exists(ServiceProvider.Settings.OverlayFolder))
            {
                string[] files = Directory.GetFiles(ServiceProvider.Settings.OverlayFolder, "*.png");
                foreach (string file in files)
                {
                    cmb_overlay.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        private void _freezeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FreezeImage = false;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (_retries > 100)
            //{
            //    _timer.Stop();

            //    Dispatcher.Invoke(new ThreadStart(delegate
            //                                        {
            //                                            image1.Visibility = Visibility.Hidden;
            //                                            //chk_grid.IsChecked = false;
            //                                        }));
            //    return;
            //}
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void ProcessMotionDetection(Bitmap bmp)
        {
            try
            {
                float movement = _detector.ProcessFrame(bmp);
                lbl_motion.Content = Math.Round(movement*100, 2);
                if (movement > ((float) upd_threshold.Value/100) && chk_tiggeronmotion.IsChecked == true &&
                    (DateTime.Now - _photoCapturedTime).TotalSeconds > upd_movewait.Value)
                {
                    if (chk_autofocus.IsChecked == true)
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
                            selectedPortableDevice.Focus(posx, posy);
                        }
                        AutoFocus();
                    }
                    selectedPortableDevice.CapturePhotoNoAf();
                    _detector.Reset();
                    _photoCapturedTime = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Motion detection error ", exception);
            }
        }

        private void GetLiveImage()
        {
            if (oper_in_progress)
                return;
            oper_in_progress = true;
            _totalframes++;
            if ((DateTime.Now - _framestart).TotalSeconds > 0)
                Fps = (int) (_totalframes/(DateTime.Now - _framestart).TotalSeconds);
            try
            {
                LiveViewData = LiveViewManager.GetLiveViewImage(SelectedPortableDevice);
            }
            catch (Exception)
            {
                _retries++;
                oper_in_progress = false;
                return;
            }

            if (LiveViewData == null || LiveViewData.ImageData == null)
            {
                _retries++;
                oper_in_progress = false;
                return;
            }
            Recording = LiveViewData.MovieIsRecording;
            Dispatcher.Invoke(new Action(delegate
                                             {
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

                                                         using (var bmp = new Bitmap(stream))
                                                         {
                                                             if (chk_motiondetect.IsChecked == true)
                                                             {
                                                                 ProcessMotionDetection(bmp);
                                                             }

                                                             if (_totalframes%DesiredFrameRate == 0)
                                                             {
                                                                 ImageStatisticsHSL hslStatistics =
                                                                     new ImageStatisticsHSL(bmp);
                                                                 this.LuminanceHistogramPoints =
                                                                     ConvertToPointCollection(
                                                                         hslStatistics.Luminance.Values);
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
                                                             preview =
                                                                 BitmapFactory.ConvertToPbgra32Format(
                                                                     BitmapSourceConvert.ToBitmapSource(bmp));

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
                                                             if (cmb_rotation.SelectedIndex != 0)
                                                             {
                                                                 switch (cmb_rotation.SelectedIndex)
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
                                                                                     LiveViewData.Rotation);
                                                                         break;
                                                                 }
                                                             }
                                                             writeableBitmap.Freeze();
                                                             image1.BeginInit();
                                                             image1.Source = writeableBitmap;
                                                             image1.EndInit();

                                                             ServiceProvider.DeviceManager.LiveViewImage[
                                                                 selectedPortableDevice] = SaveJpeg(writeableBitmap);
                                                         }
                                                         if (SelectedPortableDevice.LiveViewImageZoomRatio.Value ==
                                                             "All")
                                                         {
                                                             ImageBrush ib = new ImageBrush {ImageSource = preview};
                                                             canvas_image.BeginInit();
                                                             canvas_image.Background = ib;
                                                             canvas_image.EndInit();
                                                         }
                                                         stream.Close();
                                                     }
                                                 }
                                                 catch (Exception exception)
                                                 {
                                                     Log.Error(exception);
                                                     _retries++;
                                                     oper_in_progress = false;
                                                 }
                                             }));
            Dispatcher.BeginInvoke(new Action(delegate
                                                  {
                                                      DrawLines();
                                                      ;
                                                  }));
            _retries = 0;
            oper_in_progress = false;
        }

        public byte[] SaveJpeg(WriteableBitmap image)
        {
            var enc = new JpegBitmapEncoder();
            enc.QualityLevel = 90;
            enc.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream stm = new MemoryStream())
            {
                enc.Save(stm);
                return stm.ToArray();
            }
        }

        private void DrawGrid(WriteableBitmap writeableBitmap)
        {
            Color color = Colors.White;
            color.A = 50;
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
                    try
                    {
                        if (GridType > 4)
                        {
                            string filename = Path.Combine(ServiceProvider.Settings.OverlayFolder,
                                                           (string) cmb_overlay.SelectedValue + ".png");
                            if (File.Exists(filename))
                            {
                                BitmapImage bitmapSource = new BitmapImage();
                                bitmapSource.BeginInit();
                                bitmapSource.UriSource = new Uri(filename);
                                bitmapSource.EndInit();
                                WriteableBitmap overlay = BitmapFactory.ConvertToPbgra32Format(bitmapSource);
                                writeableBitmap.Blit(
                                    new Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                                    overlay,
                                    new Rect(0, 0, overlay.PixelWidth, overlay.PixelHeight));
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
            }
        }

        private void DrawLines()
        {
            try
            {
                if (LiveViewData == null)
                    return;

                _focusrect.BeginInit();
                _focusrect.Visibility = LiveViewData.HaveFocusData && ShowFocusRect &&
                                        selectedPortableDevice.LiveViewImageZoomRatio.Value == "All"
                                            ? Visibility.Visible
                                            : Visibility.Hidden;
                //_focusrect.Visibility = selectedPortableDevice.LiveViewImageZoomRatio.Value == "All"
                //                          ? Visibility.Visible
                //                          : Visibility.Hidden;
                double xt = image1.ActualWidth/LiveViewData.ImageWidth;
                double yt = image1.ActualHeight/LiveViewData.ImageHeight;
                _focusrect.Height = LiveViewData.FocusFrameXSize*xt;
                _focusrect.Width = LiveViewData.FocusFrameYSize*yt;
                double xx = (canvas.ActualWidth - image1.ActualWidth)/2;
                double yy = (canvas.ActualHeight - image1.ActualHeight)/2;
                //SetLinePos(_line11, (int) (xx + image1.ActualWidth/3), (int) yy, (int) (xx + image1.ActualWidth/3),
                //           (int) (yy + image1.ActualHeight));
                //SetLinePos(_line12, (int) (xx + (image1.ActualWidth/3)*2), (int) yy, (int) (xx + (image1.ActualWidth/3)*2),
                //           (int) (yy + image1.ActualHeight));

                //SetLinePos(_line21, (int) xx, (int) (yy + (image1.ActualHeight/3)), (int) (xx + image1.ActualWidth),
                //           (int) (yy + image1.ActualHeight/3));
                //SetLinePos(_line22, (int) xx, (int) (yy + (image1.ActualHeight/3)*2), (int) (xx + image1.ActualWidth),
                //           (int) (yy + (image1.ActualHeight/3)*2));

                _focusrect.SetValue(Canvas.LeftProperty, LiveViewData.FocusX*xt - (_focusrect.Height/2) + xx);
                _focusrect.SetValue(Canvas.TopProperty, LiveViewData.FocusY*yt - (_focusrect.Width/2) + yy);
                _focusrect.Stroke = new SolidColorBrush(LiveViewData.Focused ? Colors.Green : Colors.Red);
                _focusrect.EndInit();
                SmallFocusScreen();
            }
            catch (Exception exception)
            {
                Log.Error("Error draw helper lines", exception);
            }
        }

        private void SmallFocusScreen()
        {
            double aspect = image1.ActualHeight/image1.ActualWidth;
            canvas_image.Height = canvas_image.Width*aspect;
            small_focus_rect.Visibility = LiveViewData.HaveFocusData ? Visibility.Visible : Visibility.Hidden;
            double xt = canvas_image.ActualWidth/LiveViewData.ImageWidth;
            double yt = canvas_image.ActualHeight/LiveViewData.ImageHeight;
            small_focus_rect.Height = LiveViewData.FocusFrameXSize*xt;
            small_focus_rect.Width = LiveViewData.FocusFrameYSize*yt;
            small_focus_rect.SetValue(Canvas.LeftProperty, LiveViewData.FocusX*xt - (_focusrect.Height/2*xt));
            small_focus_rect.SetValue(Canvas.TopProperty, LiveViewData.FocusY*yt - (_focusrect.Width/2*yt));
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SelectedPortableDevice.StoptLiveView();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void AutoFocus()
        {
            //_timer.Stop();
            //Thread.Sleep(100);
            try
            {
                selectedPortableDevice.AutoFocus();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to autofocus", exception);
                StaticHelper.Instance.SystemMessage = exception.Message;
            }
            //FocusCounter = 0;
            //_timer.Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string resp = SelectedPortableDevice.GetProhibitionCondition(OperationEnum.AutoFocus);
            if (string.IsNullOrEmpty(resp))
            {
                Thread thread = new Thread(AutoFocus);
                thread.Start();
            }
            else
            {
                this.ShowMessageAsync(TranslationStrings.LabelError,
                                      TranslationStrings.LabelErrorUnableFocus + "\n" +
                                      TranslationManager.GetTranslation(resp));
            }
        }

        private void Capture()
        {
            Log.Debug("LiveView: Capture started");
            _timer.Stop();
            Thread.Sleep(300);
            try
            {
                if (SelectedPortableDevice.ShutterSpeed != null && SelectedPortableDevice.ShutterSpeed.Value == "Bulb")
                {
                    if (SelectedPortableDevice.GetCapability(CapabilityEnum.Bulb))
                    {
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BulbWnd_Show,
                                                                      SelectedPortableDevice);
                        return;
                    }
                    else
                    {
                        StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                        this.ShowMessageAsync(TranslationStrings.LabelError, TranslationStrings.MsgBulbModeNotSupported);
                        return;
                    }
                }
                //selectedPortableDevice.StopLiveView();
                SelectedPortableDevice.CapturePhotoNoAf();
                Log.Debug("LiveView: Capture Initialization Done");
            }
            catch (DeviceException exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Unable to take picture with no af", exception);
            }
            //_timer.Start();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_Capture, SelectedPortableDevice);
        }

        private void StartLiveView()
        {
            if (!IsVisible)
                return;
            string resp = SelectedPortableDevice.GetProhibitionCondition(OperationEnum.LiveView);
            if (string.IsNullOrEmpty(resp))
            {
                Thread thread = new Thread(StartLiveViewThread);
                thread.Start();
                thread.Join();
            }
            else
            {
                Log.Error("Error starting live view " + resp);
                this.ShowMessageAsync(TranslationStrings.LabelError,
                                      TranslationStrings.LabelLiveViewError + "\n" +
                                      TranslationManager.GetTranslation(resp));
                return;
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
                        LiveViewManager.StartLiveView(SelectedPortableDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        Dispatcher.Invoke(new Action(delegate { grid_wait.Visibility = Visibility.Visible; }));
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(200);
                            if (!IsVisible)
                                break;
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
                if (IsVisible)
                {
                    _timer.Start();
                    oper_in_progress = false;
                    _retries = 0;
                    Log.Debug("LiveView: Liveview start done");
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to start liveview ! " + exception.Message;
                //MessageBox.Show("Unable to start liveview !");
                //ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide);
            }
            Dispatcher.BeginInvoke(new Action(delegate { grid_wait.Visibility = Visibility.Hidden; }));
            Dispatcher.BeginInvoke(new Action(delegate { image1.Visibility = Visibility.Visible; }));
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
                        LiveViewManager.StopLiveView(SelectedPortableDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        Dispatcher.Invoke(new Action(delegate { grid_wait.Visibility = Visibility.Visible; }));
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
            Dispatcher.Invoke(new Action(delegate { grid_wait.Visibility = Visibility.Hidden; }));
            Dispatcher.Invoke(new Action(delegate { image1.Visibility = Visibility.Hidden; }));
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && LiveViewData != null &&
                LiveViewData.HaveFocusData && selectedPortableDevice.LiveViewImageZoomRatio.Value == "All")
            {
                try
                {
                    Point initialPoint = e.MouseDevice.GetPosition(image1);
                    double xt = LiveViewData.ImageWidth/image1.ActualWidth;
                    double yt = LiveViewData.ImageHeight/image1.ActualHeight;
                    int posx = (int) (initialPoint.X*xt);
                    int posy = (int) (initialPoint.Y*yt);
                    selectedPortableDevice.Focus(posx, posy);
                }
                catch (Exception exception)
                {
                    Log.Error("Focus Error", exception);
                    StaticHelper.Instance.SystemMessage = "Focus error: " + exception.Message;
                }
            }
        }

        private void btn_focusm_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(-ServiceProvider.Settings.SmalFocusStep);
        }

        private void btn_focusp_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(ServiceProvider.Settings.SmalFocusStep);
        }

        private void btn_focusmm_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(-ServiceProvider.Settings.MediumFocusStep);
        }

        private void btn_focuspp_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(ServiceProvider.Settings.MediumFocusStep);
        }

        private void btn_focusmmm_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(-ServiceProvider.Settings.LargeFocusStep);
        }

        private void btn_focusppp_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(ServiceProvider.Settings.LargeFocusStep);
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawLines();
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.LiveViewWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         try
                                                         {
                                                             ICameraDevice cameraparam = param as ICameraDevice;
                                                             if (cameraparam == SelectedPortableDevice && IsVisible)
                                                             {
                                                                 Activate();
                                                                 Focus();
                                                                 return;
                                                             }
                                                             _freezeTimer.Interval =
                                                                 ServiceProvider.Settings.LiveViewFreezeTimeOut*1000;
                                                             ServiceProvider.Settings.SelectedBitmap.BitmapLoaded +=
                                                                 SelectedBitmap_BitmapLoaded;
                                                             Recording = false;
                                                             SelectedPortableDevice = cameraparam;
                                                             SelectedPortableDevice.CameraDisconnected +=
                                                                 selectedPortableDevice_CameraDisconnected;
                                                             CameraProperty =
                                                                 ServiceProvider.Settings.CameraProperties.Get(
                                                                     SelectedPortableDevice);
                                                             Title = TranslationStrings.LiveViewWindowTitle + " - " +
                                                                     CameraProperty.DeviceName;
                                                             DataContext = CameraProperty.LiveviewSettings;
                                                             Show();
                                                             Activate();
                                                             //Topmost = true;
                                                             //Topmost = false;
                                                             Focus();
                                                             //ServiceProvider.Settings.Manager.PhotoTakenDone += Manager_PhotoTaked;
                                                             //Thread.Sleep(500);
                                                             StartLiveView();
                                                             //Thread.Sleep(500);
                                                             FreezeImage = false;
                                                             btn_record.IsEnabled =
                                                                 SelectedPortableDevice.GetCapability(
                                                                     CapabilityEnum.RecordMovie);
                                                             SelectedPortableDevice.CaptureCompleted +=
                                                                 selectedPortableDevice_CaptureCompleted;

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
                                                             _photoCapturedTime = DateTime.Now;
                                                             _timer.Start();
                                                         }
                                                         catch (Exception exception)
                                                         {
                                                             Log.Error("Error initialize live view window ", exception);
                                                         }
                                                     }
                                          ));
                    break;
                case WindowsCmdConsts.LiveViewWnd_Hide:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Hide();
                                                         try
                                                         {
                                                             _timer.Stop();
                                                             selectedPortableDevice.CameraDisconnected -=
                                                                 selectedPortableDevice_CameraDisconnected;
                                                             selectedPortableDevice.CaptureCompleted -=
                                                                 selectedPortableDevice_CaptureCompleted;
                                                             ServiceProvider.Settings.SelectedBitmap.BitmapLoaded -=
                                                                 SelectedBitmap_BitmapLoaded;
                                                             DataContext = null;
                                                             Thread.Sleep(100);
                                                             StopLiveView();
                                                             Recording = false;
                                                             LockA = false;
                                                             LockB = false;
                                                             LiveViewData = null;
                                                         }
                                                         catch (Exception exception)
                                                         {
                                                             Log.Error("Unable to stop live view", exception);
                                                         }
                                                         //ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FocusStackingWnd_Hide);
                                                     }));
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Hide();
                                                         Close();
                                                     }));
                    break;
                case CmdConsts.LiveView_Zoom_All:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(0);
                    break;
                case CmdConsts.LiveView_Zoom_25:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(1);
                    break;
                case CmdConsts.LiveView_Zoom_33:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(2);
                    break;
                case CmdConsts.LiveView_Zoom_50:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(3);
                    break;
                case CmdConsts.LiveView_Zoom_66:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(4);
                    break;
                case CmdConsts.LiveView_Zoom_100:
                    SelectedPortableDevice.LiveViewImageZoomRatio.SetValue(5);
                    break;
                case CmdConsts.LiveView_Focus_M:
                    btn_focusm_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus_P:
                    btn_focusp_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus_MM:
                    btn_focusmm_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus_PP:
                    btn_focuspp_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus_MMM:
                    btn_focusmm_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus_PPP:
                    btn_focuspp_Click(null, null);
                    break;
                case CmdConsts.LiveView_Focus:
                    button1_Click(null, null);
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
                case CmdConsts.LiveView_Capture:
                    {
                        Capture();
                    }
                    break;
            }
        }

        private void SetFocusPos(int x, int y)
        {
            try
            {
                selectedPortableDevice.Focus(x, y);
            }
            catch (Exception exception)
            {
                Log.Error("Error set focus pos :", exception);
                StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorSetFocusPos;
            }
        }

        private void selectedPortableDevice_CameraDisconnected(object sender, DisconnectCameraEventArgs eventArgs)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide, SelectedPortableDevice);
        }

        private void selectedPortableDevice_CaptureCompleted(object sender, EventArgs e)
        {
            if (!IsVisible)
                return;
            _detector.Reset();
            _photoCapturedTime = DateTime.Now;
            if (PhotoCount <= PhotoNo && IsBusy)
            {
                var threadPhoto = new Thread(TakePhoto);
                threadPhoto.Start();
            }
            else
            {
                IsBusy = false;
                _timer.Start();
                StartLiveView();
            }
        }

        #endregion

        private void SetFocus(int step)
        {
            try
            {
                string resp = SelectedPortableDevice.GetProhibitionCondition(OperationEnum.ManualFocus);
                if (string.IsNullOrEmpty(resp))
                {
                    Thread thread = new Thread(SetFocusThread);
                    thread.Start(step);
                    thread.Join();
                }
                else
                {
                    this.ShowMessageAsync(TranslationStrings.LabelError,
                                          TranslationStrings.LabelErrorUnableFocus + "\n" +
                                          TranslationManager.GetTranslation(resp));
                }
            }
            catch (Exception exception)
            {
                this.ShowMessageAsync(TranslationStrings.LabelError, TranslationStrings.LabelErrorUnableFocus);
                Log.Error("Unable to focus ", exception);
            }
        }

        private void SetFocusThread(object ostep)
        {
            if (_focusIProgress)
                return;
            int step = (int) ostep;
            _focusIProgress = true;
            Console.WriteLine("Focus start");
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
            //Console.WriteLine(step);
            try
            {
                _timer.Stop();
                selectedPortableDevice.StartLiveView();
                selectedPortableDevice.Focus(step);
                FocusCounter += step;
                //Console.WriteLine(FocusCounter);
            }
            catch (DeviceException exception)
            {
                Log.Error("Unable to focus", exception);
                StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus + " " + exception.Message;
            }
            catch (Exception exception)
            {
                Log.Error("Unable to focus", exception);
                StaticHelper.Instance.SystemMessage = TranslationStrings.LabelErrorUnableFocus;
            }
            if (LockB)
                Dispatcher.BeginInvoke(new Action(delegate { focus_slider.Value = FocusCounter; }));
            if (!IsBusy)
                _timer.Start();
            Console.WriteLine("Focus end");
            _focusIProgress = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide, SelectedPortableDevice);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void btn_movea_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(-FocusCounter);
        }

        private void btn_moveb_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(FocusValue - FocusCounter);
        }

        private void TakePhoto()
        {
            try
            {
                if (IsBusy)
                {
                    Log.Debug("LiveView: Stack photo capture started");
                    //FreezeImage = true;
                    //while (ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy)
                    //{
                    //  Thread.Sleep(1);
                    //}
                    StartLiveView();
                    //Thread.Sleep(500);
                    if (PhotoCount > 0)
                    {
                        SetFocus(FocusStep);
                    }
                    PhotoCount++;
                    GetLiveImage();
                    Thread.Sleep(WaitTime*1000);
                    //ServiceProvider.DeviceManager.SelectedCameraDevice.Focus(FocusStep);
                    if (PhotoCount <= PhotoNo)
                    {
                        if (!_preview)
                        {
                            Recording = false;
                            SelectedPortableDevice.CapturePhotoNoAf();
                        }
                        else
                        {
                            TakePhoto();
                        }
                    }
                    else
                    {
                        StartLiveView();
                        FreezeImage = false;
                        IsBusy = false;
                        PhotoCount = 0;
                        _timer.Start();
                    }
                }
                else
                {
                    ServiceProvider.DeviceManager.SelectedCameraDevice.StartLiveView();
                    FreezeImage = false;
                }
            }
            catch (DeviceException exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Live view. Unable to take photo", exception);
            }
        }

        private void btn_preview_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(-FocusCounter);
            //FreezeImage = true;
            GetLiveImage();
            PhotoCount = 0;
            IsBusy = true;
            _preview = true;
            Thread thread = new Thread(TakePhoto);
            thread.Start();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            IsBusy = false;
            _timer.Start();
        }

        private void btn_takephoto_Click(object sender, RoutedEventArgs e)
        {
            if (!LockA || !LockB)
            {
                this.ShowMessageAsync(TranslationStrings.LabelError, TranslationStrings.LabelLockNearFar);
                return;
            }
            Thread.Sleep(500);
            _focusIProgress = false;
            SetFocus(-FocusCounter);
            //FreezeImage = true;
            GetLiveImage();
            PhotoCount = 0;
            IsBusy = true;
            _preview = false;
            Thread thread = new Thread(TakePhoto);
            thread.Start();
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


        private void btn_record_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Recording)
                {
                    SelectedPortableDevice.StopRecordMovie();
                }
                else
                {
                    string resp = SelectedPortableDevice.GetProhibitionCondition(OperationEnum.RecordMovie);
                    if (string.IsNullOrEmpty(resp))
                    {
                        SelectedPortableDevice.StartRecordMovie();
                    }
                    else
                    {
                        this.ShowMessageAsync(TranslationStrings.LabelError,
                                              TranslationStrings.LabelErrorRecordMovie + "\n" +
                                              TranslationManager.GetTranslation(resp));
                        return;
                    }
                }
                //Recording = !Recording;
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Recording error", exception);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
        }


        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }
            if ((DateTime.Now - _focusMoveTime).TotalMilliseconds < 200)
                return;
            _focusMoveTime = DateTime.Now;
            if (e.Key == Key.Right)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_Focus_Move_Right);
            }
            if (e.Key == Key.Left)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_Focus_Move_Left);
            }
            if (e.Key == Key.Up)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_Focus_Move_Up);
            }
            if (e.Key == Key.Down)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_Focus_Move_Down);
            }
        }

        private void canvas_image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && LiveViewData != null &&
                LiveViewData.HaveFocusData)
            {
                Point initialPoint = e.MouseDevice.GetPosition(canvas_image);
                double xt = LiveViewData.ImageWidth/canvas_image.ActualWidth;
                double yt = LiveViewData.ImageHeight/canvas_image.ActualHeight;
                int posx = (int) (initialPoint.X*xt);
                int posy = (int) (initialPoint.Y*yt);
                SetFocusPos(posx, posy);
            }
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.LiveView);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(FocusCounter - e.NewValue) == 0)
                return;
            SetFocus((int) (e.NewValue - e.OldValue));
        }

        private void slider_transparent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(slider_transparent.Value == 1
                                  ? new Action(delegate { img_preview.Visibility = Visibility.Hidden; })
                                  : new Action(delegate { img_preview.Visibility = Visibility.Visible; }));
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CameraProperty.LiveviewSettings.CanvasHeight = slide_vert.ActualHeight;
            CameraProperty.LiveviewSettings.CanvasWidt = slide_horiz.ActualWidth;
        }
    }
}
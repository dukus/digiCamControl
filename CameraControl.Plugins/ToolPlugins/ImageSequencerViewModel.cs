using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ImageSequencerViewModel : ViewModelBase
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private BitmapSource _bitmap;
        private int _totalImages;
        private int _currentImages;
        private Window _window;
        private int _fps;
        private int _minValue;
        private int _maxValue;
        private bool _loop;
        private bool _isPaused;
        private BitmapSource _previewBitmap;
        private BitmapSource _previewBitmap10;
        private BitmapSource _previewBitmap11;
        private BitmapSource _previewBitmap12;
        private BitmapSource _previewBitmap20;
        private BitmapSource _previewBitmap21;
        private BitmapSource _previewBitmap22;


        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public BitmapSource PreviewBitmap
        {
            get { return _previewBitmap; }
            set
            {
                _previewBitmap = value;
                RaisePropertyChanged(() => PreviewBitmap);
            }
        }

        public BitmapSource PreviewBitmap10
        {
            get { return _previewBitmap10; }
            set
            {
                _previewBitmap10 = value;
                RaisePropertyChanged(() => PreviewBitmap10);
            }
        }

        public BitmapSource PreviewBitmap11
        {
            get { return _previewBitmap11; }
            set
            {
                _previewBitmap11 = value;
                RaisePropertyChanged(() => PreviewBitmap11);
            }
        }

        public BitmapSource PreviewBitmap12
        {
            get { return _previewBitmap12; }
            set
            {
                _previewBitmap12 = value;
                RaisePropertyChanged(() => PreviewBitmap12);
            }
        }

        public BitmapSource PreviewBitmap20
        {
            get { return _previewBitmap20; }
            set
            {
                _previewBitmap20 = value;
                RaisePropertyChanged(() => PreviewBitmap20);
            }
        }

        public BitmapSource PreviewBitmap21
        {
            get { return _previewBitmap21; }
            set
            {
                _previewBitmap21 = value;
                RaisePropertyChanged(() => PreviewBitmap21);
            }
        }

        public BitmapSource PreviewBitmap22
        {
            get { return _previewBitmap22; }
            set
            {
                _previewBitmap22 = value;
                RaisePropertyChanged(() => PreviewBitmap22);
            }
        }

        public int TotalImages
        {
            get { return _totalImages; }
            set
            {
                _totalImages = value;
                if (MaxValue > _totalImages)
                    MaxValue = TotalImages;
                RaisePropertyChanged(() => TotalImages);
            }
        }

        public int CurrentImages
        {
            get { return _currentImages; }
            set
            {
                _currentImages = value;
                try
                {
                    if (CurrentImages < ServiceProvider.Settings.DefaultSession.Files.Count)
                    {
                        var item = ServiceProvider.Settings.DefaultSession.Files[CurrentImages];
                        Bitmap = BitmapLoader.Instance.LoadImage(item, false);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
                RaisePropertyChanged(() => CurrentImages);
                RaisePropertyChanged(() => CounterText);
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


        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        public bool IsFree
        {
            get { return !_backgroundWorker.IsBusy; }
        }

        public string CounterText
        {
            get { return string.Format("[{0}] {1}/{2} [{3}]", MinValue, CurrentImages-MinValue, MaxValue - MinValue, MaxValue); }
        }

        public int MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                if (CurrentImages < MinValue)
                    CurrentImages = MinValue;
                PreviewBitmap = GetThubnail(MinValue);
                PreviewBitmap10 = GetThubnail(MinValue-1);
                PreviewBitmap11 = GetThubnail(MinValue);
                PreviewBitmap12 = GetThubnail(MinValue + 1);
                RaisePropertyChanged(() => MinValue);
                RaisePropertyChanged(() => CounterText);
            }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                if (CurrentImages > MaxValue)
                    CurrentImages = MaxValue;
                PreviewBitmap = GetThubnail(MaxValue);
                PreviewBitmap20 = GetThubnail(MaxValue - 1);
                PreviewBitmap21 = GetThubnail(MaxValue);
                PreviewBitmap22 = GetThubnail(MaxValue + 1);
                RaisePropertyChanged(() => MaxValue);
                RaisePropertyChanged(() => CounterText);
            }
        }

        public bool Loop
        {
            get { return _loop; }
            set
            {
                _loop = value;
                RaisePropertyChanged(() => Loop);
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;
                RaisePropertyChanged(() => IsPaused);
            }
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public RelayCommand CreateMovieCommand { get; set; }

        public RelayCommand PrevImageCommand1 { get; set; }
        public RelayCommand NextImageCommand1 { get; set; }

        public RelayCommand PrevImageCommand2 { get; set; }
        public RelayCommand NextImageCommand2 { get; set; }

        public ImageSequencerViewModel()
        {
            
        }

        public ImageSequencerViewModel(Window window)
        {
            _window = window;
            _window.Closed += _window_Closed;
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            PauseCommand = new RelayCommand(Pause);
            PrevImageCommand1 = new RelayCommand(() => MinValue--);
            PrevImageCommand2 = new RelayCommand(() => MaxValue--);
            NextImageCommand1 = new RelayCommand(() => MinValue++);
            NextImageCommand2 = new RelayCommand(() => MaxValue++);

            CreateMovieCommand = new RelayCommand(CreateMovie);
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerSupportsCancellation = true;
            Fps = 15;
            TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count - 1;
            MinValue = 0;
            MaxValue = TotalImages;
            CurrentImages = MinValue;
            ServiceProvider.Settings.DefaultSession.Files.CollectionChanged += Files_CollectionChanged;
        }

        void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // if no max value set the play the newlly captured image too
            if (TotalImages == MaxValue)
            {
                TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count - 1;
                MaxValue = TotalImages;
            }
            else
            {
                TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count - 1;
            }
        }

        private BitmapSource GetThubnail(int i)
        {
            try
            {
                if (i < 0)
                    return null;
                if (i >= ServiceProvider.Settings.DefaultSession.Files.Count)
                    return null;
                return ServiceProvider.Settings.DefaultSession.Files[i].Thumbnail;
            }
            catch (Exception)
            {
                
            }
            return null;
        }

        void _window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void Start()
        {
            CurrentImages = MinValue;
            IsPaused = false;
            _backgroundWorker.RunWorkerAsync();
        }

        private void Pause()
        {
            IsPaused = !IsPaused;
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => IsFree);
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => IsFree);
            while (true)
            {
                if (_backgroundWorker.CancellationPending)
                    break;
                if (!IsPaused)
                    CurrentImages++;
                Thread.Sleep(1000 / Fps);
                if (CurrentImages >= MaxValue)
                    if (Loop)
                        CurrentImages = MinValue;
                    else
                        Stop();
            }
        }

        private void Stop()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }

        private void CreateMovie()
        {
            GenMovieWindow window = new GenMovieWindow();
            var viewmodel = new GenMovieViewModel(window);
            viewmodel.Fps = Fps;
            viewmodel.MinValue = MinValue;
            viewmodel.MaxValue = MaxValue;
            window.DataContext = viewmodel;
            window.ShowDialog();   
        }
    }
}

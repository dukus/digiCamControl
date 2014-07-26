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
            get { return string.Format("{0}/{1}", CurrentImages, TotalImages); }
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
                RaisePropertyChanged(() => MinValue);
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
                RaisePropertyChanged(() => MaxValue);
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
            CreateMovieCommand = new RelayCommand(CreateMovie);
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerSupportsCancellation = true;
            Fps = 15;
            TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
            MinValue = 0;
            MaxValue = TotalImages;
            ServiceProvider.Settings.DefaultSession.Files.CollectionChanged += Files_CollectionChanged;
        }

        void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // if no max value set the play the newlly captured image too
            if (TotalImages == MaxValue)
            {
                TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
                MaxValue = TotalImages;
            }
            else
            {
                TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
            }
        }

        private BitmapSource GetThubnail(int i)
        {
            try
            {
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

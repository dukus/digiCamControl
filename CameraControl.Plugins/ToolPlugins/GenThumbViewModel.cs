using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class GenThumbViewModel : ViewModelBase
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private BitmapSource _bitmap;
        private int _totalImages;
        private int _currentImages;
        private Window _window;

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public int TotalImages
        {
            get { return _totalImages; }
            set
            {
                _totalImages = value;
                RaisePropertyChanged(()=>TotalImages);
            }
        }

        public int CurrentImages
        {
            get { return _currentImages; }
            set
            {
                _currentImages = value;
                RaisePropertyChanged(() => CurrentImages);
                RaisePropertyChanged(() => CounterText);
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

        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }

        public GenThumbViewModel()
        {
        }

        public GenThumbViewModel(Window window )
        {
            _window = window;
            _window.Closed += _window_Closed;
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerSupportsCancellation = true;
            Start();
        }

        void _window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void Start()
        {
            _backgroundWorker.RunWorkerAsync();
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => IsFree);
            if (_window != null)
                _window.Close();
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                RaisePropertyChanged(() => IsBusy);
                RaisePropertyChanged(() => IsFree);
                Thread.Sleep(500);
                CurrentImages = 0;
                TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
                foreach (FileItem item in ServiceProvider.Settings.DefaultSession.Files)
                {
                    if (_backgroundWorker.CancellationPending)
                        break;
                    CurrentImages++;
                    if (!item.HaveGeneratedThumbnail)
                    {
                        BitmapLoader.Instance.GenerateCache(item);
                        BitmapLoader.Instance.SetImageInfo(item);
                    }
                    Bitmap = item.Thumbnail;
                    GC.Collect();
                }
                ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to generate thumbs", ex);
            }
        }



        private void Stop()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }
    }

}

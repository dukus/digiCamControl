using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Timer = System.Timers.Timer;

namespace CameraControl.ViewModel
{
    public class MultipleLiveViewViewModel : ViewModelBase
    {
        private Timer _timer = new Timer(1000/15);

        private bool _operInProgress;
        private int _rows;
        private int _cols;
        public ObservableCollection<SimpleLiveViewViewModel> Cameras { get; set; }
        public RelayCommand StartLiveViewCommand { get; set; }
        public RelayCommand StopLiveViewCommand { get; set; }
        public RelayCommand AutoFocusCommand { get; set; }
        public RelayCommand StartRecordCommand { get; set; }
        public RelayCommand StopRecordCommand { get; set; }


        public bool OperInProgress
        {
            get { return _operInProgress; }
            set
            {
                _operInProgress = value;
                RaisePropertyChanged(() => OperInProgress);
            }
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                RaisePropertyChanged(()=>Rows);
            }
        }

        public int Cols
        {
            get { return _cols; }
            set
            {
                _cols = value;
                RaisePropertyChanged(() => Cols);
            }
        }


        public MultipleLiveViewViewModel()
        {
            InitCameras();
        }

        private void InitCameras()
        {
            Rows = 2;
            Cols = 2;

            StartLiveViewCommand = new RelayCommand(StartLiveView);
            StopLiveViewCommand = new RelayCommand(StopLiveView);
            AutoFocusCommand = new RelayCommand(AutoFocus);
            StartRecordCommand = new RelayCommand(StartRecord);
            StopRecordCommand = new RelayCommand(StopRecord);

            Cameras = new ObservableCollection<SimpleLiveViewViewModel>();
            if (ServiceProvider.DeviceManager != null)
            {
                foreach (
                    ICameraDevice device in
                        ServiceProvider.DeviceManager.ConnectedDevices.Where(
                            device => device.GetCapability(CapabilityEnum.LiveView)))
                {
                    Cameras.Add(new SimpleLiveViewViewModel(device));
                }
            }
            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            Thread thread = new Thread(GetLiveViewThread);
            thread.Start();
        }

        private void AutoFocus()
        {
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.Focus();
            }
        }

        private void GetLiveViewThread()
        {
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.Get();
            }
            _timer.Start();
        }

        private void StartLiveView()
        {
            OperInProgress = true;
            Thread thread=new Thread(StartLiveViewThread);
            thread.Start();
        }

        private void StartLiveViewThread()
        {
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.Star();
            }
            OperInProgress = false;
            _timer.Start();
        }

        private void StopLiveView()
        {
            OperInProgress = true;
            Thread thread = new Thread(StopLiveViewThread);
            thread.Start();
        }

        private void StopLiveViewThread()
        {
            _timer.Stop();
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.Stop();
            }
            OperInProgress = false;
        }

        private void StartRecord()
        {
            OperInProgress = true;
            Thread thread = new Thread(StartRecordThread);
            thread.Start();
        }

        private void StartRecordThread()
        {
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.RecordMovie();
            }
            OperInProgress = false;
            _timer.Start();
        }

        private void StopRecord()
        {
            OperInProgress = true;
            Thread thread = new Thread(StopRecordThread);
            thread.Start();
        }

        private void StopRecordThread()
        {
            _timer.Stop();
            foreach (SimpleLiveViewViewModel camera in Cameras)
            {
                camera.StopRecordMovie();
            }
            OperInProgress = false;
        }
    }
}

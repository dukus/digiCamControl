using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CameraControl.Core;
using CameraControl.Devices;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class MultipleLiveViewViewModel : ViewModelBase
    {
        public ObservableCollection<SimpleLiveViewViewModel> Cameras { get; set; }

        public MultipleLiveViewViewModel()
        {
            InitCameras();
        }

        private void InitCameras()
        {
            Cameras = new ObservableCollection<SimpleLiveViewViewModel>();
            foreach (ICameraDevice device in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                Cameras.Add(new SimpleLiveViewViewModel(device));
            }
        }

    }
}

using System;
using CameraControl.Core.Classes;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private DateTime _from;
        private DateTime _to;
        private AsyncObservableCollection<NamedValue<int>> _cameras;

        public DateTime From
        {
            get { return _from; }
            set
            {
                _from = value;
                RaisePropertyChanged(()=>From);
            }
        }

        public DateTime To
        {
            get { return _to; }
            set
            {
                _to = value;
                RaisePropertyChanged(()=>To);
            }
        }

        public AsyncObservableCollection<NamedValue<int>> Cameras
        {
            get { return _cameras; }
            set
            {
                _cameras = value;
                RaisePropertyChanged(() => Cameras);
            }
        }

        public RelayCommand RefreshCommand { get; set; }

        public StatisticsViewModel()
        {
            To = DateTime.Now;
            From = DateTime.Now.AddMonths(-1);
            RefreshCommand=new RelayCommand(Refresh);
        }

        private void Refresh()
        {
            Cameras=new AsyncObservableCollection<NamedValue<int>>();
            Cameras.Add(new NamedValue<int>("Raw", 10));
            Cameras.Add(new NamedValue<int>("Jpg", 90));
        }
    }
}

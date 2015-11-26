using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private DateTime _from;
        private DateTime _to;

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

        public RelayCommand RefreshCommand { get; set; }

        public StatisticsViewModel()
        {
            To = DateTime.Now;
            From = DateTime.Now.AddMonths(-1);
            RefreshCommand=new RelayCommand(Refresh);
        }

        private void Refresh()
        {
            
        }
    }
}

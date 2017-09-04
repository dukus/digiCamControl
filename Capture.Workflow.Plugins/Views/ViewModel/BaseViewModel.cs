using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        private ObservableCollection<FrameworkElement> _centerElements;
        private ObservableCollection<FrameworkElement> _bottomLeftElements;
        private ObservableCollection<FrameworkElement> _bottomRightElements;
        private ObservableCollection<FrameworkElement> _backGroundElements;

        public ObservableCollection<FrameworkElement> CenterElements
        {
            get { return _centerElements; }
            set
            {
                _centerElements = value;
                RaisePropertyChanged(() => CenterElements);
            }
        }

        public ObservableCollection<FrameworkElement> BottomLeftElements
        {
            get { return _bottomLeftElements; }
            set
            {
                _bottomLeftElements = value;
                RaisePropertyChanged(() => BottomLeftElements);
            }
        }

        public ObservableCollection<FrameworkElement> BottomRightElements
        {
            get { return _bottomRightElements; }
            set
            {
                _bottomRightElements = value;
                RaisePropertyChanged(() => BottomRightElements);
            }
        }

        public ObservableCollection<FrameworkElement> BackGroundElements
        {
            get { return _backGroundElements; }
            set
            {
                _backGroundElements = value;
                RaisePropertyChanged(() => BackGroundElements);
            }
        }
    }
}

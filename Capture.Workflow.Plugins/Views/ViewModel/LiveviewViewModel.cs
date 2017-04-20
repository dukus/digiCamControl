using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class LiveviewViewModel:BaseViewModel
    {
        private ObservableCollection<FrameworkElement> _leftElements;
        private ObservableCollection<FrameworkElement> _bottomLeftElements;
        private ObservableCollection<FrameworkElement> _bottomRightElements;

        public ObservableCollection<FrameworkElement> LeftElements
        {
            get { return _leftElements; }
            set
            {
                _leftElements = value;
                RaisePropertyChanged(()=>LeftElements);
            }
        }

        public ObservableCollection<FrameworkElement> BottomLeftElements
        {
            get { return _bottomLeftElements; }
            set
            {
                _bottomLeftElements = value;
                RaisePropertyChanged(()=>BottomLeftElements);
            }
        }

        public ObservableCollection<FrameworkElement> BottomRightElements
        {
            get { return _bottomRightElements; }
            set
            {
                _bottomRightElements = value;
                RaisePropertyChanged(()=>BottomRightElements);
            }
        }

        public LiveviewViewModel()
        {
            LeftElements = new ObservableCollection<FrameworkElement>();
            BottomLeftElements = new ObservableCollection<FrameworkElement>();
            BottomRightElements = new ObservableCollection<FrameworkElement>();
        }

    }
}

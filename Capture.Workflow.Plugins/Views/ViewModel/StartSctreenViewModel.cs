using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Devices.Classes;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class StartScreenViewModel: BaseViewModel
    {
        public StartScreenViewModel()
        {
            CenterElements = new ObservableCollection<FrameworkElement>();
            BottomLeftElements = new ObservableCollection<FrameworkElement>();
            BottomRightElements = new ObservableCollection<FrameworkElement>();
            BackGroundElements = new AsyncObservableCollection<FrameworkElement>();
        }
    }
}

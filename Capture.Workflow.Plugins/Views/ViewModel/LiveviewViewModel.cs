using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class LiveviewViewModel:BaseViewModel, IDisposable
    {
        private BitmapSource _bitmap;
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

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public LiveviewViewModel()
        {
            LeftElements = new ObservableCollection<FrameworkElement>();
            BottomLeftElements = new ObservableCollection<FrameworkElement>();
            BottomRightElements = new ObservableCollection<FrameworkElement>();
            WorkflowManager.Instance.Message += Instance_Message;
        }

        private void Instance_Message(object sender, MessageEventArgs e)
        {
            if (e.Name == Messages.LiveViewChanged)
            {
                var param = e.Param as object[];
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = (MemoryStream)param[0];
                bi.EndInit();
                bi.Freeze();
                var bitmap = BitmapFactory.ConvertToPbgra32Format(bi);
                bitmap.Freeze();
                Bitmap = bitmap;
            }
        }

        public void Dispose()
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}

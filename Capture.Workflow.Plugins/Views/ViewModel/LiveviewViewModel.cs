using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraControl.Devices.Classes;
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
        private AsyncObservableCollection<FileItem> _fileItems;
        private FileItem _fileItem;
        private FileItem _fileItem1;

        public WorkFlowView View { get; set; }

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

        public AsyncObservableCollection<FileItem> FileItems => WorkflowManager.Instance.FileItems;

        public FileItem FileItem
        {
            get { return _fileItem1; }
            set
            {
                _fileItem1 = value;
                RaisePropertyChanged(() => FileItem);
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
                if (param != null)
                {
                    var stream = (MemoryStream) param[0];
                    stream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = stream;
                    bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    bi.Freeze();
                    Bitmap = bi;
                }
            }
            if (e.Name == Messages.PhotoDownloaded)
            {
                RaisePropertyChanged(() => FileItems);
                FileItem item = e.Param as FileItem;
                if (item != null)
                    FileItem = item;
            }
        }


        public void Dispose()
        {
            WorkflowManager.Execute(View.GetEventCommands("UnLoad"), WorkflowManager.Instance.Context);
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}

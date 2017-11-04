using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class LiveviewViewModel:BaseViewModel, IDisposable
    {
        private ObservableCollection<FrameworkElement> _leftElements;
        private ObservableCollection<FrameworkElement> _rightElements;

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

        public ObservableCollection<FrameworkElement> RightElements
        {
            get { return _rightElements; }
            set
            {
                _rightElements = value;
                RaisePropertyChanged(() => RightElements);
            }
        }


        public BitmapSource Bitmap
        {
            get
            {
                return WorkflowManager.Instance.Bitmap;
            }
            set
            {
                WorkflowManager.Instance.Bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public AsyncObservableCollection<FileItem> FileItems => WorkflowManager.Instance.FileItems;

        public FileItem FileItem
        {
            get { return WorkflowManager.Instance.SelectedItem; }
            set
            {
                WorkflowManager.Instance.SelectedItem = value;
                RaisePropertyChanged(() => FileItem);
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ThumbCreate, null));

            }
        }


        public LiveviewViewModel()
        {
            LeftElements = new ObservableCollection<FrameworkElement>();
            BottomLeftElements = new ObservableCollection<FrameworkElement>();
            BottomRightElements = new ObservableCollection<FrameworkElement>();
            BackGroundElements = new AsyncObservableCollection<FrameworkElement>();
            RightElements = new ObservableCollection<FrameworkElement>();
            WorkflowManager.Instance.Message += Instance_Message;
        }

        private void Instance_Message(object sender, MessageEventArgs e)
        {
            switch (e.Name)
            {
                case Messages.LiveViewChanged:
                {
                    var param = e.Param as object[];
                    if (param != null)
                    {
                        var stream = e.Context.ImageStream;
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
                    break;
                case Messages.PhotoDownloaded:
                {
                    RaisePropertyChanged(() => FileItems);
                    FileItem item = e.Param as FileItem;
                    if (item != null)
                        FileItem = item;
                }
                    break;
                case Messages.ThumbUpdated:
                case Messages.NextPhoto:
                case Messages.PrevPhoto:
                case Messages.DeletePhoto:
                case Messages.ClearPhotos:
                    {
                    RaisePropertyChanged(() => FileItem);
                    RaisePropertyChanged(() => Bitmap);
                }
                    break;
            }
        }


        public void Dispose()
        {
            WorkflowManager.Execute(View.GetEventCommands("UnLoad"), WorkflowManager.Instance.Context);
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}

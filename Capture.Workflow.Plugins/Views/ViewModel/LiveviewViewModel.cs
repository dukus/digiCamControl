using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Plugins.Views.ViewModel
{
    public class LiveviewViewModel:BaseViewModel, IDisposable
    {
        private LiveViewData _liveViewData;

        private ObservableCollection<FrameworkElement> _leftElements;
        private ObservableCollection<FrameworkElement> _rightElements;
        private bool _fileListVisible;
        private BitmapSource _liveBitmap;
        private bool _preview;
        private bool _noPreview;
        private ObservableCollection<FrameworkElement> _previewRight;
        private bool _previewTabActive;
        private bool _showFocusArea;

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

        public ObservableCollection<FrameworkElement> PreviewRight
        {
            get { return _previewRight; }
            set
            {
                _previewRight = value;
                RaisePropertyChanged(() => PreviewRight);
            }
        }

        public bool ShowFocusArea
        {
            get { return _showFocusArea; }
            set
            {
                _showFocusArea = value;
                RaisePropertyChanged(() => ShowFocusArea);
            }
        }

        public bool FileListVisible
        {
            get { return _fileListVisible && Preview; }
            set
            {
                _fileListVisible = value;
                RaisePropertyChanged(() => FileListVisible);
            }
        }

        public bool Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                RaisePropertyChanged(()=>Preview);
                RaisePropertyChanged(() => NoPreview);
                RaisePropertyChanged(() => FileListVisible);
            }
        }

        public bool NoPreview => !Preview;

        public string PreviewTitle => String.Format("Preview ({0})", FileItems.Count);

        public bool PreviewTabActive
        {
            get { return _previewTabActive; }
            set
            {
                _previewTabActive = value;
                RaisePropertyChanged(() => PreviewTabActive);
            }
        }


        public BitmapSource LiveBitmap
        {
            get { return _liveBitmap; }
            set
            {
                _liveBitmap = value;
                RaisePropertyChanged(() => LiveBitmap);
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
            PreviewRight = new ObservableCollection<FrameworkElement>();
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
                        _liveViewData = (LiveViewData) param[1];
                        var stream = e.Context.ImageStream;
                        stream.Seek(0, SeekOrigin.Begin);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = stream;
                        bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.EndInit();
                        if (WorkflowManager.Instance.Context.CameraDevice.LiveViewImageZoomRatio?.Value == "All" && ShowFocusArea)
                        {
                            var writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bi);
                            DrawFocusPoint(writeableBitmap, _liveViewData);
                            writeableBitmap.Freeze();
                            LiveBitmap = writeableBitmap;
                        }
                        else
                        {
                            bi.Freeze();
                            LiveBitmap = bi;
                        }
                    }
                }
                    break;
                case Messages.PhotoDownloaded:
                {
                    RaisePropertyChanged(() => FileItems);
                    FileItem item = e.Param as FileItem;
                    if (item != null)
                        FileItem = item;
                    RaisePropertyChanged(() => Bitmap);
                    RaisePropertyChanged(() => PreviewTitle);
                    if (FileItems.Count < 2)
                        PreviewTabActive = true;
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
                    RaisePropertyChanged(() => PreviewTitle);
                }
                    break;
            }
        }

        public void SetFocusPos(Point initialPoint, double refWidth, double refHeight)
        {

            if (WorkflowManager.Instance.Context.CameraDevice.LiveViewImageZoomRatio.Value != "All" || !ShowFocusArea)
                return;
            if (_liveViewData != null)
            {
                double xt = (_liveViewData.ImageWidth) / (refWidth);
                double yt = (_liveViewData.ImageHeight) / (refHeight);
                int posx = (int) ((initialPoint.X) * xt);
                int posy = (int) ((initialPoint.Y) * yt);
                Task.Factory.StartNew(() => SetFocusPos(posx, posy));
            }
        }


        private void DrawFocusPoint(WriteableBitmap bitmap, LiveViewData liveViewData)
        {
            try
            {
                if (liveViewData == null)
                    return;
                double xt = bitmap.Width / liveViewData.ImageWidth;
                double yt = bitmap.Height / liveViewData.ImageHeight;

                bitmap.DrawRectangle((int)(liveViewData.FocusX * xt - (liveViewData.FocusFrameXSize * xt / 2)),
                    (int)(liveViewData.FocusY * yt - (liveViewData.FocusFrameYSize * yt / 2)),
                    (int)(liveViewData.FocusX * xt + (liveViewData.FocusFrameXSize * xt / 2)),
                    (int)(liveViewData.FocusY * yt + (liveViewData.FocusFrameYSize * yt / 2)),
                    liveViewData.HaveFocusData ? Colors.Green : Colors.Red);
            }
            catch (Exception exception)
            {
                Log.Error("Error draw helper lines", exception);
            }
        }

        public virtual void SetFocusPos(int x, int y)
        {
            try
            {
               WorkflowManager.Instance.Context.CameraDevice.Focus(x, y);
            }
            catch (Exception exception)
            {
                Log.Error("Error set focus pos :", exception);
            }
        }

        public void Dispose()
        {
            WorkflowManager.Execute(View.GetEventCommands("UnLoad"), WorkflowManager.Instance.Context);
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}

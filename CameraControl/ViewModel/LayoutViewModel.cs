using System;
using System.IO;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.ViewModel
{
    public class LayoutViewModel : ViewModelBase
    {
        private bool _zoomToFocus;
        private int _zoomIndex;




        public bool ZoomToFocus
        {
            get { return _zoomToFocus; }
            set
            {
                _zoomToFocus = value;
                RaisePropertyChanged(() => ZoomToFocus);
            }
        }

        public int ZoomIndex
        {
            get { return _zoomIndex; }
            set
            {
                _zoomIndex = value;
                RaisePropertyChanged(()=>ZoomIndex);
                switch (ZoomIndex)
                {
                    case 0:
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_Fit);
                        break;
                    case 1:
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_60);
                        break;
                    case 2:
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_100);
                        break;
                    case 3:
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_200);
                        break;
                }
            }
        }


        public AsyncObservableCollection<IPanelPlugin> PanelPlugins
        {
            get { return ServiceProvider.PluginManager.PanelPlugins; }
        }

        public RelayCommand NextImageCommand { get; }
        public RelayCommand PrevImageCommand { get; private set; }
        public RelayCommand OpenExplorerCommand { get; private set; }
        public RelayCommand DeleteItemCommand { get; private set; }
        public RelayCommand RestoreCommand { get; private set; }
        public RelayCommand ImageDoubleClickCommand { get; private set; }
        public RelayCommand RotateLeftCommand { get; private set; }
        public RelayCommand RotateRightCommand { get; private set; }
        public RelayCommand OpenInLightroomCommand { get; private set; }
        public RelayCommand SelectNoneCommand { get; private set; }
        public RelayCommand SelectAllCommand { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool LightroomIsInstalled { get; private set; }

        public bool PhotoshopIsInstalled { get; private set; }

        public LayoutViewModel()
        {
            if (!IsInDesignMode)
            {
                NextImageCommand =
                    new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Next_Image));
                PrevImageCommand =
                    new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Prev_Image));

                OpenExplorerCommand = new RelayCommand(OpenInExplorer);
                DeleteItemCommand = new RelayCommand(DeleteItem);
                RestoreCommand = new RelayCommand(Restore);
                OpenInLightroomCommand =
                    new RelayCommand(() => ServiceProvider.Settings.DefaultSession.OpenInLightroom(),
                        () => ServiceProvider.Settings.DefaultSession.IsAvailable("Lightroom"));
                LightroomIsInstalled = ServiceProvider.Settings.DefaultSession.IsAvailable("Lightroom");
                PhotoshopIsInstalled = ServiceProvider.Settings.DefaultSession.IsAvailable("Photoshop");

                SelectNoneCommand = new RelayCommand(() => ServiceProvider.Settings.DefaultSession.SelectNone());
                SelectAllCommand = new RelayCommand(() => ServiceProvider.Settings.DefaultSession.SelectAll());

                ImageDoubleClickCommand =
                    new RelayCommand(
                        () => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Show));

                
                ZoomIndex = 0;
                RotateLeftCommand =
                    new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.RotateLeft));
                RotateRightCommand =
                    new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.RotateRight));
            }
        }

        public void RefresZoomIndex(int index)
        {
            _zoomIndex = index;
            RaisePropertyChanged(() => ZoomIndex);
        }

        private void DeleteItem()
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Del_Image);
        }

        private void OpenInExplorer()
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ViewExplorer);
        }

        private void Restore()
        {
            if (ServiceProvider.Settings.SelectedBitmap == null ||
                ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            var item = ServiceProvider.Settings.SelectedBitmap.FileItem;
            if (File.Exists(item.BackupFileName))
            {
                try
                {
                    PhotoUtils.WaitForFile(item.FileName);
                    File.Copy(item.BackupFileName, item.FileName, true);
                    item.RemoveThumbs();
                    item.IsLoaded = false;
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Refresh_Image);
                }
                catch (Exception ex)
                {
                    Log.Error("Error restore", ex);
                }

            }
        }

    }
}

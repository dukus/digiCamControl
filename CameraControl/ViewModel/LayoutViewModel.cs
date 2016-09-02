using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;

namespace CameraControl.ViewModel
{
    public class LayoutViewModel : ViewModelBase
    {
        private bool _zoomFit;
        private bool _zoom11;
        private bool _zoom12;
        private bool _freeZoom;
        private bool _zoomToFocus;

        public bool ZoomFit
        {
            get { return _zoomFit; }
            set
            {
                if ( _zoomFit != value && value )
                {
                    _zoomFit = true;
                    Zoom11 = false;
                    Zoom12 = false;
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_Fit);
                }
                _zoomFit = value;
                RaisePropertyChanged(() => ZoomFit);
            }
        }

        public bool Zoom11
        {
            get { return _zoom11; }
            set
            {
                _zoom11 = value;
                if (value)
                {
                    ZoomFit = false;
                    Zoom12 = false;
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_100);
                }
                RaisePropertyChanged(() => Zoom11);
            }
        }

        public bool Zoom12
        {
            get { return _zoom12; }
            set
            {
                _zoom12 = value;
                if (value)
                {
                    ZoomFit = false;
                    Zoom11 = false;
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_200);
                }
                RaisePropertyChanged(() => Zoom12);
            }
        }


        public bool FreeZoom
        {
            get { return _freeZoom || Zoom12 || Zoom11; }
            set
            {
                if (value)
                {
                    ZoomFit = false;
                    Zoom11 = false;
                    Zoom12 = false;
                }
                _freeZoom = value;
            }
        }

        public bool ZoomToFocus
        {
            get { return _zoomToFocus; }
            set
            {
                _zoomToFocus = value;
                RaisePropertyChanged(() => ZoomToFocus);
            }
        }

        public AsyncObservableCollection<IPanelPlugin> PanelPlugins
        {
            get { return ServiceProvider.PluginManager.PanelPlugins; }
        }

        public RelayCommand NextImageCommand { get; private set; }
        public RelayCommand PrevImageCommand { get; private set; }
        public RelayCommand OpenExplorerCommand { get; private set; }
        public RelayCommandX DeleteItemCommand { get; private set; }
        public RelayCommand RestoreCommand { get; private set; }
        public RelayCommand ImageDoubleClickCommand { get; private set; }
        public RelayCommand RotateLeftCommand { get; private set; }
        public RelayCommand RotateRightCommand { get; private set; }
        public RelayCommand OpenInLightroomCommand { get; private set; }
        public RelayCommand SelectNoneCommand { get; private set; }
        public RelayCommand SelectAllCommand { get; private set; }

        


        public LayoutViewModel()
        {
            NextImageCommand =
                new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Next_Image));
            PrevImageCommand =
                new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Prev_Image));

            OpenExplorerCommand = new RelayCommand(OpenInExplorer);
            DeleteItemCommand = new RelayCommandX(p => DeleteItem(p), p => canDelete(p));
            RestoreCommand = new RelayCommand(Restore);
            OpenInLightroomCommand =
               new RelayCommand(() => ServiceProvider.Settings.DefaultSession.OpenInLightroom(), () => ServiceProvider.Settings.DefaultSession.IsAvailable("Lightroom"));

            SelectNoneCommand = new RelayCommand(() => ServiceProvider.Settings.DefaultSession.SelectNone());
            SelectAllCommand = new RelayCommand(() => ServiceProvider.Settings.DefaultSession.SelectAll());

            ImageDoubleClickCommand =
                new RelayCommand(
                    () => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Show));
            if (!IsInDesignMode)
            {
                ZoomFit = true;
            }
            RotateLeftCommand =
                new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.RotateLeft));
            RotateRightCommand =
                new RelayCommand(() => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.RotateRight));

        }

        private bool canDelete(object Param)
        {
            bool toReturn = true;
            if (Param == null)
                return false;
            ListBox bx = Param as ListBox;
            AsyncObservableCollection<FileItem> Files = (AsyncObservableCollection<FileItem>)bx.ItemsSource;

            if (Files == null)
                return false;

            toReturn = Files.FirstOrDefault(p => p.IsChecked) != null;

            return toReturn;
        }

        private void DeleteItem(object param)
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

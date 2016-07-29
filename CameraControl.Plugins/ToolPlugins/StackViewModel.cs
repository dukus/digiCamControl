using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.ToolPlugins
{
    public class StackViewModel : ViewModelBase
    {
        private ObservableCollection<FileItem> _files;
        private BitmapSource _previewBitmap;
        private AsyncObservableCollection<string> _output;
        private bool _isBusy;
        protected List<string> _filenames = new List<string>();
        protected bool _shouldStop;
        public string _resulfile = "";
        protected string _tempdir = "";
   
        public ObservableCollection<FileItem> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged(() => Files);
            }
        }

        public BitmapSource PreviewBitmap
        {
            get { return _previewBitmap; }
            set
            {
                _previewBitmap = value;
                RaisePropertyChanged(() => PreviewBitmap);
            }
        }

        public AsyncObservableCollection<string> Output
        {
            get { return _output; }
            set
            {
                _output = value;
                RaisePropertyChanged(() => Output);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
                RaisePropertyChanged(() => IsFree);
            }
        }

        public bool IsFree
        {
            get { return !IsBusy; }
        }

        public PhotoSession Session
        {
            get { return ServiceProvider.Settings.DefaultSession; }
        }

        public GalaSoft.MvvmLight.Command.RelayCommand<FileItem> RemoveItemCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand PreviewCommand { get; set; }
        public RelayCommand GenerateCommand { get; set; }

        public RelayCommand StopCommand { get; set; }
        public RelayCommand ConfPluginCommand { get; set; }

        public void InitCommands()
        {
            RemoveItemCommand = new GalaSoft.MvvmLight.Command.RelayCommand<FileItem>(RemoveItem);
            ReloadCommand=new RelayCommand(Reload);
        }

        private void RemoveItem(FileItem obj)
        {
            if (Files.Contains(obj))
                Files.Remove(obj);
        }

        public void LoadData()
        {
            Files = new AsyncObservableCollection<FileItem>();
            if (Session.Files.Count == 0 || ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            var files = Session.GetSelectedFiles();
            if (files.Count > 0)
            {
                Files = files;
            }
            else
            {
                Files = Session.GetSeries(ServiceProvider.Settings.SelectedBitmap.FileItem.Series);
            }
        }

        private void Reload()
        {
            LoadData();
        }

        public void OnProgressChange(string text)
        {
            if (text != null)
                Output.Insert(0, text);
        }

        public void OnActionDone()
        {
            try
            {
                if (Directory.Exists(_tempdir))
                    Directory.Delete(_tempdir, true);
                if (File.Exists(_resulfile))
                    File.Delete(_resulfile);
            }
            catch (Exception)
            {
                Log.Error("Error  delete temp folder");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace CameraControl.ViewModel
{
    public class AutoExportPluginEditViewModel : ViewModelBase
    {
        private PreviewWnd _wnd;
        private AutoExportPluginConfig _config;
        private TransformPluginItem _selectedTransformPluginItem;
        private bool _fullSize;

        public GalaSoft.MvvmLight.Command.RelayCommand<string> AddTransforPluginCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<TransformPluginItem> RemoveTransforPluginCommand { get; set; }
        public RelayCommand PreviewCommand { get; set; }
        public RelayCommand ApplyCommand { get; set; }
        
        public AutoExportPluginConfig Config
        {
            get { return _config; }
            set
            {
                _config = value;
                RaisePropertyChanged(() => Config);
            }
        }

        public string Name
        {
            get { return Config.Name; }
            set
            {
                Config.Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public bool IsEnabled
        {
            get { return Config.IsEnabled; }
            set
            {
                Config.IsEnabled = value;
                RaisePropertyChanged(()=>IsEnabled);
            }
        }

        public bool FullSize
        {
            get { return _fullSize; }
            set
            {
                _fullSize = value;
                RaisePropertyChanged(() => FullSize);
            }
        }

        public UserControl ConfigControl
        {
            get
            {
                var tp = ServiceProvider.PluginManager.GetAutoExportPlugin(Config.Type);
                if (tp != null)
                {
                    return tp.GetConfig(_config);
                }
                return null;
            }
        }

        public ObservableCollection<TransformPluginItem> TransformPluginItems
        {
            get
            {
                var list = new ObservableCollection<TransformPluginItem>();
                foreach (var enumerator in _config.ConfigDataCollection)
                {
                    list.Add(new TransformPluginItem(enumerator));
                }
                return list;
            }
        }

        public TransformPluginItem SelectedTransformPluginItem
        {
            get { return _selectedTransformPluginItem; }
            set
            {
                _selectedTransformPluginItem = value;
                RaisePropertyChanged(() => SelectedTransformPluginItem);
                RaisePropertyChanged(() => TransformControl);
            }
        }

        public UserControl TransformControl
        {
            get
            {
                if (SelectedTransformPluginItem == null)
                    return null;

                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(SelectedTransformPluginItem.Name);
                if (tp != null)
                {
                    return tp.GetConfig(SelectedTransformPluginItem.Config);
                }
                return null;
            }
        }

        public AutoExportPluginEditViewModel()
        {
            Config = new AutoExportPluginConfig() {Name = "Test"};
        }

        public AutoExportPluginEditViewModel(AutoExportPluginConfig config)
        {
            Config = config;
            AddTransforPluginCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(AddTransforPlugin);
            RemoveTransforPluginCommand =
                new GalaSoft.MvvmLight.Command.RelayCommand<TransformPluginItem>(RemoveTransforPlugin);
            PreviewCommand = new RelayCommand(Preview);
            ApplyCommand = new RelayCommand(Apply);
        }

        private void Apply()
        {
            PhotoSession.ApplyPlugin(Config);
            if (Config.IsError)
                MessageBox.Show(Config.Error);
        }

        public void Preview()
        {
            try
            {
                var outfile = Path.GetTempFileName();
                outfile =
                    AutoExportPluginHelper.ExecuteTransformPlugins(ServiceProvider.Settings.SelectedBitmap.FileItem,
                        Config,
                        outfile, !FullSize);
                if (_wnd == null || !_wnd.IsVisible )
                {
                    _wnd = new PreviewWnd();
                    _wnd.Owner = (Window) ServiceProvider.PluginManager.SelectedWindow;
                }
                _wnd.Show();
                _wnd.Image.BeginInit();
                _wnd.Image.Source = new BitmapImage(new Uri(outfile));
                _wnd.Image.EndInit();
                _wnd.ImageO.BeginInit();
                _wnd.ImageO.Source = new BitmapImage(new Uri(ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb));
                _wnd.ImageO.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error to preview filter \n" + ex.Message);
            }
            

        }

        public void AddTransforPlugin(string plugin)
        {
            var c = new ValuePairEnumerator();
            c["TransformPlugin"] = plugin;
            Config.ConfigDataCollection.Add(c);
            RaisePropertyChanged(() => TransformPluginItems);
            foreach (var item in TransformPluginItems)
            {
                if (item.Config == c)
                    SelectedTransformPluginItem = item;
            }
        }

        public void RemoveTransforPlugin(TransformPluginItem item)
        {
            Config.ConfigDataCollection.Remove(item.Config);
            RaisePropertyChanged(() => TransformPluginItems);
        }

        public List<string> ImageTransformPlugins
        {
            get
            {
                var l = ServiceProvider.PluginManager.ImageTransformPlugins.Select(x => x.Name).ToList();
                return l;
            }
        }


    }
}

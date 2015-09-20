using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class TransformPluginEditViewModel : ViewModelBase
    {
        private AutoExportPluginConfig _config;
        private TransformPluginItem _selectedTransformPluginItem;

        public GalaSoft.MvvmLight.Command.RelayCommand<string> AddTransforPluginCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<TransformPluginItem> RemoveTransforPluginCommand { get; set; }
        public RelayCommand PreviewCommand { get; set; }

        public AutoExportPluginConfig Config
        {
            get { return _config; }
            set
            {
                _config = value;
                RaisePropertyChanged(() => Config);
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

        public TransformPluginEditViewModel()
        {
            Config = new AutoExportPluginConfig() { Name = "Test" };
        }

        public TransformPluginEditViewModel(AutoExportPluginConfig config)
        {
            Config = config;
            AddTransforPluginCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(AddTransforPlugin);
            RemoveTransforPluginCommand =
                new GalaSoft.MvvmLight.Command.RelayCommand<TransformPluginItem>(RemoveTransforPlugin);
            PreviewCommand = new RelayCommand(Preview);
        }

        public void Preview()
        {
            try
            {
                var outfile = Path.GetTempFileName();
                outfile =
                    AutoExportPluginHelper.ExecuteTransformPlugins(ServiceProvider.Settings.SelectedBitmap.FileItem,
                        Config,
                        outfile, true);
                //if (_wnd == null || !_wnd.IsVisible)
                //{
                //    _wnd = new PreviewWnd();
                //    _wnd.Owner = (Window)ServiceProvider.PluginManager.SelectedWindow;
                //}
                //_wnd.Show();
                //_wnd.Image.BeginInit();
                //_wnd.Image.Source = new BitmapImage(new Uri(outfile));
                //_wnd.Image.EndInit();
                //_wnd.ImageO.BeginInit();
                //_wnd.ImageO.Source = new BitmapImage(new Uri(ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb));
                //_wnd.ImageO.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error to preview filter " + ex.Message);
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

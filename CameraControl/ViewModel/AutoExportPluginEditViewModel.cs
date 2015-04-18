using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class AutoExportPluginEditViewModel : ViewModelBase
    {
        private AutoExportPluginConfig _config;
        private ObservableCollection<TransformPluginItem> _transformPluginItems;
        private TransformPluginItem _selectedTransformPluginItem;

        public RelayCommand<string> AddTransforPluginCommand { get; set; }
        public RelayCommand<TransformPluginItem> RemoveTransforPluginCommand { get; set; }
        
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
            AddTransforPluginCommand = new RelayCommand<string>(AddTransforPlugin);
            RemoveTransforPluginCommand=new RelayCommand<TransformPluginItem>(RemoveTransforPlugin);
        }


        public void AddTransforPlugin(string plugin)
        {
            var c = new ValuePairEnumerator();
            c["TransformPlugin"] = plugin;
            Config.ConfigDataCollection.Add(c);
            RaisePropertyChanged(() => TransformPluginItems);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Plugin;
using CameraControl.Devices.Classes;
using CameraControl.PluginManager.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.PluginManager.ViewModel
{
    public class EditorViewModel: ViewModelBase
    {
        private PluginInfo _pluginInfo;
        private AsyncObservableCollection<ProjectFileItem> _files;

        public PluginInfo PluginInfo
        {
            get { return _pluginInfo; }
            set
            {
                _pluginInfo = value;
                RaisePropertyChanged(()=>PluginInfo);
            }
        }

        public AsyncObservableCollection<ProjectFileItem> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged(()=>Files);
            }
        }

        public RelayCommand AddFilesCommand { get; set; }


        public EditorViewModel()
        {
            PluginInfo = new PluginInfo();
            Files = new AsyncObservableCollection<ProjectFileItem>();
            AddFilesCommand=new RelayCommand(AddFiles);
        }

        private void AddFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    Files.Add(new ProjectFileItem(fileName));
                }
            }
        }
    }
}


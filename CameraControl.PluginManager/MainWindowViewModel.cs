using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Plugin;
using CameraControl.Devices;
using GalaSoft.MvvmLight;

namespace CameraControl.PluginManager
{
    public class MainWindowViewModel:ViewModelBase
    {
        private const string OnlineFIleList = "";
        private PluginInfo _selectedPlugin;
        private string _message;
        private int _progress;
        private WebClient _client = new WebClient();

        public ObservableCollection<PluginInfo> OnlinePluginS { get; set; }
        public ObservableCollection<PluginInfo> UpdatesPluginS { get; set; }
        public ObservableCollection<PluginInfo> InstalledPluginS { get; set; }


        public PluginInfo SelectedPlugin
        {
            get { return _selectedPlugin; }
            set
            {
                _selectedPlugin = value;
                RaisePropertyChanged(()=> SelectedPlugin);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(()=>Message);
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(()=>Progress);
            }
        }


        public MainWindowViewModel()
        {
            ServiceProvider.PluginManager = new Core.PluginManager();
            ServiceProvider.PluginManager.CopyPlugins();
            InstalledPluginS = new ObservableCollection<PluginInfo>();
            OnlinePluginS = new ObservableCollection<PluginInfo>();
            UpdatesPluginS = new ObservableCollection<PluginInfo>();
            LoadList();
        }

        public void LoadList()
        {
            InstalledPluginS.Clear();
            string[] folders = Directory.GetDirectories(ServiceProvider.PluginManager.PluginsFolder);
            foreach (string folder in folders)
            {
                string configFile = Path.Combine(folder, "dcc.plugin");
                if (File.Exists(configFile))
                {
                    try
                    {
                        PluginInfo pluginInfo = PluginInfo.Load(configFile);
                        pluginInfo.Folder = folder;
                        pluginInfo.Enabled = !File.Exists(Path.Combine(folder, "disabled"));
                        InstalledPluginS.Add(pluginInfo);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void DownloadList()
        {
            try
            {
                //_client.DownloadFileAsync();
            }
            catch (Exception ex)
            {
                Message = "Error download list " + ex.Message;
                Progress = 0;
            }
            
        }



    }
}

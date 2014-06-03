using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Plugin;
using Path = System.IO.Path;

namespace CameraControl.PluginManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public ObservableCollection<PluginInfo> PluginS { get; set; }
        private PluginInfo _selectedPlugin;
        public PluginInfo SelectedPlugin
        {
            get { return _selectedPlugin; }
            set
            {
                _selectedPlugin = value;
                NotifyPropertyChanged("SelectedPlugin");
            }
        }

        public MainWindow()
        {
            ServiceProvider.PluginManager = new Core.PluginManager();
            ServiceProvider.PluginManager.CopyPlugins();
            PluginS = new ObservableCollection<PluginInfo>();
            LoadList();
            InitializeComponent();
        }

        public void LoadList()
        {
            PluginS.Clear();
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
                        PluginS.Add(pluginInfo);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            foreach (PluginInfo pluginInfo in PluginS)
            {
                if (pluginInfo.Enabled)
                {
                    if(File.Exists(Path.Combine(pluginInfo.Folder,"disabled")))
                        File.Delete(Path.Combine(pluginInfo.Folder, "disabled"));
                }
                else
                {
                    if(!File.Exists(Path.Combine(pluginInfo.Folder,"disabled")))
                        File.CreateText(Path.Combine(pluginInfo.Folder,"disabled")).Close();
                }
            }
        }

        #region Implementation of INotifyPropertyChanged

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void CheckUpdates()
        {
            
        }

        private void btn_check_updates_Click(object sender, RoutedEventArgs e)
        {
            CheckUpdates();
        }
    }
}

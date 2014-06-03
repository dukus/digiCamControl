#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

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

#endregion

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
                    if (File.Exists(Path.Combine(pluginInfo.Folder, "disabled")))
                        File.Delete(Path.Combine(pluginInfo.Folder, "disabled"));
                }
                else
                {
                    if (!File.Exists(Path.Combine(pluginInfo.Folder, "disabled")))
                        File.CreateText(Path.Combine(pluginInfo.Folder, "disabled")).Close();
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
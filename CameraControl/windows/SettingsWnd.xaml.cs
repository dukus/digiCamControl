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

using System.Windows.Media;

#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Microsoft.Win32;
using Path = System.IO.Path;
using CameraControl.Core;
using HelpProvider = CameraControl.Classes.HelpProvider;
using MessageBox = System.Windows.MessageBox;

#endregion

//using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for SettingsWnd.xaml
    /// </summary>
    public partial class SettingsWnd
    {
        public AsyncObservableCollection<RotateFlipType> RotateFlipTypesValues { get; set; }
        public AsyncObservableCollection<string> AvailableKeys { get; set; }

        public SettingsWnd()
        {
            AvailableKeys = new AsyncObservableCollection<string>();
            InitializeComponent();
            foreach (string key in Enum.GetNames(typeof (Key)))
            {
                AvailableKeys.Add(key);
            }
            RotateFlipTypesValues =
                new AsyncObservableCollection<RotateFlipType>(
                    Enum.GetValues(typeof (RotateFlipType)).Cast<RotateFlipType>().Distinct());
            ServiceProvider.Settings.ApplyTheme(this);
            foreach (IMainWindowPlugin mainWindowPlugin in ServiceProvider.PluginManager.MainWindowPlugins)
            {
                cmb_mainwindow.Items.Add(mainWindowPlugin.DisplayName);
            }
            DataContext = ServiceProvider.Settings;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //cmb_themes.ItemsSource = ThemeManager.GetThemes();
            ServiceProvider.Settings.BeginEdit();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.Settings.CancelEdit();
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.Settings.EndEdit();
            ServiceProvider.Settings.Save();
            this.Close();
        }


        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "explorer";
                processStartInfo.UseShellExecute = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                processStartInfo.Arguments =
                    string.Format("/e,/select,\"{0}\"",
                                  Path.Combine(Settings.DataFolder, "Log", "app.log"));
                Process.Start(processStartInfo);
            }
            catch (Exception exception)
            {
                Log.Error("Error to show file in explorer", exception);
            }
        }

        private void btn_browse_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = ServiceProvider.Settings.ExternalViewer;
            if (dialog.ShowDialog() == true)
            {
                ServiceProvider.Settings.ExternalViewer = dialog.FileName;
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.Settings.ResetSettings();
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.Settings);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = ServiceProvider.Settings.ExternalViewerPath;
            if (dialog.ShowDialog() == true)
            {
                ServiceProvider.Settings.ExternalViewerPath = dialog.FileName;
            }
        }

        private void btn_add_device_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.ExternalDeviceManager.ExternalDevices.Count == 0)
            {
                MessageBox.Show("No external devices are defined");
                return;
            }
            ServiceProvider.Settings.DeviceConfigs.Items.Add(new CustomConfig()
                                                                 {
                                                                     Name = "New device config",
                                                                     DriverName =
                                                                         ServiceProvider.ExternalDeviceManager.
                                                                         ExternalDeviceNames[0]
                                                                 });
            ServiceProvider.ExternalDeviceManager.RefreshDeviceLists();
        }

        private void btn_del_device_Click(object sender, RoutedEventArgs e)
        {
            if (lst_device.SelectedItem != null)
            {
                if (
                    MessageBox.Show("Do you want to delete the selected device configuration ?", "Delete",
                                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    CustomConfig config = lst_device.SelectedItem as CustomConfig;
                    ServiceProvider.Settings.DeviceConfigs.Items.Remove(config);
                    ServiceProvider.ExternalDeviceManager.RefreshDeviceLists();
                }
            }
        }

        private void btn_clearcache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.Delete(Path.Combine(Settings.DataFolder, "Cache"), true);
            }
            catch (Exception exception)
            {
                Log.Error("Error delete cache directory", exception);
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ServiceProvider.Settings.OverlayFolder))
            {
                Directory.CreateDirectory(ServiceProvider.Settings.OverlayFolder);
            }
            PhotoUtils.Run("explorer", ServiceProvider.Settings.OverlayFolder, ProcessWindowStyle.Normal);
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhotoUtils.Run("https://www.transifex.com/projects/p/digicamcontrol/");
        }

        private void btn_browse_scriptfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tcl Script file(*.tcl)|*.tcl|All files|*.*";
            dialog.FileName = ServiceProvider.Settings.ExternalViewer;
            if (dialog.ShowDialog() == true)
            {
                ServiceProvider.Settings.StartupScript = dialog.FileName;
            }
        }

    }
}
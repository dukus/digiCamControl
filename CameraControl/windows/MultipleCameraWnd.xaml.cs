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
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using MahApps.Metro.Controls.Dialogs;
using HelpProvider = CameraControl.Core.Classes.HelpProvider;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for MultipleCameraWnd.xaml
    /// </summary>
    public partial class MultipleCameraWnd : IWindow
    {
        public bool DisbleAutofocus { get; set; }
        public int DelaySec { get; set; }
        public int WaitSec { get; set; }
        public int NumOfPhotos { get; set; }
        ProgressWindow dlg = new ProgressWindow();

        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        private int _secounter = 0;
        private int _photocounter = 0;

        public bool UseExternal { get; set; }

        public CustomConfig SelectedConfig { get; set; }

        public MultipleCameraWnd()
        {
            NumOfPhotos = 1;

            InitializeComponent();
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _secounter++;
            if (_secounter > WaitSec)
            {
                _secounter = 0;
                _timer.Stop();
                CapturePhotos();
            }
            else
            {
                StaticHelper.Instance.SystemMessage = string.Format("Waiting {0})", _secounter);
            }
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.MultipleCamera_Start:
                    Dispatcher.Invoke(() => btn_shot_Click(null, null));
                    break;
                case WindowsCmdConsts.MultipleCamera_Stop:
                    Dispatcher.Invoke(() => btn_stop_Click(null, null));
                    break;
                case WindowsCmdConsts.MultipleCamera_Reset:
                    Dispatcher.Invoke(() => btn_resetCounters_Click(null, null));
                    break;
                case WindowsCmdConsts.MultipleCameraWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                                                         Show();
                                                         Activate();
                                                         Focus();
                                                     }));
                    break;
                case WindowsCmdConsts.MultipleCameraWnd_Hide:
                    Hide();
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Hide();
                                                         Close();
                                                     }));
                    break;
            }
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MultipleCameraWnd_Hide);
            }
        }

        private void btn_shot_Click(object sender, RoutedEventArgs e)
        {
            if (UseExternal)
            {
                try
                {
                    if (SelectedConfig != null)
                        ServiceProvider.ExternalDeviceManager.AssertFocus(SelectedConfig);
                }
                catch (Exception exception)
                {
                    Log.Error("Error set focus", exception);
                    StaticHelper.Instance.SystemMessage = "Error set focus" + exception.Message;
                }
            }
            _secounter = 0;
            _photocounter = 0;
            _timer.Start();
        }

        private void CapturePhotos()
        {
            _photocounter++;
            StaticHelper.Instance.SystemMessage = string.Format("Capture started multiple cameras {0}", _photocounter);
            Thread thread = new Thread(new ThreadStart(delegate
                                                           {
                                                               while (CamerasAreBusy())
                                                               {
                                                               }
                                                               try
                                                               {
                                                                   if (UseExternal)
                                                                   {
                                                                       if (SelectedConfig != null)
                                                                       {
                                                                           ServiceProvider.ExternalDeviceManager.
                                                                               OpenShutter(SelectedConfig);
                                                                           Thread.Sleep(300);
                                                                           ServiceProvider.ExternalDeviceManager.
                                                                               CloseShutter(SelectedConfig);
                                                                       }
                                                                   }
                                                                   else
                                                                   {
                                                                       CameraHelper.CaptureAll(DelaySec);
                                                                   }
                                                               }
                                                               catch (Exception exception)
                                                               {
                                                                   Log.Error(exception);
                                                               }

                                                               Thread.Sleep(DelaySec);
                                                               if (_photocounter < NumOfPhotos)
                                                                   _timer.Start();
                                                               else
                                                               {
                                                                   StopCapture();
                                                               }
                                                           }));
            thread.Start();
        }


        private bool CamerasAreBusy()
        {
            return ServiceProvider.DeviceManager.ConnectedDevices.Aggregate(false,
                                                                            (current, connectedDevice) =>
                                                                            connectedDevice.IsBusy || current);
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _photocounter = NumOfPhotos;
            StopCapture();
        }

        private void StopCapture()
        {
            if (UseExternal && SelectedConfig != null)
            {
                ServiceProvider.ExternalDeviceManager.CloseShutter(SelectedConfig);
                ServiceProvider.ExternalDeviceManager.DeassertFocus(SelectedConfig);
            }
            StaticHelper.Instance.SystemMessage = "All captures done !";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (listBox1.SelectedItem != null)
            //    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.CameraPropertyWnd_Show,
            //                                                  listBox1.SelectedItem);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //if (listBox1.SelectedItem != null)
            //{
            //    CameraPreset preset = new CameraPreset();
            //    preset.Get((ICameraDevice) listBox1.SelectedItem);
            //    foreach (ICameraDevice connectedDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            //    {
            //        if (connectedDevice.IsConnected && connectedDevice.IsChecked)
            //            preset.Set(connectedDevice);
            //    }
            //}
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.MultipleCamera);
        }

        private void btn_resetCounters_Click(object sender, RoutedEventArgs e)
        {
            foreach (ICameraDevice connectedDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(connectedDevice);
                property.Counter = 0;
            }
        }

        private void btn_set_counter_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            if (int.TryParse(txt_counter.Text, out counter))
            {
                foreach (ICameraDevice connectedDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                {
                    CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(connectedDevice);
                    property.Counter = counter;
                }
            }
        }

        private void btn_focus_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedConfig != null)
                ServiceProvider.ExternalDeviceManager.Focus(SelectedConfig);
        }

        private void btn_capture_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedConfig != null)
                ServiceProvider.ExternalDeviceManager.Capture(SelectedConfig);
        }

        private void btn_liveview_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MultipleLiveViewWnd_Show);
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MultipleLiveViewWnd_Maximize);
        }

        private void chk_noautofocus_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Warning", "This feature not working reliable ");
        }

        private void btn_saveOrder_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ServiceProvider.DeviceManager.ConnectedDevices.Count; i++)
            {
                ServiceProvider.DeviceManager.ConnectedDevices[i].LoadProperties().SortOrder = i;
            }
            ServiceProvider.Settings.Save();
        }

        private void btn_format_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(TranslationStrings.LabelAskForDelete, "", MessageBoxButtons.YesNo) !=
                System.Windows.Forms.DialogResult.Yes)
                return;
            dlg.Show();
            Thread thread = new Thread(Format);
            thread.Start();
            Log.Debug("Start format multiple cameras");
            //thread.Join();
        }

        private void Format()
        {
            //Dispatcher.Invoke(new Action(dlg.Show));
            for (int i = 0; i < ServiceProvider.DeviceManager.ConnectedDevices.Count; i++)
            {
                ICameraDevice device = ServiceProvider.DeviceManager.ConnectedDevices[i];
                if (!device.IsChecked)
                    continue;
                dlg.Label = device.DisplayName;
                dlg.Progress = i;
                Thread thread = new Thread(() => FormatCard(device));
                thread.Start();
                thread.Join(5 * 1000);
            }
            Dispatcher.Invoke(new Action(dlg.Hide));
           
        }

        private void FormatCard(ICameraDevice connectedDevice)
        {
            try
            {
                connectedDevice.IsBusy = true;
                Log.Debug("Start format");
                Log.Debug(connectedDevice.PortName);
                connectedDevice.FormatStorage(null);
                Thread.Sleep(200);
                Log.Debug("Format done");
                connectedDevice.IsBusy = false;
            }
            catch (Exception exception)
            {
                Log.Error("Unable to format device ", exception);
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TriggerClass.KeyDown(e);
        }
    }
}
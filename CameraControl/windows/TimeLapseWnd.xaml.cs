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
using System.Windows;
using System.Windows.Forms;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Translation;
using CameraControl.Devices.Classes;
using HelpProvider = CameraControl.Classes.HelpProvider;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for TimeLapseWnd.xaml
    /// </summary>
    public partial class TimeLapseWnd
    {
        public TimeLapseWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.DefaultSession.TimeLapse.TimeLapseDone += TimeLapse_TimeLapseDone;
            ServiceProvider.Settings.ApplyTheme(this);
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                chk_noaf.IsEnabled =
                    ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureNoAf);
        }

        private void TimeLapse_TimeLapseDone(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate { btn_start.Content = TranslationStrings.ButtonStartTimeLapse; }));
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.Settings.DefaultSession.TimeLapse.IsDisabled)
            {
                ServiceProvider.Settings.DefaultSession.TimeLapse.Start();
                Dispatcher.Invoke(new Action(delegate { btn_start.Content = TranslationStrings.ButtonStopTimeLapse; }));
            }
            else
            {
                if (
                    MessageBox.Show(TranslationStrings.MsgStopTimeLapse, TranslationStrings.LabelTimeLapse,
                                    MessageBoxButtons.YesNo) ==
                    System.Windows.Forms.DialogResult.Yes)
                {
                    ServiceProvider.Settings.DefaultSession.TimeLapse.Stop();
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ServiceProvider.Settings.DefaultSession.TimeLapse.IsDisabled)
            {
                if (
                    MessageBox.Show(TranslationStrings.MsgStopTimeLapse, TranslationStrings.LabelTimeLapse,
                                    MessageBoxButtons.YesNo) ==
                    System.Windows.Forms.DialogResult.Yes)
                {
                    ServiceProvider.Settings.DefaultSession.TimeLapse.Stop();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.TimeLapse);
        }

        private void btn_stay_on_top_Click(object sender, RoutedEventArgs e)
        {
            Topmost = (btn_stay_on_top.IsChecked == true);
        }
    }
}
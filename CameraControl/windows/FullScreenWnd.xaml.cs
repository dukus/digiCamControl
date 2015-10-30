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
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for FullScreenWnd.xaml
    /// </summary>
    public partial class FullScreenWnd : IWindow
    {
        private Timer _timer = new Timer();

        public FullScreenWnd()
        {
            InitializeComponent();
            KeyDown += FullScreenWnd_KeyDown;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Hide);
        }

        private void FullScreenWnd_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Right)
            //{
            //    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Next_Image);
            //}
            //if (e.Key == Key.Left)
            //{
            //    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Prev_Image);
            //}
        }

        private void image1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Hide);
            }
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Hide);
        }

        private void image1_KeyUp(object sender, KeyEventArgs e)
        {
            //RaiseEvent(e);
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.FullScreenWnd_Show:
                    Dispatcher.BeginInvoke(new Action(delegate
                                                          {
                                                              Show();
                                                              Activate();
                                                              Topmost = true;
                                                              Topmost = false;
                                                              Focus();
                                                              if (ServiceProvider.Settings.FullScreenInSecondaryMonitor)
                                                              {
                                                                  var allScreens =
                                                                      System.Windows.Forms.Screen.AllScreens.ToList();
                                                                  foreach (var r1 in from item in allScreens where !item.Primary select item.WorkingArea)
                                                                  {
                                                                      Left = r1.Left;
                                                                      Top = r1.Top;
                                                                      Width = r1.Width;
                                                                      Height = r1.Height;
                                                                      Topmost = true;
                                                                      break;
                                                                  }
                                                              }
                                                              WindowState = WindowState.Maximized;
                                                              WindowStyle = WindowStyle.None;
                                                          }));
                    break;
                case WindowsCmdConsts.FullScreenWnd_ShowTimed:
                    Dispatcher.BeginInvoke(new Action(delegate
                                                          {
                                                              try
                                                              {
                                                                  Show();
                                                                  Activate();
                                                                  Topmost = true;
                                                                  Topmost = false;
                                                                  Focus();
                                                                  if (ServiceProvider.Settings.FullScreenInSecondaryMonitor)
                                                                  {
                                                                      var allScreens =
                                                                          System.Windows.Forms.Screen.AllScreens.ToList();
                                                                      foreach (var r1 in from item in allScreens where !item.Primary select item.WorkingArea)
                                                                      {
                                                                          Left = r1.Left;
                                                                          Top = r1.Top;
                                                                          Width = r1.Width;
                                                                          Height = r1.Height;
                                                                          Topmost = true;
                                                                          break;
                                                                      }
                                                                  }
                                                                  WindowState = WindowState.Maximized;
                                                                  WindowStyle = WindowStyle.None;

                                                                  _timer.Stop();
                                                                  _timer.Interval = ServiceProvider.Settings.PreviewSeconds *
                                                                                    1000;
                                                                  _timer.Start();
                                                              }
                                                              catch (Exception ex)
                                                              {
                                                                  Log.Error("Full screen ", ex);
                                                              }
                                                          }));
                    break;
                case WindowsCmdConsts.FullScreenWnd_Hide:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         _timer.Stop();
                                                         Hide();
                                                     }));
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         _timer.Stop();
                                                         Hide();
                                                         Close();
                                                     }));
                    break;
            }
        }

        #endregion

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Hide);
            }
        }

        private void ButtonPrev_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Prev_Image);
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Next_Image);
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TriggerClass.KeyDown(e);
        }
    }
}
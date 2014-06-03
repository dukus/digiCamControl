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

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Hide);
        }

        void FullScreenWnd_KeyDown(object sender, KeyEventArgs e)
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
                                                              magnifier.Visibility =
                                                                  ServiceProvider.Settings.ShowMagnifierInFullSccreen
                                                                      ? Visibility.Visible
                                                                      : Visibility.Hidden;
                                                       Show();
                                                       Activate();
                                                       Topmost = true;
                                                       Topmost = false;
                                                       Focus();

                                                   }));
                    break;
                case WindowsCmdConsts.FullScreenWnd_ShowTimed:
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Show();
                        Activate();
                        Topmost = true;
                        Topmost = false;
                        Focus();
                        _timer.Stop();
                        _timer.Interval = ServiceProvider.Settings.PreviewSeconds * 1000;
                        _timer.Start();
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
    }
}

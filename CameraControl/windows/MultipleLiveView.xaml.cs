using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.ViewModel;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for MultipleLiveView.xaml
    /// </summary>
    public partial class MultipleLiveView : IWindow
    {
        public MultipleLiveView()
        {
            InitializeComponent();
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.MultipleLiveViewWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        ((MultipleLiveViewViewModel)(DataContext)).InitCameras();
                        Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                        Show();
                        Activate();
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.MultipleLiveViewWnd_Hide:
                    Dispatcher.BeginInvoke(new Action(Hide));
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        Hide();
                        Close();
                    }));
                    break;
                case WindowsCmdConsts.MultipleLiveViewWnd_Maximize:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        this.WindowState=WindowState.Maximized;
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
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MultipleLiveViewWnd_Hide);
            }
        }
    }
}

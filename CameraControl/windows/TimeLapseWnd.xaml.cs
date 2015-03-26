using System;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.ViewModel;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for TimeLapseWnd.xaml
    /// </summary>
    public partial class TimeLapseWnd : IWindow
    {
        public TimeLapseWnd()
        {
            InitializeComponent();
        }

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.TimeLapseWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        if (DataContext == null)
                            DataContext = new TimelapseViewModel();
                        Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                        Show();
                        Activate();
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.TimeLapseWnd_Hide:
                    Dispatcher.Invoke(new Action(Hide));
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.TimeLapseWnd_Hide);
            }
        }
    }
}

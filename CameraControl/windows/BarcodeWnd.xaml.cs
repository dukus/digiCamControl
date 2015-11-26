using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.ViewModel;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for BarcodeWnd.xaml
    /// </summary>
    public partial class BarcodeWnd : IWindow
    {
        private Timer _timer = new Timer(1000);
        public BarcodeWnd()
        {
            InitializeComponent();
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsVisible)
                return;
            Dispatcher.Invoke(new Action(SetFocus));
        }

        private void SetFocus()
        {
            if (!((BarcodeViewModel) DataContext).KeepActive)
                return;

            if (!TextBox.IsFocused || !IsActive)
            {
                Activate();
                TextBox.Focus();
                TextBox.SelectAll();
            }
        }

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.BarcodeWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        if (DataContext == null)
                            DataContext = new BarcodeViewModel();
                        Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                        Show();
                        Activate();
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.BarcodeWnd_Hide:
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

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BarcodeWnd_Hide);
            }
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox.SelectAll();
                ((BarcodeViewModel) DataContext).EnterPressed();
            }
        }
    }
}

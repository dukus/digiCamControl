using System;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for PrintWnd.xaml
    /// </summary>
    public partial class PrintWnd : IWindow
    {
        public PrintWnd()
        {
            InitializeComponent();
        }

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.PrintWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        var data = new PrintViewModel();
                        Owner = (Window)ServiceProvider.PluginManager.SelectedWindow;
                        data.LoadPrinterSettings();
                        DataContext = data;
                        Show();
                        Activate();
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.PrintWnd_Hide:
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.PrintWnd_Hide);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((PrintViewModel)DataContext).Dlg.PrintVisual(ItemsControl, "");
            }
            catch (Exception exception)
            {
                this.ShowMessageAsync("Eror", exception.Message);
                Log.Error("Error print", exception);
            }
        }

        private void JpgButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog {Filter = "Jpg files (*.jpg)|*.jpg|All files|*.*"};
                if (dialog.ShowDialog() == true)
                {
                    BitmapLoader.Save2Jpg(
                        BitmapLoader.SaveImageSource(ItemsControl, (int) ItemsControl.Width, (int) ItemsControl.Height),
                        dialog.FileName);
                }
            }
            catch (Exception exception)
            {
                this.ShowMessageAsync("Save error", exception.Message);
                Log.Error("Save error", exception);
            }

        }

    }
}

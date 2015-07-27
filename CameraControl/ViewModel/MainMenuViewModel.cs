using System;
using System.Reflection;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.ViewModel
{
    public class MainMenuViewModel : ViewModelBase
    {
        public GalaSoft.MvvmLight.Command.RelayCommand<string> SendCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<int> ThumbSizeCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<string> SetLayoutCommand { get; set; }
        public RelayCommand SelectAllCommand { get; private set; }

        public RelayCommand SelectLiked { get; private set; }
        public RelayCommand SelectUnLiked { get; private set; }
        public RelayCommand SelectNoneCommand { get; private set; }
        public RelayCommand SelectInvertCommand { get; private set; }
        public RelayCommand SelectSeries { get; private set; }

        public MainMenuViewModel()
        {
            SendCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(Send);
            SettingsCommand = new RelayCommand(Settings);
            ThumbSizeCommand = new GalaSoft.MvvmLight.Command.RelayCommand<int>(ThumbSize);
            SetLayoutCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(SetLayout);
            SelectAllCommand =new RelayCommand(delegate { ServiceProvider.Settings.DefaultSession.SelectAll(); });
            SelectNoneCommand =new RelayCommand(delegate { ServiceProvider.Settings.DefaultSession.SelectNone(); });
            SelectLiked = new RelayCommand(delegate { ServiceProvider.Settings.DefaultSession.SelectLiked(); });
            SelectUnLiked =new RelayCommand(delegate { ServiceProvider.Settings.DefaultSession.SelectUnLiked(); });
            SelectInvertCommand =new RelayCommand(delegate { ServiceProvider.Settings.DefaultSession.SelectInver(); });
            SelectSeries =
                new RelayCommand(delegate
                {
                    try
                    {
                        ServiceProvider.Settings.DefaultSession.SelectSameSeries(
                            ServiceProvider.Settings.SelectedBitmap.FileItem.Series);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("SelectSeries", ex);
                    }
                });
        }

        private void Send(string command)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(command);
        }

        private void Settings()
        {
            SettingsWnd wnd = new SettingsWnd();
            wnd.ShowDialog();
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true);

                if (rk == null) return;

                if (ServiceProvider.Settings.StartupWithWindows)
                {
                    rk.SetValue(Core.Classes.Settings.AppName, Assembly.GetExecutingAssembly().Location);
                }
                else
                    rk.DeleteValue(Core.Classes.Settings.AppName, false);
            }
            catch (Exception ex)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, "Usable to set startup\n"+ ex.Message);
                Log.Error("Usable to set startup", ex);
            }
        }

        private void ThumbSize(int size)
        {
            ServiceProvider.Settings.ThumbHeigh = size;
        }

        private void SetLayout(string type)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.SetLayout, type);
        }
    }
}

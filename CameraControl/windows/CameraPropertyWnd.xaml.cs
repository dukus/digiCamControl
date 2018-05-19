using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.ViewModel;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for CameraPropertyWnd.xaml
    /// </summary>
    public partial class CameraPropertyWnd : IWindow, INotifyPropertyChanged
    {
        public AsyncObservableCollection<string> AvailableKeys { get; set; }

        private ICameraDevice _cameraDevice;

        public CameraProperty CameraProperty
        {
            get { return ((CameraPropertyViewModel)DataContext).CameraProperty; }
            set
            {
                ((CameraPropertyViewModel)DataContext).CameraProperty = value;
                NotifyPropertyChanged("CameraProperty");
            }
        }

        private AsyncObservableCollection<string> _cameraPresets;
        public AsyncObservableCollection<string> CameraPresets
        {
            get { return _cameraPresets; }
            set
            {
                _cameraPresets = value;
                NotifyPropertyChanged("CameraPresets");
            }
        }

        public CameraPropertyWnd()
        {
            AvailableKeys = new AsyncObservableCollection<string>();
            InitializeComponent();
            foreach (string key in Enum.GetNames(typeof(Key)))
            {
                AvailableKeys.Add(key);
            }
            CameraPresets = new AsyncObservableCollection<string>();
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.CameraPropertyWnd_Show:
                    ((CameraPropertyViewModel)DataContext).Init();

                    _cameraDevice = param as ICameraDevice;
                    if (_cameraDevice == null)
                        return;
                    CameraProperty = _cameraDevice.LoadProperties();
                    CameraProperty.BeginEdit();
                    Dispatcher.Invoke(new Action(delegate
                    {
                        Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                        Show();
                        Activate();
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.CameraPropertyWnd_Hide:
                    CameraProperty = null;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.CameraPropertyWnd_Hide);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            CameraProperty.EndEdit();
            ServiceProvider.Settings.Save();
            _cameraDevice.LoadProperties(); 
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.CameraPropertyWnd_Hide);
            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.SortCameras);
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            CameraProperty.CancelEdit();
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.CameraPropertyWnd_Hide);
        }

        private void btn_identify_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate
                                                         {
                                                             for (int i = 0; i < 5; i++)
                                                             {
                                                                 _cameraDevice.LockCamera();
                                                                 Thread.Sleep(800);
                                                                 _cameraDevice.UnLockCamera();
                                                                 Thread.Sleep(800);
                                                             }
                                                         }));
            thread.Start();
        }

        private void btn_dateTime_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                ServiceProvider.DeviceManager.SelectedCameraDevice.DateTime = DateTime.Now;
        }


    }
}

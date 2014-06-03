using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CameraControl.Actions;
using CameraControl.Actions.Enfuse;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

namespace CameraControl
{
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        private IMainWindowPlugin _basemainwindow;
        private Timer _timer = new Timer(2000);
        public StartUpWindow()
        {
            InitializeComponent();
            lbl_vers.Content = "V." + Assembly.GetExecutingAssembly().GetName().Version;
            string procName = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name

            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                MessageBox.Show(TranslationStrings.LabelApplicationAlreadyRunning);
                Close();
            }
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(InitApplication));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            //InitApplication();
            //Thread thread = new Thread(InitApplication);
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        private void InitApplication()
        {
            // prevent some application crash
            WpfCommands.DisableWpfTabletSupport();
            
            ServiceProvider.Configure();

            ServiceProvider.Settings = new Settings();
            ServiceProvider.Settings = ServiceProvider.Settings.Load();
            ServiceProvider.Branding = ServiceProvider.Settings.LoadBranding();
            if (!string.IsNullOrEmpty(ServiceProvider.Branding.StartupScreenImage) && File.Exists(ServiceProvider.Branding.StartupScreenImage))
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(ServiceProvider.Branding.StartupScreenImage);
                bi.EndInit();
                background.Source = bi;
            }
            ServiceProvider.ActionManager.Actions = new AsyncObservableCollection<IMenuAction>
                                                {
                                                  new CmdFocusStackingCombineZP(),
                                                  new CmdEnfuse(),
                                                  new CmdToJpg(),
                                                  //new CmdExpJpg()
                                                };

            if (ServiceProvider.Settings.DisableNativeDrivers && MessageBox.Show(TranslationStrings.MsgDisabledDrivers, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                ServiceProvider.Settings.DisableNativeDrivers = false;
            ServiceProvider.Settings.LoadSessionData();
            TranslationManager.LoadLanguage(ServiceProvider.Settings.SelectedLanguage);

            ServiceProvider.WindowsManager = new WindowsManager();
            ServiceProvider.WindowsManager.Add(new FullScreenWnd());
            ServiceProvider.WindowsManager.Add(new LiveViewManager());
            ServiceProvider.WindowsManager.Add(new MultipleCameraWnd());
            ServiceProvider.WindowsManager.Add(new CameraPropertyWnd());
            ServiceProvider.WindowsManager.Add(new BrowseWnd());
            ServiceProvider.WindowsManager.Add(new TagSelectorWnd());
            ServiceProvider.WindowsManager.Add(new DownloadPhotosWnd());
            ServiceProvider.WindowsManager.Add(new BulbWnd());
            ServiceProvider.WindowsManager.Add(new AstroLiveViewWnd());
            ServiceProvider.WindowsManager.Add(new ScriptWnd());
            ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            ServiceProvider.WindowsManager.ApplyTheme();
            ServiceProvider.WindowsManager.RegisterKnowCommands();
            ServiceProvider.Settings.SyncActions(ServiceProvider.WindowsManager.WindowCommands);

            ServiceProvider.Trigger.Start();
            ServiceProvider.PluginManager.CopyPlugins();
            ServiceProvider.PluginManager.LoadPlugins(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Plugins"));
            _basemainwindow = new MainWindow();
            ServiceProvider.PluginManager.MainWindowPlugins.Add(_basemainwindow);
            ServiceProvider.PluginManager.ToolPlugins.Add(new ScriptWnd());
            // event handlers
            ServiceProvider.Settings.SessionSelected += Settings_SessionSelected;
            ServiceProvider.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
            ServiceProvider.DeviceManager.CameraSelected += DeviceManager_CameraSelected;
            ServiceProvider.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
            //-------------------
            ServiceProvider.DeviceManager.DisableNativeDrivers = ServiceProvider.Settings.DisableNativeDrivers;
            if(ServiceProvider.Settings.AddFakeCamera)
                ServiceProvider.DeviceManager.AddFakeCamera();
            try
            {
                ServiceProvider.DeviceManager.ConnectToCamera();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to initialize device manager", exception);
                if (exception.Message.Contains("0AF10CEC-2ECD-4B92-9581-34F6AE0637F3"))
                {
                    MessageBox.Show(
                        "Unable to initialize device manager !\nMissing some components! Please install latest Windows Media Player! ");
                    Application.Current.Shutdown(1);
                }
            }
            Thread.Sleep(500);
            Dispatcher.Invoke(new Action(Hide));
            StartApplication();
        }

        void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
        {
            cameraDevice.CameraInitDone -= cameraDevice_CameraInitDone;
        }

        private void StartApplication()
        {
            if (!string.IsNullOrEmpty(ServiceProvider.Settings.SelectedMainForm) && ServiceProvider.Settings.SelectedMainForm != _basemainwindow.DisplayName)
            {
                SelectorWnd wnd = new SelectorWnd();
                wnd.ShowDialog();
            }
            IMainWindowPlugin mainWindowPlugin = _basemainwindow;
            foreach (IMainWindowPlugin windowPlugin in ServiceProvider.PluginManager.MainWindowPlugins)
            {
                if (windowPlugin.DisplayName == ServiceProvider.Settings.SelectedMainForm)
                    mainWindowPlugin = windowPlugin;
            }
            mainWindowPlugin.Show();
            if (mainWindowPlugin is Window)
                ((Window) mainWindowPlugin).Activate();
        }

        void WindowsManager_Event(string cmd, object o)
        {
            Log.Debug("Window command received :" + cmd);
            if (cmd == CmdConsts.All_Close)
            {
                ServiceProvider.WindowsManager.Event -= WindowsManager_Event;
                if (ServiceProvider.Settings != null)
                {
                    ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
                    ServiceProvider.Settings.Save();
                    if (ServiceProvider.Trigger != null)
                    {
                        ServiceProvider.Trigger.Stop();
                    }
                }
                ServiceProvider.ScriptManager.Stop();
                ServiceProvider.DeviceManager.CloseAll();
                Thread.Sleep(1000);
                Application.Current.Shutdown();
            }
            switch (cmd)
            {
                case CmdConsts.Capture:
                    CameraHelper.Capture();
                    break;
                case CmdConsts.CaptureNoAf:
                    CameraHelper.CaptureNoAf();
                    break;
                case CmdConsts.CaptureAll:
                    CameraHelper.CaptureAll(0);
                    break;
            }
            ICameraDevice device = ServiceProvider.DeviceManager.SelectedCameraDevice;
            if(device!=null && device.IsConnected)
            {
                switch (cmd)
                {
                    case CmdConsts.NextAperture:
                        if (device.FNumber != null)
                            device.FNumber.NextValue();
                        break;
                    case CmdConsts.PrevAperture:
                        if (device.FNumber != null)
                            device.FNumber.PrevValue();
                        break;
                    case CmdConsts.NextIso:
                        if (device.IsoNumber != null)
                            device.IsoNumber.NextValue();
                        break;
                    case CmdConsts.PrevIso:
                        if (device.IsoNumber != null)
                            device.IsoNumber.PrevValue();
                        break;
                    case CmdConsts.NextShutter:
                        if (device.ShutterSpeed != null)
                            device.ShutterSpeed.NextValue();
                        break;
                    case CmdConsts.PrevShutter:
                        if (device.ShutterSpeed != null)
                            device.ShutterSpeed.PrevValue();
                        break;
                    case CmdConsts.NextWhiteBalance:
                        if (device.WhiteBalance != null)
                            device.WhiteBalance.NextValue();
                        break;
                    case CmdConsts.PrevWhiteBalance:
                        if (device.WhiteBalance != null)
                            device.WhiteBalance.PrevValue();
                        break;
                    case CmdConsts.NextExposureCompensation:
                        if (device.ExposureCompensation != null)
                            device.ExposureCompensation.NextValue();
                        break;
                    case CmdConsts.PrevExposureCompensation:
                        if (device.ExposureCompensation != null)
                            device.ExposureCompensation.PrevValue();
                        break;
                    case CmdConsts.NextCamera:
                        ServiceProvider.DeviceManager.SelectNextCamera();
                        break;
                    case CmdConsts.PrevCamera:
                        ServiceProvider.DeviceManager.SelectPrevCamera();
                        break;
                }
            }
        }

        #region eventhandlers
        /// <summary>
        /// Called when default session is assigned or changed
        /// </summary>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        void Settings_SessionSelected(PhotoSession oldvalue, PhotoSession newvalue)
        {
            // check if same session is used 
            if (oldvalue == newvalue)
                return;
            if (oldvalue != null && ServiceProvider.Settings.PhotoSessions.Contains(oldvalue))
                ServiceProvider.Settings.Save(oldvalue);
            ServiceProvider.QueueManager.Clear();
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                ServiceProvider.DeviceManager.SelectedCameraDevice.AttachedPhotoSession = newvalue;
        }

        void DeviceManager_CameraSelected(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice)
        {
            if (newcameraDevice == null)
                return;
            var thread = new Thread(delegate()
                                               {
                                                   CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(newcameraDevice);
                                                   // load session data only if not session attached to the selected camera
                                                   if (newcameraDevice.AttachedPhotoSession == null)
                                                   {
                                                       newcameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
                                                   }
                                                   if (newcameraDevice.AttachedPhotoSession != null)
                                                       ServiceProvider.Settings.DefaultSession = (PhotoSession)newcameraDevice.AttachedPhotoSession;
                                               });
            thread.Start();
        }

        void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            cameraDevice.CameraInitDone += cameraDevice_CameraInitDone;
        }

        void cameraDevice_CameraInitDone(ICameraDevice cameraDevice)
        {
            var property = cameraDevice.LoadProperties();

            CameraPreset preset = ServiceProvider.Settings.GetPreset(property.DefaultPresetName);
            if (preset != null)
            {
                var thread = new Thread(delegate()
                {
                    try
                    {
                        Thread.Sleep(1500);
                        cameraDevice.WaitForCamera(5000);
                        preset.Set(cameraDevice);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Unable to load default preset", e);
                    }

                });
                thread.Start();
            }

            if (ServiceProvider.Settings.SyncCameraDateTime)
            {
                try
                {
                    cameraDevice.DateTime = DateTime.Now;
                }
                catch (Exception exception)
                {
                    Log.Error("Unable to sysnc date time", exception);
                }
            }            
        }

        #endregion
    }
}

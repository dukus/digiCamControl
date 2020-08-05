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
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Database;
using CameraControl.Core.Interfaces;
using CameraControl.Core.TclScripting;
using CameraControl.Core.Translation;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using CameraControl.Devices.Canon;
using CameraControl.Devices.Classes;
using CameraControl.Layouts;
using CameraControl.ViewModel;
using CameraControl.windows;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using HelpProvider = CameraControl.Core.Classes.HelpProvider;
//using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindowPlugin, INotifyPropertyChanged
    {
        public string DisplayName { get; set; }

        private object _locker = new object();
        private FileItem _selectedItem = null;
        private Timer _selectiontimer = new Timer(4000);
        private DateTime _lastLoadTime = DateTime.Now;

        private bool _sortCameraOreder = true;

        public RelayCommand<AutoExportPluginConfig> ConfigurePluginCommand { get; set; }
        public RelayCommand<IAutoExportPlugin> AddPluginCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            DisplayName = "Default";
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (sender1, args) => this.Close()));

            SelectPresetCommand = new RelayCommand<CameraPreset>(SelectPreset);
            DeletePresetCommand = new RelayCommand<CameraPreset>(DeletePreset,
                (o) => ServiceProvider.Settings.CameraPresets.Count > 0);
            LoadInAllPresetCommand = new RelayCommand<CameraPreset>(LoadInAllPreset);
            VerifyPresetCommand = new RelayCommand<CameraPreset>(VerifyPreset);
            ConfigurePluginCommand = new RelayCommand<AutoExportPluginConfig>(ConfigurePlugin);
            AddPluginCommand = new RelayCommand<IAutoExportPlugin>(AddPlugin);
            InitializeComponent();


            if (!string.IsNullOrEmpty(ServiceProvider.Branding.ApplicationTitle))
            {
                Title = ServiceProvider.Branding.ApplicationTitle;
            }
            if (!string.IsNullOrEmpty(ServiceProvider.Branding.LogoImage) &&
                File.Exists(ServiceProvider.Branding.LogoImage))
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(PhotoUtils.GetFullPath(ServiceProvider.Branding.LogoImage));
                bi.EndInit();
                Icon = bi;
            }
            _selectiontimer.Elapsed += _selectiontimer_Elapsed;
            _selectiontimer.AutoReset = false;
            ServiceProvider.WindowsManager.Event += WindowsManager_Event;

        }

        private void AddPlugin(IAutoExportPlugin obj)
        {
            ConfigurePlugin(ServiceProvider.Settings.DefaultSession.AddPlugin(obj));
        }

        private void ConfigurePlugin(AutoExportPluginConfig plugin)
        {
            var pluginEdit = new AutoExportPluginEdit
            {
                DataContext = new AutoExportPluginEditViewModel(plugin),
                Owner = this
            };
            pluginEdit.ShowDialog();
        }

        private void WindowsManager_Event(string cmd, object o)
        {
            switch (cmd)
            {
                case CmdConsts.SortCameras:
                    SortCameras();
                    break;
                case WindowsCmdConsts.MainWnd_Message:
                    this.ShowMessageAsync("", o.ToString());
                    break;
                case WindowsCmdConsts.SetLayout:
                    SetLayout(o.ToString());
                    break;
                case WindowsCmdConsts.Restore:
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        try
                        {
                            this.Show();
                            this.WindowState = WindowState.Normal;
                            this.Activate();
                            this.Focus();
                        }
                        catch (Exception e)
                        {
                            Log.Debug("Unable to restore window", e);
                        }
                    }));
                    break;
                case CmdConsts.All_Minimize:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        WindowState = WindowState.Minimized;
                    }));
                    break;
            }
        }

        private void LoadInAllPreset(CameraPreset preset)
        {
            if (preset == null)
                return;
            var dlg = new ProgressWindow();
            dlg.Show();
            try
            {
                int i = 0;
                dlg.MaxValue = ServiceProvider.DeviceManager.ConnectedDevices.Count;
                foreach (ICameraDevice connectedDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                {
                    if (connectedDevice == null || !connectedDevice.IsConnected)
                        continue;
                    try
                    {

                        dlg.Label = connectedDevice.DisplayName;
                        dlg.Progress = i;
                        i++;
                        preset.Set(connectedDevice);
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Unable to set property ", exception);
                    }
                    Thread.Sleep(250);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to set property ", exception);
            }
            dlg.Hide();
        }

        private void VerifyPreset(CameraPreset preset)
        {
            if (preset == null)
                return;
            var dlg = new ProgressWindow();
            dlg.Show();
            try
            {
                int i = 0;
                dlg.MaxValue = ServiceProvider.DeviceManager.ConnectedDevices.Count;
                foreach (ICameraDevice connectedDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                {
                    if (connectedDevice == null || !connectedDevice.IsConnected)
                        continue;
                    try
                    {
                        dlg.Label = connectedDevice.DisplayName;
                        dlg.Progress = i;
                        i++;
                        preset.Verify(connectedDevice);
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Unable to set property ", exception);
                    }
                    Thread.Sleep(250);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to set property ", exception);
            }
            dlg.Hide();
        }

        private void DeletePreset(CameraPreset obj)
        {
            if (obj == null)
                return;
            ServiceProvider.Settings.CameraPresets.Remove(obj);
            try
            {
                File.Delete(obj.FileName);
            }
            catch (Exception)
            {

            }
        }

        private void _selectiontimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_selectedItem != null)
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Select_Image, _selectedItem);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //WiaManager = new WIAManager();
            //ServiceProvider.Settings.Manager = WiaManager;
            ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;

            DataContext = ServiceProvider.Settings;
            ServiceProvider.DeviceManager.CameraSelected += DeviceManager_CameraSelected;
            ServiceProvider.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
            ServiceProvider.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
            SetLayout(ServiceProvider.Settings.SelectedLayout);
            var thread = new Thread(StartupThread);
            thread.Start();
            if (ServiceProvider.Settings.StartMinimized)
                this.WindowState = WindowState.Minimized;
            if (ServiceProvider.Settings.DefaultSession.TimeLapseSettings.Started)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.TimeLapseWnd_Show);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.TimeLapse_Start);
            }
            SortCameras();
        }

        void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!ServiceProvider.Settings.HideTrayNotifications)
                {
                    MyNotifyIcon.HideBalloonTip();
                    MyNotifyIcon.ShowBalloonTip("Camera disconnected", cameraDevice.LoadProperties().DeviceName,
                        BalloonIcon.Info);
                }
            }));
        }

        void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!ServiceProvider.Settings.HideTrayNotifications)
                {
                    MyNotifyIcon.HideBalloonTip();
                    MyNotifyIcon.ShowBalloonTip("Camera connected", cameraDevice.LoadProperties().DeviceName,
                        BalloonIcon.Info);
                }
                SortCameras();
            }));
        }

        private void StartupThread()
        {
            foreach (var cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                Log.Debug("cameraDevice_CameraInitDone 1");
                var property = cameraDevice.LoadProperties();
                CameraPreset preset = ServiceProvider.Settings.GetPreset(property.DefaultPresetName);
                // multiple canon cameras block with this settings

                if ((cameraDevice is CanonSDKBase && ServiceProvider.Settings.LoadCanonTransferMode) || !(cameraDevice is CanonSDKBase))
                    cameraDevice.CaptureInSdRam = property.CaptureInSdRam;

                Log.Debug("cameraDevice_CameraInitDone 1a");
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
                if (preset != null)
                {
                    try
                    {
                        Thread.Sleep(500);
                        cameraDevice.WaitForCamera(5000);
                        preset.Set(cameraDevice);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Unable to load default preset", e);
                    }
                }
            }
            var scriptFile = ServiceProvider.Settings.StartupScript;
            if (scriptFile != null && File.Exists(scriptFile))
            {
                if (Path.GetExtension(scriptFile.ToLower()) == ".tcl")
                {
                    try
                    {
                        var manager = new TclScripManager();
                        manager.Execute(File.ReadAllText(scriptFile));
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Script error", exception);
                        StaticHelper.Instance.SystemMessage = "Script error :" + exception.Message;
                    }
                }
                else
                {
                    var script = ServiceProvider.ScriptManager.Load(scriptFile);
                    script.CameraDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
                    ServiceProvider.ScriptManager.Execute(script);
                }
            }
            if ((DateTime.Now - ServiceProvider.Settings.LastUpdateCheckDate).TotalDays > 7)
            {
                if (!ServiceProvider.Branding.CheckForUpdate)
                    return;

                Thread.Sleep(2000);
                ServiceProvider.Settings.LastUpdateCheckDate = DateTime.Now;
                ServiceProvider.Settings.Save();
                Dispatcher.Invoke(new Action(() => NewVersionWnd.CheckForUpdate(false)));
            }
            else
            {
                if (!ServiceProvider.Branding.ShowWelcomeScreen || !ServiceProvider.Branding.OnlineReference)
                    return;

                // show welcome screen only if not start minimized
                if (!ServiceProvider.Settings.StartMinimized)
                {
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            var wnd = new Welcome();
                            wnd.ShowDialog();
                        }
                        catch
                        {
                        }
                    });
                }
            }

            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        Thread.Sleep(1500);
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Zoom_Image_Fit);
                    }));

        }

        private void DeviceManager_CameraSelected(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice)
        {
            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            Title = (ServiceProvider.Branding.ApplicationTitle ?? "digiCamControl") + " - " +
                                    (newcameraDevice == null ? "" : newcameraDevice.DisplayName);
                        }));
        }


        private void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            if (ServiceProvider.Settings.UseParallelTransfer)
            {
                PhotoCaptured(eventArgs);
            }
            else
            {
                lock (_locker)
                {
                    PhotoCaptured(eventArgs);
                }
            }
        }

        /// <summary>
        /// Photoes the captured.
        /// </summary>
        /// <param name="o">The o.</param>
        private void PhotoCaptured(object o)
        {
            PhotoCapturedEventArgs eventArgs = o as PhotoCapturedEventArgs;
            if (eventArgs == null)
                return;
            try
            {
                Log.Debug(TranslationStrings.MsgPhotoTransferBegin);
                eventArgs.CameraDevice.IsBusy = true;
                var extension = Path.GetExtension(eventArgs.FileName);

                // the capture is for live view preview 
                if (LiveViewManager.PreviewRequest.ContainsKey(eventArgs.CameraDevice) &&
                    LiveViewManager.PreviewRequest[eventArgs.CameraDevice])
                {
                    LiveViewManager.PreviewRequest[eventArgs.CameraDevice] = false;
                    var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);
                    eventArgs.CameraDevice.TransferFile(eventArgs.Handle, file);
                    eventArgs.CameraDevice.IsBusy = false;
                    eventArgs.CameraDevice.TransferProgress = 0;
                    eventArgs.CameraDevice.ReleaseResurce(eventArgs.Handle);
                    LiveViewManager.Preview[eventArgs.CameraDevice] = file;
                    LiveViewManager.OnPreviewCaptured(eventArgs.CameraDevice, file);
                    return;
                }

                CameraProperty property = eventArgs.CameraDevice.LoadProperties();
                PhotoSession session = (PhotoSession)eventArgs.CameraDevice.AttachedPhotoSession ??
                                       ServiceProvider.Settings.DefaultSession;
                StaticHelper.Instance.SystemMessage = "";


                if (!eventArgs.CameraDevice.CaptureInSdRam || PhotoUtils.IsMovie(eventArgs.FileName))
                {
                    if (property.NoDownload)
                    {
                        eventArgs.CameraDevice.IsBusy = false;
                        eventArgs.CameraDevice.ReleaseResurce(eventArgs.Handle);
                        StaticHelper.Instance.SystemMessage = "File transfer disabled";
                        return;
                    }
                    if (extension != null && (session.DownloadOnlyJpg && extension.ToLower() != ".jpg"))
                    {
                        eventArgs.CameraDevice.IsBusy = false;
                        eventArgs.CameraDevice.ReleaseResurce(eventArgs.Handle);
                        return;
                    }
                }

                StaticHelper.Instance.SystemMessage = TranslationStrings.MsgPhotoTransferBegin;

                string tempFile = Path.GetTempFileName();

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                Stopwatch stopWatch = new Stopwatch();
                // transfer file from camera to temporary folder 
                // in this way if the session folder is used as hot folder will prevent write errors
                stopWatch.Start();
                if (!eventArgs.CameraDevice.CaptureInSdRam && session.DownloadThumbOnly)
                    eventArgs.CameraDevice.TransferFileThumb(eventArgs.Handle, tempFile);
                else
                    eventArgs.CameraDevice.TransferFile(eventArgs.Handle, tempFile);
                eventArgs.CameraDevice.TransferProgress = 0;
                eventArgs.CameraDevice.IsBusy = false;
                stopWatch.Stop();
                string strTransfer = "Transfer time : " + stopWatch.Elapsed.TotalSeconds.ToString("##.###") + " Speed :" +
                                     Math.Round(
                                         new System.IO.FileInfo(tempFile).Length / 1024.0 / 1024 /
                                         stopWatch.Elapsed.TotalSeconds, 2)+" Mb/s";
                Log.Debug(strTransfer);

                string fileName = "";
                if (!session.UseOriginalFilename || eventArgs.CameraDevice.CaptureInSdRam)
                {
                    fileName =
                        session.GetNextFileName(eventArgs.FileName, eventArgs.CameraDevice, tempFile);
                }
                else
                {
                    fileName = Path.Combine(session.Folder, eventArgs.FileName);
                    if (File.Exists(fileName) && !session.AllowOverWrite)
                        fileName =
                            StaticHelper.GetUniqueFilename(
                                Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) +
                                "_", 0,
                                Path.GetExtension(fileName));
                }

                if (session.AllowOverWrite && File.Exists(fileName))
                {
                    PhotoUtils.WaitForFile(fileName);
                    File.Delete(fileName);
                }

                // make lower case extension 
                if (session.LowerCaseExtension && !string.IsNullOrEmpty(Path.GetExtension(fileName)))
                {
                    fileName = Path.Combine(Path.GetDirectoryName(fileName),
                        Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower());
                }


                if (session.AskSavePath)
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "All files|*.*";
                    dialog.Title = "Save captured photo";
                    dialog.FileName = fileName;
                    dialog.InitialDirectory = Path.GetDirectoryName(fileName);
                    if (dialog.ShowDialog() == true)
                    {
                        fileName = dialog.FileName;
                    }
                    else
                    {
                        eventArgs.CameraDevice.IsBusy = false;
                        if (File.Exists(tempFile))
                            File.Delete(tempFile);
                        return;
                    }
                }

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }


                string backupfile = null;
                if (session.BackUp)
                {
                    backupfile = session.CopyBackUp(tempFile, fileName);
                    if (string.IsNullOrEmpty(backupfile))
                        StaticHelper.Instance.SystemMessage = "Unable to save the backup";
                }

                // execute plugins which are executed before transfer 
                if (ServiceProvider.Settings.DefaultSession.AutoExportPluginConfigs.Count((x) => !x.RunAfterTransfer) > 0)
                {
                    FileItem tempitem = new FileItem(tempFile);
                    tempitem.Name = Path.GetFileName(fileName);
                    tempitem.BackupFileName = backupfile;
                    tempitem.Series = session.Series;
                    tempitem.AddTemplates(eventArgs.CameraDevice, session);
                    ExecuteAutoexportPlugins(eventArgs.CameraDevice, tempitem, false);
                }


                if ((!eventArgs.CameraDevice.CaptureInSdRam || PhotoUtils.IsMovie(fileName)) && session.DeleteFileAfterTransfer)
                    eventArgs.CameraDevice.DeleteObject(new DeviceObject() { Handle = eventArgs.Handle });

                File.Copy(tempFile, fileName);

                if (File.Exists(tempFile))
                {
                    PhotoUtils.WaitForFile(tempFile);
                    File.Delete(tempFile);
                }

                if (session.WriteComment)
                {
                    if (!string.IsNullOrEmpty(session.Comment))
                        Exiv2Helper.SaveComment(fileName, session.Comment);
                    if (session.SelectedTag1 != null && !string.IsNullOrEmpty(session.SelectedTag1.Value))
                        Exiv2Helper.AddKeyword(fileName, session.SelectedTag1.Value);
                    if (session.SelectedTag2 != null && !string.IsNullOrEmpty(session.SelectedTag2.Value))
                        Exiv2Helper.AddKeyword(fileName, session.SelectedTag2.Value);
                    if (session.SelectedTag3 != null && !string.IsNullOrEmpty(session.SelectedTag3.Value))
                        Exiv2Helper.AddKeyword(fileName, session.SelectedTag3.Value);
                    if (session.SelectedTag4 != null && !string.IsNullOrEmpty(session.SelectedTag4.Value))
                        Exiv2Helper.AddKeyword(fileName, session.SelectedTag4.Value);
                }

                if (session.ExternalData != null)
                    session.ExternalData.FileName = fileName;

                // prevent crash og GUI when item count updated
                Dispatcher.Invoke(new Action(delegate
                {
                    try
                    {
                        _selectedItem = session.GetNewFileItem(fileName);
                        _selectedItem.BackupFileName = backupfile;
                        _selectedItem.Series = session.Series;
                        // _selectedItem.Transformed = tempitem.Transformed;
                        _selectedItem.AddTemplates(eventArgs.CameraDevice, session);
                        ServiceProvider.Database.Add(new DbFile(_selectedItem, eventArgs.CameraDevice.SerialNumber, eventArgs.CameraDevice.DisplayName, session.Name));
                    }
                    catch (Exception ex)
                    {

                    }
                }));

                // execute plugins which are executed after transfer  
                ExecuteAutoexportPlugins(eventArgs.CameraDevice, _selectedItem, true);

                Dispatcher.Invoke(() =>
                {
                    _selectedItem.RemoveThumbs();
                    session.Add(_selectedItem);
                    ServiceProvider.OnFileTransfered(_selectedItem);
                });


                if (ServiceProvider.Settings.MinimizeToTrayIcon && !IsVisible && !ServiceProvider.Settings.HideTrayNotifications)
                {
                    MyNotifyIcon.HideBalloonTip();
                    MyNotifyIcon.ShowBalloonTip("Photo transfered", fileName, BalloonIcon.Info);
                }

                ServiceProvider.DeviceManager.LastCapturedImage[eventArgs.CameraDevice] = fileName;

                //select the new file only when the multiple camera support isn't used to prevent high CPU usage on raw files
                if (ServiceProvider.Settings.AutoPreview &&
                    !ServiceProvider.WindowsManager.Get(typeof(MultipleCameraWnd)).IsVisible &&
                    !ServiceProvider.Settings.UseExternalViewer)
                {
                    if ((Path.GetExtension(fileName).ToLower() == ".jpg" && ServiceProvider.Settings.AutoPreviewJpgOnly) ||
                        !ServiceProvider.Settings.AutoPreviewJpgOnly)
                    {
                        if ((DateTime.Now - _lastLoadTime).TotalSeconds < 4)
                        {
                            _selectiontimer.Stop();
                            _selectiontimer.Start();
                        }
                        else
                        {
                            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Select_Image, _selectedItem);
                        }
                    }
                }
                _lastLoadTime = DateTime.Now;
                //ServiceProvider.Settings.Save(session);
                StaticHelper.Instance.SystemMessage = TranslationStrings.MsgPhotoTransferDone + " "+ strTransfer;

                if (ServiceProvider.Settings.UseExternalViewer &&
                    File.Exists(ServiceProvider.Settings.ExternalViewerPath))
                {
                    string arg = ServiceProvider.Settings.ExternalViewerArgs;
                    arg = arg.Contains("%1") ? arg.Replace("%1", fileName) : arg + " " + fileName;
                    PhotoUtils.Run(ServiceProvider.Settings.ExternalViewerPath, arg, ProcessWindowStyle.Normal);
                }
                if (ServiceProvider.Settings.PlaySound)
                {
                    PhotoUtils.PlayCaptureSound();
                }
                eventArgs.CameraDevice.ReleaseResurce(eventArgs.Handle);
                
                //show fullscreen only when the multiple camera support isn't used
                if (ServiceProvider.Settings.Preview &&
                    !ServiceProvider.WindowsManager.Get(typeof(MultipleCameraWnd)).IsVisible &&
                    !ServiceProvider.Settings.UseExternalViewer)

                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_ShowTimed);

                Log.Debug("Photo transfer done.");
            }
            catch (Exception ex)
            {
                eventArgs.CameraDevice.IsBusy = false;
                StaticHelper.Instance.SystemMessage = TranslationStrings.MsgPhotoTransferError + " " + ex.Message;
                Log.Error("Transfer error !", ex);
            }
            // not indicated to be used 
            GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        private void ExecuteAutoexportPlugins(ICameraDevice cameraDevice, FileItem fileItem, bool runOnTransfered)
        {
            foreach (AutoExportPluginConfig plugin in ServiceProvider.Settings.DefaultSession.AutoExportPluginConfigs)
            {
                if (plugin.RunAfterTransfer != runOnTransfered)
                    continue;
                if (!plugin.IsEnabled)
                    continue;
                if (!plugin.Evaluate(cameraDevice))
                    continue;

                var pl = ServiceProvider.PluginManager.GetAutoExportPlugin(plugin.Type);
                try
                {
                    pl.Execute(fileItem, plugin);
                    ServiceProvider.Analytics.PluginExecute(plugin.Type);
                    Log.Debug("AutoexportPlugin executed " + plugin.Type);
                }
                catch (Exception ex)
                {
                    plugin.IsError = true;
                    plugin.Error = ex.Message;
                    plugin.IsRedy = true;
                    Log.Error("Error to apply plugin", ex);
                }
            }
        }

        public RelayCommand<CameraPreset> SelectPresetCommand { get; private set; }
        public RelayCommand<CameraPreset> DeletePresetCommand { get; private set; }
        public RelayCommand<CameraPreset> LoadInAllPresetCommand { get; private set; }
        public RelayCommand<CameraPreset> VerifyPresetCommand { get; private set; }


        private void SelectPreset(CameraPreset preset)
        {
            if (preset == null)
                return;
            try
            {
                preset.Set(ServiceProvider.DeviceManager.SelectedCameraDevice);
            }
            catch (Exception exception)
            {
                Log.Error("Error set preset", exception);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.DeviceManager.SelectedCameraDevice == null)
                return;
            Log.Debug("Main window capture started");
            try
            {
                if (ServiceProvider.DeviceManager.SelectedCameraDevice.ShutterSpeed != null &&
                    ServiceProvider.DeviceManager.SelectedCameraDevice.ShutterSpeed.Value == "Bulb")
                {
                    if (ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.Bulb))
                    {
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BulbWnd_Show,
                                                                      ServiceProvider.DeviceManager.SelectedCameraDevice);
                        return;
                    }
                    else
                    {
                        this.ShowMessageAsync("Error", TranslationStrings.MsgBulbModeNotSupported);
                        return;
                    }
                }
                //CameraHelper.Capture(ServiceProvider.DeviceManager.SelectedCameraDevice);
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.Capture);
            }
            catch (DeviceException exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Take photo", exception);
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Take photo", exception);
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.All_Close);
        }

        private void but_timelapse_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.TimeLapseWnd_Show);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ServiceProvider.Settings.MinimizeToTrayIcon)
            {
                WindowState = WindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void but_fullscreen_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FullScreenWnd_Show);
        }

        private void btn_br_Click(object sender, RoutedEventArgs e)
        {
            BraketingWnd wnd = new BraketingWnd(ServiceProvider.DeviceManager.SelectedCameraDevice,
                                                ServiceProvider.Settings.DefaultSession);
            wnd.ShowDialog();
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BrowseWnd_Show);
        }

        private void SetLayout(string enumname)
        {
            LayoutTypeEnum type;
            if (Enum.TryParse(enumname, true, out type))
            {
                SetLayout(type);
            }
        }

        private void SetLayout(LayoutTypeEnum type)
        {
            ServiceProvider.Settings.SelectedLayout = type.ToString();
            if (StackLayout.Children.Count > 0)
            {
                var cnt = StackLayout.Children[0] as LayoutBase;
                if (cnt != null)
                    cnt.UnInit();
            }
            switch (type)
            {
                case LayoutTypeEnum.Normal:
                    {
                        StackLayout.Children.Clear();
                        LayoutNormal control = new LayoutNormal();
                        StackLayout.Children.Add(control);
                    }
                    break;
                case LayoutTypeEnum.Grid:
                    {
                        StackLayout.Children.Clear();
                        LayoutGrid control = new LayoutGrid();
                        StackLayout.Children.Add(control);
                    }
                    break;
                case LayoutTypeEnum.GridRight:
                    {
                        StackLayout.Children.Clear();
                        LayoutGridRight control = new LayoutGridRight();
                        StackLayout.Children.Add(control);
                    }
                    break;
            }
        }


        private void but_download_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.DownloadPhotosWnd_Show,
                                                          ServiceProvider.DeviceManager.SelectedCameraDevice);
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (txt_CaptureName.IsFocused)
                return;
            TriggerClass.KeyDown(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MultipleCameraWnd_Show);
        }

        private void btn_sort_Click(object sender, RoutedEventArgs e)
        {
            SortCameras(true);
        }

        private void btn_sort_desc_Click(object sender, RoutedEventArgs e)
        {
            SortCameras(false);
        }

        private void SortCameras()
        {
            SortCameras(_sortCameraOreder);
        }

        private void SortCameras(bool asc)
        {
            _sortCameraOreder = asc;

            // making sure the camera names are refreshed from properties
            foreach (var device in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                device.LoadProperties();
            }
            if (asc)
            {
                ServiceProvider.DeviceManager.ConnectedDevices =
                    new AsyncObservableCollection<ICameraDevice>(
                        ServiceProvider.DeviceManager.ConnectedDevices.OrderBy(x => x.LoadProperties().SortOrder).ThenBy(x => x.DisplayName));
            }
            else
            {
                ServiceProvider.DeviceManager.ConnectedDevices =
                    new AsyncObservableCollection<ICameraDevice>(
                        ServiceProvider.DeviceManager.ConnectedDevices.OrderByDescending(x => x.LoadProperties().SortOrder).ThenByDescending(x => x.DisplayName));
            }
        }

        private void but_star_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BulbWnd_Show,
                                                          ServiceProvider.DeviceManager.SelectedCameraDevice);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Del_Image);
                e.Handled = true;
            }
        }


        #region Implementation of INotifyPropertyChanged

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void but_wifi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ServiceProvider.Settings.WifiIp))
                    ServiceProvider.Settings.WifiIp = "192.168.1.1";
                var wnd = new GetIpWnd();
                wnd.Owner = this;
                if (wnd.ShowDialog() == true)
                {
                    ServiceProvider.DeviceManager.AddDevice(wnd.WifiDeviceProvider.Connect(wnd.Ip));
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect to WiFi device", exception);
                this.ShowMessageAsync("Error", "Unable to connect to WiFi device " + exception.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CameraPreset cameraPreset = new CameraPreset();
            SavePresetWnd wnd = new SavePresetWnd(cameraPreset);
            wnd.Owner = this;
            if (wnd.ShowDialog() == true)
            {
                foreach (CameraPreset preset in ServiceProvider.Settings.CameraPresets)
                {
                    if (preset.Name == cameraPreset.Name)
                    {
                        cameraPreset = preset;
                        break;
                    }
                }
                cameraPreset.Get(ServiceProvider.DeviceManager.SelectedCameraDevice);
                if (!ServiceProvider.Settings.CameraPresets.Contains(cameraPreset))
                    ServiceProvider.Settings.CameraPresets.Add(cameraPreset);
                ServiceProvider.Settings.Save();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PresetEditWnd wnd = new PresetEditWnd();
            wnd.Owner = this;
            wnd.ShowDialog();
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && ServiceProvider.Settings.MinimizeToTrayIcon &&
                !ServiceProvider.Settings.HideTrayNotifications)
            {
                this.Hide();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MyNotifyIcon.HideBalloonTip();
                    MyNotifyIcon.ShowBalloonTip("digiCamControl", "Application was minimized \n Double click to restore",
                        BalloonIcon.Info);
                }));
            }
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void but_print_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.PrintWnd_Show);
        }

        private void but_qr_Click(object sender, RoutedEventArgs e)
        {
            QrCodeWnd wnd = new QrCodeWnd();
            wnd.Owner = this;
            wnd.Show();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(e.Delta > 0
                ? WindowsCmdConsts.Next_Image
                : WindowsCmdConsts.Prev_Image);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var res = e.GetPosition(PrviewImage);
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ZoomPoint + "_" +
                                                          (res.X / PrviewImage.ActualWidth) + "_" +
                                                          (res.Y / PrviewImage.ActualHeight));
        }

        private void PrviewImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var res = e.GetPosition(PrviewImage);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ZoomPoint + "_" +
                                                              (res.X / PrviewImage.ActualWidth) + "_" +
                                                              (res.Y / PrviewImage.ActualHeight) + "_!");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ShowInputAsync(TranslationStrings.LabelEmailPublicWebAddress, "Email").ContinueWith(s =>
            {
                if (!string.IsNullOrEmpty(s.Result))
                    HelpProvider.SendEmail(
                        "digiCamControl public web address " + ServiceProvider.Settings.PublicWebAdress,
                        "digiCamControl public web address ", "postmaster@digicamcontrol.com", s.Result);
            }
                );

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.RefreshDisplay);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            EditSession();
        }

        private void EditSession()
        {
            try
            {
                EditSession editSession = new EditSession(ServiceProvider.Settings.DefaultSession);
                editSession.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                editSession.ShowDialog();
                ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
            }
            catch (Exception ex)
            {
                Log.Error("Error refresh session ", ex);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, ex.Message);
            }
        }

    }
}
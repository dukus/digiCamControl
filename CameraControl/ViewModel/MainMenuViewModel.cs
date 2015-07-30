using System;
using System.IO;
using System.Reflection;
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.ViewModel
{
    public class MainMenuViewModel : ViewModelBase
    {
        private bool _showFocusPoints;
        private bool _flipPreview;
        private Branding _branding;
        public GalaSoft.MvvmLight.Command.RelayCommand<string> SendCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<int> ThumbSizeCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand<string> SetLayoutCommand { get; set; }

        public RelayCommand ToggleFocusCommand { get; set; }

        public RelayCommand NewSessionCommand { get; private set; }
        public RelayCommand EditSessionCommand { get; private set; }
        public RelayCommand DelSessionCommand { get; private set; }

        public RelayCommand RefreshSessionCommand { get; private set; }
        public RelayCommand ShowSessionCommand { get; private set; }

        public RelayCommand SelectLiked { get; private set; }
        public RelayCommand SelectUnLiked { get; private set; }
        public RelayCommand SelectNoneCommand { get; private set; }
        public RelayCommand SelectInvertCommand { get; private set; }
        public RelayCommand SelectSeries { get; private set; }
        public RelayCommand SelectAllCommand { get; private set; }
        
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand FlipPreviewCommand { get; set; }

        public RelayCommand ManualPageCommand { get; set; }
        public RelayCommand HomePageCommand { get; set; }
        public RelayCommand CheckUpdateCommand { get; set; }
        public RelayCommand ForumCommand { get; set; }
        public RelayCommand SendLogFileCommand { get; set; }
        public RelayCommand ShowChangeLogCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }

        public bool ShowFocusPoints
        {
            get { return ServiceProvider.Settings.ShowFocusPoints; }
            set
            {
                ServiceProvider.Settings.ShowFocusPoints = value;
                RaisePropertyChanged(() => ShowFocusPoints);
            }
        }

        public bool FlipPreview
        {
            get { return ServiceProvider.Settings.FlipPreview; }
            set
            {
                ServiceProvider.Settings.FlipPreview = value;
                RaisePropertyChanged(()=>FlipPreview);
            }
        }

        public Branding Branding
        {
            get { return ServiceProvider.Branding; }
        }

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
            NewSessionCommand = new RelayCommand(NewSession);
            EditSessionCommand = new RelayCommand(EditSession);
            DelSessionCommand=new RelayCommand(DelSession);
            RefreshSessionCommand = new RelayCommand(RefreshSession);
            ShowSessionCommand = new RelayCommand(ShowSession);
            RefreshCommand = new RelayCommand(Refresh);

            ToggleFocusCommand = new RelayCommand(() => ShowFocusPoints = !ShowFocusPoints);
            FlipPreviewCommand = new RelayCommand(() => FlipPreview = !FlipPreview);
            HomePageCommand = new RelayCommand(() => PhotoUtils.Run("http://www.digicamcontrol.com/", ""));
            CheckUpdateCommand = new RelayCommand(() => NewVersionWnd.CheckForUpdate(true));
            ForumCommand = new RelayCommand(() => PhotoUtils.Run("http://www.digicamcontrol.com/forum/", ""));
            SendLogFileCommand = new RelayCommand(() => new ErrorReportWnd("Log file").ShowDialog());
            ShowChangeLogCommand = new RelayCommand(NewVersionWnd.ShowChangeLog);
            AboutCommand = new RelayCommand(() => new AboutWnd().ShowDialog());
            ManualPageCommand = new RelayCommand(() => HelpProvider.Run(HelpSections.MainMenu));
        }

        private void NewSession()
        {
            try
            {
                var defaultsessionfile = Path.Combine(Core.Classes.Settings.SessionFolder, "Default.xml");
                var session = new PhotoSession();
                // copy session with default name
                if (File.Exists(defaultsessionfile))
                {
                    session = ServiceProvider.Settings.LoadSession(defaultsessionfile);
                    session.Files.Clear();
                }
                var editSession = new EditSession(session);
                editSession.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                ServiceProvider.Settings.ApplyTheme(editSession);
                if (editSession.ShowDialog() == true)
                {
                    ServiceProvider.Settings.Add(editSession.Session);
                    ServiceProvider.Settings.DefaultSession = editSession.Session;
                }   
            }
            catch (Exception ex)
            {
                Log.Error("Error create session ", ex);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, "Error create session " + ex.Message);
            }
        }

        private void EditSession()
        {
            try
            {
                EditSession editSession = new EditSession(ServiceProvider.Settings.DefaultSession);
                editSession.Owner = ServiceProvider.PluginManager.SelectedWindow as Window; ;
                ServiceProvider.Settings.ApplyTheme(editSession);
                editSession.ShowDialog();
                ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
            }
            catch (Exception ex)
            {
                Log.Error("Error refresh session ", ex);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, ex.Message);
            }
        }

        private void DelSession()
        {
            if (ServiceProvider.Settings.PhotoSessions.Count > 1)
            {
                try
                {
                    if (
                        MessageBox.Show(
                            string.Format(TranslationStrings.MsgDeleteSessionQuestion,
                                          ServiceProvider.Settings.DefaultSession.Name),
                            TranslationStrings.LabelDeleteSession, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        PhotoSession session = ServiceProvider.Settings.DefaultSession;
                        if (!string.IsNullOrEmpty(session.ConfigFile) && File.Exists(session.ConfigFile))
                            File.Delete(session.ConfigFile);
                        ServiceProvider.Settings.PhotoSessions.Remove(session);
                        ServiceProvider.Settings.DefaultSession = ServiceProvider.Settings.PhotoSessions[0];
                        ServiceProvider.Settings.Save();
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Unable to remove session", exception);
                    ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, TranslationStrings.LabelUnabletoDeleteSession + exception.Message);
                }
            }
            else
            {
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, TranslationStrings.MsgLastSessionCantBeDeleted);
            }
        }

        private void RefreshSession()
        {
            try
            {
                ServiceProvider.Settings.LoadData(ServiceProvider.Settings.DefaultSession);
            }
            catch (Exception ex)
            {
                Log.Error("Error refresh session ",ex);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, ex.Message);
            }
        }

        private void ShowSession()
        {
            try
            {
                PhotoUtils.Run(ServiceProvider.Settings.DefaultSession.Folder);
            }
            catch (Exception ex)
            {
                Log.Error("Error refresh session ", ex);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, ex.Message);
            }
        }

        private void Refresh()
        {
            try
            {
                ServiceProvider.DeviceManager.ConnectToCamera();
            }
            catch (Exception exception)
            {
                Log.Error("Error to connect", exception);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.MainWnd_Message, "Unable to connect \n" + exception.Message);
            }
        }

        private void Send(string command)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(command);
        }

        private void Settings()
        {
            SettingsWnd wnd = new SettingsWnd();
            wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
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

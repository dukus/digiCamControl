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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using MahApps.Metro;
using MaterialDesignColors;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class Settings : BaseFieldClass
    {
        public static string AppName = "digiCamControl";
        private string ConfigFile = "";

        private PhotoSession _defaultSession;

        [XmlIgnore]
        [JsonIgnore]
        public PhotoSession DefaultSession
        {
            get { return _defaultSession; }
            set
            {
                if (value != null)
                {
                    PhotoSession oldvalue = _defaultSession;
                    _defaultSession = value;
                    DefaultSessionName = _defaultSession.Name;
                    var thread = new Thread(new ThreadStart(delegate
                                                                {
                                                                    if (SessionSelected != null)
                                                                        SessionSelected(oldvalue, value);
                                                                    LoadData(_defaultSession);
                                                                    NotifyPropertyChanged("DefaultSession");
                                                                }));
                    thread.Start();
                }
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public ObservableCollection<PhotoSession> PhotoSessions { get; set; }

        private BitmapFile _selectedBitmap;

        [XmlIgnore]
        [JsonIgnore]
        public BitmapFile SelectedBitmap
        {
            get { return _selectedBitmap; }
            set
            {
                _selectedBitmap = value;
                NotifyPropertyChanged("SelectedBitmap");
            }
        }

        private bool _imageLoading;

        [XmlIgnore]
        [JsonIgnore]
        public bool ImageLoading
        {
            get { return _imageLoading; }
            set
            {
                _imageLoading = value;
                NotifyPropertyChanged("ImageLoading");
            }
        }

        private AsyncObservableCollection<WindowCommandItem> _actions;

        public AsyncObservableCollection<WindowCommandItem> Actions
        {
            get { return _actions; }
            set
            {
                _actions = value;
                NotifyPropertyChanged("Actions");
            }
        }

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                NotifyPropertyChanged("SelectedLanguage");
            }
        }

        private bool _disableNativeDrivers;

        public bool DisableNativeDrivers
        {
            get { return _disableNativeDrivers; }
            set
            {
                _disableNativeDrivers = value;
                NotifyPropertyChanged("DisableNativeDrivers");
            }
        }

        private string _currentThemeName;

        public string CurrentThemeName
        {
            get
            {
                if (!ServiceProvider.Branding.UseThemeSelector)
                    return ServiceProvider.Branding.DefaultTheme;
                return _currentThemeName;
            }
            set
            {
                _currentThemeName = value;
                NotifyPropertyChanged("CurrentThemeName");
                if (ServiceProvider.WindowsManager != null)
                    ServiceProvider.WindowsManager.ApplyTheme();
            }
        }

        public string CurrentThemeNameNew
        {
            get
            {
                if (!ServiceProvider.Branding.UseThemeSelector)
                    return ServiceProvider.Branding.DefaultTheme;

                return _currentThemeNameNew;
            }
            set
            {
                _currentThemeNameNew = value;
                NotifyPropertyChanged("CurrentThemeNameNew");
            }
        }


        private int _liveViewFreezeTimeOut;

        public int LiveViewFreezeTimeOut
        {
            get { return _liveViewFreezeTimeOut; }
            set
            {
                _liveViewFreezeTimeOut = value;
                NotifyPropertyChanged("LiveViewFreezeTimeOut");
            }
        }

        private bool _previewLiveViewImage;

        public bool PreviewLiveViewImage
        {
            get { return _previewLiveViewImage; }
            set
            {
                _previewLiveViewImage = value;
                NotifyPropertyChanged("PreviewLiveViewImage");
            }
        }

        public DateTime LastUpdateCheckDate { get; set; }

        private bool _useWebserver;

        public bool UseWebserver
        {
            get { return _useWebserver; }
            set
            {
                _useWebserver = value;
                NotifyPropertyChanged("UseWebserver");
            }
        }

        private int _webserverPort;

        public int WebserverPort
        {
            get { return _webserverPort; }
            set
            {
                _webserverPort = value;
                NotifyPropertyChanged("WebserverPort");
                NotifyPropertyChanged("Webaddress");
            }
        }

        public bool AllowWebserverActions
        {
            get { return _allowWebserverActions; }
            set
            {
                _allowWebserverActions = value;
                NotifyPropertyChanged("AllowWebserverActions");
            }
        }

        public bool PublicWebserver
        {
            get { return _publicWebserver; }
            set
            {
                _publicWebserver = value;
                NotifyPropertyChanged("PublicWebserver");
            }
        }

        private bool _playSound;

        public bool PlaySound
        {
            get { return _playSound; }
            set
            {
                _playSound = value;
                NotifyPropertyChanged("PlaySound");
            }
        }

        private bool _showFocusPoints;

        public bool ShowFocusPoints
        {
            get { return _showFocusPoints; }
            set
            {
                _showFocusPoints = value;
                NotifyPropertyChanged("ShowFocusPoints");
            }
        }

        private string _defaultSessionName;

        public string DefaultSessionName
        {
            get { return _defaultSessionName; }
            set
            {
                _defaultSessionName = value;
                NotifyPropertyChanged("DefaultSessionName");
            }
        }

        [JsonIgnore]
        [XmlIgnore]
        public Branding Branding => ServiceProvider.Branding;

        public List<string> AvaiableWebAddresses
        {
            get
            {
                var res = new List<string>();
                try
                {
                    //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                    //var hostEntry = Dns.GetHostEntry(localAddr);
                    //var ips = (
                    //              from addr in hostEntry.AddressList
                    //              where addr.AddressFamily.ToString() == "InterNetwork"
                    //              select addr.ToString()
                    //          ).ToList();
                    string sHostName = Dns.GetHostName();
                    IPAddress[] ipA =
                        Dns.GetHostAddresses(sHostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToArray();
                    res.AddRange(ipA.Select(ip => string.Format("http://{0}:{1}", ip, WebserverPort)));
                    if (PublicWebserver)
                    {
                        res.Add(PublicWebAdress);
                        _webaddress = PublicWebAdress;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Error get web address ", exception);
                }
                return res;
            }
        }

        public string PublicWebAdress
        {
            get { return "http://digicamcontrol.com/remote/" + ClientId; }
        }

        private string _webaddress;

        public string Webaddress
        {
            get
            {
                if ((_webaddress == null || !AvaiableWebAddresses.Contains(_webaddress)) &&
                    AvaiableWebAddresses.Count > 0)
                {
                    return AvaiableWebAddresses[0];
                }
                return _webaddress;
            }
            set
            {
                _webaddress = value;
                NotifyPropertyChanged("Webaddress");
            }
        }

        private bool _preview;

        /// <summary>
        /// preview in full screen
        /// </summary>
        public bool Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                NotifyPropertyChanged("Preview");
            }
        }

        private Color _fullScreenColor;

        public Color FullScreenColor
        {
            get { return _fullScreenColor; }
            set
            {
                _fullScreenColor = value;
                NotifyPropertyChanged("FullScreenColor");
            }
        }

        public string FullScreenPassword
        {
            get { return _fullScreenPassword; }
            set
            {
                _fullScreenPassword = value;
                NotifyPropertyChanged("FullScreenPassword");
            }
        }


        public bool ShowFullscreenControls
        {
            get { return _hideFullscreenControls; }
            set
            {
                _hideFullscreenControls = value;
                NotifyPropertyChanged("ShowFullscreenControls");
            }
        }

        private int _previewSeconds;

        public int PreviewSeconds
        {
            get { return _previewSeconds; }
            set
            {
                _previewSeconds = value;
                NotifyPropertyChanged("PreviewSeconds");
            }
        }

        private bool _autoPreview;

        public bool AutoPreview
        {
            get { return _autoPreview; }
            set
            {
                _autoPreview = value;
                NotifyPropertyChanged("AutoPreview");
            }
        }

        private int _rotateIndex;

        public int RotateIndex
        {
            get { return _rotateIndex; }
            set
            {
                _rotateIndex = value;
                NotifyPropertyChanged("RotateIndex");
            }
        }

        public int SmallFocusStepCanon
        {
            get { return _smallFocusStepCanon; }
            set
            {
                _smallFocusStepCanon = value;
                NotifyPropertyChanged("SmallFocusStepCanon");
            }
        }

        public int MediumFocusStepCanon
        {
            get { return _mediumFocusStepCanon; }
            set
            {
                _mediumFocusStepCanon = value;
                NotifyPropertyChanged("MediumFocusStepCanon");
            }
        }

        public int LargeFocusStepCanon
        {
            get { return _largeFocusStepCanon; }
            set
            {
                _largeFocusStepCanon = value;
                NotifyPropertyChanged("LargeFocusStepCanon");
            }
        }

        public int CanonFocusStepWait
        {
            get { return _canonFocusStepWait; }
            set
            {
                _canonFocusStepWait = value;
                NotifyPropertyChanged("CanonFocusStepWait");
            }
        }


        private int _smalFocusStep;

        public int SmalFocusStep
        {
            get { return _smalFocusStep; }
            set
            {
                _smalFocusStep = value;
                NotifyPropertyChanged("SmalFocusStep");
            }
        }

        private int _mediumFocusStep;

        public int MediumFocusStep
        {
            get { return _mediumFocusStep; }
            set
            {
                _mediumFocusStep = value;
                NotifyPropertyChanged("MediumFocusStep");
            }
        }

        private int _largeFocusStep;

        public int LargeFocusStep
        {
            get { return _largeFocusStep; }
            set
            {
                _largeFocusStep = value;
                NotifyPropertyChanged("LargeFocusStep");
            }
        }

        private int _focusMoveStep;

        public int FocusMoveStep
        {
            get { return _focusMoveStep; }
            set
            {
                _focusMoveStep = value;
                NotifyPropertyChanged("FocusMoveStep");
            }
        }

        private bool _dontLoadThumbnails;

        public bool DontLoadThumbnails
        {
            get { return _dontLoadThumbnails; }
            set
            {
                _dontLoadThumbnails = value;
                NotifyPropertyChanged("DontLoadThumbnails");
            }
        }

        private string _externalViewer;

        public string ExternalViewer
        {
            get { return _externalViewer; }
            set
            {
                _externalViewer = value;
                NotifyPropertyChanged("ExternalViewer");
            }
        }

        private int _motionBlockSize;

        public int MotionBlockSize
        {
            get { return _motionBlockSize; }
            set
            {
                _motionBlockSize = value;
                NotifyPropertyChanged("MotionBlockSize");
            }
        }

        private int _detectionType;

        public int DetectionType
        {
            get { return _detectionType; }
            set
            {
                _detectionType = value;
                NotifyPropertyChanged("DetectionType");
            }
        }

        private bool _useExternalViewer;

        public bool UseExternalViewer
        {
            get { return _useExternalViewer; }
            set
            {
                _useExternalViewer = value;
                NotifyPropertyChanged("UseExternalViewer");
            }
        }

        private string _externalViewerPath;

        public string ExternalViewerPath
        {
            get { return _externalViewerPath; }
            set
            {
                _externalViewerPath = value;
                NotifyPropertyChanged("ExternalViewerPath");
            }
        }

        private string _externalViewerArgs;

        public string ExternalViewerArgs
        {
            get { return _externalViewerArgs; }
            set
            {
                _externalViewerArgs = value;
                NotifyPropertyChanged("ExternalViewerArgs");
            }
        }

        private string _selectedMainForm;

        public string SelectedMainForm
        {
            get { return _selectedMainForm; }
            set
            {
                _selectedMainForm = value;
                NotifyPropertyChanged("SelectedMainForm");
            }
        }

        private bool _useParallelTransfer;

        public bool UseParallelTransfer
        {
            get { return _useParallelTransfer; }
            set
            {
                _useParallelTransfer = value;
                NotifyPropertyChanged("UseParallelTransfer");
            }
        }

        private bool _showUntranslatedLabelId;

        public bool ShowUntranslatedLabelId
        {
            get { return _showUntranslatedLabelId; }
            set
            {
                _showUntranslatedLabelId = value;
                NotifyPropertyChanged("ShowUntranslatedLabelId");
            }
        }

        private CustomConfigEnumerator _deviceConfigs;

        public CustomConfigEnumerator DeviceConfigs
        {
            get { return _deviceConfigs; }
            set
            {
                _deviceConfigs = value;
                NotifyPropertyChanged("DeviceConfigs");
            }
        }


        private bool _highlightUnderExp;

        public bool HighlightUnderExp
        {
            get { return _highlightUnderExp; }
            set
            {
                _highlightUnderExp = value;
                NotifyPropertyChanged("HighlightUnderExp");
            }
        }

        private bool _highlightOverExp;

        public bool HighlightOverExp
        {
            get { return _highlightOverExp; }
            set
            {
                _highlightOverExp = value;
                NotifyPropertyChanged("HighlightOverExp");
            }
        }

        private bool _easyLiveViewControl;

        public bool EasyLiveViewControl
        {
            get { return _easyLiveViewControl; }
            set
            {
                _easyLiveViewControl = value;
                NotifyPropertyChanged("EasyLiveViewControl");
            }
        }

        private bool _addFakeCamera;

        public bool AddFakeCamera
        {
            get { return _addFakeCamera; }
            set
            {
                _addFakeCamera = value;
                NotifyPropertyChanged("AddFakeCamera");
            }
        }

        private bool _syncCameraDateTime;

        public bool SyncCameraDateTime
        {
            get { return _syncCameraDateTime; }
            set
            {
                _syncCameraDateTime = value;
                NotifyPropertyChanged("SyncCameraDateTime");
            }
        }

        private bool _showThumbUpDown;

        public bool ShowThumbUpDown
        {
            get { return _showThumbUpDown; }
            set
            {
                _showThumbUpDown = value;
                NotifyPropertyChanged("ShowThumbUpDown");
            }
        }

        private bool _autoPreviewJpgOnly;

        public bool AutoPreviewJpgOnly
        {
            get { return _autoPreviewJpgOnly; }
            set
            {
                _autoPreviewJpgOnly = value;
                NotifyPropertyChanged("AutoPreviewJpgOnly");
            }
        }

        private string _clientId;

        public string ClientId
        {
            get { return _clientId; }
            set
            {
                _clientId = value;
                NotifyPropertyChanged("ClientId");
            }
        }

        public string ClientTrackId { get; set; }

        public bool LoadThumbsDownload
        {
            get { return _loadThumbsDownload; }
            set
            {
                _loadThumbsDownload = value;
                NotifyPropertyChanged("LoadThumbsDownload");
            }
        }

        public int ThumbHeigh
        {
            get { return _thumbHeigh; }
            set
            {
                _thumbHeigh = value;
                NotifyPropertyChanged("ThumbHeigh");
            }
        }

        public CameraPropertyEnumerator CameraProperties { get; set; }
        public string SelectedLayout { get; set; }


        private ObservableCollection<CameraPreset> _cameraPresets;
        private bool _minimizeToTrayIcon;
        private bool _startMinimized;
        private bool _startupWithWindows;
        private bool _loadThumbsDownload;
        private int _smallFocusStepCanon;
        private int _mediumFocusStepCanon;
        private int _largeFocusStepCanon;
        private int _canonFocusStepWait;
        private bool _sendUsageStatistics;
        private bool _flipPreview;
        private int _thumbHeigh;
        private bool _allowWebserverActions;
        private int _selectedWifi;
        private string _wifiIp;
        private string _startupScript;
        private bool _publicWebserver;
        private bool _loadCanonTransferMode;
        private bool _showThumbInfo;
        private bool _enhancedThumbs;
        private bool _hideFullscreenControls;
        private bool _hideTrayNotifications;
        private bool _disableHardwareAcceleration;
        private string _fullScreenPassword;
        private string _currentThemeNameNew;
        private bool _webcamSupport;

        [XmlIgnore]
        [JsonIgnore]
        public ObservableCollection<CameraPreset> CameraPresets
        {
            get { return _cameraPresets; }
            set
            {
                _cameraPresets = value;
                NotifyPropertyChanged("CameraPresets");
            }
        }

        public bool MinimizeToTrayIcon
        {
            get { return _minimizeToTrayIcon; }
            set
            {
                _minimizeToTrayIcon = value;
                NotifyPropertyChanged("MinimizeToTrayIcon");
            }
        }

        public bool HideTrayNotifications
        {
            get { return _hideTrayNotifications; }
            set
            {
                _hideTrayNotifications = value;
                NotifyPropertyChanged("HideTrayNotifications");
            }
        }

        public bool StartMinimized
        {
            get { return _startMinimized; }
            set
            {
                _startMinimized = value;
                NotifyPropertyChanged("StartMinimized");
            }
        }

        public bool StartupWithWindows
        {
            get { return _startupWithWindows; }
            set
            {
                _startupWithWindows = value;
                NotifyPropertyChanged("StartupWithWindows");
            }
        }

        public string StartupScript
        {
            get { return _startupScript; }
            set
            {
                _startupScript = value;
                NotifyPropertyChanged("StartupScript");
            }
        }

        public bool SendUsageStatistics
        {
            get { return _sendUsageStatistics; }
            set
            {
                _sendUsageStatistics = value;
                NotifyPropertyChanged("SendUsageStatistics");
            }
        }

        public bool FlipPreview
        {
            get { return _flipPreview; }
            set
            {
                _flipPreview = value;
                NotifyPropertyChanged("FlipPreview");
            }
        }

        public bool EnhancedThumbs
        {
            get { return _enhancedThumbs; }
            set
            {
                _enhancedThumbs = value;
                NotifyPropertyChanged("EnhancedThumbs");
            }
        }

        public int SelectedWifi
        {
            get { return _selectedWifi; }
            set
            {
                _selectedWifi = value;
                NotifyPropertyChanged("SelectedWifi");
            }
        }

        public string WifiIp
        {
            get { return _wifiIp; }
            set
            {
                _wifiIp = value;
                NotifyPropertyChanged("WifiIp");
            }
        }

        public bool LoadCanonTransferMode
        {
            get { return _loadCanonTransferMode; }
            set
            {
                _loadCanonTransferMode = value;
                NotifyPropertyChanged("LoadCanonTransferMode");
            }
        }

        public bool FullScreenInSecondaryMonitor { get; set; }
        public bool Autorotate { get; set; }

        public int ExternalDeviceWaitForFocus { get; set; }
        public int ExternalDeviceWaitForCapture { get; set; }

        public bool WebcamSupport
        {
            get
            {
                return _webcamSupport;
            }
            set { _webcamSupport = value; }
        }

        public bool WiaDeviceSupport { get; set; }

        public ObservableCollection<PluginSetting> PluginSettings { get; set; }

        public bool ShowThumbInfo
        {
            get { return _showThumbInfo; }
            set
            {
                _showThumbInfo = value;
                NotifyPropertyChanged("ShowThumbInfo");
            }
        }

        public bool DisableHardwareAccelerationNew
        {
            get { return _disableHardwareAcceleration; }
            set
            {
                _disableHardwareAcceleration = value;
                NotifyPropertyChanged("DisableHardwareAccelerationNew");
            }
        }

        public static string BrandingFolder
        {
            get { return Path.Combine(ApplicationFolder, "Branding"); }
        }

        public string OverlayFolder
        {
            get { return Path.Combine(DataFolder, "LiveViewOverlay"); }
        }

        public static string DataFolder
        {
            get
            {
                return ServiceProvider.Branding !=null && !string.IsNullOrEmpty(ServiceProvider.Branding.ApplicationDataFolder) ? Path.GetFullPath(ServiceProvider.Branding.ApplicationDataFolder) : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
            }
        }

        public static string SessionFolder
        {
            get
            {
                return Path.Combine(DataFolder, "Sessions"); 
            }
        }

        public static string PresetFolder
        {
            get { return Path.Combine(DataFolder, "Presets"); }
        }

        public static string WebServerFolder
        {
            get { return Path.Combine(ApplicationFolder, "WebServer\\"); }
        }

        public static string BrandingWebServerFolder
        {
            get { return Path.Combine(BrandingFolder, "WebServer\\"); }
        }

        public static string ApplicationFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        /// Return plugin settings with specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PluginSetting this[string name]
        {
            get
            {
                foreach (var pluginSetting in PluginSettings)
                {
                    if (pluginSetting.Name == name)
                        return pluginSetting;
                }
                var pl=new PluginSetting(){Name = name};
                PluginSettings.Add(pl);
                return pl;
            }
        }

        public Settings()
        {
            ConfigFile = Path.Combine(DataFolder, "settings.json");
            CameraPresets = new AsyncObservableCollection<CameraPreset>();
            DefaultSession = new PhotoSession();
            PhotoSessions = new ObservableCollection<PhotoSession>();
            SelectedBitmap = new BitmapFile();
            ImageLoading = false;
            CameraProperties = new CameraPropertyEnumerator();
            DeviceConfigs = new CustomConfigEnumerator();
            PluginSettings = new ObservableCollection<PluginSetting>();
            ResetSettings();
        }


        public void ResetSettings()
        {
            Actions = new AsyncObservableCollection<WindowCommandItem>();
            DisableNativeDrivers = false;
            AutoPreview = true;
            LastUpdateCheckDate = DateTime.MinValue;
            UseWebserver = false;
            WebserverPort = 5513;
            Preview = false;
            PreviewSeconds = 3;
            LiveViewFreezeTimeOut = 3;
            PreviewLiveViewImage = true;
            SmalFocusStep = 10;
            MediumFocusStep = 100;
            LargeFocusStep = 500;
            SmallFocusStepCanon = 1;
            MediumFocusStepCanon = 25;
            LargeFocusStepCanon = 100;
            CanonFocusStepWait = 400;
            RotateIndex = 0;
            FullScreenColor = Colors.Black;
            ShowFullscreenControls = true;
            SelectedLanguage = Thread.CurrentThread.CurrentCulture.Name;
            FocusMoveStep = 50;
            MotionBlockSize = 40;
            UseExternalViewer = false;
            ExternalViewerArgs = string.Empty;
            ShowFocusPoints = true;
            EnhancedThumbs = true;

            UseParallelTransfer = false;
            ShowUntranslatedLabelId = false;
            HighlightOverExp = false;
            HighlightUnderExp = false;
           
            AddFakeCamera = false;
            SyncCameraDateTime = false;
            ShowThumbUpDown = false;
            AutoPreviewJpgOnly = false;
            ClientId = Guid.NewGuid().ToString();
            ClientTrackId = string.Format("{0}{1}", new object[]
            {
                new Random((int) DateTime.UtcNow.Ticks).Next(100000000, 999999999),
                "00145214523"
            });
            if (ServiceProvider.WindowsManager != null)
                SyncActions(ServiceProvider.WindowsManager.WindowCommands);
            MinimizeToTrayIcon = false;
            StartMinimized = false;
            StartupWithWindows = false;
            LoadThumbsDownload = true;
            FullScreenInSecondaryMonitor = false;
            SendUsageStatistics = true;
            ThumbHeigh = 100;
            CurrentThemeNameNew = "Dark\\grey";
            AllowWebserverActions = true;
            PublicWebserver = false;
            LoadCanonTransferMode = true;
            Autorotate = true;
            ShowThumbInfo = true;
            SelectedLayout = "Normal";
            DisableHardwareAccelerationNew = true;

            ExternalDeviceWaitForCapture = 1000;
            ExternalDeviceWaitForFocus = 1000;

            WebcamSupport = false;
            WiaDeviceSupport = true;
        }


        /// <summary>
        /// Add new WinddowssCommands from items
        /// </summary>
        /// <param name="items">The items.</param>
        public void SyncActions(IEnumerable<WindowCommandItem> items)
        {
            List<WindowCommandItem> itemsToAdd = (from windowCommandItem in items
                                                  let found =
                                                      Actions.Any(
                                                          commandItem => windowCommandItem.Name == commandItem.Name)
                                                  where !found
                                                  select windowCommandItem).ToList();
            foreach (WindowCommandItem item in itemsToAdd)
            {
                Actions.Add(item);
            }
        }

        public void Add(PhotoSession session)
        {
            Save(session);
            PhotoSessions.Add(session);
        }

        /// <summary>
        /// Load files attached to a session
        /// </summary>
        /// <param name="session"></param>
        public void LoadData(PhotoSession session)
        {
            try
            {
                if (session == null)
                    return;
                if (Directory.Exists(session.Folder))
                {
                    if (session.AlowFolderChange && session.ReloadOnFolderChange)
                    {
                        session.Files.Clear();
                    }

                    FileItem[] fileItems = new FileItem[session.Files.Count];
                    session.Files.CopyTo(fileItems, 0);
                    ////session.Files.Clear();
                    //if (!Directory.Exists(session.Folder))
                    //{
                    //    Directory.CreateDirectory(session.Folder);
                    //}
                    string[] files = Directory.GetFiles(session.Folder);
                    foreach (string file in files)
                    {
                        if (session.SupportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                        {
                            if (!string.IsNullOrEmpty(file) && !session.ContainFile(file))
                                session.AddFile(file);
                        }
                    }
                }
                // hide files which was deleted or not exist
                List<FileItem> removedItems = session.Files.Where(fileItem => !File.Exists(fileItem.FileName)).ToList();
                foreach (FileItem removedItem in removedItems)
                {
                    removedItem.Visible = false;
                }
                //session.Files = new AsyncObservableCollection<FileItem>(session.Files.OrderBy(x => x.FileDate));
            }
            catch (Exception exception)
            {
                Log.Error("Error loading session ", exception);
            }
        }

        public void Save(PhotoSession session)
        {
            if (session == null)
                return;
            try
            {
                string filename = Path.Combine(SessionFolder, session.Name + ".json");
                SaveSession(session, filename);
                session.ConfigFile = filename;
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save session " + session.Name, exception);
            }
        }

        public void SaveSession(PhotoSession session, string filename)
        {
            var json = JsonConvert.SerializeObject(session);
            File.WriteAllText(filename, json);
        }

        public void Save(CameraPreset preset)
        {
            if (preset == null)
                return;
            try
            {
                if (!Directory.Exists(PresetFolder))
                    Directory.CreateDirectory(PresetFolder);

                preset.Save(preset.FileName);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save preset " + preset.Name, exception);
            }
        }

        public void Save(Branding branding)
        {
            if (branding == null)
                return;
            try
            {
                string filename = Path.Combine(DataFolder, "Branding.xml");
                XmlSerializer serializer = new XmlSerializer(typeof (Branding));
                // Create a FileStream to write with.

                Stream writer = new FileStream(filename, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, branding);
                writer.Close();
            }
            catch (Exception)
            {
                Log.Error("Unable to save session branding file");
            }
        }

        public PhotoSession LoadSession(string filename)
        {
            PhotoSession photoSession = new PhotoSession();
            try
            {
                if (File.Exists(filename))
                {
                    if (Path.GetExtension(filename) == ".json")
                    {
                        photoSession = JsonConvert.DeserializeObject<PhotoSession>(File.ReadAllText(filename));
                    }
                    else
                    {
                        XmlSerializer mySerializer =
                            new XmlSerializer(typeof(PhotoSession));
                        FileStream myFileStream = new FileStream(filename, FileMode.Open);
                        photoSession = (PhotoSession) mySerializer.Deserialize(myFileStream);
                        myFileStream.Close();
                        photoSession.ConfigFile = filename;
                        // upgrade to new fie template
                        var s = photoSession.FileNameTemplate;
                        s = s.Replace("$C", "[Counter 4 digit]");
                        s = s.Replace("$N", "[Session Name]");
                        s = s.Replace("$E", "[Exposure Compensation]");
                        s = s.Replace("$D", "[Date yyyy-MM-dd]");
                        s = s.Replace("$B", "[Barcode]");
                        s = s.Replace("$Type", "[File format]");
                        s = s.Replace("$X", "[Camera Name]");
                        s = s.Replace("$Tag1", "[Selected Tag1]");
                        s = s.Replace("$Tag2", "[Selected Tag2]");
                        s = s.Replace("$Tag3", "[Selected Tag3]");
                        s = s.Replace("$Tag4", "[Selected Tag4]");
                        photoSession.FileNameTemplate = s;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (photoSession == null)
                photoSession = new PhotoSession();

            return photoSession;
        }

        public Settings Load()
        {
            Settings settings = new Settings();
            if (File.Exists(ServiceProvider.Branding.DefaultSettings))
                settings = LoadSettings(ServiceProvider.Branding.DefaultSettings, settings);

            if (ServiceProvider.Branding.ResetSettingsOnLoad)
                return settings;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(ConfigFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile));
                }
                if (File.Exists(ConfigFile) || File.Exists(ConfigFile.Replace(".json", ".xml")))
                {
                    settings = LoadSettings(ConfigFile, settings);
                }
                else
                {
                    settings.Save();
                }
                // There is a bug somewhere which cause null exception
                // but too lazy to find it ..
                if (settings == null)
                {
                    settings = new Settings();
                    settings.Save();
                }
                settings.LoadPresetData();
            }
            catch (Exception exception)
            {
                Log.Error("Error loading config file ", exception);
            }
            return settings;
        }

        public Settings LoadSettings(string fileName, Settings defaultSettings)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    if (Path.GetExtension(fileName) == ".json")
                    {
                        defaultSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(fileName));
                    }
                    else
                    {
                        XmlSerializer mySerializer =
                            new XmlSerializer(typeof(Settings));
                        FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                        defaultSettings = (Settings)mySerializer.Deserialize(myFileStream);
                        myFileStream.Close();
                    }
                }
                else
                {
                    defaultSettings =
                        LoadSettings(fileName.Replace(".json", ".xml"), defaultSettings);
                }

            }
            catch (Exception exception)
            {
                Log.Error("Error loading config file ", exception);
            }

            if (defaultSettings==null)
                defaultSettings=new Settings();

            return defaultSettings;
        }

        public PhotoSession GetSession(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return null;
                if (!string.IsNullOrEmpty(name))
                {
                    return PhotoSessions.FirstOrDefault(photoSession => photoSession.Name == name);
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error find photo session ",e);
            }
            return null;
        }

        public CameraPreset GetPreset(string name)
        {
            return string.IsNullOrEmpty(name) ? null : CameraPresets.FirstOrDefault(cameraPreset => cameraPreset.Name == name);
        }

        /// <summary>
        /// Loads al saved sessions 
        /// </summary>
        public void LoadSessionData()
        {
            string sesionFolder = SessionFolder;
            if (!Directory.Exists(sesionFolder))
            {
                Directory.CreateDirectory(sesionFolder);
            }

            string[] sesions = Directory.GetFiles(sesionFolder, "*.json");
            foreach (string sesion in sesions)
            {
                try
                {
                    Add(LoadSession(sesion));
                }
                catch (Exception e)
                {
                    Log.Error("Error loading session :" + sesion, e);
                }
            }
            // first run after using json format in place of xml
            if (PhotoSessions.Count == 0)
            {
                sesions = Directory.GetFiles(sesionFolder, "*.xml");
                foreach (string sesion in sesions)
                {
                    try
                    {
                        var sesiondata = LoadSession(sesion);
                        Add(sesiondata);
                        // remove old xml file
                        File.Delete(sesion);
                        Save(sesiondata);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error loading session :" + sesion, e);
                    }
                }

            }
            if (PhotoSessions.Count > 0)
            {
                DefaultSession = GetSession(DefaultSessionName) ?? PhotoSessions[0];
            }
            if (PhotoSessions.Count == 0)
            {
                Add(DefaultSession);
            }
        }

        public void LoadPresetData()
        {
            if (!Directory.Exists(PresetFolder))
            {
                Directory.CreateDirectory(PresetFolder);
            }

            string[] presets = Directory.GetFiles(PresetFolder, "*.xml");
            foreach (string presetname in presets)
            {
                try
                {
                    var preset = CameraPreset.Load(presetname);
                    if (preset != null)
                        CameraPresets.Add(preset);
                }
                catch (Exception e)
                {
                    Log.Error("Error loading preset :" + presetname, e);
                }
            }
        }

        public void Save()
        {
            try
            {
                //XmlSerializer serializer = new XmlSerializer(typeof (Settings));
                //// Create a FileStream to write with.

                //Stream writer = new FileStream(ConfigFile, FileMode.Create);
                //// Serialize the object, and close the TextWriter
                //serializer.Serialize(writer, this);
                //writer.Close();

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
                // save preset in separated files
                foreach (var cameraPreset in this.CameraPresets)
                {
                    Save(cameraPreset);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save settings ", exception);
            }
        }


        public List<string> Themes
        {
            get
            {
                try
                {
                    
                    var themes = new SwatchesProvider().Swatches.Select(accent => "Light\\" + accent.Name).ToList();
                    themes.AddRange(new SwatchesProvider().Swatches.Select(accent => "Dark\\" + accent.Name));
                    return themes;
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }
        }

        public delegate void SessionSelectedEventHandler(PhotoSession oldvalu, PhotoSession newvalue);

        public event SessionSelectedEventHandler SessionSelected;
    }
}
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Scripting;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Nikon;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for BulbWnd.xaml
    /// </summary>
    public partial class BulbWnd :MetroWindow, INotifyPropertyChanged, IWindow
    {
        private Timer _captureTimer = new Timer(1000);
        private Timer _waitTimer = new Timer(1000);
        private int _captureSecs;
        private int _waitSecs;
        private int _photoCount = 0;
        private string _defaultScriptFile = "";

        public ICameraDevice CameraDevice { get; set; }
        private bool _noAutofocus;
        public bool NoAutofocus
        {
            get { return _noAutofocus; }
            set
            {
                _noAutofocus = value;
                NotifyPropertyChanged("NoAutofocus");
            }
        }

        private int _captureTime;
        public int CaptureTime
        {
            get { return _captureTime; }
            set
            {
                _captureTime = value;
                NotifyPropertyChanged("CaptureTime");
            }
        }


        private int _numOfPhotos;
        public int NumOfPhotos
        {
            get { return _numOfPhotos; }
            set
            {
                _numOfPhotos = value;
                NotifyPropertyChanged("NumOfPhotos");
            }
        }

        private int _waitTime;
        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                _waitTime = value;
                NotifyPropertyChanged("WaitTime");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }


        private ScriptObject _defaultScript;
        public ScriptObject DefaultScript
        {
            get { return _defaultScript; }
            set
            {
                _defaultScript = value;
                NotifyPropertyChanged("DefaultScript");
            }
        }

        private int _phdType;
        public int PhdType
        {
            get { return _phdType; }
            set
            {
                _phdType = value;
                NotifyPropertyChanged("PhdType");
            }
        }

        private int _phdWait;
        public int PhdWait
        {
            get { return _phdWait; }
            set
            {
                _phdWait = value;
                NotifyPropertyChanged("PhdWait");
            }
        }

        private int _countDown;
        public int CountDown
        {
            get { return _countDown; }
            set
            {
                _countDown = value;
                NotifyPropertyChanged("CountDown");
            }
        }

        private string _event;
        public string Event
        {
            get { return _event; }
            set
            {
                _event = value;
                NotifyPropertyChanged("Event");
            }
        }

        private int _photoLeft;
        public int PhotoLeft
        {
            get { return _photoLeft; }
            set
            {
                _photoLeft = value;
                NotifyPropertyChanged("PhotoLeft");
            }
        }

        private bool _automaticGuiding;
        public bool AutomaticGuiding
        {
            get { return _automaticGuiding; }
            set
            {
                _automaticGuiding = value;
                if (_automaticGuiding && PhdType == 0)
                    PhdType = 1;

                NotifyPropertyChanged("AutomaticGuiding");
            }
        }

        public BulbWnd()
        {
            CameraDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
            //NoAutofocus = true;
            AddCommand = new RelayCommand<IScriptCommand>(AddCommandMethod);
            EditCommand = new RelayCommand<IScriptCommand>(EditCommandMethod);
            DelCommand = new RelayCommand<IScriptCommand>(DelCommandMethod);
            InitializeComponent();
            CaptureTime = 60;
            NumOfPhotos = 1;
            WaitTime = 0;
            PhdType = 0;
            PhdWait = 5;
            CountDown = 0;
            PhotoLeft = 0;
            AutomaticGuiding = false;
            _captureTimer.Elapsed += _captureTimer_Elapsed;
            _waitTimer.Elapsed += _waitTimer_Elapsed;
            ServiceProvider.Settings.ApplyTheme(this);
        }

        private void Init()
        {
            DefaultScript = new ScriptObject();
            _defaultScriptFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.AppName,
                "default.dccscript");
            try
            {
                if (File.Exists(_defaultScriptFile))
                    DefaultScript = ServiceProvider.ScriptManager.Load(_defaultScriptFile);
            }
            catch (Exception exception)
            {
                Log.Error("Error loading default scrip", exception);
            }
        }

        public void AddCommandMethod(IScriptCommand command)
        {
            IScriptCommand scriptCommand = command.Create();
            EditCommandMethod(scriptCommand);
            DefaultScript.Commands.Add(scriptCommand);
            lst_commands.SelectedItem = scriptCommand;
        }

        public void EditCommandMethod(IScriptCommand command)
        {
            if (command == null)
                return;
            if(!command.HaveEditControl)
            {
                MessageBox.Show("Use script editor to edit this script");
                return;
            }
            var dlg = new ScriptCommandEdit(command.GetConfig());
            dlg.ShowDialog();
        }

        public void DelCommandMethod(IScriptCommand command)
        {
            if (command == null)
                return;
            int ind = DefaultScript.Commands.IndexOf(command);
            DefaultScript.Commands.Remove(command);
            if (ind >= DefaultScript.Commands.Count)
                ind = DefaultScript.Commands.Count - 1;
            if (ind > 0)
                lst_commands.SelectedItem = DefaultScript.Commands[ind];
        }

        void _waitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _waitSecs++;
            CountDown--;
            Message = string.Format("Waiting for next capture {0} sec. Photo done {1}/{2}",
                                    _waitSecs, _photoCount, NumOfPhotos);
            if (_waitSecs >= WaitTime)
            {
                _waitTimer.Stop();
                StartCapture();
            }
        }

        void _captureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _captureSecs++;
            if (_captureSecs >= CaptureTime)
            {
                _captureTimer.Stop();
                StopCapture();
                _photoCount++;
                _waitSecs = 0;
                if (_photoCount < NumOfPhotos)
                {
                    if (PhdType > 0)
                        PhdGuiding(PhdType);

                    Event = "Waiting";
                    CountDown = WaitTime;
                    _waitTimer.Start();
                }
            }
//            Message = string.Format("Capture time {0}/{1} sec. Photo done {2}/{3}", _captureSecs, CaptureTime, _photoCount,
//                        NumOfPhotos);
            CountDown--;
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            _photoCount = 0;
            AutomaticGuiding = false;
            PhotoLeft = NumOfPhotos;
            StartCapture();
        }

        void StartCapture()
        {
            Thread thread = new Thread(StartCaptureThread);
            thread.Start();
        }

        void StartCaptureThread()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    Log.Debug("Bulb capture started");
                    CountDown = CaptureTime;
                    Event = "Capture";
                    PhotoLeft--;
                    if (DefaultScript.UseExternal)
                    {
                        if (DefaultScript.SelectedConfig != null)
                        {
                            NikonBase nikonBase = CameraDevice as NikonBase;
                            if(nikonBase!=null)
                                nikonBase.StopEventTimer();
                            ServiceProvider.ExternalDeviceManager.OpenShutter(DefaultScript.SelectedConfig);
                        }
                        else
                        {
                            MessageBox.Show(TranslationStrings.LabelNoExternalDeviceSelected);
                        }
                    }
                    else
                    {
                        if (CameraDevice.GetCapability(CapabilityEnum.Bulb))
                        {
                            CameraDevice.LockCamera();
                            CameraDevice.StartBulbMode();
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                        }
                    }
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY)
                        retry = true;
                    else
                    {
                        StaticHelper.Instance.SystemMessage = deviceException.Message;
                        Log.Error("Bulb start", deviceException);
                    }
                }
                catch (Exception exception)
                {
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    Log.Error("Bulb start", exception);
                    Event = "Error";
                    CountDown = 0;
                    return;
                }
            } while (retry);
            _waitSecs = 0;
            _captureSecs = 0;
            _captureTimer.Start();
        }

        public RelayCommand<IScriptCommand> AddCommand
        {
            get;
            private set;
        }

        public RelayCommand<IScriptCommand> EditCommand
        {
            get;
            private set;
        }

        public RelayCommand<IScriptCommand> DelCommand
        {
            get;
            private set;
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

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            StopCapture();
        }

        private void StopCapture()
        {
            _captureTimer.Stop();
            _waitTimer.Stop();
            Thread thread = new Thread(StopCaptureThread);
            thread.Start();
            StaticHelper.Instance.SystemMessage = "Capture stopped";
            Log.Debug("Bulb capture stopped");
            Event = "Done";
        }


        private void StopCaptureThread()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    if (DefaultScript.UseExternal)
                    {
                        if (DefaultScript.SelectedConfig != null)
                        {
                            ServiceProvider.ExternalDeviceManager.CloseShutter(DefaultScript.SelectedConfig);
                            NikonBase nikonBase = CameraDevice as NikonBase;
                            if (nikonBase != null)
                                nikonBase.StartEventTimer();
                        }
                        else
                        {
                            MessageBox.Show(TranslationStrings.LabelNoExternalDeviceSelected);
                        }
                    }
                    else
                    {
                        if (CameraDevice.GetCapability(CapabilityEnum.Bulb))
                        {
                            CameraDevice.EndBulbMode();
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                        }
                    }
                    CountDown = 0;
                    StaticHelper.Instance.SystemMessage = "Capture done";
                    Log.Debug("Bulb capture done");
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY)
                        retry = true;
                    else
                    {
                        StaticHelper.Instance.SystemMessage = deviceException.Message;
                        Log.Error("Bulb done", deviceException);
                    }

                }
                catch (Exception exception)
                {
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    Log.Error("Bulb done", exception);
                }
            } while (retry);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.Bulb);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Script file(*.dccscript)|*.dccscript|All files|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    ServiceProvider.ScriptManager.Save(DefaultScript, dlg.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error saving script file" + exception.Message);
                    Log.Error("Error saving script file", exception);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Script file(*.dccscript)|*.dccscript|All files|*.*";
            if(dlg.ShowDialog()==true)
            {
                try
                {
                    DefaultScript = ServiceProvider.ScriptManager.Load(dlg.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error loading script file" + exception.Message);
                    Log.Error("Error loading script file", exception);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DefaultScript.CameraDevice = CameraDevice;
            ServiceProvider.ScriptManager.Execute(DefaultScript);
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.BulbWnd_Show:
                    CameraDevice = param as ICameraDevice;
                    if (CameraDevice == null)
                        return;
                    Init();
                    CameraDevice.PhotoCaptured += CameraDevice_PhotoCaptured;
                    ServiceProvider.ScriptManager.OutPutMessageReceived += ScriptManager_OutPutMessageReceived;
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Show();
                                                         Activate();
                                                         //Topmost = true;
                                                         //Topmost = false;
                                                         Focus();
                                                     }));
                    break;
                case WindowsCmdConsts.BulbWnd_Hide:
                    if (_captureTimer.Enabled)
                    {
                        StopCaptureThread();
                        CameraDevice.UnLockCamera();
                    }
                    _captureTimer.Stop();
                    _waitTimer.Stop();
                    ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
                    CameraDevice.PhotoCaptured -= CameraDevice_PhotoCaptured;
                    ServiceProvider.ScriptManager.Save(DefaultScript, _defaultScriptFile);
                    Hide();
                    break;
                case CmdConsts.All_Close:
                    ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
                    CameraDevice.PhotoCaptured -= CameraDevice_PhotoCaptured;
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Hide();
                                                         Close();
                                                     }));
                    break;
            }
        }

        void CameraDevice_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            if (AutomaticGuiding && PhdType > 0)
            {

                var thread = new Thread(() => PhdGuiding(PhdType));
                thread.Start();
            }
        }

        void ScriptManager_OutPutMessageReceived(object sender, MessageEventArgs e)
        {
            AddOutput(e.Message);
        }

        #endregion

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BulbWnd_Hide);
            }
        }

        private void btn_stop_script_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.ScriptManager.Stop();
        }

        private void btn_astrolv_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.AstroLiveViewWnd_Show, CameraDevice);
        }

        public void PhdGuiding(int movetype)
        {
            try
            {
                Event = "PhdGuiding";
                TcpClient socket = new TcpClient("localhost", 4300);
                Thread.Sleep(200);
                switch (movetype)
                {
                    case 1:
                        SendReceiveTest2(socket, 3);
                        break;
                    case 2:
                        SendReceiveTest2(socket, 4);
                        break;
                    case 3:
                        SendReceiveTest2(socket, 5);
                        break;
                    case 4:
                        SendReceiveTest2(socket, 12);
                        break;
                    case 5:
                        SendReceiveTest2(socket, 13);
                        break;
                }
                socket.Close();
                CountDown = PhdWait;
                for (var i = 1; i < PhdWait + 1; i++)
                {
                    CountDown--;
                    Thread.Sleep(1000);
                }
                Event = "";
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = "PHDGuiding error " + exception.Message;
                Log.Error("PHDGuiding error", exception);
            }
        }
        
        public static int SendReceiveTest2(TcpClient server, byte opersEnum)
        {
            byte[] bytes = new byte[256];
            try
            {
                // Blocks until send returns. 
                int byteCount = server.Client.Send(new[] { opersEnum }, SocketFlags.None);
                //Console.WriteLine("Sent {0} bytes.", byteCount);

                // Get reply from the server.
                byteCount = server.Client.Receive(bytes, SocketFlags.None);
                //Console.WriteLine(byteCount);
                //if (byteCount > 0)
                //    Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
            catch (SocketException e)
            {
                //Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                return (e.ErrorCode);
            }
            return 0;
        }

        public void AddOutput(string msg)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lst_output.Items.Add(msg);
                lst_output.ScrollIntoView(lst_output.Items[lst_output.Items.Count - 1]);
            }));
        }


        private void btn_stay_on_top_Click(object sender, RoutedEventArgs e)
        {
            Topmost = (btn_stay_on_top.IsChecked == true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoViewModel : ViewModelBase
    {
        private ArduinoCommandWindow _commandWindow;
        private PluginSetting _pluginSetting;
        private List<string> _ports;
        private SerialPort _sp = new SerialPort();
        private ArduinoCommandViewModel _commandViewModel;

        public RelayCommand ShowButtonsCommand { get; set; } 
        
        public PluginSetting PluginSetting
        {
            get { return _pluginSetting ?? (_pluginSetting = ServiceProvider.Settings["Arduino"]); }
        }

        public List<string> Ports
        {
            get { return _ports; }
            set
            {
                _ports = value;
                RaisePropertyChanged(() => Ports);
            }
        }

        public ArduinoCommandViewModel CommandViewModel
        {
            get { return _commandViewModel; }
            set
            {
                _commandViewModel = value;
                RaisePropertyChanged(() => CommandViewModel);
            }
        }

        public List<string> Actions
        {
            get { return ServiceProvider.Settings.Actions.Select((x) => x.Name).OrderBy(x=>x).ToList(); }
        }

        public string Command1
        {
            get
            {
                return PluginSetting["Command1"] as string;
            }
            set
            {
                PluginSetting["Command1"] = value;
                RaisePropertyChanged(() => Command1);
            }
        }

        public string Action1
        {
            get
            {
                return PluginSetting["Action1"] as string;
            }
            set
            {
                PluginSetting["Action1"] = value;
                RaisePropertyChanged(() => Action1);
            }
        }

        public string Command2
        {
            get
            {
                return PluginSetting["Command2"] as string;
            }
            set
            {
                PluginSetting["Command2"] = value;
                RaisePropertyChanged(() => Command2);
            }
        }

        public string Action2
        {
            get
            {
                return PluginSetting["Action2"] as string;
            }
            set
            {
                PluginSetting["Action2"] = value;
                RaisePropertyChanged(() => Action2);
            }
        }

        public string Command3
        {
            get
            {
                return PluginSetting["Command3"] as string;
            }
            set
            {
                PluginSetting["Command3"] = value;
                RaisePropertyChanged(() => Command3);
            }
        }

        public string Action3
        {
            get
            {
                return PluginSetting["Action3"] as string;
            }
            set
            {
                PluginSetting["Action3"] = value;
                RaisePropertyChanged(() => Action3);
            }
        }

        public string Command4
        {
            get
            {
                return PluginSetting["Command4"] as string;
            }
            set
            {
                PluginSetting["Command4"] = value;
                RaisePropertyChanged(() => Command4);
            }
        }

        public string Action4
        {
            get
            {
                return PluginSetting["Action4"] as string;
            }
            set
            {
                PluginSetting["Action4"] = value;
                RaisePropertyChanged(() => Action4);
            }
        }
        public string Port
        {
            get
            {
                RefreshPorts();
                return PluginSetting["Port"] as string;
            }
            set
            {
                PluginSetting["Port"] = value;
                RaisePropertyChanged(()=>Port);
            }
        }

        public string AfterTransfer
        {
            get
            {
                return PluginSetting["AfterTransfer"] as string;
            }
            set
            {
                PluginSetting["AfterTransfer"] = value;
                RaisePropertyChanged(() => AfterTransfer);
            }
        }

        public string ArduinoLabel
        {
            get
            {
                return PluginSetting["ArduinoLabel"] as string;
            }
            set
            {
                PluginSetting["ArduinoLabel"] = value;
                RaisePropertyChanged(() => ArduinoLabel);
            }
        }
        
        public bool Active
        {
            get { return PluginSetting.GetBool("Active"); }
            set
            {
                PluginSetting["Active"] = value;
                if(value)
                    OpenPort();
                else
                    ClosePort();
                RaisePropertyChanged(() => Active);
            }
        }

        public bool ButtonsStartup
        {
            get { return PluginSetting.GetBool("ButtonsStartup"); }
            set
            {
                PluginSetting["ButtonsStartup"] = value;
                RaisePropertyChanged(() => ButtonsStartup);
            }
        }

        public bool SendCommand
        {
            get { return PluginSetting.GetBool("SendCommand"); }
            set
            {
                PluginSetting["SendCommand"] = value;
                RaisePropertyChanged(() => SendCommand);
            }
        }

        public bool ReceiveCommand
        {
            get { return PluginSetting.GetBool("ReceiveCommand"); }
            set
            {
                PluginSetting["ReceiveCommand"] = value;
                RaisePropertyChanged(() => ReceiveCommand);
            }
        }



        public AsyncObservableCollection<string> Outs { get; set; }

        public ArduinoViewModel()
        {
            Outs = new AsyncObservableCollection<string>();
            ShowButtonsCommand = new RelayCommand(ShowButtons);
            if (!IsInDesignMode)
            {
                ServiceProvider.WindowsManager.Event += WindowsManager_Event;
                ServiceProvider.FileTransfered += ServiceProvider_FileTransfered;
            }
            RefreshPorts();
            CommandViewModel = new ArduinoCommandViewModel() {ArduinoViewModel = this};
            try
            {
                var json = PluginSetting["Buttons"] as string;
                if (!string.IsNullOrEmpty(json))
                {
                    CommandViewModel.Buttons = JsonConvert.DeserializeObject<List<ArduinoButton>>(json);                    
                }
            }
            catch (Exception ex)
            {
                Log.Error("",ex);
            }
        }

        public void SaveButtons()
        {
            PluginSetting["Buttons"] = JsonConvert.SerializeObject(CommandViewModel.Buttons);
        }

        public void ShowButtons()
        {
            if (_commandWindow == null || !_commandWindow.IsVisible)
                _commandWindow = new ArduinoCommandWindow();
            _commandWindow.DataContext = CommandViewModel;
            _commandWindow.Show();
        }

        void ServiceProvider_FileTransfered(object sender, FileItem fileItem)
        {
            if (Active && !string.IsNullOrEmpty(AfterTransfer) && SendCommand)
                Send(AfterTransfer);
        }

        void WindowsManager_Event(string cmd, object o)
        {
            if (Active && SendCommand)
                Send(cmd);
        }

        private void RefreshPorts()
        {
            Ports = SerialPort.GetPortNames().ToList();
        }

        public void OpenPort()
        {
            try
            {
                if (Active && !string.IsNullOrEmpty(Port) && !_sp.IsOpen)
                {
                    _sp.PortName = Port;
                    _sp.BaudRate = 9600;
                    _sp.WriteTimeout = 3500;
                    _sp.Open();
                    _sp.DataReceived += sp_DataReceived;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Port open error", ex);
            }
        }

        public void Send(string str)
        {
            try
            {
                if (_sp.IsOpen)
                    _sp.WriteLine(str);
            }
            catch (Exception ex)
            {
                Log.Error("Error send text " + str, ex);
            }
        }

        private void ClosePort()
        {
            try
            {
                if (_sp.IsOpen)
                {
                    _sp.DataReceived -= sp_DataReceived;
                    _sp.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error closing port", ex);
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort spL = (SerialPort) sender;
                string str = spL.ReadLine().Replace("\r", "");
                Outs.Add(str);

                if (!string.IsNullOrEmpty(Action1) && str == Command1)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(Action1);
                    return;
                }
                if (!string.IsNullOrEmpty(Action2) && str == Command2)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(Action2);
                    return;
                }
                if (!string.IsNullOrEmpty(Action3) && str == Command3)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(Action3);
                    return;
                }
                if (!string.IsNullOrEmpty(Action4) && str == Command4)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(Action4);
                    return;
                }
                if (ReceiveCommand)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(str);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Data error ", ex);
            }
        }
    }
}

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

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoViewModel : ViewModelBase
    {
        private PluginSetting _pluginSetting;
        private List<string> _ports;
        private SerialPort _sp = new SerialPort();

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
            ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            RefreshPorts();
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
                SerialPort spL = (SerialPort)sender;
                string str = spL.ReadLine().Replace("\r","");
                Outs.Add(str);
                if (ReceiveCommand)
                    ServiceProvider.WindowsManager.ExecuteCommand(str);
            }
            catch (Exception ex)
            {
                Log.Error("Data error ", ex);
            }
        }
    }
}

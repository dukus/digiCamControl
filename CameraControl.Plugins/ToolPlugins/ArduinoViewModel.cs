using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoViewModel : ViewModelBase
    {
        private PluginSetting _pluginSetting;
        private List<string> _ports;

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
            get { return PluginSetting["Port"] as string; }
            set
            {
                PluginSetting["Port"] = value;
                RaisePropertyChanged(()=>Port);
            }
        }

        public AsyncObservableCollection<string> Outs { get; set; }

        public ArduinoViewModel()
        {
            Outs = new AsyncObservableCollection<string>();
            RefreshPorts();
        }

        private void RefreshPorts()
        {
            Ports = SerialPort.GetPortNames().ToList();
        }

        private void ConnectPort()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Outs.Add(ex.Message);
            }
        }
    }
}

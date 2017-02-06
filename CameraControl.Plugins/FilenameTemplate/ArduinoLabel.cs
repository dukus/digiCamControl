using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using ZXing;
using ZXing.Common;

namespace CameraControl.Plugins.FilenameTemplate
{
    public class ArduinoLabel : IFilenameTemplate
    {
        private PluginSetting _pluginSetting;
        private SerialPort _sp = new SerialPort();


        public PluginSetting PluginSetting => _pluginSetting ?? (_pluginSetting = ServiceProvider.Settings["Arduino"]);

        public string Port
        {
            get
            {
                return PluginSetting["Port"] as string;
            }
            set
            {
                PluginSetting["Port"] = value;
            }
        }

        public string ArduinoLabelCommand
        {
            get
            {
                return PluginSetting["ArduinoLabel"] as string;
            }
            set
            {
                PluginSetting["ArduinoLabel"] = value;
            }
        }


        public bool IsRuntime => true;

        public string Pharse(string template, PhotoSession session, ICameraDevice device, string fileName,
            string tempfileName)
        {
            if (!File.Exists(tempfileName))
                return "";
            try
            {
                _sp.PortName = Port;
                _sp.BaudRate = 9600;
                _sp.WriteTimeout = 3500;
                _sp.ReadTimeout = 3500;
                _sp.Open();
                _sp.WriteLine(ArduinoLabelCommand);
                Thread.Sleep(500);
                return _sp.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Debug("ArduinoLabel error", ex);
                StaticHelper.Instance.SystemMessage = ex.Message;
            }
            finally
            {
                if (_sp != null && _sp.IsOpen)
                    _sp.Close();
            }
            return template;
        }
    }
}

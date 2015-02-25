using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.ExternalDevices
{
    /// <summary>
    /// Interaction logic for UsbRelayReleaseConfig.xaml
    /// </summary>
    public partial class UsbRelayReleaseConfig : UserControl
    {
        public CustomConfig CustomConfig { get; set; }
        private SerialPort sp = new SerialPort();

        public string Port
        {
            get { return CustomConfig.Get("Port"); }
            set { CustomConfig.Set("Port", value); }
        }

        public string Init
        {
            get { return CustomConfig.Get("Init"); }
            set { CustomConfig.Set("Init", value); }
        }

        public string CaptureOn
        {
            get { return CustomConfig.Get("CaptureOn"); }
            set { CustomConfig.Set("CaptureOn", value); }
        }

        public string CaptureOff
        {
            get { return CustomConfig.Get("CaptureOff"); }
            set { CustomConfig.Set("CaptureOff", value); }
        }

        public UsbRelayReleaseConfig()
        {
            CustomConfig = new CustomConfig();
            InitializeComponent();
        }

        public UsbRelayReleaseConfig(CustomConfig config)
        {
            CustomConfig = config;
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmb_ports.Items.Add(port);
            }
        }

    }
}

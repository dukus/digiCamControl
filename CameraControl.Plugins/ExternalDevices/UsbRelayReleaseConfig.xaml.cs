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

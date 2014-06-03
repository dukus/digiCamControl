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

namespace CameraControl.Plugins
{
    /// <summary>
    /// Interaction logic for SerialPortShutterReleaseConfig.xaml
    /// </summary>
    public partial class SerialPortShutterReleaseConfig : UserControl
    {
        public CustomConfig CustomConfig { get; set; }


        public string Port
        {
            get { return CustomConfig.Get("Port"); }
            set { CustomConfig.Set("Port", value); }
        }

        public SerialPortShutterReleaseConfig()
        {
            InitializeComponent();
            CustomConfig = new CustomConfig();
        }

        public SerialPortShutterReleaseConfig(CustomConfig config)
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

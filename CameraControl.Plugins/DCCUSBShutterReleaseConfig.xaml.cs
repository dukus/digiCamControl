using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Plugins
{
    /// <summary>
    /// Interaction logic for SerialPortShutterReleaseConfig.xaml
    /// </summary>
    public partial class DCCUSBShutterReleaseConfig : UserControl
    {
        public CustomConfig CustomConfig { get; set; }
        private SerialPort sp = new SerialPort();


        public string Port
        {
            get { return CustomConfig.Get("Port"); }
            set { CustomConfig.Set("Port", value); }
        }

        public bool IrRemote
        {
            get { return CustomConfig.Get("IrRemote") == "True"; }
            set { CustomConfig.Set("IrRemote", value.ToString()); }
        }


        public DCCUSBShutterReleaseConfig()
        {
            InitializeComponent();
            CustomConfig = new CustomConfig();
            
        }

        public DCCUSBShutterReleaseConfig(CustomConfig config)
        {
            CustomConfig = config;
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmb_ports.Items.Add(port);
            }
        }

        private void btn_vers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sp.IsOpen)
                {
                    sp.DataReceived -= sp_DataReceived;
                    sp.Close();
                }
                sp.PortName = Port;
                sp.BaudRate = 9600;
                sp.WriteTimeout = 3500;
                sp.Open();
                sp.DataReceived += sp_DataReceived;
                sp.WriteLine("v");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Log.Error("Serial error", ex);
                Dispatcher.Invoke(new Action(delegate { lbl_mess.Content = "Error"; }));
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort)sender;
            string str = spL.ReadLine();
            Dispatcher.Invoke(new Action(delegate { lbl_mess.Content = str; }));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sp.IsOpen)
                {
                    sp.DataReceived -= sp_DataReceived;
                    sp.Close();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

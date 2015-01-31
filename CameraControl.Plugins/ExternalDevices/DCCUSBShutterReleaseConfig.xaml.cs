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
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Devices;

#endregion

namespace CameraControl.Plugins.ExternalDevices
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

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort) sender;
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
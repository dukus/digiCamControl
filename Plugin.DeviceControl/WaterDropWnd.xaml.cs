using System;
using System.IO.Ports;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CameraControl.Core;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Nikon;
using MahApps.Metro.Controls;

namespace Plugin.DeviceControl
{
    /// <summary>
    /// Interaction logic for WaterDropWnd.xaml
    /// </summary>
    public partial class WaterDropWnd :MetroWindow, IToolPlugin
    {
        private SerialPort sp = new SerialPort();
        private object _locker = new object();
        private Timer _timer = new Timer();


        public WaterDropWnd()
        {
            InitializeComponent();
            Title = "Water drop control";
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmb_ports.Items.Add(port);
            }
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var camera = ServiceProvider.DeviceManager.SelectedCameraDevice as NikonBase;
            if (camera != null)
            {
                camera.StartEventTimer();
            }
        }

        #region Implementation of IToolPlugin

        public bool Execute()
        {
            WaterDropWnd wnd = new WaterDropWnd();
            wnd.Show();
            return true;
        }

        public string Id
        {
            get { return "{D69B4655-0FA9-453F-A995-89C17D69D8DD}"; }
        }

        public void Init()
        {
            
        }

        #endregion

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ServiceProvider.Settings.DefaultSession.WriteComment)
                {
                    ServiceProvider.Settings.DefaultSession.Comment = "c=" + slider_cmera.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "dw=" + slider_drop_wait.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "dw2=" + slider_drop2_wait.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "d1=" + slider_drop1.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "d2=" + slider_drop2.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "d3=" + slider_drop3.Value + "|";
                    ServiceProvider.Settings.DefaultSession.Comment += "f=" + slider_flash.Value.ToString() + "|";
                }
                if (chk_external.IsChecked == true)
                {

                    NikonBase camera = ServiceProvider.DeviceManager.SelectedCameraDevice as NikonBase;
                    if (camera != null)
                    {
                        camera.StopEventTimer();
                        _timer.Interval = GetTotalLength() + 100;
                        _timer.Start();

                    }
                }
                else
                {
                    ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
                }
                OpenPort();
                sp.WriteLine(" ");
            }
            catch (Exception exception)
            {
                lst_message.Items.Add(exception.Message);
            }
        }

        private void OpenPort()
        {
            if (!sp.IsOpen)
            {
                sp.PortName = (string)cmb_ports.SelectedItem;
                sp.BaudRate = 9600;
                sp.WriteTimeout = 3500;
                sp.Open();
                sp.DataReceived += sp_DataReceived;
            }
        }

        private void ClosePort()
        {
            if(sp.IsOpen)
            {
                sp.DataReceived -= sp_DataReceived;
                sp.Close();
            }
        }
         
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort spL = (SerialPort)sender;
                string str = spL.ReadLine();
                Dispatcher.Invoke(new Action(delegate { lst_message.Items.Add(str); }));
                if (str.Contains("??"))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            if (chk_external.IsChecked != true)
                            {
                                ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
                            }
                        }
                        catch (Exception ex)
                        {
                            StaticHelper.Instance.SystemMessage = ex.Message;

                        }
                    }));
                }
                if (str.Contains("="))
                {
                    string command = str.Split('=')[0];
                    int value = 0;
                    if (int.TryParse(str.Split('=')[1], out value))
                    {
                        switch (command)
                        {
                            case "camera_timer":
                                Dispatcher.Invoke(new Action(delegate { slider_cmera.Value = value; }));
                                break;
                            case "drop1_time":
                                Dispatcher.Invoke(new Action(delegate { slider_drop1.Value = value; }));
                                break;
                            case "drop_wait_time":
                                Dispatcher.Invoke(new Action(delegate { slider_drop_wait.Value = value; }));
                                break;
                            case "drop2_wait_time":
                                Dispatcher.Invoke(new Action(delegate { slider_drop2_wait.Value = value; }));
                                break;
                            case "drop2_time":
                                Dispatcher.Invoke(new Action(delegate { slider_drop2.Value = value; }));
                                break;
                            case "drop3_time":
                                Dispatcher.Invoke(new Action(delegate { slider_drop3.Value = value; }));
                                break;
                            case "flash_time":
                                Dispatcher.Invoke(new Action(delegate { slider_flash.Value = value; }));
                                break;
                            case "sound":
                                Dispatcher.Invoke(new Action(delegate { prg_threshold.Value = value; }));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Data error ", ex);
            }
        }


        private int GetTotalLength()
        {
            return slider_cmera.Value + slider_drop1.Value + slider_drop_wait.Value + slider_drop2_wait.Value +
                   slider_drop2.Value + slider_drop3.Value + slider_flash.Value;
        }

        private void SendData()
        {
            lock (_locker)
            {
                try
                {
                    lst_message.Items.Clear();
                    ClosePort();
                    OpenPort();
                    sp.WriteLine("c=" + slider_cmera.Value);
                    sp.WriteLine("dw=" + slider_drop_wait.Value);
                    sp.WriteLine("dw2=" + slider_drop2_wait.Value);
                    sp.WriteLine("d1=" + slider_drop1.Value);
                    sp.WriteLine("d2=" + slider_drop2.Value);
                    sp.WriteLine("d3=" + slider_drop3.Value);
                    sp.WriteLine("f=" + slider_flash.Value.ToString());
                }
                catch (Exception exception)
                {
                    lst_message.Items.Add(exception.Message);
                }
            }
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                ClosePort();
            }
            catch (Exception)
            {
            }
        }

        private void btn_get_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("?");
        }

        private void cmb_ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SendCommand("?");
        }

        private void btn_valve_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("|");
        }

        private void SendCommand(string cmd)
        {
            lock (_locker)
            {
                try
                {
                    ClosePort();
                    OpenPort();
                    sp.Write(cmd);
                }
                catch (Exception exception)
                {
                    lst_message.Items.Add(exception.Message);
                }
            }
        }

        private void btn_drop_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("<");
        }

        private void btn_valve_close_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("\\");
        }

        private void MetroWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                btn_start_Click(null, null);
                e.Handled = true;
            }
        }

        private void btn_start_detect_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("m=3");
        }

        private void btn_stop_detect_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("m=9");
        }

    }
}

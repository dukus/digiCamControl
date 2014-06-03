using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PortableDeviceLib;

namespace MtpTester
{
    /// <summary>
    /// Interaction logic for SelectDevice.xaml
    /// </summary>
    public partial class SelectDevice 
    {
        private const string AppName = "CameraControl";
        private const int AppMajorVersionNumber = 1;
        private const int AppMinorVersionNumber = 0;
        private List<PortableDevice> Devices = new List<PortableDevice>();

        public PortableDevice SelectedDevice { get; set; }


        public SelectDevice()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (PortableDeviceCollection.Instance == null)
            {
                PortableDeviceCollection.CreateInstance(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
                PortableDeviceCollection.Instance.AutoConnectToPortableDevice = false;
            }
            foreach (PortableDevice portableDevice in PortableDeviceCollection.Instance.Devices)
            {
                portableDevice.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
                Devices.Add(portableDevice);
            }
            cmb_devices.ItemsSource = Devices;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = (PortableDevice) cmb_devices.SelectedItem;
            DialogResult = true;
        }
    }
}

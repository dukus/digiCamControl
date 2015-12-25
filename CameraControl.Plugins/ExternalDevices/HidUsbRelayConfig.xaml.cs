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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.ExternalDevices
{
    /// <summary>
    /// Interaction logic for HidUsbRelayConfig.xaml
    /// </summary>
    public partial class HidUsbRelayConfig : UserControl
    {
        public CustomConfig CustomConfig { get; set; }
        public HidUsbRelayConfig(CustomConfig config)
        {
            CustomConfig = config;
            InitializeComponent();
        }
        public HidUsbRelayConfig()
        {
            CustomConfig = new CustomConfig();
            InitializeComponent();
        }

    }
}

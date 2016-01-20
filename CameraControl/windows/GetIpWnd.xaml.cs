using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Annotations;
using CameraControl.Core;
using CameraControl.Devices;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for GetIpWnd.xaml
    /// </summary>
    public partial class GetIpWnd :INotifyPropertyChanged
    {
        private IWifiDeviceProvider _wifiDeviceProvider;
        private string _ip;

        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged();
            }
        }

        public int Type { get; set; }

        public List<IWifiDeviceProvider> Providers
        {
            get { return ServiceProvider.DeviceManager.WifiDeviceProviders; }
        }

        public IWifiDeviceProvider WifiDeviceProvider
        {
            get { return _wifiDeviceProvider; }
            set
            {
                _wifiDeviceProvider = value;
                OnPropertyChanged();
                Ip = _wifiDeviceProvider.DefaultIp;
            }
        }

        
        public GetIpWnd()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //txt_ip.Text = Ip;
        }

        private void btn_conect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            //Ip = txt_ip.Text;
            //Type = cmb_type.SelectedIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

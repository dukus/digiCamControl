using System;
using System.Windows.Controls;
using System.Windows.Input;
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for QrCodeWnd.xaml
    /// </summary>
    public partial class QrCodeWnd 
    {
        public QrCodeWnd()
        {
            InitializeComponent();
            if (qrcode != null)
                qrcode.Text = ServiceProvider.Settings.Webaddress;
        }

        private void textBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (qrcode != null)
                qrcode.Text = ServiceProvider.Settings.Webaddress;
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch
            {
            }
        }

        private void qrcode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhotoUtils.Run(ServiceProvider.Settings.Webaddress);
            //Close();
        }

    }
}

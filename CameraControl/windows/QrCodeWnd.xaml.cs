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
            ServiceProvider.Settings.ApplyTheme(this);
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

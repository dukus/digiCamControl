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

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for GetIpWnd.xaml
    /// </summary>
    public partial class GetIpWnd 
    {
        public string Ip { get; set; }
        public int Type { get; set; }
        

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

    }
}

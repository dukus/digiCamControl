using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using CameraControl.ServerTest.ServiceReference1;

namespace CameraControl.ServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost _mProcessingServiceHost;

        private PipeClientT _pipeClient;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_sen_Click(object sender, RoutedEventArgs e)
        {
            _pipeClient = new PipeClientT();
            txt_resp.Text = _pipeClient.Send(txt_mess.Text , "DCCPipe", 4000);
        }

        private void ButConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mProcessingServiceHost = new ServiceHost(typeof(CameraServiceClient));
                    
                _mProcessingServiceHost.Open();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

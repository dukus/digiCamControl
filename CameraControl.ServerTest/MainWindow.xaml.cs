using System;
using System.Windows;

namespace CameraControl.ServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

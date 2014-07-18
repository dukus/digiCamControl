using System;
using System.Collections.Generic;
using System.IO;
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
                CameraServiceClient client = new CameraServiceClient();
                var base64String = client.TakePhotoAsBase64String(20, 200, 200, 0);

                byte[] byteBuffer = Convert.FromBase64String(base64String);
                File.WriteAllBytes("d:\\1\\test.jpg", byteBuffer);
                MemoryStream memoryStream = new MemoryStream(byteBuffer);

                memoryStream.Position = 0;
               
                imgPreview.BeginInit();
                imgPreview.Source = BitmapFrame.Create(memoryStream,
                                       BitmapCreateOptions.None,
                                       BitmapCacheOption.OnLoad); ;
                imgPreview.EndInit();
                memoryStream.Close();
                //memoryStream = null;
                //byteBuffer = null;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

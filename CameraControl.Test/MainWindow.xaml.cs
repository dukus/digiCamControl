using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace CameraControl.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TcpClient client = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                client = new TcpClient("192.168.42.1", 7878);
            Thread.Sleep(30);
            try
            {
                //Stream s = client.GetStream();
                client.Client.Send(Encoding.ASCII.GetBytes(TextBox_send.Text.ToCharArray()));
                client.ReceiveTimeout = 2000;
                byte[] buffer = new byte[80];
                Receive(client.Client, buffer, 0, buffer.Length, 10000);
                string str = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                TextBox_receive.Text = str;
                Receive(client.Client, buffer, 0, buffer.Length, 10000);
                str = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                TextBox_receive.Text = str;
            }
            finally
            {
                // code in finally block is guranteed 
                // to execute irrespective of 
                // whether any exception occurs or does 
                // not occur in the try block
                client.Close();
            }
        }

        public static void Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    var recived = socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                    received += recived;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex; // any serious error occurr
                }
                Console.WriteLine(socket.Available);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                do
                {
                    read = input.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, read);
                } while (read == buffer.Length);
                return ms.ToArray();
            }
        }
    }
}

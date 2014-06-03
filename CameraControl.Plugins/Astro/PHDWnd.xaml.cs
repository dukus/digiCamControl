using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using CameraControl.Devices;

namespace CameraControl.Plugins.Astro
{
    internal enum AggressivenessENUM
    {
        LOW = 3,
        MEDIUM = 4,
        HIGH = 5,
        VERYHIGH = 12,
        EXTREME = 13,
    }

    public enum OpersEnum
    {
        MSG_PAUSE = 1,
        MSG_RESUME = 2,
        MSG_MOVE1 = 3,
        MSG_MOVE2 = 4,
        MSG_MOVE3 = 5,
        MSG_IMAGE = 6,
        MSG_GUIDE = 7,
        MSG_CAMCONNECT = 8,
        MSG_CAMDISCONNECT = 9,
        MSG_REQDIST = 10,
        MSG_REQFRAME = 11,
        MSG_MOVE4 = 12,
        MSG_MOVE5 = 13,
    }

    /// <summary>
    /// Interaction logic for PHDWnd.xaml
    /// </summary>
    public partial class PHDWnd
    {
        public bool Cancel { get; set; }


        private TcpClient socket;

        public PHDWnd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            try
            {
                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 4300);

                // Create a TCP/IP  socket.
                socket = new TcpClient("localhost", 4300);
                //socket.Connect(remoteEP);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect PHD", exception);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            SendReceiveTest2(socket, OpersEnum.MSG_PAUSE);
        }

        public static int SendReceiveTest2(TcpClient server, OpersEnum opersEnum)
        {
            byte[] msg = Encoding.UTF8.GetBytes("This is a test");
            byte[] bytes = new byte[256];
            try
            {
                // Blocks until send returns. 
                int byteCount = server.Client.Send(new[] {(byte) opersEnum}, SocketFlags.None);
                Console.WriteLine("Sent {0} bytes.", byteCount);

                // Get reply from the server.
                byteCount = server.Client.Receive(bytes, SocketFlags.None);
                Console.WriteLine(byteCount);
                //if (byteCount > 0)
                //    Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                return (e.ErrorCode);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Unable to comunicate with PhD" + exception.Message);
            }
            return 0;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            SendReceiveTest2(socket, OpersEnum.MSG_RESUME);
        }

        private void but_move_Click(object sender, RoutedEventArgs e)
        {
            SendReceiveTest2(socket, OpersEnum.MSG_MOVE2);
        }

        private void btn_guid_Click(object sender, RoutedEventArgs e)
        {
            SendReceiveTest2(socket, OpersEnum.MSG_GUIDE);
        }

        private void DoDither(TcpClient socket, AggressivenessENUM moveX, float settleAt)
        {
            DateTime now = DateTime.Now;
            DateTime dateTime = now.AddSeconds(45.0);
            int num1 = SetAggressiveness(socket, moveX);
            byte[] buffer = new byte[1];
            float num2 = (float) byte.MaxValue/100f;
            int num3 = 3;
            int num4 = 0;
            while (!Cancel && (!(DateTime.Now > dateTime) && (double) num2 > (double) settleAt))
            {
                Thread.Sleep(num1*1000);
                socket.GetStream().Flush();
                socket.Client.Send(new byte[1]
                                       {
                                           10
                                       });
                while (!Cancel && socket.Available > 0)
                {
                    socket.Client.Receive(buffer, 1, SocketFlags.None);
                    num2 = (float) int.Parse(buffer[0].ToString())/100f;
                    Log.Debug("PHD Dithering pixel value = " + num2.ToString());
                }
                if ((double) num2 > 0.0 && num4 < num3)
                    num2 = 2.55f;
                ++num4;
            }
            if (Cancel)
            {
                Log.Debug("Dither canceled");
            }
            else
            {
                Log.Debug(string.Format("PHD Dithering ran for {0:0} seconds",
                                        (object) DateTime.Now.Subtract(now).TotalSeconds));
            }
        }

        private int SetAggressiveness(TcpClient socket, AggressivenessENUM moveX)
        {
            byte[] buffer = new byte[1];
            int val2 = 0;
            if (socket != null)
            {
                socket.GetStream().Flush();
                socket.Client.Send(new byte[1]
                                       {
                                           (byte) (ushort) moveX
                                       });
                Thread.Sleep(1000);
                while (!Cancel && socket.Available > 0)
                {
                    socket.Client.Receive(buffer, 1, SocketFlags.None);
                    val2 = int.Parse(buffer[0].ToString());
                }
            }
            return Math.Max(1, val2);
        }

        private void btn_diter_Click(object sender, RoutedEventArgs e)
        {
            DoDither(socket,AggressivenessENUM.HIGH, (float) 2.55);
        }
    }
}

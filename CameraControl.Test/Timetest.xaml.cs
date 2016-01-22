using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace CameraControl.Test
{
    /// <summary>
    /// Interaction logic for Timetest.xaml
    /// </summary>
    public partial class Timetest : Window
    {
        private Timer _timer = new Timer(1000);
        private DateTime _oldTime = DateTime.Now;
        private double difs = 0;

        public Timetest()
        {
            InitializeComponent();
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            _oldTime = DateTime.Now;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var t = DateTime.Now;
                                        ListBox.Items.Add(t.ToString("O")+" >"+(t-_oldTime).TotalMilliseconds+" <> "+difs);
                difs += ((t - _oldTime).TotalMilliseconds - 1000);
                _oldTime = t;
            }
                );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

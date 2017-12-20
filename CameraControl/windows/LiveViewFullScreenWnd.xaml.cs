using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for LiveViewFullScreenWnd.xaml
    /// </summary>
    public partial class LiveViewFullScreenWnd 
    {
        public LiveViewFullScreenWnd()
        {
            InitializeComponent();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TriggerClass.KeyDown(e);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void image1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void image1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            //ShowTitleBar = WindowState != WindowState.Maximized;
        }
    }
}

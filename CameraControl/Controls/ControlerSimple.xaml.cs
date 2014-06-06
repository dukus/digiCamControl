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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Controls
{
    /// <summary>
    /// Interaction logic for ControlerSimple.xaml
    /// </summary>
    public partial class ControlerSimple : UserControl
    {
        public ControlerSimple()
        {
            InitializeComponent();
        }

        private void btn_capture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CameraHelper.Capture(DataContext);
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = exception.Message;
                Log.Error("Take photo", exception);
            }
        }
    }
}

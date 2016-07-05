using System;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Devices;

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

using System;
using System.Windows.Controls;
using System.Windows.Input;
using CameraControl.Devices;
using Capture.Workflow.Plugins.Views.ViewModel;

namespace Capture.Workflow.Plugins.Views.View
{
    /// <summary>
    /// Interaction logic for LiveViewUI.xaml
    /// </summary>
    public partial class LiveViewUI : UserControl
    {
        public LiveViewUI()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left )
            {
                try
                {
                    var _image = (Image) sender;
                    ((LiveviewViewModel)DataContext).SetFocusPos(e.MouseDevice.GetPosition(_image), _image.ActualWidth,
                        _image.ActualHeight);
                }
                catch (Exception exception)
                {
                    Log.Error("Focus Error", exception);

                }
            }
        }
    }
}

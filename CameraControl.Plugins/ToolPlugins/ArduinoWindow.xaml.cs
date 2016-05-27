using System;

namespace CameraControl.Plugins.ToolPlugins
{
    /// <summary>
    /// Interaction logic for ArduinoWindow.xaml
    /// </summary>
    public partial class ArduinoWindow 
    {
        public ArduinoWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            ((ArduinoViewModel) DataContext).SaveButtons();
        }
    }
}

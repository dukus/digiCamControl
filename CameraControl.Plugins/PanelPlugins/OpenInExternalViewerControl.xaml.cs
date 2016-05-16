using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.PanelPlugins
{
    /// <summary>
    /// Interaction logic for OpenInExternalViewerControl.xaml
    /// </summary>
    public partial class OpenInExternalViewerControl : UserControl
    {
        public OpenInExternalViewerControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ViewExternal);
        }
    }
}

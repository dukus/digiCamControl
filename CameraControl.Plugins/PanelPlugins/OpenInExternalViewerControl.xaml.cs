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
            if (ServiceProvider.Settings.SelectedBitmap == null ||
                ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            if (!string.IsNullOrWhiteSpace(ServiceProvider.Settings.ExternalViewer) &&
                File.Exists(ServiceProvider.Settings.ExternalViewer))
            {
                PhotoUtils.Run(ServiceProvider.Settings.ExternalViewer,
                   "\"" + ServiceProvider.Settings.SelectedBitmap.FileItem.FileName + "\"", ProcessWindowStyle.Maximized);
            }
            else
            {
                PhotoUtils.Run("\"" + ServiceProvider.Settings.SelectedBitmap.FileItem.FileName + "\"", "",
                    ProcessWindowStyle.Maximized);
            }
        }
    }
}

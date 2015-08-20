using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileName, ProcessWindowStyle.Maximized);
            }
            else
            {
                PhotoUtils.Run(ServiceProvider.Settings.SelectedBitmap.FileItem.FileName, "",
                    ProcessWindowStyle.Maximized);
            }
        }
    }
}

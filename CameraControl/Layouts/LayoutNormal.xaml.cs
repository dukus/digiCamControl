using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using Microsoft.VisualBasic.FileIO;
using Xceed.Wpf.Toolkit.Core.Input;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace CameraControl.Layouts
{
    /// <summary>
    /// Interaction logic for LayoutNormal.xaml
    /// </summary>
    public partial class LayoutNormal : LayoutBase
    {
        public LayoutNormal()
        {
            InitializeComponent();
            ImageLIst = ImageLIstBox;
            InitServices();
            zoombox.RelativeZoomModifiers.Clear();
            zoombox.RelativeZoomModifiers.Add(KeyModifier.None);
            zoombox.DragModifiers.Clear();
            zoombox.DragModifiers.Add(KeyModifier.None);
            zoombox.KeepContentInBounds = true;
        }

        private void zoombox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           
        }

        private void zoombox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
        }

        private void zoombox_ViewStackIndexChanged(object sender, Xceed.Wpf.Toolkit.Core.IndexChangedEventArgs e)
        {
            LoadFullRes();
        }

        public override void OnImageLoaded()
        {
            Dispatcher.Invoke(new Action(() => zoombox.FitToBounds()));
        }
    }
}

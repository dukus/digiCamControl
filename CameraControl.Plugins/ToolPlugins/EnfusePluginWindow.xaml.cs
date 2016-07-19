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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraControl.Plugins.ToolPlugins
{
    /// <summary>
    /// Interaction logic for EnfusePluginWindow.xaml
    /// </summary>
    public partial class EnfusePluginWindow 
    {
        public EnfusePluginWindow()
        {
            InitializeComponent();
        }

        private void zoomAndPanControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            zoomAndPanControl.ScaleToFit();
        }

        private void zoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point curContentMousePoint = e.GetPosition(Image);
            if (zoomAndPanControl.ContentScale <= zoomAndPanControl.FitScale())
            {
                zoomAndPanControl.ZoomAboutPoint(4, curContentMousePoint);
            }
            else
            {
                zoomAndPanControl.ScaleToFit();
            }
        }

        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            Point curContentMousePoint = e.GetPosition(Image);
            if (e.Delta > 0)
            {
                zoomAndPanControl.ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                // don't allow zoomout les that original image 
                if (zoomAndPanControl.ContentScale - 0.2 > zoomAndPanControl.FitScale())
                {
                    zoomAndPanControl.ZoomOut(curContentMousePoint);
                }
                else
                {
                    zoomAndPanControl.ScaleToFit();
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            zoomAndPanControl.ScaleToFit();
        }
    }
}

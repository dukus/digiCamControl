using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class PrintPlugin : IAutoExportPlugin
    {

        public bool Execute(string filename, AutoExportPluginConfig configData)
        {
            //if (!configData.IsRedy)
            //    return false;
            //Print(filename, configData);
            Thread thread = new Thread(() => Print(filename, configData));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return true;
        }

        private void Print(string filename, AutoExportPluginConfig configData)
        {
            try
            {
                PrintDialog dlg = new PrintDialog();
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new PrintPluginViewModel(configData);
                var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(conf.TransformPlugin);

                outfile = tp != null && conf.TransformPlugin != BasePluginViewModel.EmptyTransformFilter
                    ? tp.Execute(filename, outfile, configData.ConfigData)
                    : filename;

                System.Printing.PrintCapabilities capabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
                var PageWidth = (int)capabilities.PageImageableArea.ExtentWidth;
                var PageHeight = (int)capabilities.PageImageableArea.ExtentHeight;

                var panel = new StackPanel
                {
                    Margin = new Thickness(conf.Margin),
                };

                var image = new Image
                {
                    Source = BitmapLoader.Instance.LoadImage(outfile, PageWidth > PageHeight ? PageWidth : PageHeight, conf.Rotate ? 90 : 0),
                    Width = PageWidth ,
                    Height = PageHeight,
                    Stretch = Stretch.Uniform,
                };

                
                panel.Children.Add(image);
                panel.UpdateLayout();
                panel.Measure(new Size(PageWidth, PageHeight));
                panel.Arrange(new Rect(new Point(0, 0), panel.DesiredSize));
                panel.UpdateLayout();
                dlg.PrintVisual(panel, filename);
                image.Source = null;
                panel.Children.Clear();
            }
            catch (Exception exception)
            {
                Log.Error("Error print file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }

            configData.IsRedy = true;            
        }
        

        public bool Configure(AutoExportPluginConfig config)
        {
            PrintPluginConfig wnd = new PrintPluginConfig();
            wnd.DataContext = new PrintPluginViewModel(config);
            wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Print File"; }
        }
    }
}

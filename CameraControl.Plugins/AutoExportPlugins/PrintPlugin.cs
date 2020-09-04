using System;
using System.IO;
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

        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            //if (!configData.IsRedy)
            //    return false;
            //Print(filename, configData);
            Thread thread = new Thread(() => Print(item, configData));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return true;
        }

        private void Print(FileItem item, AutoExportPluginConfig configData)
        {
            try
            {
                PrintDialog dlg = new PrintDialog();
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new PrintPluginViewModel(configData);
                var outfile = PhotoUtils.ReplaceExtension(Path.GetTempFileName(), Path.GetExtension(item.Name));

                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                System.Printing.PrintCapabilities capabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
                var PageWidth = (int)capabilities.PageImageableArea.ExtentWidth;
                var PageHeight = (int)capabilities.PageImageableArea.ExtentHeight;

                var panel = new StackPanel
                {
                    Margin = new Thickness(conf.Margin),
                };

                var image = new Image
                {
                    Source = BitmapLoader.Instance.LoadImage(outfile, 0, conf.Rotate ? 90 : 0),
                    Width = PageWidth ,
                    Height = PageHeight,
                    Stretch = Stretch.Uniform,
                };

               
                panel.Children.Add(image);
                panel.UpdateLayout();
                panel.Measure(new Size(PageWidth, PageHeight));
                panel.Arrange(new Rect(new Point(0, 0), panel.DesiredSize));
                panel.UpdateLayout();
                dlg.PrintVisual(panel, item.Name);
                image.Source = null;
                panel.Children.Clear();
                // remove unused file
                if (outfile != item.FileName)
                {
                    PhotoUtils.WaitForFile(outfile);
                    File.Delete(outfile);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error print file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }

            configData.IsRedy = true;            
        }
        
        public string Name
        {
            get { return "Print File"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cntr = new PrintPluginConfig()
            {
                DataContext = new PrintPluginViewModel(configData)
            };
            return cntr;
        }
    }
}

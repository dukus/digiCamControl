using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class TransformPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var filename = item.FileName;
            var conf = new TransformPluginViewModel(configData);
            var outfile = Path.GetTempFileName();
            outfile = PhotoUtils.ReplaceExtension(outfile, Path.GetExtension(filename));
            outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);
            if (conf.CreateNew)
            {
                string newFile = Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + "_transformed" + ".jpg");
                newFile = PhotoUtils.GetNextFileName(newFile);

                File.Copy(outfile, newFile, true);

                if (ServiceProvider.Settings.DefaultSession.GetFile(newFile) == null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        FileItem im = new FileItem(newFile);
                        im.Transformed = true;
                        var i = ServiceProvider.Settings.DefaultSession.Files.IndexOf(item);
                        if (ServiceProvider.Settings.DefaultSession.Files.Count - 1 == i)
                        {
                            ServiceProvider.Settings.DefaultSession.Files.Add(im);
                        }
                        else
                        {
                            ServiceProvider.Settings.DefaultSession.Files.Insert(i + 1, im);
                        }
                    }));
                }
            }
            else
            {
                // wait for file to be not locked
                PhotoUtils.WaitForFile(filename);
                File.Copy(outfile, filename, true);
                item.IsLoaded = false;
                item.RemoveThumbs();
                item.Transformed = true;
            }
            // remove unused file
            if (outfile != item.FileName)
            {
                PhotoUtils.WaitForFile(outfile);
                File.Delete(outfile);
            }
            configData.IsRedy = true;
            return true; 
        }

        public string Name
        {
            get { return "Transform"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new TransformPluginConfig
            {
                DataContext = new TransformPluginViewModel(configData)
            };
            return cnt;
        }
    }
}

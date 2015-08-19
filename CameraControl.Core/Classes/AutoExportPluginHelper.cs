using System;
using System.IO;
using CameraControl.Devices;

namespace CameraControl.Core.Classes
{
    public class AutoExportPluginHelper
    {
        public static string ExecuteTransformPlugins(FileItem item, AutoExportPluginConfig configData, string outfile,
            bool preview = false)
        {
            File.Copy(preview ? item.LargeThumb : item.FileName, outfile, true);
            if (configData.ConfigDataCollection == null || configData.ConfigDataCollection.Count == 0)
                return outfile;
            foreach (var enumerator in configData.ConfigDataCollection)
            {
                try
                {
                    var plugin = enumerator["TransformPlugin"];
                    var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(plugin);
                    if (tp != null)
                        outfile = tp.Execute(item, outfile, outfile, enumerator);
                }
                catch (Exception exception)
                {
                    Log.Error("Error execute transform olugin ", exception);
                }
            }
            return outfile;
        }

    }
}

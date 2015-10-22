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
            return ExecuteTransformPlugins(item, configData, preview ? item.LargeThumb : item.FileName, outfile);
        }

        public static string ExecuteTransformPlugins(FileItem item, AutoExportPluginConfig configData, string infile,
            string outfile)
        {
            if (infile != outfile)
                File.Copy(infile, outfile, true);
            if (configData.ConfigDataCollection == null || configData.ConfigDataCollection.Count == 0)
                return outfile;
            foreach (var enumerator in configData.ConfigDataCollection)
            {
                var plugin = enumerator["TransformPlugin"];
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(plugin);
                if (tp != null)
                {
                    outfile = tp.Execute(item, outfile, outfile, enumerator);
                    ServiceProvider.Analytics.TransformPluginExecute(plugin);
                    Log.Debug("TransformPlugin executed " + plugin);
                }
            }
            return outfile;
        }

    }
}

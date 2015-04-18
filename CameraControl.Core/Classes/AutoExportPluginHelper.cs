using System.IO;

namespace CameraControl.Core.Classes
{
    public class AutoExportPluginHelper
    {
        public static string ExecuteTransformPlugins(FileItem item, AutoExportPluginConfig configData, string outfile)
        {
            File.Copy(item.FileName, outfile, true);
            if (configData.ConfigDataCollection == null || configData.ConfigDataCollection.Count == 0)
                return outfile;
            foreach (var enumerator in configData.ConfigDataCollection)
            {
                var plugin = enumerator["TransformPlugin"];
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(plugin);
                if (tp != null)
                    outfile = tp.Execute(item, outfile, outfile, enumerator);
            }
            return outfile;
        }

    }
}

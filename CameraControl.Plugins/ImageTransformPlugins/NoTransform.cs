using System.IO;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class NoTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "NoTransform"; } 
        }

        public string Execute(FileItem item,string infile, string dest, ValuePairEnumerator configData)
        {
            if (infile != dest)
                File.Copy(infile, dest, true);
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            return null;
        }
    }
}

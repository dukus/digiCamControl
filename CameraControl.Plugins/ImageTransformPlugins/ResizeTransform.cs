using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using ImageMagick;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    class ResizeTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Resize"; }
        }

        public string Execute(FileItem fileItem,string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new ResizeTransformViewModel(configData);
            dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");
            using (MagickImage image = new MagickImage(infile))
            {
                MagickGeometry geometry = new MagickGeometry(conf.Width, conf.Height);
                geometry.IgnoreAspectRatio = !conf.KeepAspect;

                image.Resize(geometry);
                image.Format = MagickFormat.Jpeg;
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ResizeTransformView();
            control.DataContext = new ResizeTransformViewModel(configData);
            return control;
        }


    }
}

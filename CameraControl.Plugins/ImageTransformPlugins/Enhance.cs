using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using ImageMagick;
using System.IO;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class Enhance : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Enhance"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new EnhanceViewModel(configData);
            dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");
            using (MagickImage image = new MagickImage(infile))
            {
                if (conf.Normalize)
                    image.Normalize();
                if (conf.AutoGamma)
                    image.AutoGamma();
                image.BrightnessContrast(new Percentage(conf.Brightness), new Percentage(conf.Contrast));
                if (conf.SContrast > 0)
                    image.SigmoidalContrast(true, conf.SContrast);
                if (conf.Edge)
                    image.AdaptiveSharpen();
                if (conf.Sharpen > 0)
                    image.Unsharpmask(1.5, 1.5, conf.Sharpen/100.0, 0.2);
                image.Format = MagickFormat.Jpeg;
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new EnhanceView();
            control.DataContext = new EnhanceViewModel(configData);
            return control;
        }
    }
}

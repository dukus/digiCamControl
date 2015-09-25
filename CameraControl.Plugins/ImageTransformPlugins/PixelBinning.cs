using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using ImageMagick;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class PixelBinning : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Pixel Binning"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new PixelBinningViewModel(configData);
            dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");

            using (MagickImage image = new MagickImage(infile))
            {
                int newx = image.Width/(conf.SelectedMode + 2);
                int newy = image.Height / (conf.SelectedMode + 2);
                int cropx = newx * (conf.SelectedMode + 2);
                int cropy = newy * (conf.SelectedMode + 2);
                if (cropx != image.Width || cropy != image.Height)
                    image.Crop(cropx, cropy, Gravity.Center);
                image.FilterType = FilterType.Box;
                image.Resize(newx,newy);
                image.Format = MagickFormat.Jpeg;
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new PixelBinningView();
            control.DataContext = new PixelBinningViewModel(configData);
            return control;
        }
    }
}

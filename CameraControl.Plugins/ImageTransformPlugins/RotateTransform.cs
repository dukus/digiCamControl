using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using ImageMagick;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class RotateTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Rotate"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new RotateTransformViewModel(configData);
            // Read from file
            using (MagickImage image = new MagickImage(infile))
            {
                
                image.BackgroundColor = new MagickColor(Color.Black.R, Color.Black.G, Color.Black.B);
                if (conf.ManualRotate)
                    image.Rotate(item.RotationAngle);
                else
                {
                    if (conf.AutoRotate)
                    {
                        IExifProfile profile = image.GetExifProfile();
                        image.AutoOrient();
                        profile.SetValue(ExifTag.Orientation, (UInt16) 0);
                    }
                }
                if (conf.Angle > 0)
                    image.Rotate(conf.Angle);

                if(conf.FlipHorizontal)
                    image.Flop();

                if (conf.FlipVertical)
                    image.Flip();

                image.Format = MagickFormat.Jpeg;
                // Save the result
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new RotateTransformView { DataContext = new RotateTransformViewModel(configData) };
            return control;
        }
    }
}

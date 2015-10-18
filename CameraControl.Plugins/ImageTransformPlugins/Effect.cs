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
    public class Effect : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Effect"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new EffectViewModel(configData);
            dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");
            using (MagickImage image = new MagickImage(infile))
            {
                switch (conf.SelectedMode)
                {
                    case 0:
                        image.SepiaTone(new Percentage(conf.Param1));
                        break;
                    case 1:
                        image.OilPaint(conf.Param1);
                        break;
                    case 2:
                        image.Sketch();
                        break;
                    case 3:
                        image.Charcoal();
                        break;
                    case 4:
                        image.Solarize();
                        break;
                    case 5:
                        image.Swirl(conf.Param1);
                        break;
                    case 6:
                        image.Wave(conf.Param1, conf.Param2);
                        break;
                    case 7:
                        image.BlueShift();
                        break;
                    case 8:
                        image.RotationalBlur(conf.Param1);
                        break;
                    case 9:
                        image.Raise(conf.Param1);
                        break;
                }
                image.Format = MagickFormat.Jpeg;
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new EffectView();
            control.DataContext = new EffectViewModel(configData);
            return control;
        }
    }
}

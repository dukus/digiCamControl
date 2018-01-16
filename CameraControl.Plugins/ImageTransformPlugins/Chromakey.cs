using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;
using Kaliko.ImageLibrary.Scaling;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class Chromakey : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Chromakey"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            try
            {

            var conf = new ChromakeyViewModel(configData);
            dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");

            KalikoImage image = new KalikoImage(infile);
            var x = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(conf.BackgroundColor);
            var filter = new ChromaKeyFilter();
            filter.KeyColor = Color.FromArgb(x.R, x.G, x.B);
            filter.ToleranceHue = conf.Hue;
            filter.ToleranceSaturnation = conf.Saturnation/100f;
            filter.ToleranceBrightness = conf.Brigthness / 100f;
            image.ApplyFilter(filter);
            var res = image.Clone();
            
            if (conf.UnsharpMask)
                res.ApplyFilter(new UnsharpMaskFilter(1.4f, 1.32f, 5));

            var backdrop = new KalikoImage(conf.BackgroundFile);
            backdrop = backdrop.Scale(new FitScaling(image.Width, image.Height));
            backdrop.BlitImage(res);

            backdrop.SaveJpg(dest, 90);
            return dest;
            }
            catch (Exception e)
            {
                Log.Debug("Chromakey error", e);
            }
            return null;
        }


        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ChromakeyView();
            control.DataContext = new ChromakeyViewModel(configData);
            return control;
        }
    }
}

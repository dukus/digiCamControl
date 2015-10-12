using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using ICSharpCode.AvalonEdit;
using ImageMagick;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ScriptTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Script"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            dest = PhotoUtils.ReplaceExtension(dest, ".jpg");
            var conf = new ScriptTransformViewModel(configData);
            using (Stream stream = ToStream(conf.Script))
            {
                MagickScript script = new MagickScript(stream);

                // Read image from file
                using (MagickImage image = new MagickImage(infile))
                {
                    // Execute script with the image and write it to a jpg file
                    script.Execute(image);
                    image.Format = MagickFormat.Jpeg;
                    image.Write(dest);
                }
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ScriptTransformView { DataContext = new ScriptTransformViewModel(configData) };
            return control;
        }

        public static Stream ToStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}

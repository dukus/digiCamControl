using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using ImageMagick;


namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class OverlayTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Overlay"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            Thread thread = new Thread(() => ExecuteThread(item, infile, dest, configData));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return dest;
        }

        public string ExecuteThread(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new OverlayTransformViewModel(configData);
            using (MagickImage image = new MagickImage(infile))
            {
                Gravity gravity = Gravity.Center;
                if (conf.A11)
                {
                    gravity = Gravity.Northwest;
                }
                if (conf.A12)
                {
                    gravity = Gravity.North;
                }
                if (conf.A13)
                {
                    gravity = Gravity.Northeast;
                }
                if (conf.A21)
                {
                    gravity = Gravity.West;
                }
                if (conf.A22)
                {
                    gravity = Gravity.Center;
                }
                if (conf.A23)
                {
                    gravity = Gravity.East;
                }
                if (conf.A31)
                {
                    gravity = Gravity.Southwest;
                }
                if (conf.A32)
                {
                    gravity = Gravity.South;
                }
                if (conf.A33)
                {
                    gravity = Gravity.Southeast;
                }


                if (File.Exists(conf.OverlayFile))
                {
                    // Read the watermark that will be put on top of the image
                    using (MagickImage watermark = new MagickImage(conf.OverlayFile))
                    {
                        if (conf.StrechOverlay)
                            watermark.Resize(image.Width, image.Height);
                        // Optionally make the watermark more transparent
                        watermark.Evaluate(Channels.Alpha, EvaluateOperator.Add, -(255*(100 - conf.Transparency)/100));
                        // Draw the watermark in the bottom right corner
                        image.Composite(watermark, gravity, CompositeOperator.Over);

                        //// Optionally make the watermark more transparent
                        //watermark.Evaluate(Channels.Alpha, EvaluateOperator.Divide, 4);

                        //// Or draw the watermark at a specific location
                        //image.Composite(watermark, 200, 50, CompositeOperator.Over);
                    }
                }

                string text = "";
                if (!string.IsNullOrEmpty(conf.Text))
                {
                    Regex regPattern = new Regex(@"\[(.*?)\]", RegexOptions.Singleline);
                    MatchCollection matchX = regPattern.Matches(conf.Text);
                    text = matchX.Cast<Match>()
                        .Aggregate(conf.Text,
                            (current1, match) =>
                                item.FileNameTemplates.Where(
                                    template =>
                                        String.Compare(template.Name, match.Value,
                                            StringComparison.InvariantCultureIgnoreCase) == 0).Aggregate(current1,
                                                (current, template) => current.Replace(match.Value, template.Value)));

                    image.Font = conf.Font;
                    image.FontPointsize = conf.FontSize;
                    Color color = (Color) ColorConverter.ConvertFromString(conf.FontColor);
                    image.FillColor = new MagickColor(color.R, color.G, color.B, color.A);
                    image.StrokeColor = new MagickColor(color.R, color.G, color.B, color.A);
                    image.Annotate(text, gravity);
                }
                image.Format = MagickFormat.Jpeg;
                image.Write(dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new OverlayTransformView {DataContext = new OverlayTransformViewModel(configData)};
            return control;
        }
    }
}

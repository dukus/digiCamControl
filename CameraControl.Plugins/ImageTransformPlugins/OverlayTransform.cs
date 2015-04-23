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

        public string ExecuteThread(FileItem item,string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new OverlayTransformViewModel(configData);
            using (var fileStream = new MemoryStream(File.ReadAllBytes(infile)))
            {
                BitmapDecoder bmpDec = BitmapDecoder.Create(fileStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                WriteableBitmap writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);
                writeableBitmap.Freeze();
                
                Grid grid = new Grid
                {
                    Width = writeableBitmap.PixelWidth,
                    Height = writeableBitmap.PixelHeight,
                    ClipToBounds = true,
                    SnapsToDevicePixels = true
                };
                grid.UpdateLayout();
                var size = new Size(writeableBitmap.PixelWidth, writeableBitmap.PixelWidth);
                grid.Measure(size);
                grid.Arrange(new Rect(size));
                
                Image overlay = new Image();
                Image image = new Image { Width = writeableBitmap.PixelWidth, Height = writeableBitmap.PixelHeight };
                image.BeginInit();
                image.Source = writeableBitmap;
                image.EndInit();
                image.Stretch = Stretch.Fill;
                grid.Children.Add(image);
                grid.UpdateLayout();

                Regex regPattern = new Regex(@"\[(.*?)\]", RegexOptions.Singleline);
                MatchCollection matchX = regPattern.Matches(conf.Text);
                var text = matchX.Cast<Match>().Aggregate(conf.Text, (current1, match) => item.FileNameTemplates.Where(template => System.String.Compare(template.Name, match.Value, System.StringComparison.InvariantCultureIgnoreCase) == 0).Aggregate(current1, (current, template) => current.Replace(match.Value, template.Value)));

                TextBlock textBlock = new TextBlock
                {
                    Text = text,
                    Foreground = (SolidColorBrush) new BrushConverter().ConvertFromString(conf.FontColor),
                    FontFamily = (FontFamily) new FontFamilyConverter().ConvertFromString(conf.Font),
                    FontSize = conf.FontSize,
                    Opacity = conf.Transparency/100.00
                };
                if (conf.A11)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Top;
                }
                if (conf.A12)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Top;
                }
                if (conf.A13)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlock.VerticalAlignment = VerticalAlignment.Top;
                }
                if (conf.A21)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                }
                if (conf.A22)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                }
                if (conf.A23)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                }
                if (conf.A31)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                }
                if (conf.A32)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                }
                if (conf.A33)
                {
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                }

                textBlock.Margin = new Thickness(conf.Margins);
                if (File.Exists(conf.OverlayFile))
                {
                    overlay.Source = BitmapLoader.Instance.LoadImage(conf.OverlayFile, 0, 0);
                    overlay.Opacity = textBlock.Opacity;
                    if (!conf.StrechOverlay)
                    {
                        overlay.HorizontalAlignment = textBlock.HorizontalAlignment;
                        overlay.VerticalAlignment = textBlock.VerticalAlignment;
                        overlay.Stretch = Stretch.None;
                    }
                    else
                    {
                        overlay.HorizontalAlignment = HorizontalAlignment.Stretch;
                        overlay.VerticalAlignment = VerticalAlignment.Stretch;
                        overlay.Stretch = Stretch.UniformToFill;
                    }
                    grid.Children.Add(overlay);
                    grid.UpdateLayout();
                }
               
                grid.Children.Add(textBlock);
                grid.UpdateLayout();
  
                BitmapLoader.Save2Jpg(
                    BitmapLoader.SaveImageSource(grid, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), dest);
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

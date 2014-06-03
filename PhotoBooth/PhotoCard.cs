using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    public abstract class PhotoCard : Window
    {
        public PhotoCard()
        {
        }

        public abstract UIElement RootVisual
        {
            get;
        }

        /// 
        /// Gets a JPG "screenshot" of the current UIElement 
        /// 
        /// UIElement to screenshot 
        /// Scale to render the screenshot 
        /// JPG Quality 
        /// Byte array of JPG data 
        public byte[] GetJpgImage(double scale, int quality, int dpi)
        {
            double actualHeight = this.RootVisual.RenderSize.Height;
            double actualWidth = this.RootVisual.RenderSize.Width;

            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, dpi, dpi, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(this.RootVisual);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }

            renderTarget.Render(drawingVisual);
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            Byte[] _imageArray;
            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                _imageArray = outputStream.ToArray();
            }

            return _imageArray;
        }
    }
}

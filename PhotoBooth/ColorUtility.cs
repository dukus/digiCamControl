using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    public enum ColorConversion
    {
        None = 0,
        BlackAndWhite = 1,
        GrayScale = 2
    }

    public static class ColorUtility
    {
        public static ImageSource Convert(ImageSource imageSource, ColorConversion conversion)
        {
            ImageSource result = null;
            BitmapSource bitmapSource = imageSource as BitmapSource;
            if (bitmapSource != null)
            {
                PixelFormat format = PixelFormats.Default;
                switch (conversion)
                {
                    case ColorConversion.BlackAndWhite:
                        format = PixelFormats.BlackWhite;
                        break;
                    case ColorConversion.GrayScale:
                        format = PixelFormats.Gray32Float;
                        break;
                }

                if (format != PixelFormats.Default)
                {
                    result = Convert(bitmapSource, format);
                }
            }

            return result;
        }

        private static ImageSource Convert(BitmapSource input, PixelFormat format)
        {
            ////////// Convert the BitmapSource to a new format //////////// 
            // Use the BitmapImage created above as the source for a new BitmapSource object 
            // which is set to a gray scale format using the FormatConvertedBitmap BitmapSource.                                                
            // Note: New BitmapSource does not cache. It is always pulled when required.

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();

            // BitmapSource objects like FormatConvertedBitmap can only have their properties 
            // changed within a BeginInit/EndInit block.
            newFormatedBitmapSource.BeginInit();

            // Use the BitmapSource object defined above as the source for this new  
            // BitmapSource (chain the BitmapSource objects together).
            newFormatedBitmapSource.Source = input;

            // Set the new format to Gray32Float (grayscale).
            newFormatedBitmapSource.DestinationFormat = format;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        }
    }
}

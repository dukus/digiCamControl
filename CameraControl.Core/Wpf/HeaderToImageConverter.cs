using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CameraControl.Core.Wpf
{
  [ValueConversion(typeof(string), typeof(bool))]
  public class HeaderToImageConverter : IValueConverter
  {
    public static HeaderToImageConverter Instance = new HeaderToImageConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value as string).Contains(@"\"))
      {
        Uri uri = new Uri("pack://application:,,,/Images/diskdrive.png");
        BitmapImage source = new BitmapImage(uri);
        return source;
      }
      else
      {
        Uri uri = new Uri("pack://application:,,,/Images/folder.png");
        BitmapImage source = new BitmapImage(uri);
        return source;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }
}

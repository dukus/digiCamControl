using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;

namespace CameraControl.Controls
{
    public class CameraToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            try
            {
                string file = Path.Combine(Settings.ApplicationFolder, "Data", "Camera", value.ToString()) + ".png";
                if (File.Exists(file))
                    return new BitmapImage(new Uri(file));
            }
            catch
            {
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

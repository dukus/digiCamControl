using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;

namespace CameraControl.Controls
{
    public class CameraToImageSourceConverter : IValueConverter
    {
        private Dictionary<string, ImageSource> _cache = new Dictionary<string, ImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            try
            {
                if (_cache.ContainsKey(value.ToString()))
                    return _cache[value.ToString()];

                string file = Path.Combine(Settings.ApplicationFolder, "Data", "Camera", value.ToString()) + ".png";
                if (File.Exists(file))
                {
                    ImageSource bp = BitmapLoader.Instance.LoadImage(file, 400, 0);
                    _cache.Add(value.ToString(), bp);
                    return bp;
                }
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

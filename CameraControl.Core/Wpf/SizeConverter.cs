using System;
using System.Globalization;
using System.Windows.Data;

namespace CameraControl.Core.Wpf
{
    public class SizeConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = value is double ? (double) value : 0;
            double param = parameter is double ? (double) parameter : 0;
            return val*param/100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZoomAndPan
{
    public class ZoomAdjustConverter : MarkupExtension, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;
            return Math.Log(doubleValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;
            return Math.Exp(doubleValue);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

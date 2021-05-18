using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZoomAndPan
{
    public class ZoomAndPanViewBoxClampConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //NOTE: Cannot pass ExtentWidth or ExtentHeight as one of the values because it does not seem to update
            var zoomAndPanControl = (ZoomAndPanControl)values[3];
            if (values[0] == null || zoomAndPanControl == null) return DependencyProperty.UnsetValue;
            var size = (double)values[0];
            var offset = (double)values[1];
            var zoom = (double)values[2];
            return Math.Max((parameter?.ToString().ToLower() == "width")
                 ? Math.Min(zoomAndPanControl.ExtentWidth / zoom - offset, size)
                 : Math.Min(zoomAndPanControl.ExtentHeight / zoom - offset, size), 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Capture.Workflow.Core.Classes;
using MahApps.Metro.Controls;

namespace Capture.Workflow.Wpf.Converters
{
    public class ParameterToControlConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var property = value as CustomProperty;
            if (property != null)
                switch (property.PropertyType)
                {
                    case CustomPropertyType.Number:
                        NumericUpDown numericUpDown = new NumericUpDown();
                        numericUpDown.Minimum = property.RangeMin;
                        numericUpDown.Maximum = property.RangeMax;
                        numericUpDown.DataContext = property;
                        numericUpDown.SetBinding(NumericUpDown.ValueProperty, "Value");
                        return numericUpDown;
                    case CustomPropertyType.String:
                        TextBox text = new TextBox();
                        text.DataContext = property;
                        text.SetBinding(TextBox.TextProperty, "Value");
                        return text;
                    case CustomPropertyType.ValueList:
                        ComboBox comboBox=new ComboBox();
                        comboBox.DataContext = property;
                        comboBox.SetBinding(ComboBox.ItemsSourceProperty, "ValueList");
                        comboBox.SetBinding(ComboBox.SelectedItemProperty, "Value");
                        return comboBox;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

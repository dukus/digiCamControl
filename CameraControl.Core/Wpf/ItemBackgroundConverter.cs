using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Wpf
{
    public sealed class ItemBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            try
            {
                var item = (ListBoxItem)value;
                var listView =
                    ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
                var fileItem = item.DataContext as FileItem;
                // Get the index of a ListViewItem
                int index =listView.ItemContainerGenerator.IndexFromContainer(item);
                if (index == 0 || fileItem == null)
                    return Brushes.Transparent;
                var oldItem = (listView.ItemContainerGenerator.ContainerFromIndex(index - 1)) as ListBoxItem;
                if (oldItem == null)
                    return Brushes.Transparent;
                var oldFileItem = oldItem.DataContext as FileItem;
                if (oldFileItem == null)
                    return Brushes.Transparent;
                if (fileItem.Series == oldFileItem.Series)
                {
                    fileItem.Alternate = oldFileItem.Alternate;
                }
                else
                {
                    fileItem.Alternate = !oldFileItem.Alternate;
                }
                return Brushes.Transparent;
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

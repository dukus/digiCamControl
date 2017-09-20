using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Wpf.Converters
{
    public class WorkflowToCardBitmapConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as WorkFlow;
            try
            {
                if (val != null)
                {
                    var file = val.Properties["CardBackground"].Value;
                    if (!string.IsNullOrEmpty(file))
                    {
                        using (Stream stream = val.GetFileStream(file))
                        {
                            return Utils.LoadImage(stream, 300, 0);
                        }
                    }
                }
            }
            catch (Exception e)
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

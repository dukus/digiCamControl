using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CameraControl.Devices;

namespace CameraControl.Core.Translation
{
  public class TranslateConverter : IValueConverter    
  {
    #region Implementation of IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string val = value as string;
      if (val != null)
      {
        double d = 0;
        if (double.TryParse(val, out d))
          return value;
        string key = val.Trim();
        key = val.Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("-", "_");
        key = key.ToUpper();
        if (!TranslationManager.HaveTranslation(key))
        {
          Log.Debug("No translation for:" + key + "|" + val);
          return ServiceProvider.Settings.ShowUntranslatedLabelId ? key : value;
        }
        return TranslationManager.GetTranslation(key);
      }
      return parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for SelectorWnd.xaml
  /// </summary>
  public partial class SelectorWnd : Window
  {
    public SelectorWnd()
    {
      InitializeComponent();
      foreach (IMainWindowPlugin mainWindowPlugin in ServiceProvider.PluginManager.MainWindowPlugins)
      {
        lst_windows.Items.Add(mainWindowPlugin.DisplayName);
      }
      btn_select.Focus();
    }

    private void btn_select_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}

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

namespace CameraControl.Actions.Enfuse
{
  /// <summary>
  /// Interaction logic for EnfuseSettingsWnd.xaml
  /// </summary>
  public partial class EnfuseSettingsWnd
  {
    public EnfuseSettingsWnd()
    {
      InitializeComponent();
      ServiceProvider.Settings.ApplyTheme(this);
    }

    private void btn_enfuse_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      this.Close();
    }
  }
}

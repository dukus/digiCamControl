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
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for EditTagWnd.xaml
  /// </summary>
  public partial class EditTagWnd
  {
    private TagItem _tagItem;
    public EditTagWnd(TagItem item)
    {
      _tagItem = item;
      InitializeComponent();
      _tagItem.BeginEdit();
      DataContext = _tagItem;
      ServiceProvider.Settings.ApplyTheme(this);
    }

    private void btn_save_Click(object sender, RoutedEventArgs e)
    {
      _tagItem.EndEdit();
      DialogResult = true;
    }

    private void btn_cancel_Click(object sender, RoutedEventArgs e)
    {
      _tagItem.CancelEdit();
      DialogResult = false;
    }
  }
}

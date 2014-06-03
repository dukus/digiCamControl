using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraControl.Controls
{
  /// <summary>
  /// Interaction logic for FolderBrowserComboBox.xaml
  /// </summary>
  public partial class FolderBrowserComboBox : UserControl,  INotifyPropertyChanged
  {
    public event EventHandler ValueChanged;

    public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(
        "SelectedPath", typeof(string), typeof(FolderBrowserComboBox), new PropertyMetadata(""));

    public string SelectedPath
    {
      get { return folderBrowser.SelectedImagePath; }
      set
      {
        folderBrowser.SelectedImagePath = value;
        NotifyPropertyChanged("SelectedPath");
      }
    }


    public FolderBrowserComboBox()
    {
      InitializeComponent();
    }


    private void Tree1_Initialized(object sender, EventArgs e)
    {
      var trv = sender as TreeView;
      var trvItem = new TreeViewItem() { Header = "Initialized item" };
      var trvItemSel = trv.Items[1] as TreeViewItem;
      trvItemSel.Items.Add(trvItem);
    }

    private void header_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!PopupTest.IsOpen)
      {
        PopupTest.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
        PopupTest.VerticalOffset = header.ActualHeight;
        PopupTest.StaysOpen = true;
        PopupTest.Height = folderBrowser.Height;
        PopupTest.Width = header.ActualWidth;
        PopupTest.IsOpen = true;
      }
      else
      {
        PopupTest.IsOpen = false;
      }
    }

    private void folderBrowser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      //header.Text = folderBrowser.SelectedImagePath;
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      PopupTest.IsOpen = false;
    }

    private void header_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      PopupTest.IsOpen = false;
    }

    #region Implementation of INotifyPropertyChanged

    public virtual event PropertyChangedEventHandler PropertyChanged;

    public virtual void NotifyPropertyChanged(String info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

    #endregion

    private void PopupTest_Opened(object sender, EventArgs e)
    {

    }
 
  }
}

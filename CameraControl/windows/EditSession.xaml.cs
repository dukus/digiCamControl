using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for EditSession.xaml
  /// </summary>
  public partial class EditSession
  {
    public PhotoSession Session { get; set; }
    public EditSession(PhotoSession session)
    {
      Session = session;
      Session.BeginEdit();
      InitializeComponent();
      DataContext = Session;
      ServiceProvider.Settings.ApplyTheme(this);
    }

    private void btn_browse_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new System.Windows.Forms.FolderBrowserDialog();
      dialog.SelectedPath = Session.Folder;
      if(dialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
      {
        Session.Folder = dialog.SelectedPath;
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      Session.EndEdit();
      DialogResult = true;
      Close();
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      Session.CancelEdit();
      Close();
    }

    private void btn_add_tag_Click(object sender, RoutedEventArgs e)
    {
      TagItem item = new TagItem();
      EditTagWnd wnd = new EditTagWnd(item);
      if (wnd.ShowDialog() == true)
      {
        Session.Tags.Add(item);
      }
    }

    private void btn_del_tag_Click(object sender, RoutedEventArgs e)
    {
      TagItem item = lst_tags.SelectedItem as TagItem;
      if (item != null)
      {
        Session.Tags.Remove(item);
      }
    }

    private void btn_edit_tag_Click(object sender, RoutedEventArgs e)
    {
      TagItem item = lst_tags.SelectedItem as TagItem;
      if (item != null)
      {
        EditTagWnd wnd = new EditTagWnd(item);
        wnd.ShowDialog();
      }
    }

    private void btn_help_Click(object sender, RoutedEventArgs e)
    {
      HelpProvider.Run(HelpSections.Session);
    }


  }
}

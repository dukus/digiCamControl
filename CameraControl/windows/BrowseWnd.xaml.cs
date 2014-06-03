using System;
using System.ComponentModel;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for BrowseWnd.xaml
  /// </summary>
  public partial class BrowseWnd :  INotifyPropertyChanged,IWindow 
  {
    private PhotoSession _selectedPhotoSession;
    public PhotoSession SelectedPhotoSession
    {
      get { return _selectedPhotoSession; }
      set
      {
        _selectedPhotoSession = value;
        NotifyPropertyChanged("SelectedPhotoSession");
      }
    }

    public BrowseWnd()
    {
      InitializeComponent();
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

    #region Implementation of IWindow

    public void ExecuteCommand(string cmd, object param)
    {
      switch (cmd)
      {
        case WindowsCmdConsts.BrowseWnd_Show:
          ServiceProvider.Settings.PropertyChanged += Settings_PropertyChanged;
          Dispatcher.Invoke(new Action(delegate
                                         {
                                           SelectedPhotoSession = ServiceProvider.Settings.DefaultSession;
                                           Show();
                                           Activate();
                                           Topmost = true;
                                           Focus();
                                         }));
          break;
        case WindowsCmdConsts.BrowseWnd_Hide:
          {
            ServiceProvider.Settings.PropertyChanged -= Settings_PropertyChanged;
            Dispatcher.Invoke(new Action(Hide));
          }
          break;
        case CmdConsts.All_Close:
          Dispatcher.Invoke(new Action(delegate
                                         {
                                           Hide();
                                           Close();
                                         }));
          break;
      }
    }

    void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DefaultSession")
      {
        SelectedPhotoSession = ServiceProvider.Settings.DefaultSession;
      }
    }

    #endregion

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      if (IsVisible)
      {
        e.Cancel = true;
        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BrowseWnd_Hide);
      }
    }

    private void lst_profiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (lst_profiles.SelectedItem != null)
      {
        ServiceProvider.Settings.DefaultSession = SelectedPhotoSession;
        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BrowseWnd_Hide);
      }
    }

    private void folderBrowser1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ServiceProvider.Settings.DefaultSession.Folder = folderBrowser1.SelectedImagePath;
      ServiceProvider.QueueManager.Clear();
      ServiceProvider.Settings.DefaultSession.Files.Clear();
      ServiceProvider.Settings.LoadData(ServiceProvider.Settings.DefaultSession);
    }

    private void lst_files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

    }

  }
}

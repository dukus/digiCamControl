using System;
using System.Windows;
using System.Windows.Media;
using CameraControl.Devices;

namespace CameraControl.Core.Wpf
{
  /// <summary>
  /// Interaction logic for ProgressWindow.xaml
  /// </summary>
  public partial class ProgressWindow : Window
  {
    public ProgressWindow()
    {
      InitializeComponent();
    }

    public string Label
    {
      get { return (string) lbl_label.Content; }
      set { Dispatcher.Invoke(new Action(delegate { lbl_label.Content = value; })); }
    }

    public double Progress
    {
      get { return progressBar.Value; }
      set
      {
        Dispatcher.Invoke(
          new Action(delegate { progressBar.Value = progressBar.Maximum < value ? value : progressBar.Maximum; }));
      }
    }

    public double MaxValue
    {
      get { return progressBar.Maximum; }
      set { Dispatcher.Invoke(new Action(delegate { progressBar.Maximum = value; })); }
    }

    public new void Hide()
    {
      try
      {
        Dispatcher.Invoke(new Action(() => base.Hide()));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
    
    public ImageSource ImageSource
    {
      get { return image.Source; }
      set
      {
        Dispatcher.Invoke(new Action(delegate
                                       {
                                         image.BeginInit();
                                         image.Source = value;
                                         image.EndInit();
                                       }));
      }
    }
  }
}

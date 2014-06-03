using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for SavePresetWnd.xaml
  /// </summary>
  public partial class SavePresetWnd
  {
    public CameraPreset CameraPreset { get; set; }

    public SavePresetWnd(CameraPreset cameraPreset)
    {
      InitializeComponent();
      CameraPreset = cameraPreset;
      CameraPreset.BeginEdit();
      ServiceProvider.Settings.ApplyTheme(this);
    }

    private void btn_save_Click(object sender, RoutedEventArgs e)
    {
      CameraPreset.EndEdit();
      DialogResult = true;
      Close();
    }

    private void btn_cancel_Click(object sender, RoutedEventArgs e)
    {
      CameraPreset.CancelEdit(); 
      Close();
    }
  }
}

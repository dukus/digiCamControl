using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for AboutWnd.xaml
  /// </summary>
  public partial class AboutWnd 
  {
    public AboutWnd()
    {
      InitializeComponent();
      Title = "About " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      ServiceProvider.Settings.ApplyTheme(this);
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      PhotoUtils.Run("http://www.digicamcontrol.com/");
    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
      PhotoUtils.Run("http://www.gnu.org/licenses/gpl-3.0.txt");
    }

    private void btn_donate_Click(object sender, RoutedEventArgs e)
    {
      PhotoUtils.Donate();
    }
  }
}

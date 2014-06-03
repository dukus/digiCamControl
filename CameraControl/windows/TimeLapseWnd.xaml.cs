using System;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Forms;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using HelpProvider = CameraControl.Classes.HelpProvider;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for TimeLapseWnd.xaml
  /// </summary>
  public partial class TimeLapseWnd 
  {
    public TimeLapseWnd()
    {
      InitializeComponent();
      ServiceProvider.Settings.DefaultSession.TimeLapse.TimeLapseDone += TimeLapse_TimeLapseDone;
      ServiceProvider.Settings.ApplyTheme(this);
      if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
          chk_noaf.IsEnabled =
              ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureNoAf);
    }

    void TimeLapse_TimeLapseDone(object sender, EventArgs e)
    {
      Dispatcher.Invoke(new Action(delegate { btn_start.Content = TranslationStrings.ButtonStartTimeLapse; }));
    }

    private void btn_start_Click(object sender, RoutedEventArgs e)
    {
      if (ServiceProvider.Settings.DefaultSession.TimeLapse.IsDisabled)
      {
        ServiceProvider.Settings.DefaultSession.TimeLapse.Start();
        Dispatcher.Invoke(new Action(delegate { btn_start.Content = TranslationStrings.ButtonStopTimeLapse; }));
      }
      else
      {
        if (MessageBox.Show(TranslationStrings.MsgStopTimeLapse, TranslationStrings.LabelTimeLapse, MessageBoxButtons.YesNo) ==
            System.Windows.Forms.DialogResult.Yes)
        {
          ServiceProvider.Settings.DefaultSession.TimeLapse.Stop();
        }
      }
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      if(!CheckCodec())
      {
        if (MessageBox.Show(TranslationStrings.MsgInstallXvidCodec, TranslationStrings.LabelVideoCodecProblem, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
          System.Diagnostics.Process.Start("http://www.xvid.org/Downloads.15.0.html");
        }
        return;
      }
      Hide();
      CreateTimeLapseWnd wnd = new CreateTimeLapseWnd();
      wnd.ShowDialog();
      Show();
    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog();
      dialog.Filter = "Avi files (*.avi)|*.avi|All files (*.*)|*.*";
      dialog.AddExtension = true;
      dialog.FileName = ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName;
      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName = dialog.FileName;
      }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
    }

    private bool CheckCodec()
    {
      try
      {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_CodecFile");
        ManagementObjectCollection collection = searcher.Get();
        return collection.Cast<ManagementObject>().Any(obj => (string)obj["Description"] == "Xvid MPEG-4 Video Codec");
      }
      catch (Exception exception)
      {
        Log.Error("Check codec",exception);
      }
      return true;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (!ServiceProvider.Settings.DefaultSession.TimeLapse.IsDisabled)
      {
        if (MessageBox.Show(TranslationStrings.MsgStopTimeLapse, TranslationStrings.LabelTimeLapse, MessageBoxButtons.YesNo) ==
            System.Windows.Forms.DialogResult.Yes)
        {
          ServiceProvider.Settings.DefaultSession.TimeLapse.Stop();
        }
        else
        {
          e.Cancel = true;
        }
      }
    }

    private void btn_help_Click(object sender, RoutedEventArgs e)
    {
      HelpProvider.Run(HelpSections.TimeLapse);
    }

    private void btn_stay_on_top_Click(object sender, RoutedEventArgs e)
    {
        Topmost = (btn_stay_on_top.IsChecked == true);
    }
  }
}

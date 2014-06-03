using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Wpf;
using CameraControl.Devices.Classes;
using Ionic.Zip;

namespace CameraControl.Plugins.ExportPlugins
{
  public class ExportToZip : IExportPlugin
  {
    private ProgressWindow dlg = new ProgressWindow();
    private string destfile = "";
    #region Implementation of IExportPlugin

    private string _title;

    public bool Execute()
    {
      if (dlg.IsVisible)
        return true;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
      saveFileDialog1.Filter = "Zip file|*.zip";
      saveFileDialog1.Title = "Save zip file";
      saveFileDialog1.ShowDialog();

      // If the file name is not an empty string open it for saving.
      if (saveFileDialog1.FileName != "")
      {
        destfile = saveFileDialog1.FileName;
        dlg.Show();
        Thread thread = new Thread(ZipFiles);
        thread.Start(ServiceProvider.Settings.DefaultSession.Files);

      }
      return true;
    }

    public string Title
    {
      get { return "Export to zip"; }
      set { _title = value; }
    }

    #endregion
    private void ZipFiles(object o)
    {
      AsyncObservableCollection<FileItem> items = o as AsyncObservableCollection<FileItem>;
      items = new AsyncObservableCollection<FileItem>(items.Where(file => file.IsChecked));
      dlg.MaxValue = items.Count;
      int i = 0;
      using (ZipFile zip = new ZipFile(destfile))
      {
        foreach (var fileItem in items)
        {
          dlg.Progress = i;
          dlg.ImageSource = fileItem.Thumbnail;
          dlg.Label = Path.GetFileName(fileItem.FileName);
          zip.AddFile(fileItem.FileName,"");
          i++;
          zip.Save(destfile);
        }
      }
      dlg.Hide();
    }
  }
}

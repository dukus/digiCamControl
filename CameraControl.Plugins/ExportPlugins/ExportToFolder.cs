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
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Plugins.ExportPlugins
{
  public class ExportToFolder : IExportPlugin
  {
    #region Implementation of IExportPlugin

    private ProgressWindow dlg = new ProgressWindow();
    private string destfolder = "";

    public bool Execute()
    {
      if (dlg.IsVisible)
        return true;
      MessageBox.Show("All existing files will be overwritten !");
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        destfolder = dialog.SelectedPath;
        dlg.Show();
        Thread thread = new Thread(CopyFiles);
        thread.Start(ServiceProvider.Settings.DefaultSession.Files);
      }
      return true;
    }

    private string _title;

    public string Title
    {
      get { return "Export To Folder"; }
      set { _title = value; }
    }

    private void CopyFiles(object o)
    {
      AsyncObservableCollection<FileItem> items = o as AsyncObservableCollection<FileItem>;
      items = new AsyncObservableCollection<FileItem>(items.Where(file => file.IsChecked));
      dlg.MaxValue = items.Count;
      int i = 0;
      foreach (var fileItem in items)
      {
        dlg.Progress = i;
        dlg.ImageSource = fileItem.Thumbnail;
        dlg.Label = Path.GetFileName(fileItem.FileName);
        if (File.Exists(fileItem.FileName))
        {
          try
          {
            File.Copy(fileItem.FileName, Path.Combine(destfolder, Path.GetFileName(fileItem.FileName)),true);
          }
          catch (Exception exception)
          {
            Log.Error("ErrorCopy file", exception);
          }
        }
        //Thread.Sleep(100);
        i++;
      }
      dlg.Hide();
    }

    #endregion
  }
}

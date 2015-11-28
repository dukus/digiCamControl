using System;
using System.IO;
using System.Windows.Forms;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace CameraControl.Plugins.ExportPlugins
{
    public class ExportCsv : IExportPlugin
    {
        public bool Execute()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "export.csv";
            dialog.Filter = "*.csv|*.csv|All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (TextWriter writer = File.CreateText(dialog.FileName))
                    {
                        writer.WriteLine("File,Time,TimeDif");
                        for (int i = 0; i < ServiceProvider.Settings.DefaultSession.Files.Count; i++)
                        {
                            var file = ServiceProvider.Settings.DefaultSession.Files[i];
                            writer.WriteLine("{0},{1},{2}", file.FileName, file.FileDate.ToString("O"),
                                (i > 0
                                    ? Math.Round(
                                        (file.FileDate - ServiceProvider.Settings.DefaultSession.Files[i - 1].FileDate)
                                            .TotalMilliseconds, 0)
                                    : 0));
                        }
                        PhotoUtils.Run(dialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error to export " + ex.Message);
                    Log.Error("Error to export ", ex);
                }
            }
            return true;
        }

        public string Title
        {
            get { return "Export file list to csv"; }
            set { }
        }
    }
}

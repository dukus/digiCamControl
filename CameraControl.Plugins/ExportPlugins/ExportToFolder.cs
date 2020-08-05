#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Drawing;
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
using ImageMagick;

#endregion

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
                        var dest = Path.Combine(destfolder, Path.GetFileName(fileItem.FileName));
                        if (fileItem.RotationAngle == 0 || fileItem.IsRaw ||fileItem.IsMovie)
                            File.Copy(fileItem.FileName, dest, true);
                        else
                        {
                            using (MagickImage image = new MagickImage(fileItem.FileName))
                            {
                                image.BackgroundColor = new MagickColor(Color.Black.R, Color.Black.G, Color.Black.B);
                                image.Rotate(fileItem.RotationAngle);
                                image.Format = MagickFormat.Jpeg;
                                // Save the result
                                image.Write(dest);
                            }
                        }
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
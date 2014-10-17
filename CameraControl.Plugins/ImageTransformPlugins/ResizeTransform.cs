using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    class ResizeTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Resize"; }
        }

        public string Execute(string filename, string dest, ValuePairEnumerator configData)
        {
            var conf = new ResizeTransformViewModel(configData);
            bool deleteFile = false;
            FileItem fileItem = new FileItem(filename);
            if (fileItem.IsRaw)
            {
                string s = Path.Combine(Path.GetDirectoryName(fileItem.FileName),
                    Path.GetFileNameWithoutExtension(fileItem.FileName) + ".jpg");
                if (File.Exists(s))
                {
                    filename = s;
                }
                else
                {
                    string dcraw_exe = Path.Combine(Settings.ApplicationFolder, "dcraw.exe");
                    if (File.Exists(dcraw_exe))
                    {
                        PhotoUtils.RunAndWait(dcraw_exe, string.Format(" -e {0}", fileItem.FileName));
                        string thumb = Path.Combine(Path.GetDirectoryName(fileItem.FileName),
                            Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
                        if (File.Exists(thumb))
                        {
                            deleteFile = true;
                            filename = thumb;
                        }
                    }
                }
                dest = Path.Combine(Path.GetDirectoryName(dest), Path.GetFileNameWithoutExtension(dest) + ".jpg");
            }
            using (MemoryStream fileStream = new MemoryStream(File.ReadAllBytes(filename)))
            {
                BitmapDecoder bmpDec = BitmapDecoder.Create(fileStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                WriteableBitmap writeableBitmap ;
                if (conf.KeepAspect)
                {
                    double dw = (double)conf.Width / bmpDec.Frames[0].PixelWidth;
                    writeableBitmap =
                        BitmapFactory.ConvertToPbgra32Format(BitmapLoader.GetBitmapFrame(bmpDec.Frames[0],
                            (int)(bmpDec.Frames[0].PixelWidth * dw),
                            (int)(bmpDec.Frames[0].PixelHeight * dw),
                            BitmapScalingMode.Linear));
                }
                else
                {
                    writeableBitmap =
                        BitmapFactory.ConvertToPbgra32Format(BitmapLoader.GetBitmapFrame(bmpDec.Frames[0], conf.Width, conf.Height, BitmapScalingMode.Linear));
                   
                }

                BitmapLoader.Save2Jpg(writeableBitmap, dest);

                // remove temporally file created by dcraw
                if (deleteFile)
                    File.Delete(filename);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ResizeTransformView();
            control.DataContext = new ResizeTransformViewModel(configData);
            return control;
        }


    }
}

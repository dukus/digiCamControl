using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using ImageMagick;
using ZXing;
using ZXing.Common;

namespace CameraControl.Plugins.FilenameTemplate
{
    public class BarcodeFromImage: IFilenameTemplate
    {
        public bool IsRuntime => true;
        public string Pharse(string template, PhotoSession session, ICameraDevice device, string fileName, string tempfileName)
        {
            if (!File.Exists(tempfileName))
                return "";

            // load a bitmap
            PhotoUtils.WaitForFile(tempfileName);

            var file = tempfileName;
            if (Path.GetExtension(fileName).ToLower() == ".cr2" || Path.GetExtension(fileName).ToLower() == ".nef")
            {
                string dcraw_exe = Path.Combine(Settings.ApplicationFolder, "dcraw.exe");
                if (File.Exists(dcraw_exe))
                {
                    string thumb = Path.Combine(Path.GetTempPath(),
                        Path.GetFileNameWithoutExtension(tempfileName) + ".thumb.jpg");
                    PhotoUtils.RunAndWait(dcraw_exe,
                        string.Format(" -e -O \"{0}\" \"{1}\"", thumb, tempfileName));
                    if (File.Exists(thumb))
                    {
                        var res= GetBarcode(thumb, template);
                        File.Delete(thumb);
                        return res;
                    }
                }
            }
            else
            {
                return GetBarcode(tempfileName, template);
            }
            return template;
        }

        private string GetBarcode(string filename, string template)
        {

            IBarcodeReader reader = new BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            using (var barcodeBitmap = new MagickImage(filename))
            {

                // detect and decode the barcode inside the bitmap
                var result = reader.Decode(barcodeBitmap.ToBitmap());
                if (result != null)
                    // do something with the result
                {
                    return result.Text;
                }
            }

            return template;
        }
    }
}

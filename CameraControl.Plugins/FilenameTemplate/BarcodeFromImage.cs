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

            IBarcodeReader reader = new BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions { TryHarder = true }
            };
            // load a bitmap
            PhotoUtils.WaitForFile(tempfileName);
            using (var barcodeBitmap = (Bitmap)Bitmap.FromFile(tempfileName))
            {
                // detect and decode the barcode inside the bitmap
                var result = reader.Decode(barcodeBitmap);
                // do something with the result
                if (result != null)
                {
                    return result.Text;
                }
            }
            return template;
        }
    }
}

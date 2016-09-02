using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.FilenameTemplate
{
    public class ExifTemplate : IFilenameTemplate
    {
        public bool IsRuntime => true;

        public string Pharse(string template, PhotoSession session, ICameraDevice device, string fileName, string tempfileName)
        {
            if (!File.Exists(tempfileName))
                return "";
            FileItem item = new FileItem(tempfileName);
            BitmapLoader.Instance.GetMetadata(item);
            string tag = template.Replace("[", "").Replace("]", "");
            if (item.FileInfo.ExifTags.ContainName(tag))
                return item.FileInfo.ExifTags[tag].Replace(":", "_").Replace("?", "_").Replace("*", "_").Replace("\\", "_"); ;
            return template;
        }
    }
}

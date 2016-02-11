using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using SQLite;

namespace CameraControl.Core.Database
{
    public class DbFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public int FileId { get; set; }
        
        public string File { get; set; }
        public DateTime? Date { get; set; }
        public string CameraSerial { get; set; }
        public string Camera { get; set; }
        public string F { get; set; }
        public string E { get; set; }
        public string Iso { get; set; }
        public string FocalLength { get; set; }
        public string ExposureBias { get; set; }
        public string Session { get; set; }

        public DbFile()
        {
            
        }

        public DbFile(FileItem item, string serial="", string camera="", string session="")
        {
            Copy(item);
            CameraSerial = serial;
            Camera = camera;
            Session = session;
        }

        public void Copy(FileItem item)
        {
            File = item.FileName;
            if (string.IsNullOrEmpty(CameraSerial))
                CameraSerial = item.CameraSerial;
            F = item.F;
            E = item.E;
            Iso = item.Iso;
            FocalLength = item.FocalLength;
            ExposureBias = item.ExposureBias;
            FileId = item.Id;
            if (item.FileInfo != null && item.FileInfo.ExifTags.ContainName("Exif.Image.DateTime"))
            {
                DateTime date;
                Date = DateTime.TryParseExact(item.FileInfo.ExifTags["Exif.Image.DateTime"], "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date) ? date : item.FileDate;
            }
        }
    }
}

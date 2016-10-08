using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Core.Classes
{
    /// <summary>
    /// Helper class used by websevice filelist.json
    /// </summary>
    public class FileListItem
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Original { get; set; }
        public string SmallThumb { get; set; }
        public string LargeThumb { get; set; }
    }
}

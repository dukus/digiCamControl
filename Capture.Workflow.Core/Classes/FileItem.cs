using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Capture.Workflow.Core.Classes
{
    public class FileItem
    {
        public string ThumbFile { get; set; }
        public string FileName { get; set; }
        public string TempFile { get; set; }
        public BitmapSource Thumb { get; set; }

        public void Clear()
        {
            Utils.WaitForFile(TempFile);
            File.Delete(TempFile);
        }
    }
}

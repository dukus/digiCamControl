using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.PluginManager.Classes
{
    public class ProjectFileItem
    {
        public string FileName { get; set; }
        public string Name { get; set; }

       
        public ProjectFileItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(FileName);
        }

        public ProjectFileItem()
        {
        }
    }
}

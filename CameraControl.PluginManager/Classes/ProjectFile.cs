using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Plugin;
using CameraControl.Devices.Classes;

namespace CameraControl.PluginManager.Classes
{
    public class ProjectFile
    {
        public PluginInfo PluginInfo { get; set; }
        public AsyncObservableCollection<ProjectFileItem> Files { get; set; }

        public ProjectFile()
        {
            Files = new AsyncObservableCollection<ProjectFileItem>();
        }

    }
}

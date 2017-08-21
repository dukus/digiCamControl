using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices;

namespace Capture.Workflow.Core.Classes
{
    public class Context
    {
        public WorkFlow WorkFlow { get; set; }
        public ICameraDevice CameraDevice { get; set; }
        public FileItem FileItem { get; set; }
    }
}

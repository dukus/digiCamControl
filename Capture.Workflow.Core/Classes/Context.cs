using System;
using System.Collections.Generic;
using System.IO;
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
        public ContextTargetEnum Target { get; set; }
        public Stream ImageStream { get; set; }
    }
}

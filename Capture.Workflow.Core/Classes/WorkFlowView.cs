using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowView : BaseItem
    {
        public IViewPlugin Instance { get; set; }

        public AsyncObservableCollection<WorkFlowViewElement> Elements { get; set; }

        public WorkFlowView()
        {
            Properties = new CustomPropertyCollection();
            Elements = new AsyncObservableCollection<WorkFlowViewElement>();
        }
    }
}

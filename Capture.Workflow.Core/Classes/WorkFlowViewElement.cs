using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowViewElement: BaseItem
    {

        public IViewElementPlugin Instance { get; set; }

        public WorkFlowViewElement()
        {
            Properties = new CustomPropertyCollection();
        }
    }
}

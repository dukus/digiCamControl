using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowViewElement: BaseItem
    {
        public WorkFlowViewElement()
        {
            Properties = new CustomPropertyCollection();
        }
    }
}

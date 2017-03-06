using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Core.Classes
{
    public class ActionsList
    {
        public string Name { get; set; }
        public List<IActionPlugin> Actions{ get; set; }

        public ActionsList()
        {
             Actions = new List<IActionPlugin>();
        }
    }
}

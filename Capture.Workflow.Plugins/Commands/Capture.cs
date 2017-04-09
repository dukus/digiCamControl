using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Capture")]
    public class Capture:IWorkflowCommand
    {
        public string Name { get; set; }
        public WorkFlowCommand CreateCommand()
        {
            return new WorkFlowCommand();
        }

        public bool Execute(WorkFlowCommand command)
        {
            throw new NotImplementedException();
        }
    }
}

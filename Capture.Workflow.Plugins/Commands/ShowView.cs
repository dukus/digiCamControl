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
    [DisplayName("ShowView")]
    public class ShowView : IWorkflowCommand
    {
        public string Name { get; set; }
        public WorkFlowCommand CreateCommand()
        {
            var command = new WorkFlowCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "ViewName",
                PropertyType = CustomPropertyType.View
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command)
        {
            throw new NotImplementedException();
        }
    }
}

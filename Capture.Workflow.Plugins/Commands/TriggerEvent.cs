using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("TriggerEvent")]
    public class TriggerEvent: IWorkflowCommand
    {
        public string Name { get; set; }
        public WorkFlowCommand CreateCommand()
        {
            var command = new WorkFlowCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Event",
                PropertyType = CustomPropertyType.String
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Message",
                PropertyType = CustomPropertyType.String
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (string.IsNullOrWhiteSpace(command.Properties["Event"].Value))
            {
                Log.Debug("No event specified for " + command.Name);
                return false;
            }
            
            WorkflowManager.Instance.OnMessage(new MessageEventArgs(command.Properties["Event"].Value, command.Properties["Message"].Value));
            return true;
        }
    }
}

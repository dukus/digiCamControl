using System.ComponentModel;
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
    public class TriggerEvent: BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
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
            if (string.IsNullOrWhiteSpace(command.Properties["Event"].ToString(context)))
            {
                Log.Debug("No event specified for " + command.Name);
                return false;
            }

            if (CheckCondition(command, context))
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(command.Properties["Event"].ToString(context),
                    command.Properties["Message"].ToString(context)));

            return true;
        }
    }
}

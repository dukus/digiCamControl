using System.ComponentModel;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using Jint;

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
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "Condition",
                PropertyType = CustomPropertyType.Code
            });
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
            var var = new Engine();

            foreach (var variable in context.WorkFlow.Variables.Items)
            {
                //e.Parameters[variable.Name] = new Exception(variable.Value);
                var.SetValue(variable.Name, variable.GetAsObject());
            }

            if (var.Execute(command.Properties["Condition"].Value).GetCompletionValue().AsBoolean())
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(command.Properties["Event"].Value, command.Properties["Message"].Value));

            return true;
        }
    }
}

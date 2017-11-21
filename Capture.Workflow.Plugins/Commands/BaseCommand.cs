using System;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Jint;

namespace Capture.Workflow.Plugins.Commands
{
    public class BaseCommand
    {
        public string Name { get; set; }

        public WorkFlowCommand GetCommand()
        {
            var command = new WorkFlowCommand();
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "(Name)",
                PropertyType = CustomPropertyType.String
            });
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "Condition",
                PropertyType = CustomPropertyType.Code
            });
            return command;
        }

        public bool CheckCondition(WorkFlowCommand command, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.Properties["Condition"].ToString(context)))
                    return true;

                var var = new Engine();

                foreach (var variable in context.WorkFlow.Variables.Items)
                {
                    //e.Parameters[variable.Name] = new Exception(variable.Value);
                    var.SetValue(variable.Name, variable.GetAsObject());
                }
                return var.Execute(command.Properties["Condition"].ToString(context)).GetCompletionValue().AsBoolean();
            }
            catch (Exception e)
            {
                Log.Error("CheckCondition error " + command.Name, e);
            }
            return false;
        }
    }
}

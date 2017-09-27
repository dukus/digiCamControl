using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using Jint;


namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("MathAction")]
    public class MathAction : BaseCommand, IWorkflowCommand
    {

        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Formula",
                PropertyType = CustomPropertyType.Code,
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            var var = new Engine();

            foreach (var variable in context.WorkFlow.Variables.Items)
            {
                //e.Parameters[variable.Name] = new Exception(variable.Value);
                var.SetValue(variable.Name, variable.GetAsObject());
            }
            
            context.WorkFlow.Variables[command.Properties["Variable"].Value].Value = var.Execute(command.Properties["Formula"].Value).GetCompletionValue().ToString(); 

            return true;
        }
    }
}

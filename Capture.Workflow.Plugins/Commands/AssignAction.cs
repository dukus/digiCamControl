using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using Capture.Workflow.Core.Scripting;



namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Assign")]
    [Icon("Equal")]
    public class AssignAction : BaseCommand, IWorkflowCommand
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

            context.WorkFlow.Variables[command.Properties["Variable"].Value].Value =
                ScriptEngine.Instance.ExecuteLine(command.Properties["Formula"].ToString(context), context);
            
            return true;
        }
    }
}

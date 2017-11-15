using System.ComponentModel;
using System.IO;
using System.Linq;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("AssignList")]
    public class AssignListAction : BaseCommand, IWorkflowCommand
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
                Name = "Source",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = {"Folder"},
                Value = "Folder"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Param",
                PropertyType = CustomPropertyType.String,
                Value = ""
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            switch (command.Properties["Source"].ToString(context))
            {
                case "Folder":
                {
                    var folder = command.Properties["Param"].ToString(context);
                    if (string.IsNullOrEmpty(folder))
                    {
                        Log.Error(" No folder specified in Param " + Name);
                        return false;
                    }
                    if (!Directory.Exists(folder))
                    {
                        Log.Error(" Folder not exist " + Name + " " + folder);
                        return false;
                    }

                    var folders = Directory.GetDirectories(folder);
                    var val = "";
                    foreach (var item in folders.OrderBy(x => x))
                    {
                        val += Path.GetFileName(item) + "|";
                    }
                    context.WorkFlow.Variables[command.Properties["Variable"].Value].Value = val;
                }
                    break;
            }
            return true;
        }
    }
}

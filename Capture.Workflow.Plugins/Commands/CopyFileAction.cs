using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using SmartFormat;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("CopyFileAction")]
    public class CopyFileAction : IWorkflowCommand
    {
        public string Name { get; set; }
        public WorkFlowCommand CreateCommand()
        {
            var command = new WorkFlowCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "FileNameTemplate",
                PropertyType = CustomPropertyType.ParamString,
                Value = @"{SessionFolder}\\{SessionName}\\IMG_{Counter}"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Overwrite",
                PropertyType = CustomPropertyType.Bool
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            Smart.Default.Settings.ConvertCharacterStringLiterals = false;
            var filename = Smart.Format(command.Properties["FileNameTemplate"].Value,
                context.WorkFlow.Variables.GetAsDictionary());
            filename = filename + Path.GetExtension(context.FileItem.TempFile);
            Utils.CreateFolder(filename);
            File.Copy(context.FileItem.TempFile, filename, command.Properties["Overwrite"].ToBool());
            context.FileItem.FileName = filename;
            return true;
        }
    }
}

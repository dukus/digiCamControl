using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Database;
using Capture.Workflow.Core.Interface;
using SmartFormat;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("CopyFile")]
    [Icon("FileRestore")]
    public class CopyFileAction : BaseCommand, IWorkflowCommand, IWorkflowQueueCommand
    {
        
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
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
            command.Properties.Add(new CustomProperty()
            {
                Name = "EnqueueAction",
                PropertyType = CustomPropertyType.Bool,
                Description = "Will execute this command in a queue in background"
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            Smart.Default.Settings.ConvertCharacterStringLiterals = false;
            var filename = command.Properties["FileNameTemplate"].ToString(context);

            filename = filename + Path.GetExtension(context.FileItem.TempFile).ToLower();
            if (command.Properties["EnqueueAction"].ToBool(context))
            {
                var file = Path.Combine(Settings.Instance.QueueFolder, Path.GetRandomFileName());
                Utils.CreateFolder(file);
                File.Copy(context.FileItem.TempFile, file, true);
                WorkflowManager.Instance.Database.Add(new DbQueue(file, "CopyFile", filename));
                Log.Debug("Adding to queue " + filename);
            }
            else
            {
                Utils.CreateFolder(filename);
                File.Copy(context.FileItem.TempFile, filename, command.Properties["Overwrite"].ToBool(context));
                context.FileItem.FileName = filename;
            }
            return true;
        }

        public bool ExecuteQueue(DbQueue queue)
        {
            if (!File.Exists(queue.SourceFile))
                return true;

            Utils.CreateFolder(queue.ActionParam);
            File.Copy(queue.SourceFile, queue.ActionParam, true);
            Utils.WaitForFile(queue.SourceFile);
            File.Delete(queue.SourceFile);
            return true;
        }
    }
}

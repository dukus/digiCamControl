using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("WorkflowAction")]
    [Icon("Apps")]
    public class WorkflowAction :BaseCommand, IWorkflowCommand
    {
     
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Action",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>
                {
                    "ShowView",
                    "FinishSession",
                    "SaveVariables",
                    "CancelSession",
                    "PreviousView",
                    "UpdateThumb",
                    "NextPhoto",
                    "PrevPhoto",
                    "DeletePhoto",
                    "ClearPhotos",
                    "ShowHelp",
                    "LoadImage",
                    "UnLoadImage"
                }
            });

            command.Properties.Add(new CustomProperty()
            {
                Name = "ViewName",
                PropertyType = CustomPropertyType.View
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            switch (command.Properties["Action"].Value)
            {
                case "ShowView":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ShowView,
                        command.Properties["ViewName"].ToString(context)));
                    break;
                case "FinishSession":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.SessionFinished, context));
                    break;
                case "SaveVariables":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.SaveVariables, context));
                    break;
                case "CancelSession":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.SessionCanceled, context));
                    break;
                case "PreviousView":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.PreviousView, context));
                    break;
                case "UpdateThumb":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ThumbCreate, context));
                    break;
                case "NextPhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.NextPhoto, context));
                    break;
                case "PrevPhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.PrevPhoto, context));
                    break;
                case "DeletePhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.DeletePhoto, context));
                    break;
                case "ClearPhotos":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ClearPhotos, context));
                    break;
                case "ShowHelp":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ShowHelp, context));
                    break;
                case "LoadImage":
                {
                    try
                    {
                        var stream = new MemoryStreamEx();
                        var buffer = File.ReadAllBytes(context.FileItem.TempFile);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        context.ImageStream = stream;
                        Log.Debug("LoadImage executed");
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Error unload image", e);
                    }
                }
                    break;
                case "UnLoadImage":
                {
                    try
                    {
                        context.ImageStream?.Close();
                        Log.Debug("UnLoadImage executed");
                        }
                    catch (Exception e)
                    {
                            Log.Debug("Error unload image", e);
                    }
                    }
                    
                    break;
            }
            return true;
        }
    }
}

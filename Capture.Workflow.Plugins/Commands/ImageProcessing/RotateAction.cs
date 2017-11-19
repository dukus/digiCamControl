using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using ImageMagick;

namespace Capture.Workflow.Plugins.Commands.ImageProcessing
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Rotate")]
    [Group("ImageProcessing")]
    [Icon("Rotate3d")]
    public class RotateAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Angle",
                PropertyType = CustomPropertyType.Variable,
                Value = "0"

            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (context?.ImageStream == null)
                return true;
            if (!CheckCondition(command, context))
                return true;
            context.ImageStream.Seek(0, SeekOrigin.Begin);
            using (MagickImage image = new MagickImage(context.ImageStream))
            {
                context.ImageStream.Seek(0, SeekOrigin.Begin);
                image.Rotate((double)context.WorkFlow.Variables[command.Properties["Angle"].ToString(context)].GetAsObject());
                image.Write(context.ImageStream, MagickFormat.Jpg);
            }
            return true;
        }
    }
}

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
    [DisplayName("Flip")]
    [Group("ImageProcessing")]
    public class FlipAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Orientation",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>() {"Horizontal", "Vertical"},
                Value = "Horizontal"

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
                if (command.Properties["Orientation"].Value == "Horizontal")
                    image.Flop();
                else
                    image.Flip();
                image.Write(context.ImageStream, MagickFormat.Jpg);
            }
            return true;
        }
    }
}

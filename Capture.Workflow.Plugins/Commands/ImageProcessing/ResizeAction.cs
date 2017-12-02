using System.ComponentModel;
using System.IO;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using ImageMagick;

namespace Capture.Workflow.Plugins.Commands.ImageProcessing
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Resize")]
    [Group("ImageProcessing")]
    [Icon("MoveResize")]
    public class ResizeAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "KeepAspectRatio",
                PropertyType = CustomPropertyType.Bool,

            });
            command.Properties.Add(new CustomProperty()
            {
                Description = "Can be absolute value or percentage (ex: 50%)",
                Name = "Width",
                PropertyType = CustomPropertyType.Number,

            });
            command.Properties.Add(new CustomProperty()
            {
                Description = "Can be absolute value or percentage (ex: 50%)",
                Name = "Height",
                PropertyType = CustomPropertyType.Number,

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
                MagickGeometry geometry = new MagickGeometry(command.Properties["Width"].ToInt(context), command.Properties["Height"].ToInt(context));

                if (command.Properties["Width"].IsPercentage(context) ||
                    command.Properties["Height"].IsPercentage(context))
                    geometry = new MagickGeometry(new Percentage(command.Properties["Width"].ToInt(context)),
                        new Percentage(command.Properties["Height"].ToInt(context)));

                geometry.IgnoreAspectRatio = !command.Properties["KeepAspectRatio"].ToBool(context);

                image.Resize(geometry);
                image.Write(context.ImageStream, MagickFormat.Bmp);
            }
            return true;
        }
    }
}

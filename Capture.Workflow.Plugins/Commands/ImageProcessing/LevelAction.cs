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
    [DisplayName("Level")]
    [Group("ImageProcessing")]
    public class LevelAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "BlackPoint",
                PropertyType = CustomPropertyType.Variable,
                RangeMin = -100,
                RangeMax = 100,
                Value = "0"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "MidPoint",
                PropertyType = CustomPropertyType.Variable,
                RangeMin = -100,
                RangeMax = 100,
                Value = "0"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "WhitePoint",
                PropertyType = CustomPropertyType.Variable,
                RangeMin = -100,
                RangeMax = 100,
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
                var midpoint = 1.0;
                if ((double)context.WorkFlow.Variables[command.Properties["MidPoint"].ToString(context)].GetAsObject() < 0)
                {
                    midpoint = -(double)context.WorkFlow.Variables[command.Properties["MidPoint"].ToString(context)].GetAsObject() / 10.0;
                    midpoint++;
                }
                if ((double)context.WorkFlow.Variables[command.Properties["MidPoint"].ToString(context)].GetAsObject() > 0)
                {
                    midpoint = (100 - (double)context.WorkFlow.Variables[command.Properties["MidPoint"].ToString(context)].GetAsObject()) / 100.0;
                }

                image.Level(
                    new Percentage((double) context.WorkFlow.Variables[command.Properties["BlackPoint"].ToString(context)]
                        .GetAsObject()),
                    new Percentage((double) context.WorkFlow.Variables[command.Properties["WhitePoint"].ToString(context)]
                        .GetAsObject()), midpoint);
                image.Write(context.ImageStream, MagickFormat.Bmp);
            }
            return true;
        }
    }
}

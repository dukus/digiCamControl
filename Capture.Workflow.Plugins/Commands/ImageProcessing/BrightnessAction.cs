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
    [DisplayName("BrightnessContrast")]
    [Group("ImageProcessing")]
    [Icon("Meteor")]
    public class BrightnessAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Brightness",
                PropertyType = CustomPropertyType.Variable,
                RangeMin = -100,
                RangeMax = 100,
                Value = "0"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Contrast",
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
                image.BrightnessContrast(new Percentage((double)context.WorkFlow.Variables[command.Properties["Brightness"].ToString(context)].GetAsObject()),
                    new Percentage((double)context.WorkFlow.Variables[command.Properties["Contrast"].ToString(context)].GetAsObject()));
                image.Write(context.ImageStream, MagickFormat.Bmp);
            }
            return true;
        }
    }
}

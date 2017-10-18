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
    public class BrightnessAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Brightness",
                PropertyType = CustomPropertyType.Number,
                RangeMin = -100,
                RangeMax = 100,
                Value = "0"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Contrast",
                PropertyType = CustomPropertyType.Number,
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
                image.BrightnessContrast(new Percentage(command.Properties["Brightness"].ToInt(context)),
                    new Percentage(command.Properties["Contrast"].ToInt(context)));
                image.Write(context.ImageStream, MagickFormat.Jpg);
            }
            return true;
        }
    }
}

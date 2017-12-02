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
    [DisplayName("Saturation")]
    [Group("ImageProcessing")]
    [Icon("InvertColors")]
    public class SaturationAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Saturation",
                PropertyType = CustomPropertyType.Variable,
                Value = "0",
                RangeMin = 0,
                RangeMax = 200
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
                image.Modulate(new Percentage(100), new Percentage((double)context.WorkFlow.Variables[command.Properties["Saturation"].ToString(context)].GetAsObject()));
                image.Write(context.ImageStream, MagickFormat.Bmp);
            }
            return true;
        }
    }
}

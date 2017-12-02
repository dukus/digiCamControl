using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using ImageMagick;

namespace Capture.Workflow.Plugins.Commands.ImageProcessing
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("BlackAndWhite")]
    [Group("ImageProcessing")]
    [Icon("ImageFilterBlackWhite")]
    public class BlackAndWhiteAction: BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
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
                image.Grayscale(PixelIntensityMethod.Average);
                image.Write(context.ImageStream,MagickFormat.Bmp);
            }
            return true;
        }
    }
}

using System.ComponentModel;
using System.IO;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using ImageMagick;
using SmartFormat;

namespace Capture.Workflow.Plugins.Commands.ImageProcessing
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("SaveFile")]
    [Group("ImageProcessing")]
    [Icon("Floppy")]
    public class SaveFileAction : BaseCommand, IWorkflowCommand
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
                Name = "FileFormat",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = {"Jpg","Png","Bmp"},
                Value = "Jpg"
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (context?.ImageStream == null)
                return true;
            if (!CheckCondition(command, context))
                return true;
            Smart.Default.Settings.ConvertCharacterStringLiterals = false;
            var filename = command.Properties["FileNameTemplate"].ToString(context);

            Utils.CreateFolder(filename);
            
            context.ImageStream.Seek(0, SeekOrigin.Begin);
            using (MagickImage image = new MagickImage(context.ImageStream))
            {
                context.ImageStream.Seek(0, SeekOrigin.Begin);
                switch (command.Properties["FileFormat"].Value)
                {
                    case "Jpg":
                        image.Write(filename + ".jpg");
                        break;
                    case "Png":
                        image.Write(filename + ".png");
                        break;
                    case "Bmp":
                        image.Write(filename + ".bmp");
                        break;
                }
            }
            return true;
        }
    }
}

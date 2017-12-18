using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using ImageMagick;
using Enum = System.Enum;

namespace Capture.Workflow.Plugins.Commands.ImageProcessing
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Overlay")]
    [Group("ImageProcessing")]
    [Icon("ImageMultiple")]
    public  class OverlayAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "OverlayFile",
                PropertyType = CustomPropertyType.File,
                Value = ""
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "StrechOverlay",
                PropertyType = CustomPropertyType.Bool,
                Value = "true"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Position",
                PropertyType = CustomPropertyType.ValueList,
                Value = Enum.GetNames(typeof(Gravity)).ToList()[0],
                ValueList = Enum.GetNames(typeof(Gravity)).ToList()
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Transparency",
                PropertyType = CustomPropertyType.Number,
                Value = "100",
                RangeMin = 0,
                RangeMax = 100
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Text",
                PropertyType = CustomPropertyType.String,
                Description = "Multiline can be used use \\n for a new line",
                Value = ""
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "TextPointSize",
                PropertyType = CustomPropertyType.Number,
                Value = "50",
                RangeMin = 8,
                RangeMax = 500
            });
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "TextFillColor",
                PropertyType = CustomPropertyType.Color,
                Value = "Red"
            });
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "TextStrokeColor",
                PropertyType = CustomPropertyType.Color,
                Value = "Blue"
            });
            command.Properties.Items.Add(new CustomProperty()
            {
                Name = "TextFont",
                PropertyType = CustomPropertyType.String,
                Value = "Arial",
                ValueList = Fonts()
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "TextPosition",
                PropertyType = CustomPropertyType.ValueList,
                Value = Enum.GetNames(typeof(Gravity)).ToList()[0],
                ValueList = Enum.GetNames(typeof(Gravity)).ToList()
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "TextTransparency",
                PropertyType = CustomPropertyType.Number,
                Value = "100",
                RangeMin = 0,
                RangeMax = 100
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
            MagickImage watermark = null;
            using (MagickImage image = new MagickImage(context.ImageStream))
            {
                // check the cache if the image is already is loaded 
                if (context.Cache.ContainsKey(command.GetHashCode().ToString()))
                {
                    var cache = context.Cache[command.GetHashCode().ToString()];
                    if (cache.Id == command.Properties["OverlayFile"].ToString(context))
                    {
                        watermark = (MagickImage) cache.Object;
                    }
                    else
                    {
                        cache.DisposeObject();
                        context.Cache.Remove(command.GetHashCode().ToString());
                    }

                }
                if (watermark == null && File.Exists(command.Properties["OverlayFile"].ToString(context)))
                {
                    // Read the watermark that will be put on top of the image
                    watermark = new MagickImage(command.Properties["OverlayFile"].ToString(context));

                    if (command.Properties["StrechOverlay"].ToBool(context))
                        watermark.Resize(image.Width, image.Height);
                    // Optionally make the watermark more transparent
                    context.Cache.Add(command.GetHashCode().ToString(),
                        new CacheObject()
                        {
                            Id = command.Properties["OverlayFile"].ToString(context),
                            Object = watermark
                        });
                }

                if (watermark != null)
                {
                    if (command.Properties["Transparency"].ToInt(context) != 100)
                        watermark.Evaluate(Channels.Alpha, EvaluateOperator.Add,
                            -(255 * (100 - command.Properties["Transparency"].ToInt(context)) / 100));
                    image.Composite(watermark,
                        (Gravity)Enum.Parse(typeof(Gravity), command.Properties["Position"].ToString(context)),
                        CompositeOperator.Over);
                }

                if (!string.IsNullOrEmpty(command.Properties["Text"].ToString(context)))
                {
                    image.Settings.Font = command.Properties["TextFont"].ToString(context);
                    image.Settings.FontPointsize = command.Properties["TextPointSize"].ToInt(context); ;

                    Color color = (Color)ColorConverter.ConvertFromString(command.Properties["TextFillColor"].ToString(context));
                    image.Settings.FillColor = new MagickColor(color.R, color.G, color.B, (byte) (255 * (100 - command.Properties["TextTransparency"].ToInt(context)) / 100));
                    color = (Color)ColorConverter.ConvertFromString(command.Properties["TextStrokeColor"].ToString(context));
                    image.Settings.StrokeColor = new MagickColor(color.R, color.G, color.B, (byte)(255 * (100 - command.Properties["TextTransparency"].ToInt(context)) / 100));

                    image.Annotate(command.Properties["Text"].ToString(context).Replace("\\n","\n"),
                        (Gravity) Enum.Parse(typeof(Gravity), command.Properties["TextPosition"].ToString(context)));
                }

                image.Write(context.ImageStream, MagickFormat.Jpg);
            }
            return true;
        }

        public List<string> Fonts()
        {
                var fonts = new List<string>();
                foreach (var aFontFamily in System.Windows.Media.Fonts.SystemFontFamilies)
                {
                    var ltypFace = new Typeface(aFontFamily, FontStyles.Normal, FontWeights.Normal,
                        FontStretches.Normal);

                    try
                    {
                        GlyphTypeface lglyphTypeFace;
                        if (ltypFace.TryGetGlyphTypeface(out lglyphTypeFace) &&
                            !string.IsNullOrEmpty(aFontFamily.Source))
                        {
                            fonts.Add(aFontFamily.Source);
                        }
                    }
                    catch
                    {
                    }
                }

                return fonts;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("Image")]
    public class ImageElement: IViewElementPlugin
    {
        public string Name { get; set; }
        public WorkFlowViewElement CreateElement(WorkFlowView view)
        {
            WorkFlowViewElement element = new WorkFlowViewElement();
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "(Name)",
                PropertyType = CustomPropertyType.String
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Position",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = view.Instance.GetPositions(),
                Value = view.Instance.GetPositions()[0]
            });

            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Width",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "0"
            });

            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Height",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "0"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Margins",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "0"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "ImageFile",
                PropertyType = CustomPropertyType.File,
            });
            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement,Context context)
        {
            var image = new System.Windows.Controls.Image();
            viewElement.SetSize(image,context);

            try
            {
                image.Source = Utils.LoadImage(
                    viewElement.Parent.Parent.GetFileStream(viewElement.Properties["ImageFile"].ToString(context)), 0, 0);
                image.Stretch = Stretch.Fill;
            }
            catch (Exception e)
            {
                Log.Debug("Unable to load image ", e);
            }
            return image;
        }
    }
}

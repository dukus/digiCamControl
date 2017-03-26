using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("Button")]
    public class Button:IViewElementPlugin
    {
        public string Name { get; set; }
        public WorkFlowViewElement CreateElement(WorkFlowView view)
        {
            WorkFlowViewElement element = new WorkFlowViewElement();
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Caption",
                PropertyType = CustomPropertyType.String
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Position",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = view.Instance.GetPositions(),
                Value = view.Instance.GetPositions()[0]
            });
            //element.Properties.Items.Add(new CustomProperty()
            //{
            //    Name = "HorizontalAlignment",
            //    PropertyType = CustomPropertyType.ValueList,
            //    ValueList = {"Left","Center","Right"},
            //    Value = "Left"
            //});
            //element.Properties.Items.Add(new CustomProperty()
            //{
            //    Name = "VerticalAlignment",
            //    PropertyType = CustomPropertyType.ValueList,
            //    ValueList = { "Top", "Center", "Bottom" },
            //    Value = "Top"
            //});
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Width",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "150"
            });

            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Height",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "50"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Margins",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "5"
            });

            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "FontSize",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 6,
                RangeMax = 400,
                Value = "15"
            });
            return element;
        }

        public Control GetControl(WorkFlowViewElement viewElement)
        {
            var button = new System.Windows.Controls.Button()
            {
                Width = viewElement.Properties["Width"].ToInt(),
                Height = viewElement.Properties["Height"].ToInt(),
                Content = viewElement.Properties["Caption"].Value,
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt()),
                FontSize = viewElement.Properties["FontSize"].ToInt(),
            };
            return button;
        }


    }
}

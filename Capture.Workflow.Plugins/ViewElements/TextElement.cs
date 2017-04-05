using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("TextElement")]
    public class TextElement: IViewElementPlugin
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
                Name = "LabelWidth",
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
                Name = "Orientation",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>() {"Horizontal", "Vertical"},
                Value = "Horizontal"
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
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "BackgroundColor",
                PropertyType = CustomPropertyType.Color,
                Value = "Transparent"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "ForegroundColor",
                PropertyType = CustomPropertyType.Color,
                Value = "Transparent"
            });
            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement)
        {
            var textBox = new System.Windows.Controls.TextBox()
            {
                Width = viewElement.Properties["Width"].ToInt(),
                Height = viewElement.Properties["Height"].ToInt(),
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt()),
                FontSize = viewElement.Properties["FontSize"].ToInt(),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            if (viewElement.Properties["BackgroundColor"].Value != "Transparent" && viewElement.Properties["BackgroundColor"].Value != "#00FFFFFF")
                textBox.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].Value));

            if (viewElement.Properties["ForegroundColor"].Value != "Transparent" && viewElement.Properties["ForegroundColor"].Value != "#00FFFFFF")
                textBox.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].Value));

            var label = new System.Windows.Controls.Label()
            {
                Width = viewElement.Properties["LabelWidth"].ToInt(),
                Height = viewElement.Properties["Height"].ToInt(),
                Content = viewElement.Properties["Caption"].Value,
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt()),
                FontSize = viewElement.Properties["FontSize"].ToInt(),
            };
            if (viewElement.Properties["BackgroundColor"].Value != "Transparent" && viewElement.Properties["BackgroundColor"].Value != "#00FFFFFF")
                label.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].Value));
            if (viewElement.Properties["ForegroundColor"].Value != "Transparent" && viewElement.Properties["ForegroundColor"].Value != "#00FFFFFF")
                label.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].Value));

            var stackpanel = new StackPanel();
            stackpanel.Children.Add(label);
            stackpanel.Children.Add(textBox);
            stackpanel.Orientation = viewElement.Properties["Orientation"].Value == "Horizontal" ? Orientation.Horizontal : Orientation.Vertical;
            return stackpanel;
        }
    }
}

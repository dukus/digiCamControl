using System.Collections.Generic;
using System.ComponentModel;
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
    [DisplayName("Text")]
    public class TextElement: IViewElementPlugin
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
                Name = "IsReadOnly",
                PropertyType = CustomPropertyType.Bool
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
                Value = "0"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Height",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "35"
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
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
                Value = ""
            });
            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement,Context context)
        {
            var textBox = new TextBox()
            {
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                IsReadOnly = viewElement.Properties["IsReadOnly"].ToBool(context),
            };
            viewElement.SetSize(textBox,context);

            textBox.DataContext = viewElement.Parent.Parent.Variables[viewElement.Properties["Variable"].ToString(context)];
            textBox.SetBinding(TextBox.TextProperty, "Value");

            if (viewElement.Properties["BackgroundColor"].ToString(context) != "Transparent" && viewElement.Properties["BackgroundColor"].ToString(context) != "#00FFFFFF")
                textBox.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].ToString(context)));

            if (viewElement.Properties["ForegroundColor"].ToString(context) != "Transparent" && viewElement.Properties["ForegroundColor"].ToString(context) != "#00FFFFFF")
                textBox.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].ToString(context)));

            var label = new System.Windows.Controls.Label()
            {
                Height = viewElement.Properties["Height"].ToInt(context),
                Content = viewElement.Properties["Caption"].ToString(context),
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt(context)),
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            if (viewElement.Properties["Orientation"].ToString(context) == "Vertical")
                label.HorizontalContentAlignment = HorizontalAlignment.Center;

            if (viewElement.Properties["LabelWidth"].ToInt(context) > 0)
                label.Width = viewElement.Properties["LabelWidth"].ToInt(context);

            //if (viewElement.Properties["BackgroundColor"].Value != "Transparent" && viewElement.Properties["BackgroundColor"].Value != "#00FFFFFF")
            //    label.Background =
            //        new SolidColorBrush(
            //            (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].Value));
            if (viewElement.Properties["ForegroundColor"].ToString(context) != "Transparent" && viewElement.Properties["ForegroundColor"].ToString(context) != "#00FFFFFF")
                label.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].ToString(context)));

            var stackpanel = new StackPanel();
            stackpanel.Children.Add(label);
            stackpanel.Children.Add(textBox);
            stackpanel.Orientation = viewElement.Properties["Orientation"].ToString(context) == "Horizontal" ? Orientation.Horizontal : Orientation.Vertical;
            return stackpanel;
        }
    }
}

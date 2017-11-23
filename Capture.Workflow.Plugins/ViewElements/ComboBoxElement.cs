using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using MaterialDesignThemes.Wpf;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("ComboBox")]
    public class ComboBoxElement: IViewElementPlugin
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
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
                Value = ""
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "ValueList",
                PropertyType = CustomPropertyType.String,
                Value = ""
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "ValueListVariable",
                PropertyType = CustomPropertyType.Variable,
                Value = ""
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Orientation",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = { "Horizontal", "Vertical" },
                Value = "Vertical"
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
                Value = "0"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Margins",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 0,
                RangeMax = 9999,
                Value = "2"
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
                Name = "Editable",
                PropertyType = CustomPropertyType.Bool,
                Value = "True"
            });
            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement, Context context)
        {
            ComboBox comboBox = new ComboBox()
            {
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            viewElement.SetSize(comboBox, context);
            var variable = viewElement.Properties["Variable"].ToString(context);
            comboBox.DataContext = viewElement.Parent.Parent;
            if (string.IsNullOrEmpty(viewElement.Properties["ValueListVariable"].ToString(context)))
            {
                comboBox.ItemsSource = viewElement.Properties["ValueList"].ToString(context).Split('|');
            }
            else
            {
                comboBox.SetBinding(ComboBox.ItemsSourceProperty, "Variables[" + viewElement.Properties["ValueListVariable"].ToString(context) + "].ValueList");
            }

            comboBox.SetBinding(ComboBox.TextProperty, "Variables["+variable+"].Value");
            comboBox.IsEditable = viewElement.Properties["Editable"].ToBool(context);


            if (viewElement.Properties["BackgroundColor"].ToString(context) != "Transparent" &&
                viewElement.Properties["BackgroundColor"].ToString(context) != "#00FFFFFF")
                comboBox.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].ToString(context)));

            if (viewElement.Properties["ForegroundColor"].ToString(context) != "Transparent" &&
                viewElement.Properties["ForegroundColor"].ToString(context) != "#00FFFFFF")
                comboBox.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].ToString(context)));

            var label = new TextBlock()
            {
                Text = viewElement.Properties["Caption"].ToString(context),
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
                VerticalAlignment = VerticalAlignment.Center,
            };
            if (viewElement.Properties["Orientation"].ToString(context) == "Vertical")
                label.TextAlignment = TextAlignment.Center;


            if (viewElement.Properties["LabelWidth"].ToInt(context) > 0)
                label.Width = viewElement.Properties["LabelWidth"].ToInt(context);

            if (viewElement.Properties["Height"].ToInt(context) > 0)
                label.Height = viewElement.Properties["Height"].ToInt(context);

            if (viewElement.Properties["ForegroundColor"].ToString(context) != "Transparent" &&
                viewElement.Properties["ForegroundColor"].ToString(context) != "#00FFFFFF")
                label.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].ToString(context)));

            var stackpanel = new StackPanel()
            {
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt(context)),
                Orientation = viewElement.Properties["Orientation"].ToString(context) == "Horizontal"
                    ? Orientation.Horizontal
                    : Orientation.Vertical
            };
            stackpanel.Children.Add(label);
            stackpanel.Children.Add(comboBox);

            return stackpanel;

        }

    }
}

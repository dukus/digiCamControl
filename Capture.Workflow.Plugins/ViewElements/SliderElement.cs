using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("Slider")]
    public class SliderElement : IViewElementPlugin
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
                Name = "FontSize",
                PropertyType = CustomPropertyType.Number,
                RangeMin = 6,
                RangeMax = 400,
                Value = "15"
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
                ValueList = new List<string>() { "Horizontal", "Vertical" },
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
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
                Value = ""
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Minimum",
                PropertyType = CustomPropertyType.Number,
                RangeMin = int.MinValue,
                RangeMax = int.MaxValue,
                Value = "-100"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Maximum",
                PropertyType = CustomPropertyType.Number,
                RangeMin = int.MinValue,
                RangeMax = int.MaxValue,
                Value = "100"
            });

            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement, Context context)
        {
            var slider = new Slider()
            {
                Orientation = Orientation.Horizontal,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            viewElement.SetSize(slider, context);
            
            slider.DataContext = viewElement.Parent.Parent.Variables[viewElement.Properties["Variable"].ToString(context)];
            slider.SetBinding(RangeBase.ValueProperty, "Value");

            slider.Minimum= viewElement.Properties["Minimum"].ToInt(context);
            slider.Maximum = viewElement.Properties["Maximum"].ToInt(context);

            var label = new System.Windows.Controls.Label()
            {
                Height = viewElement.Properties["Height"].ToInt(context),
                Content = viewElement.Properties["Caption"].ToString(context),
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt(context)),
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (viewElement.Properties["LabelWidth"].ToInt(context) > 0)
                label.Width = viewElement.Properties["LabelWidth"].ToInt(context);


            var stackpanel = new StackPanel();
            stackpanel.Children.Add(label);
            stackpanel.Children.Add(slider);
            stackpanel.Orientation = viewElement.Properties["Orientation"].ToString(context) == "Horizontal" ? Orientation.Horizontal : Orientation.Vertical;
            return stackpanel;
        }
    }
}

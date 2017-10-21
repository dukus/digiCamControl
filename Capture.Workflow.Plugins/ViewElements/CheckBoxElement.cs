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
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using MaterialDesignThemes.Wpf;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("CheckBox")]
    public class CheckBoxElement: IViewElementPlugin
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
                Name = "Position",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = view.Instance.GetPositions(),
                Value = view.Instance.GetPositions()[0]
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Style",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = { "Default"},
                Value = "Default"
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

        public FrameworkElement GetControl(WorkFlowViewElement viewElement, Context context)
        {
            var checkbox = new System.Windows.Controls.CheckBox()
            {
                Content = viewElement.Properties["Caption"].ToString(context),
                FontSize = viewElement.Properties["FontSize"].ToInt(context),
            };
            viewElement.SetSize(checkbox, context);

            checkbox.DataContext = viewElement.Parent.Parent.Variables[viewElement.Properties["Variable"].ToString(context)];
            checkbox.SetBinding(ToggleButton.IsCheckedProperty, "Value");

            //if (viewElement.Properties["Style"].Value == "Rounded")
            //{
            //    checkbox.Style = Application.Current.Resources["MaterialDesignFloatingActionButton"] as Style;
            //}


            if (viewElement.Properties["BackgroundColor"].ToString(context) != "Transparent" && viewElement.Properties["BackgroundColor"].ToString(context) != "#00FFFFFF")
                checkbox.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].ToString(context)));
            if (viewElement.Properties["ForegroundColor"].ToString(context) != "Transparent" && viewElement.Properties["ForegroundColor"].ToString(context) != "#00FFFFFF")
                checkbox.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].ToString(context)));

            return checkbox;
        }


    }
}

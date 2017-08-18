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
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;


namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("CameraProperty")]
    public class CameraProperty: IViewElementPlugin
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
                Name = "Property",
                PropertyType = CustomPropertyType.ValueList,
                ValueList =
                {
                    "Mode",
                    "CompressionSetting",
                    "ExposureCompensation",
                    "ExposureMeteringMode",
                    "FNumber",
                    "IsoNumber",
                    "ShutterSpeed",
                    "WhiteBalance",
                    "FocusMode"
                },
                Value = "Mode"
            });
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Orientation",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = {"Horizontal","Vertical"},
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
            return element;
        }

        public FrameworkElement GetControl(WorkFlowViewElement viewElement)
        {
            //< StackPanel Margin = "2,0" >

            //    < Label Content = "{T:TranslateExtension Iso}" Target = "{Binding ElementName=cmb_iso}" Padding = "0" />

            //    < Border BorderThickness = "1" BorderBrush = "{Binding Path=IsoNumber.ErrorColor}" >

            //    < ComboBox Name = "cmb_iso" IsEnabled = "{Binding Path=IsoNumber.IsEnabled}" ItemsSource = "{Binding Path=IsoNumber.Values}" SelectedValue = "{Binding Path=IsoNumber.Value}" />

            //    </ Border >

            //    </ StackPanel >

            var property = GetProperty(viewElement.Properties["Property"].Value,
                ServiceProvider.Instance.DeviceManager.SelectedCameraDevice);
            ComboBox comboBox = new ComboBox()
            {
                FontSize = viewElement.Properties["FontSize"].ToInt(),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            viewElement.SetSize(comboBox);
            
            comboBox.DataContext = property;
            comboBox.SetBinding(ComboBox.IsEnabledProperty, "IsEnabled");
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, "Values");
            comboBox.SetBinding(ComboBox.SelectedValueProperty, "Value");

            if (viewElement.Properties["BackgroundColor"].Value != "Transparent" &&
                viewElement.Properties["BackgroundColor"].Value != "#00FFFFFF")
                comboBox.Background =
                    new SolidColorBrush(
                        (Color) ColorConverter.ConvertFromString(viewElement.Properties["BackgroundColor"].Value));

            if (viewElement.Properties["ForegroundColor"].Value != "Transparent" &&
                viewElement.Properties["ForegroundColor"].Value != "#00FFFFFF")
                comboBox.Foreground =
                    new SolidColorBrush(
                        (Color) ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].Value));

            var label = new TextBlock()
            {
                Text = viewElement.Properties["Caption"].Value,
                FontSize = viewElement.Properties["FontSize"].ToInt(),
                VerticalAlignment = VerticalAlignment.Center,
            };

            if (viewElement.Properties["LabelWidth"].ToInt() > 0)
                label.Width = viewElement.Properties["LabelWidth"].ToInt();

            if (viewElement.Properties["Height"].ToInt() > 0)
                label.Height = viewElement.Properties["Height"].ToInt();

            if (viewElement.Properties["ForegroundColor"].Value != "Transparent" &&
                viewElement.Properties["ForegroundColor"].Value != "#00FFFFFF")
                label.Foreground =
                    new SolidColorBrush(
                        (Color) ColorConverter.ConvertFromString(viewElement.Properties["ForegroundColor"].Value));

            var stackpanel = new StackPanel()
            {
                Margin = new Thickness(viewElement.Properties["Margins"].ToInt()),
                Orientation = viewElement.Properties["Orientation"].Value == "Horizontal"
                    ? Orientation.Horizontal
                    : Orientation.Vertical
            };
            stackpanel.Children.Add(label);
            stackpanel.Children.Add(comboBox);

            return stackpanel;
         
        }

        private PropertyValue<long> GetProperty(string name, ICameraDevice device)
        {
            switch (name)
            {
                case "Mode":
                    return device.Mode;
                case "CompressionSetting":
                    return device.CompressionSetting;
                case "ExposureCompensation":
                    return device.ExposureCompensation;
                case "ExposureMeteringMode":
                    return device.ExposureCompensation;
                case "FNumber":
                    return device.FNumber;
                case "IsoNumber":
                    return device.IsoNumber;
                case "ShutterSpeed":
                    return device.ShutterSpeed;
                case "WhiteBalance":
                    return device.WhiteBalance;
                case "FocusMode":
                    return device.FocusMode;
            }
            return null;
        }

    }
}

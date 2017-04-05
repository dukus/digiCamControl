using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Plugins.Views.View;
using Capture.Workflow.Plugins.Views.ViewModel;

namespace Capture.Workflow.Plugins.Views
{
    [Description("Vindow to query parameter on start")]
    [PluginType(PluginType.View)]
    [DisplayName("StartScreen")]
    public class StartScreen : BaseView
    {
        public override WorkFlowView CreateView()
        {
            WorkFlowView view = new WorkFlowView();
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "ViewTitle",
                PropertyType = CustomPropertyType.String
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "BorderColor",
                PropertyType = CustomPropertyType.Color,
                Value = "Transparent"
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "BorderBackground",
                PropertyType = CustomPropertyType.Color,
                Value = "Transparent"
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "BorderThickness",
                PropertyType = CustomPropertyType.Number,
                Value = "0",
                RangeMax = 100
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "CornerRadius",
                PropertyType = CustomPropertyType.Number,
                Value = "0",
                RangeMax = 90

            });


            return view;
        }

        public override List<string> GetPositions()
        {
            return new List<string> { "Center", "BottomLeft", "BottomRight" };
        }

        public override UserControl GetPreview(WorkFlowView view)
        {
            var model = new StartSctreenViewModel();
            foreach (var element in view.Elements)
            {
                switch (element.Properties["Position"].Value)
                {
                    case "Center":
                        model.CenterElements.Add(element.Instance.GetControl(element));
                        break;
                    case "BottomLeft":
                        model.BottomLeftElements.Add(element.Instance.GetControl(element));
                        break;

                    case "BottomRight":
                        model.BottomRightElements.Add(element.Instance.GetControl(element));
                        break;
                }
            }
            model.BorderColor = view.Properties["BorderColor"].Value;
            model.BorderBackground = view.Properties["BorderBackground"].Value;
            model.BorderThickness= view.Properties["BorderThickness"].ToInt();
            model.CornerRadius = view.Properties["CornerRadius"].ToInt();
            var res = new StartScreenView();
            res.DataContext = model;
            return res;
        }
    }
}

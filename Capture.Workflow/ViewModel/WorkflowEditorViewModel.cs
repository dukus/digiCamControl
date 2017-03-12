using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowEditorViewModel: ViewModelBase
    {
        public AsyncObservableCollection<WorkFlowView> Views { get; set; }

        public CustomPropertyCollection PropertyCollection { get; set; }


        public RelayCommand NewCommand { get; set; }

        public WorkflowEditorViewModel()
        {
            NewCommand = new RelayCommand(New);
            Views = new AsyncObservableCollection<WorkFlowView>();
            PropertyCollection = new CustomPropertyCollection();
            PropertyCollection.Items.Add(new CustomProperty()
            {
                PropertyType = CustomPropertyType.String,
                Name = "TestParam",
                Description = "Test param description",
                Value = "Test value"
            });
            PropertyCollection.Items.Add(new CustomProperty()
            {
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>() { "Value 1","value 2"},
                Value = "value 2",
                Name = "TestParam 2",
                Description = "Test param 2 description"
            });
        }

        private void New()
        {
            var wnd = new NewViewSelectorView();
            if (wnd.ShowDialog()==true)
            {
                var contex = wnd.DataContext as NewViewSelectorViewModel;
                if (contex != null)
                {
                    Views.Add(new WorkFlowView()
                    {
                        PluginInfo = contex.SelectedItem,
                        Name = contex.Name ?? contex.SelectedItem.Name
                    });
                }
            }
            
        }
    }
}

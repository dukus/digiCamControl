using System;
using System.Collections.Generic;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Interface;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowEditorViewModel: ViewModelBase
    {
        private WorkFlowView _selectedView;
        private WorkFlowViewElement _selectedElement;
        public AsyncObservableCollection<WorkFlowView> Views { get; set; }
        
        public WorkFlowView SelectedView
        {
            get { return _selectedView; }
            set
            {
                _selectedView = value;
                RaisePropertyChanged(()=>SelectedView);
            }
        }

        public WorkFlowViewElement SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                RaisePropertyChanged(() => SelectedElement);
            }
        }


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
                    IViewPlugin plugin = (IViewPlugin) Activator.CreateInstance(contex.SelectedItem.Class);
                    WorkFlowView view = plugin.CreateView();
                    view.Instance = plugin;
                    view.PluginInfo = contex.SelectedItem;
                    view.Name = contex.Name ?? contex.SelectedItem.Name;
                    Views.Add(view);
                }
            }
            
        }
    }
}

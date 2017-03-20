using System;
using System.Collections.Generic;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Interface;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowEditorViewModel : ViewModelBase
    {
        private WorkFlowView _selectedView;
        private WorkFlowViewElement _selectedElement;
        public AsyncObservableCollection<WorkFlowView> Views { get; set; }

        public List<PluginInfo> ViewsPlugins { get; set; }
        public List<PluginInfo> ViewElementsPlugins { get; set; }

        public WorkFlowView SelectedView
        {
            get { return _selectedView; }
            set
            {
                _selectedView = value;
                RaisePropertyChanged(() => SelectedView);
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


        public RelayCommand<PluginInfo> NewViewCommand { get; set; }
        public RelayCommand<PluginInfo> NewViewElementCommand { get; set; }

        public WorkflowEditorViewModel()
        {
            NewViewCommand = new RelayCommand<PluginInfo>(NewView);
            NewViewElementCommand = new RelayCommand<PluginInfo>(NewViewElement);
            Views = new AsyncObservableCollection<WorkFlowView>();
            ViewsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.View);
            ViewElementsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.ViewElement);
        }

        private void NewViewElement(PluginInfo pluginInfo)
        {
            try
            {
                IViewElementPlugin plugin = (IViewElementPlugin)Activator.CreateInstance(pluginInfo.Class);
                WorkFlowViewElement view = plugin.CreateElement();
                view.Instance = plugin;
                view.PluginInfo = pluginInfo;
                view.Name = pluginInfo.Name;
                SelectedView.Elements.Add(view);
            }
            catch (Exception ex)
            {
                Log.Error("Error create element", ex);
                
            }
        }

        private void NewView(PluginInfo pluginInfo)
        {
            IViewPlugin plugin = (IViewPlugin)Activator.CreateInstance(pluginInfo.Class);
            WorkFlowView view = plugin.CreateView();
            view.Instance = plugin;
            view.PluginInfo = pluginInfo;
            view.Name = pluginInfo.Name;
            Views.Add(view);
        }
    }
}

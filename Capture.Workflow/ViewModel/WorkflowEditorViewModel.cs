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
        private WorkFlow _currentWorkFlow;

        public List<PluginInfo> ViewsPlugins { get; set; }
        public List<PluginInfo> ViewElementsPlugins { get; set; }

        public WorkFlow CurrentWorkFlow
        {
            get { return _currentWorkFlow; }
            set
            {
                _currentWorkFlow = value;
                RaisePropertyChanged(() => CurrentWorkFlow);
            }
        }


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
        public RelayCommand PreviewViewCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }

        public WorkflowEditorViewModel()
        {
            NewViewCommand = new RelayCommand<PluginInfo>(NewView);
            NewViewElementCommand = new RelayCommand<PluginInfo>(NewViewElement);
            PreviewViewCommand=new RelayCommand(PreviewView);
            SaveCommand=new RelayCommand(Save);

            ViewsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.View);
            ViewElementsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.ViewElement);
            CurrentWorkFlow = new WorkFlow();
        }

        private void Save()
        {
            WorkflowManager.Instance.Save(CurrentWorkFlow,"Test.xml");
        }

        private void PreviewView()
        {
            var wnd = new ViewPreviewView();
            wnd.ContentControl.Content = SelectedView.Instance.GetPreview(SelectedView);
            wnd.ShowDialog();
        }

        private void NewViewElement(PluginInfo pluginInfo)
        {
            try
            {
                IViewElementPlugin plugin = WorkflowManager.Instance.GetElementPlugin(pluginInfo.Class);
                WorkFlowViewElement element = plugin.CreateElement(SelectedView);
                element.Instance = plugin;
                element.PluginInfo = pluginInfo;
                element.Name = pluginInfo.Name;
                SelectedView.Elements.Add(element);
                SelectedElement = element;
            }
            catch (Exception ex)
            {
                Log.Error("Error create element", ex);
                
            }
        }

        private void NewView(PluginInfo pluginInfo)
        {
            IViewPlugin plugin = WorkflowManager.Instance.GetViewPlugin(pluginInfo.Class);
            WorkFlowView view = plugin.CreateView();
            view.Instance = plugin;
            view.PluginInfo = pluginInfo;
            view.Name = pluginInfo.Name;
            CurrentWorkFlow.Views.Add(view);
            SelectedView = view;
        }
    }
}

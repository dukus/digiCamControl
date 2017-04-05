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
        private Variable _selectedVariable;

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

        public Variable SelectedVariable
        {
            get { return _selectedVariable; }
            set
            {
                _selectedVariable = value;
                RaisePropertyChanged(() => SelectedVariable);
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
        public RelayCommand DeleteViewCommand { get; set; }

        public RelayCommand<PluginInfo> NewViewElementCommand { get; set; }
        public RelayCommand DeleteViewElementCommand { get; set; }

        public RelayCommand NewVariableCommand { get; set; }
        public RelayCommand DeleteVariableCommand { get; set; }
        public RelayCommand PreviewViewCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public WorkflowEditorViewModel()
        {
            NewViewCommand = new RelayCommand<PluginInfo>(NewView);
            DeleteViewCommand=new RelayCommand(DeleteView);

            NewViewElementCommand = new RelayCommand<PluginInfo>(NewViewElement);
            DeleteViewElementCommand=new RelayCommand(DeleteViewElement);

            NewVariableCommand = new RelayCommand(NewVariable);
            DeleteVariableCommand = new RelayCommand(DeleteVariable);
            PreviewViewCommand = new RelayCommand(PreviewView);
            SaveCommand = new RelayCommand(Save);

            ViewsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.View);
            ViewElementsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.ViewElement);
            CurrentWorkFlow = WorkflowManager.Instance.CreateWorkFlow();
            LoadCommand = new RelayCommand(Load);
        }

        private void DeleteViewElement()
        {
            if (SelectedElement != null && SelectedView!=null)
            {
                SelectedView.Elements.Remove(SelectedElement);
                if (SelectedView.Elements.Count > 0)
                    SelectedElement = SelectedView.Elements[SelectedView.Elements.Count - 1];
            }
        }

        private void DeleteView()
        {
            if (SelectedView != null)
            {
                CurrentWorkFlow.Views.Remove(SelectedView);
                if (CurrentWorkFlow.Views.Count > 0)
                    SelectedView = CurrentWorkFlow.Views[CurrentWorkFlow.Views.Count - 1];
            }
        }

        private void DeleteVariable()
        {
            if (SelectedVariable != null)
            {
                CurrentWorkFlow.Variables.Items.Remove(SelectedVariable);
                if (CurrentWorkFlow.Variables.Items.Count > 0)
                {
                    SelectedVariable = CurrentWorkFlow.Variables.Items[CurrentWorkFlow.Variables.Items.Count - 1];
                }
            }
        }

        private void NewVariable()
        {
            CurrentWorkFlow.Variables.Items.Add(new Variable() { Name = "Variable#" + CurrentWorkFlow.Variables.Items.Count });
        }

        private void Load()
        {
            CurrentWorkFlow = WorkflowManager.Instance.Load("Test.xml");
            if (CurrentWorkFlow.Views.Count > 0)
            {
                SelectedView = CurrentWorkFlow.Views[0];
                if (SelectedView.Elements.Count > 0)
                    SelectedElement = SelectedView.Elements[0];
            }
            if (CurrentWorkFlow.Variables.Items.Count > 0)
                SelectedVariable = CurrentWorkFlow.Variables.Items[0];

        }

        private void Save()
        {
            WorkflowManager.Instance.Save(CurrentWorkFlow, "Test.xml");
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

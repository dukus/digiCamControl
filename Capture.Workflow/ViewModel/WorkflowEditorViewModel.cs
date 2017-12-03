using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Interface;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowEditorViewModel : ViewModelBase
    {
        private string _file = "";

        private WorkFlowView _selectedView;
        private WorkFlowViewElement _selectedElement;
        private WorkFlow _currentWorkFlow;
        private Variable _selectedVariable;
        private CommandCollection _selectedCommandCollection;
        private WorkFlowCommand _selectedCommand;
        private bool _haveEvents;
        private CommandCollection _selectedViewCommandCollection;
        private WorkFlowCommand _selectedViewCommand;
        private WorkFlowEvent _selectedEvent;
        private WorkFlowCommand _selectedEventCommand;
        private PluginInfo _selectedNewEventPlugin;

        public List<PluginInfo> ViewsPlugins { get; set; }
        public List<PluginInfo> CommandPlugins { get; set; }
        public List<PluginInfo> ViewElementsPlugins { get; set; }
        public List<PluginInfo> EventsPlugins { get; set; }

        public WorkFlow CurrentWorkFlow
        {
            get { return _currentWorkFlow; }
            set
            {
                _currentWorkFlow = value;
                WorkflowManager.Instance.Context.WorkFlow = _currentWorkFlow;
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
                RaisePropertyChanged(() => IsViewSelected);
                RaisePropertyChanged(() => HaveViewEvents);
                if (_selectedView?.Elements.Count > 0)
                    SelectedElement = _selectedView.Elements[0];
                if (_selectedView?.Events.Count > 0)
                    SelectedViewCommandCollection = _selectedView?.Events[0];
            }
        }

        public bool HaveViewEvents => _selectedView?.Events.Count>0 || IsInDesignMode;

        public bool IsViewSelected => SelectedView != null || IsInDesignMode;

        public WorkFlowEvent SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                RaisePropertyChanged(()=>SelectedEvent);
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


        public bool HaveEvents
        {
            get { return _haveEvents; }
            set
            {
                _haveEvents = value;
                RaisePropertyChanged(() => HaveEvents);
            }
        }

        public WorkFlowViewElement SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                RaisePropertyChanged(() => SelectedElement);
                RaisePropertyChanged(() => IsElementSelected);
                if (_selectedElement?.Events.Count > 0)
                {
                    HaveEvents = true;
                    SelectedCommandCollection = _selectedElement.Events[0];
                    if (SelectedCommandCollection.Items.Count > 0)
                    {
                        SelectedCommand = SelectedCommandCollection.Items[0];
                    }
                }
                else
                {
                    HaveEvents = false;
                    SelectedCommandCollection = null;
                }
            }
        }

        public bool IsElementSelected => SelectedElement != null || IsInDesignMode;

        public CommandCollection SelectedCommandCollection
        {
            get { return _selectedCommandCollection; }
            set
            {
                _selectedCommandCollection = value;
                RaisePropertyChanged(() => SelectedCommandCollection);
            }
        }

        public CommandCollection SelectedViewCommandCollection
        {
            get { return _selectedViewCommandCollection; }
            set
            {
                _selectedViewCommandCollection = value;
                RaisePropertyChanged(()=>SelectedViewCommandCollection);
            }
        }


        public WorkFlowCommand SelectedCommand
        {
            get { return _selectedCommand; }
            set
            {
                _selectedCommand = value;
                RaisePropertyChanged(() => SelectedCommand);
            }
        }

        public WorkFlowCommand SelectedEventCommand
        {
            get { return _selectedEventCommand; }
            set
            {
                _selectedEventCommand = value;
                RaisePropertyChanged(()=>SelectedEventCommand);
            }
        }


        public WorkFlowCommand SelectedViewCommand
        {
            get { return _selectedViewCommand; }
            set
            {
                _selectedViewCommand = value;
                RaisePropertyChanged(()=>SelectedViewCommand);
            }
        }

        public PluginInfo SelectedNewEventPlugin
        {
            get { return _selectedNewEventPlugin; }
            set
            {
                _selectedNewEventPlugin = value;
                RaisePropertyChanged(() => SelectedNewEventPlugin);
            }
        }


        public CustomPropertyCollection PropertyCollection { get; set; }


        public RelayCommand<PluginInfo> NewViewCommand { get; set; }
        public RelayCommand<WorkFlowView> DeleteViewCommand { get; set; }

        public RelayCommand<PluginInfo> NewViewElementCommand { get; set; }
        public RelayCommand<WorkFlowViewElement> DeleteViewElementCommand { get; set; }

        public RelayCommand NewVariableCommand { get; set; }
        public RelayCommand DeleteVariableCommand { get; set; }

        public RelayCommand<PluginInfo> NewCommandCommand { get; set; }
        public RelayCommand<WorkFlowCommand> DeleteCommandCommand { get; set; }

        public RelayCommand PreviewViewCommand { get; set; }
        public RelayCommand RunCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SavePackageCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public RelayCommand<PluginInfo> NewViewCommandCommand { get; set; }
        public RelayCommand<WorkFlowCommand> DeleteViewCommandCommand { get; set; }


        public RelayCommand<PluginInfo> NewEventCommand { get; set; }
        public RelayCommand<WorkFlowEvent> DeleteEventCommand { get; set; }

        public RelayCommand<PluginInfo> NewEventCommandCommand { get; set; }
        public RelayCommand<WorkFlowCommand> DeleteEventCommandCommand { get; set; }

        public WorkflowEditorViewModel()
        {
            NewViewCommand = new RelayCommand<PluginInfo>(NewView);
            DeleteViewCommand = new RelayCommand<WorkFlowView>(DeleteView);

            NewViewElementCommand = new RelayCommand<PluginInfo>(NewViewElement);
            DeleteViewElementCommand = new RelayCommand<WorkFlowViewElement>(DeleteViewElement);

            NewVariableCommand = new RelayCommand(NewVariable);
            DeleteVariableCommand = new RelayCommand(DeleteVariable);
            PreviewViewCommand = new RelayCommand(PreviewView);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            SavePackageCommand = new RelayCommand(SavePackage);

            ViewsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.View);
            ViewElementsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.ViewElement);
            CommandPlugins = WorkflowManager.Instance.GetPlugins(PluginType.Command);
            EventsPlugins = WorkflowManager.Instance.GetPlugins(PluginType.Event);
            CurrentWorkFlow = WorkflowManager.Instance.CreateWorkFlow();
            LoadCommand = new RelayCommand(Load);

            NewCommandCommand = new RelayCommand<PluginInfo>(NewCommand);
            DeleteCommandCommand = new RelayCommand<WorkFlowCommand>(DeleteCommand);

            NewViewCommandCommand = new RelayCommand<PluginInfo>(AddViewCommand);
            DeleteViewCommandCommand = new RelayCommand<WorkFlowCommand>(RemoveViewCommand);

            RunCommand = new RelayCommand(Run);

            NewEventCommand = new RelayCommand<PluginInfo>(NewEvent);
            DeleteEventCommand = new RelayCommand<WorkFlowEvent>(DeleteEvent);

            NewEventCommandCommand=new RelayCommand<PluginInfo>(NewEventCommandMethod);
            DeleteEventCommandCommand=new RelayCommand<WorkFlowCommand>(DeleteEventCommandMethod);
        }

        private void DeleteEventCommandMethod(WorkFlowCommand command)
        {
            if (SelectedEvent != null && command != null)
            {
                SelectedEvent.CommandCollection.Items.Remove(command);
                if (SelectedEvent.CommandCollection.Items.Count > 0)
                    SelectedEventCommand = SelectedEvent.CommandCollection.Items[0];
            }
        }

        private void NewEventCommandMethod(PluginInfo pluginInfo)
        {
            if (SelectedEvent != null)
            {
                IWorkflowCommand commandPlugin = WorkflowManager.Instance.GetCommandPlugin(pluginInfo.Class);
                var wCommand = commandPlugin.CreateCommand();
                wCommand.Instance = commandPlugin;
                wCommand.PluginInfo = pluginInfo;
                wCommand.Name = GetNewName(pluginInfo.Name, SelectedEvent.CommandCollection.Items.Select(x => x.Name).ToList()); 
                SelectedEvent.CommandCollection.Items.Add(wCommand);
                SelectedEventCommand = wCommand;
            }
        }

        private void DeleteEvent(WorkFlowEvent flowEvent)
        {
            if (flowEvent != null)
            {
                CurrentWorkFlow.Events.Remove(flowEvent);
                if (CurrentWorkFlow.Events.Count > 0)
                    SelectedEvent = CurrentWorkFlow.Events[0];
            }
        }

        private void NewEvent(PluginInfo pluginInfo)
        {
            IEventPlugin plugin =WorkflowManager.Instance.GetEventPlugin(pluginInfo.Class);
            WorkFlowEvent event_ = plugin.CreateEvent();
            event_.Parent = CurrentWorkFlow;
            event_.Instance = plugin;
            event_.PluginInfo = pluginInfo;
            event_.Name = GetNewName(pluginInfo.Name, CurrentWorkFlow.Events.Select(x => x.Name).ToList());
            CurrentWorkFlow.Events.Add(event_);
            SelectedEvent = event_;
        }

        private void RemoveViewCommand(WorkFlowCommand command)
        {
            if (command != null)
                SelectedViewCommandCollection?.Items.Remove(command);
            if (SelectedViewCommandCollection?.Items.Count > 0)
            {
                SelectedViewCommand = SelectedViewCommandCollection.Items[0];
            }
        }

        private void AddViewCommand(PluginInfo pluginInfo)
        {
            if (SelectedViewCommandCollection != null)
            {
                IWorkflowCommand commandPlugin = WorkflowManager.Instance.GetCommandPlugin(pluginInfo.Class);
                var wCommand = commandPlugin.CreateCommand();
                wCommand.Instance = commandPlugin;
                wCommand.PluginInfo = pluginInfo;
                wCommand.Name = GetNewName(pluginInfo.Name,
                    SelectedViewCommandCollection.Items.Select(x => x.Name).ToList());
                SelectedViewCommandCollection.Items.Add(wCommand);
                SelectedViewCommand = wCommand;
            }
        }

        private void Run()
        {
            WorkflowViewView wnd = new WorkflowViewView();
            wnd.ShowDialog();
        }

        private void DeleteCommand(WorkFlowCommand command)
        {
            if (command != null)
                SelectedCommandCollection?.Items.Remove(command);

            if (SelectedCommandCollection?.Items.Count > 0)
            {
                SelectedCommand = SelectedCommandCollection.Items[0];
            }
        }



        private void NewCommand(PluginInfo pluginInfo)
        {
            if (SelectedCommandCollection != null)
            {
                IWorkflowCommand commandPlugin = WorkflowManager.Instance.GetCommandPlugin(pluginInfo.Class);
                var wCommand = commandPlugin.CreateCommand();
                wCommand.Instance = commandPlugin;
                wCommand.PluginInfo = pluginInfo;
                wCommand.Name = GetNewName(pluginInfo.Name,
                    SelectedCommandCollection.Items.Select(x => x.Name).ToList());
                SelectedCommandCollection.Items.Add(wCommand);
                SelectedCommand = wCommand;
            }
        }

        private void DeleteViewElement(WorkFlowViewElement element)
        {
            if (element != null && SelectedView != null)
            {
                SelectedView.Elements.Remove(element);
                if (SelectedView.Elements.Count > 0)
                    SelectedElement = SelectedView.Elements[SelectedView.Elements.Count - 1];
            }
        }

        private void DeleteView(WorkFlowView flowView)
        {
            if (flowView != null)
            {
                CurrentWorkFlow.Views.Remove(flowView);
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
            CurrentWorkFlow.Variables.Items.Add(new Variable()
                {
                    Name = GetNewName("Variable#", CurrentWorkFlow.Variables.Items.Select(x => x.Name).ToList())
                }
            );
        }

        private void Load()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Package xml file (*.cwxml)|*.xml|All files (*.*)|*.*";
            if (dialog.ShowDialog() != true) return;
            LoadXml(dialog.FileName);
        }

        public void LoadXml(string file)
        {
            try
            {
                CurrentWorkFlow = WorkflowManager.Instance.Load(file);
                if (CurrentWorkFlow.Views.Count > 0)
                {
                    SelectedView = CurrentWorkFlow.Views[0];
                    if (SelectedView.Elements.Count > 0)
                        SelectedElement = SelectedView.Elements[0];
                }
                if (CurrentWorkFlow.Variables.Items.Count > 0)
                    SelectedVariable = CurrentWorkFlow.Variables.Items[0];
                _file = file;
            }
            catch (Exception e)
            {
                Log.Debug("Unable to open file", e);
            }

        }

        private void Save()
        {
            try
            {
                if (File.Exists(_file))
                    WorkflowManager.Instance.Save(CurrentWorkFlow, _file);
                else
                {
                    SaveAs();
                }
            }
            catch (Exception e)
            {
                Log.Debug("Unable to save file", e);
            }
        }

        private void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = _file;
            dialog.Filter = "Package xml file (*.cwxml)|*.xml|All files (*.*)|*.*";
            if (dialog.ShowDialog() != true) return;
            try
            {
                WorkflowManager.Instance.Save(CurrentWorkFlow, dialog.FileName);
            }
            catch (Exception e)
            {
                Log.Debug("Unable to save file", e);
            }
        }

        private void SavePackage()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Package file (*.cwpkg)|*.cwpkg|All files (*.*)|*.*";
            if (dialog.ShowDialog() != true) return;
            try
            {
                WorkflowManager.Instance.SaveAsPsackage(CurrentWorkFlow, dialog.FileName);
            }
            catch (Exception e)
            {
                Log.Debug("Unable to save file", e);
            }
        }

        private void PreviewView()
        {
            var wnd = new ViewPreviewView();
            wnd.ContentControl.Content = SelectedView.Instance.GetPreview(SelectedView,new Context());
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
                element.Name = GetNewName(pluginInfo.Name, SelectedView.Elements.Select(x => x.Name).ToList()); 
                element.Parent = SelectedView;
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
            view.Name = GetNewName(pluginInfo.Name, CurrentWorkFlow.Views.Select(x => x.Name).ToList());
            view.Parent = CurrentWorkFlow;
            CurrentWorkFlow.Views.Add(view);
            SelectedView = view;
        }

        private string GetNewName(string baseName, IList<string> names)
        {
            int i = 1;
            while (true)
            {
                var newName = baseName + " #" + i;
                if (!names.Contains(newName))
                    return newName;
                i++;
            }
        }


    }
}

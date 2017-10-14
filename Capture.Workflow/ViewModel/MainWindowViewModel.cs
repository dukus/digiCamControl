using System;
using System.IO;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Capture.Workflow.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private AsyncObservableCollection<WorkFlowItem> _workFlows;
        public RelayCommand NewCommand { get; set; }
        public RelayCommand<WorkFlowItem> RunCommand { get; set; }
        public RelayCommand<WorkFlowItem> EditCommand { get; set; }

        public AsyncObservableCollection<WorkFlowItem> WorkFlows
        {
            get { return _workFlows; }
            set
            {
                _workFlows = value;
                RaisePropertyChanged(()=>WorkFlows);
            }
        }


        public MainWindowViewModel()
        {
            EditCommand = new RelayCommand<WorkFlowItem>(Edit);
            RunCommand=new RelayCommand<WorkFlowItem>(Run);
            if (!IsInDesignMode)
            {
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                ServiceProvider.Instance.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
                ServiceProvider.Instance.DeviceManager.AddFakeCamera();
                ServiceProvider.Instance.DeviceManager.ConnectToCamera();
                LoadWorkFlows();
            }
        }

        private void Run(WorkFlowItem obj)
        {
            WorkflowManager.Instance.Context.WorkFlow = obj.IsPackage ? WorkflowManager.Instance.LoadFromPackage(obj.File) : WorkflowManager.Instance.Load(obj.File);
            WorkflowViewView wnd = new WorkflowViewView();
            wnd.ShowDialog();
        }

        private void LoadWorkFlows()
        {
            WorkFlows = new AsyncObservableCollection<WorkFlowItem>();
            try
            {
                if (!Directory.Exists(Settings.Instance.WorkflowFolder))
                    Directory.CreateDirectory(Settings.Instance.WorkflowFolder);
            }
            catch (Exception e)
            {
                Log.Error("Unable to create workflow folder");
            }

            var files = Directory.GetFiles(Settings.Instance.WorkflowFolder, "*.cwpkg");
            foreach (var file in files)
            {
                try
                {
                    WorkFlowItem item = new WorkFlowItem()
                    {
                        Workflow = WorkflowManager.Instance.LoadFromPackage(file),
                        IsEditable = false,
                        IsPackage = true,
                        File = file
                    };
                    WorkFlows.Add(item);
                }
                catch (Exception e)
                {
                    Log.Debug("Unable to load package" + file, e);
                }
            }

            files = Directory.GetFiles(Settings.Instance.WorkflowFolder, "*.cwxml");
            foreach (var file in files)
            {
                try
                {
                    WorkFlowItem item = new WorkFlowItem()
                    {
                        Workflow = WorkflowManager.Instance.Load(file),
                        IsEditable = true,
                        IsPackage = false,
                        File = file

                    };
                    WorkFlows.Add(item);
                }
                catch (Exception e)
                {
                    Log.Debug("Unable to load package" + file, e);
                }
            }
        }

        private void DeviceManager_CameraDisconnected(CameraControl.Devices.ICameraDevice cameraDevice)
        {
         
        }

        private void DeviceManager_CameraConnected(CameraControl.Devices.ICameraDevice cameraDevice)
        {
         
        }

        private void Edit(WorkFlowItem item)
        {
            WorkflowEditorView wnd = new WorkflowEditorView();
            ((WorkflowEditorViewModel)wnd.DataContext).LoadXml(item.File);
            wnd.ShowDialog();
            LoadWorkFlows();
        }
    }
}

using System;
using System.IO;
using System.Windows;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Scripting;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Capture.Workflow.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private AsyncObservableCollection<WorkFlowItem> _workFlows;
        public RelayCommand NewCommand { get; set; }
        public RelayCommand LocateLogCommand { get; set; }
        public RelayCommand ClearQueueCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand ForumCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        public RelayCommand<WorkFlowItem> RunCommand { get; set; }
        public RelayCommand<WorkFlowItem> EditCommand { get; set; }
        public RelayCommand<WorkFlowItem> RevertCommand { get; set; }
        public RelayCommand<WorkFlowItem> CopyCommand { get; set; }

        public AsyncObservableCollection<WorkFlowItem> WorkFlows
        {
            get { return _workFlows; }
            set
            {
                _workFlows = value;
                RaisePropertyChanged(()=>WorkFlows);
            }
        }

        public QueueManager QueueManager => QueueManager.Instance;

        public MainWindowViewModel()
        {
            EditCommand = new RelayCommand<WorkFlowItem>(Edit);
            RunCommand = new RelayCommand<WorkFlowItem>(Run);
            NewCommand = new RelayCommand(New);
            RevertCommand = new RelayCommand<WorkFlowItem>(Revert);
            CopyCommand = new RelayCommand<WorkFlowItem>(Copy);
            LocateLogCommand = new RelayCommand(LocateLog);
            ClearQueueCommand = new RelayCommand(ClearQueue);
            SettingsCommand = new RelayCommand(ShowSettings);
            ForumCommand =
                new RelayCommand(() => Utils.Run("http://digicamcontrol.com/phpbb/viewtopic.php?f=6&p=5483"));
            HelpCommand = new RelayCommand(() => Utils.Run("http://digicamcontrol.com/doc/workflow"));
            if (!IsInDesignMode)
            {
                // copy default workflows from install folder
                CopyDefaultWorkFlows();
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                ServiceProvider.Instance.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
                ServiceProvider.Instance.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
                LoadWorkFlows();
            }
        }

        private void ShowSettings()
        {
            SettingsView view = new SettingsView();
            view.ShowDialog();
        }

        private void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            MessageBox.Show("Photo capture detected !\nFirst please run a workflow!");
            Log.Debug("Wrong time capture detected");
        }

        private void ClearQueue()
        {
            if (MessageBox.Show("All not executed queued actions will be cleared !!!\nContinue ?", "Warning",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                WorkflowManager.Instance.Database.Clear();
            }
            catch (Exception e)
            {
                Log.Error("Unable to clear queue database ", e);
            }
        }

        private void LocateLog()
        {
            Utils.Run(Settings.Instance.LogFolder);
        }

        private void Copy(WorkFlowItem obj)
        {
            var workflow = obj.Workflow;
            workflow.Name = workflow.Name+" Copy";
            workflow.Id = Guid.NewGuid().ToString();
            WorkflowManager.Instance.Save(workflow,
                Path.Combine(Settings.Instance.WorkflowFolder, workflow.Id + ".cwxml"));
            LoadWorkFlows();
        }

        private void Revert(WorkFlowItem obj)
        {
            if(!obj.IsRevertable)
                return;
            if (MessageBox.Show("If you made any changes, all changes will be reverted !!!\nContinue ?", "Warning",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                var file = Path.GetFileName(obj.File);
                File.Copy(Path.Combine(Settings.Instance.DefaultWorkflowFolder, file), obj.File, true);
            }
            catch (Exception e)
            {
                Log.Debug("Unable to revert", e);
                MessageBox.Show("Unable to revert workflow.");
            }
            LoadWorkFlows();
        }

        private void CopyDefaultWorkFlows()
        {
            try
            {
                Utils.CopyFiles(Settings.Instance.DefaultWorkflowFolder, Settings.Instance.WorkflowFolder);
                var folders = Directory.GetFiles(Settings.Instance.DefaultWorkflowFolder);
                foreach (string folder in folders)
                {
                    Utils.CopyFiles(folder, Path.Combine(Settings.Instance.WorkflowFolder, Path.GetFileName(folder)));
                }
            }
            catch (Exception e)
            {
                Log.Error("Unable to copy default workflows");
            }
        }

        private void Run(WorkFlowItem obj)
        {
            var workFlow = obj.IsPackage
                ? WorkflowManager.Instance.LoadFromPackage(obj.File)
                : WorkflowManager.Instance.Load(obj.File);

            if (File.Exists(Path.Combine(Settings.Instance.DefaultWorkflowFolder,Path.GetFileName( obj.File))))
            {
                var defFile = Path.Combine(Settings.Instance.DefaultWorkflowFolder, Path.GetFileName(obj.File));
                var defWorkflow= obj.IsPackage
                    ? WorkflowManager.Instance.LoadFromPackage(defFile)
                    : WorkflowManager.Instance.Load(defFile);

                if (defWorkflow.GetVersion() > workFlow.GetVersion())
                {
                    if (MessageBox.Show("New version is available do  you want update the workflow ?", "Warning",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Revert(obj);
                        workFlow = obj.IsPackage
                            ? WorkflowManager.Instance.LoadFromPackage(obj.File)
                            : WorkflowManager.Instance.Load(obj.File);
                    }
                }
            }

            if (workFlow.Views.Count == 0)
            {
                MessageBox.Show("No view(s) are defined !");
                return;
            }
            ServiceProvider.Instance.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;
            WorkflowManager.Instance.Context.WorkFlow = workFlow;
            WorkflowViewView wnd = new WorkflowViewView();
            wnd.ShowDialog();
            WorkflowManager.Instance.Context.Clear();
            ServiceProvider.Instance.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
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
                var fileName = Path.GetFileName(file);
                try
                {
                    WorkFlowItem item = new WorkFlowItem()
                    {
                        Workflow = WorkflowManager.Instance.Load(file),
                        IsEditable = true,
                        IsPackage = false,
                        File = file,
                        IsRevertable = File.Exists(Path.Combine(Settings.Instance.DefaultWorkflowFolder, fileName))
                    };
                    WorkFlows.Add(item);
                }
                catch (Exception e)
                {
                    Log.Debug("Unable to load package" + file, e);
                }
            }
        }

        private void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
        {
            GoogleAnalytics.Instance.TrackEvent("Camera", "Disconnected", cameraDevice.DeviceName);
        }

        private void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            GoogleAnalytics.Instance.TrackEvent("Camera", "Connected", cameraDevice.DeviceName);
        }

        private void Edit(WorkFlowItem item)
        {
            WorkflowEditorView wnd = new WorkflowEditorView();
            ((WorkflowEditorViewModel)wnd.DataContext).LoadXml(item.File);
            wnd.ShowDialog();
            LoadWorkFlows();
        }

        private void New()
        {
            var workflow = new WorkFlow();
            workflow.Name = "New Workflow";
            WorkflowManager.Instance.Save(workflow,
                Path.Combine(Settings.Instance.WorkflowFolder, workflow.Id + ".cwxml"));
            LoadWorkFlows();

        }
    }
}

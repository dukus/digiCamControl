using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Capture")]
    public class Capture:IWorkflowCommand
    {
        public string Name { get; set; }
        public WorkFlowCommand CreateCommand()
        {
            return new WorkFlowCommand();
        }

        public bool Execute(WorkFlowCommand command)
        {
            CaptureAsync();
            return true;
        }


        private void CaptureAsync()
        {
            Task.Factory.StartNew(CaptureCamera);
        }


        private void CaptureCamera()
        {
            try
            {
                WorkflowManager.Instance.Context.CameraDevice.CapturePhoto();
                Log.Debug("LiveView: Capture Initialization Done");
            }
            catch (Exception exception)
            {
                //Message = exception.Message;
                Log.Error("Unable to take picture ", exception);
            }
        }
    }
}

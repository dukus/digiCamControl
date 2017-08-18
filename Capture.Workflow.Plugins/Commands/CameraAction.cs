using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Devices;
using CameraControl.Devices.Canon;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("CameraAction")]
    public class CameraAction : IWorkflowCommand
    {
        public string Name { get; set; }

        public WorkFlowCommand CreateCommand()
        {
            var command = new WorkFlowCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Action",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>() {"Capture", "StartLiveView", "StopLiveView"}
            });

            command.Properties.Add(new CustomProperty()
            {
                Name = "Param1",
                PropertyType = CustomPropertyType.String
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "Param2",
                PropertyType = CustomPropertyType.String
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            try
            {

                switch (command.Properties["Action"].Value)
                {
                    case "Capture":
                        CaptureAsync();
                        break;
                    case "StartLiveView":
                        StartLiveView();
                        break;
                    case "StopLiveView":
                        StopLiveView();
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Debug("Error execute command", ex);
                return false;
            }
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

        public void StartLiveView()
        {

            string resp = WorkflowManager.Instance.Context.CameraDevice.GetProhibitionCondition(OperationEnum.LiveView);
            if (string.IsNullOrEmpty(resp))
            {
                Task.Factory.StartNew(StartLiveViewThread);
            }
            else
            {
                Log.Error("Error starting live view " + resp);
                StaticHelper.Instance.SystemMessage = "Error starting live view " + resp;
            }
        }

        private void StartLiveViewThread()
        {
            try
            {
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview started");
                do
                {
                    try
                    {
                        var cam = WorkflowManager.Instance.Context.CameraDevice as CanonSDKBase;
                        if (cam != null)
                        {
                            cam.StartLiveViewCamera();
                        }
                        else
                        {
                            WorkflowManager.Instance.Context.CameraDevice.StartLiveView();
                        }

                    }
                    catch (DeviceException deviceException)
                    {
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(100);
                            Log.Debug("Retry live view :" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                } while (retry && retryNum < 35);

                Log.Debug("LiveView: Liveview start done");
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.StartLiveView, null));
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to start liveview ! " + exception.Message;
            }
        }

        private void StopLiveView()
        {
            Task.Factory.StartNew(StopLiveViewThread);
        }

        private void StopLiveViewThread()
        {
            try
            {
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview stopping");
                do
                {
                    try
                    {
                        WorkflowManager.Instance.Context.CameraDevice.StopLiveView();
                    }
                    catch (DeviceException deviceException)
                    {
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(500);
                            Log.Debug("Retry live view stop:" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                } while (retry && retryNum < 35);
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.StopLiveView, null));
            }
            catch (Exception exception)
            {
                Log.Error("Unable to stop liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to stop liveview ! " + exception.Message;
            }
        }
    }
}

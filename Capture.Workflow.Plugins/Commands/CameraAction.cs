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
    [Icon("CameraEnhance")]
    public class CameraAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Action",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>() { "Capture", "CaptureNoAf", "StartLiveView", "StopLiveView", "Autofocus", "CaptureToPc", "CaptureToCard","EnableCapture", "DisableCapture" }
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
                if (!CheckCondition(command, context))
                    return true;

                switch (command.Properties["Action"].Value)
                {
                    case "Capture":
                        CaptureAsync(true);
                        break;
                    case "CaptureNoAf":
                        CaptureAsync(false);
                        break;
                    case "StartLiveView":
                        StartLiveView();
                        break;
                    case "StopLiveView":
                        StopLiveView();
                        break;
                    case "Autofocus":
                        context.CameraDevice.AutoFocus();
                        break;
                    case "CaptureToPc":
                        try
                        {
                            context.CameraDevice.CaptureInSdRam = true;
                        }
                        catch (Exception e)
                        {
                            Log.Debug("Unable to set capture destination PC",e);
                        }
                        break;
                    case "CaptureToCard":
                        try
                        {
                            context.CameraDevice.CaptureInSdRam = false;
                        }
                        catch (Exception e)
                        {
                            Log.Debug("Unable to set capture destination CARD", e);
                        }
                        break;
                    case "EnableCapture":
                        context.CaptureEnabled = true;
                        break;
                    case "DisableCapture":
                        context.CaptureEnabled = false;
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


        private void CaptureAsync(bool autoFocus)
        {
            Task.Factory.StartNew(()=>CaptureCamera(autoFocus));
        }

        private void CaptureCamera(bool autoFocus)
        {
            try
            {
                if (autoFocus)
                    WorkflowManager.Instance.Context.CameraDevice.CapturePhoto();
                else
                    WorkflowManager.Instance.Context.CameraDevice.CapturePhotoNoAf();
                Log.Debug("Camera Action: Capture Initialization Done");
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

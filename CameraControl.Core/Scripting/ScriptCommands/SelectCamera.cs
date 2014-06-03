using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class SelectCamera : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            try
            {
                int cameranum = 0;
                if (int.TryParse(scriptObject.ParseString(LoadedParams["cameranum"]), out cameranum))
                {
                    if (cameranum > -1 && cameranum < ServiceProvider.DeviceManager.ConnectedDevices.Count)
                    {
                        ServiceProvider.DeviceManager.SelectedCameraDevice =
                            ServiceProvider.DeviceManager.ConnectedDevices[cameranum];
                        scriptObject.CameraDevice = ServiceProvider.DeviceManager.ConnectedDevices[cameranum];
                    }
                    else
                    {
                        ServiceProvider.ScriptManager.OutPut("Camera with number " + cameranum + "not exist");
                    }
                }
                else
                {
                    ServiceProvider.ScriptManager.OutPut("Wrong camera number");
                }
            }
            catch (Exception exception)
            {
                ServiceProvider.ScriptManager.OutPut("Exception on select camera " + exception.Message);
            }
            return true;
        }

        public SelectCamera()
        {
            Name = "selectcamera";
            Description = "If multiple cameras are connected select a camera identified by a number";
            DefaultValue = "selectcamera cameranum=\"1\"";
        }
    }
}

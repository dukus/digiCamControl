using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Capture:BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            try
            {
                ServiceProvider.ScriptManager.OutPut("Capture started");
                var thread = new Thread(() => CaptureAsync(scriptObject));
                thread.Start();
                Thread.Sleep(100);
            }
            catch (Exception exception)
            {
                ServiceProvider.ScriptManager.OutPut("Capture error " + exception.Message);
                Log.Debug("Script capture error", exception);
            }
            return true;
        }

        private void CaptureAsync(ScriptObject scriptObject)
        {
            try
            {
                scriptObject.CameraDevice.IsBusy = true;
                CameraHelper.Capture(scriptObject.CameraDevice);
            }
            catch (Exception exception)
            {
                scriptObject.CameraDevice.IsBusy = false;
                ServiceProvider.ScriptManager.OutPut("Capture error " + exception.Message);
                Log.Debug("Script capture error", exception);
            }
        }

        public Capture()
        {
            Name = "capture";
            Description = "Trigger capture command on camera";
            DefaultValue = "capture";
        }
    }
}

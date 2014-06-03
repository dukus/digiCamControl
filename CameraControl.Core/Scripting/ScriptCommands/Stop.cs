using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Stop:BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            ServiceProvider.ScriptManager.Stop();
            return true;
        }

        public Stop()
        {
            Name = "stop";
            Description = "stop script execution";
            DefaultValue = "stop";
        }
    }
}

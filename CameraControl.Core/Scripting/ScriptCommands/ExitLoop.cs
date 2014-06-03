using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class ExitLoop:BaseScript
    {

        public override bool Execute(ScriptObject scriptObject)
        {
            scriptObject.ExitLoop = true;
            return true;
        }

        public ExitLoop()
        {
            Name = "exitloop";
            Description = "exit from current loop";
            DefaultValue = "exitloop";
        }
    }
}

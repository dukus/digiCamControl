using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Echo : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            ServiceProvider.ScriptManager.OutPut(scriptObject.ParseString(LoadedParams["text"]));
            return true;
        }

        public Echo()
        {
            Name = "echo";
            DefaultValue = "echo text=\"message\"";
            Description = "Send a text message to output window";
        }
    }
}

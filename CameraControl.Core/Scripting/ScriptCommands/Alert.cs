using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Alert : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            MessageBox.Show(scriptObject.ParseString(LoadedParams["text"]),"DCC Script");
            return true;
        }

        public Alert()
        {
            Name = "alert";
            DefaultValue = "alert text=\"message\"";
            Description = "Show a message box with message";
        }
    }
}

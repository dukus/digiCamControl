using System.Windows.Controls;
using System.Xml;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Scripting
{
    public interface IScriptCommand
    {
        bool Execute(ScriptObject scriptObject);
        IScriptCommand Create();
        XmlNode Save(XmlDocument doc);
        IScriptCommand Load(XmlNode node);
        bool IsExecuted { get; set; }
        bool Executing { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        string DefaultValue { get; set; }
        UserControl GetConfig();
        bool HaveEditControl { get; set; }

    }
}
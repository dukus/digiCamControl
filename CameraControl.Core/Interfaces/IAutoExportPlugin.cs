using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IAutoExportPlugin
    {
        bool Execute(string filename, AutoExportPluginConfig configData);
        bool Configure(AutoExportPluginConfig config);
        string Name { get;}
    }
}
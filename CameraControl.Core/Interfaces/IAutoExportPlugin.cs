using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IAutoExportPlugin
    {
        bool Execute(FileItem item, AutoExportPluginConfig configData);
        bool Configure(AutoExportPluginConfig config);
        string Name { get;}
    }
}
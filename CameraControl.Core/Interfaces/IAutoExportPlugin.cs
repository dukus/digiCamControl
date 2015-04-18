using System.Windows.Controls;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IAutoExportPlugin
    {
        bool Execute(FileItem item, AutoExportPluginConfig configData);
        string Name { get;}
        UserControl GetConfig(AutoExportPluginConfig configData);
    }
}
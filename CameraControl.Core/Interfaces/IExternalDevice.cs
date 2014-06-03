using System.Windows.Controls;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IExternalDevice
    {
        string Name { get; set; }
        bool Capture(CustomConfig config);
        bool Focus(CustomConfig config);
        bool CanExecute(CustomConfig config);
        UserControl GetConfig(CustomConfig config);
        SourceEnum DeviceType { get; set; }
        bool OpenShutter(CustomConfig config);
        bool CloseShutter(CustomConfig config);
        bool AssertFocus(CustomConfig config);
        bool DeassertFocus(CustomConfig config);
    }
}
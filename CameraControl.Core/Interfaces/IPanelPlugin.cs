using System.Windows.Controls;

namespace CameraControl.Core.Interfaces
{
    public interface IPanelPlugin
    {
        UserControl Control { get;}
        bool Visible { get; set; }
        string Id { get; }
    }
}
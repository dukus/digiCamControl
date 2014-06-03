namespace CameraControl.Core.Interfaces
{
  public interface IWindow
  {
    void ExecuteCommand(string cmd, object param);
    bool IsVisible { get; }
  }
}
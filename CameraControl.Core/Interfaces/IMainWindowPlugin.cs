namespace CameraControl.Core.Interfaces
{
  public interface IMainWindowPlugin
  {
    string DisplayName { get; set; }
    void Show();
  }
}
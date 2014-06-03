namespace CameraControl.Core.Interfaces
{
  public interface IToolPlugin
  {
    bool Execute();
    string Title { get; set; }
  }
}
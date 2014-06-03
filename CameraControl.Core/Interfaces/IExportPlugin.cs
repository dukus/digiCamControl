namespace CameraControl.Core.Interfaces
{
  public interface IExportPlugin
  {
    bool Execute();
    string Title { get; set; }
  }
}
using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
  public interface IQueueItem
  {
    bool Execute(QueueManager manager);
  }
}
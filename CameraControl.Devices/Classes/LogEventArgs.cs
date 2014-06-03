using System;

namespace CameraControl.Devices.Classes
{
  public class LogEventArgs : EventArgs
  {
    public object Message { get; set; }
    public Exception Exception { get; set; }
  }
}

using System;
using System.Collections.Generic;

namespace CameraControl.Core.Interfaces
{
  public interface IMenuAction
  {
    event EventHandler ProgressChanged;
    event EventHandler ActionDone;

    int Progress { get; set; }
    string Title { get; set; }
    bool IsBusy { get; set; }
    void Run(List<string> files);
    void Stop();
  }
}
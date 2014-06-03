using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Core.Classes
{
  public class QueueManager
  {
    public BlockingCollection<IQueueItem> Queue { get; set; }
    private readonly BackgroundWorker _worker;
    private object _locker = new object();


    public QueueManager()
    {
      Queue =new BlockingCollection<IQueueItem>();
      _worker = new BackgroundWorker();
      _worker.DoWork += _worker_DoWork;
    }

    void _worker_DoWork(object sender, DoWorkEventArgs e)
    {
      IQueueItem item;
      while (true)
      {
        item = Queue.Take();
        try
        {
          if (!item.Execute(this))
          {
            break;
          }
          //Thread.Sleep(10);
        }
        catch (Exception exception)
        {
          Log.Error("Queue manager error:", exception);
        }
      }
    }

    public void Clear()
    {
      while (Queue.Count > 0)
      {
        IQueueItem item;
        Queue.TryTake(out item);
      }
    }

    public void Add(IQueueItem item)
    {
      Queue.Add(item);
      if (!_worker.IsBusy)
        _worker.RunWorkerAsync();
    }

  }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace CameraControl.Devices.Classes
{
  public class AsyncObservableCollection<T> : ObservableCollection<T>
  {
    private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public AsyncObservableCollection()
    {
    }

    public AsyncObservableCollection(IEnumerable<T> list)
      : base(list)
    {
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (SynchronizationContext.Current == _synchronizationContext)
      {
        // Execute the CollectionChanged event on the current thread
              RaiseCollectionChanged(e);
      }
      else
      {
        // Post the CollectionChanged event on the creator thread
          if (_synchronizationContext != null)
              _synchronizationContext.Post(RaiseCollectionChanged, e);
      }
    }

    private void RaiseCollectionChanged(object param)
    {
      // We are in the creator thread, call the base implementation directly
      try
      {
        base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
      }
      catch (Exception)
      {
      }

    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (SynchronizationContext.Current == _synchronizationContext)
      {
        // Execute the PropertyChanged event on the current thread
        RaisePropertyChanged(e);
      }
      else
      {
        // Post the PropertyChanged event on the creator thread
          if (_synchronizationContext != null)
              _synchronizationContext.Post(RaisePropertyChanged, e);
      }
    }

    private void RaisePropertyChanged(object param)
    {
      // We are in the creator thread, call the base implementation directly
      base.OnPropertyChanged((PropertyChangedEventArgs)param);
    }
  }
}

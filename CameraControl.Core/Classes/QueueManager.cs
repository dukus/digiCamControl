#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

#endregion

namespace CameraControl.Core.Classes
{
    public class QueueManager
    {
        private BlockingCollection<IQueueItem> PriorityQueue { get; set; }
        private BlockingCollection<IQueueItem> Queue { get; set; }
        private BlockingCollection<IQueueItem> LowPriorityQueue { get; set; }
        private readonly BackgroundWorker _worker;
        private object _locker = new object();


        public QueueManager()
        {
            PriorityQueue = new BlockingCollection<IQueueItem>();
            Queue = new BlockingCollection<IQueueItem>();
            LowPriorityQueue = new BlockingCollection<IQueueItem>();
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                IQueueItem item;
                try
                {
                    if (PriorityQueue.Count > 0 && PriorityQueue.TryTake(out item))
                    {
                        item.Execute(this);
                        continue;
                    }
                    if (Queue.Count > 0 && Queue.TryTake(out item))
                    {
                        item.Execute(this);
                        continue;
                    }
                    if (LowPriorityQueue.Count > 0 && LowPriorityQueue.TryTake(out item))
                    {
                        item.Execute(this);
                    }
                    if (Queue.Count == 0 && PriorityQueue.Count == 0 && LowPriorityQueue.Count == 0)
                        Thread.Sleep(100);
                }
                catch (Exception exception)
                {
                    Log.Error("Queue manager error:", exception);
                }
            }
        }

        public void Clear()
        {
            while (PriorityQueue.Count > 0)
            {
                IQueueItem item;
                Queue.TryTake(out item);
            }
            while (Queue.Count > 0)
            {
                IQueueItem item;
                Queue.TryTake(out item);
            }
            while (LowPriorityQueue.Count > 0)
            {
                IQueueItem item;
                Queue.TryTake(out item);
            }
        }

        public void AddWithPriority(IQueueItem item)
        {
            PriorityQueue.Add(item);
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        public void AddWithLowPriority(IQueueItem item)
        {
            LowPriorityQueue.Add(item);
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }
        public void Add(IQueueItem item)
        {
            Queue.Add(item);
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }
    }
}
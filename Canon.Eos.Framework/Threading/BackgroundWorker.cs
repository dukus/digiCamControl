using System;
using System.Threading;

namespace Canon.Eos.Framework.Threading
{
    internal class BackgroundWorker
    {
        public void Work(Action callback)
        {
            var state = new State<bool>(false) { Callback = () => { callback(); return true; } };
            if (!ThreadPool.QueueUserWorkItem(BackgroundWorker.PerformUserWork<bool>, state))
                throw new ApplicationException("Unable to queue user work item.");
        }

        public TResult WorkAndWait<TResult>(Func<TResult> callback, int millisecondsWaitTimeout = Timeout.Infinite)
        {
            var state = new State<TResult> { Callback = callback };
            if (!ThreadPool.QueueUserWorkItem(BackgroundWorker.PerformUserWork<TResult>, state))
                throw new ApplicationException("Unable to queue user work item.");
            if (!state.WaitHandle.WaitOne(millisecondsWaitTimeout))
                throw new TimeoutException();
            return state.Result;
        }

        private static void PerformUserWork<T>(object workItem)
        {
            var state = (State<T>)workItem;
            state.Result = state.Callback();
            if(state.WaitHandle != null)
                state.WaitHandle.Set();
        }

        private class State<T>
        {
            public State(bool wait = true)
            {
                if(wait)
                    this.WaitHandle = new ManualResetEvent(false);
            }

            public T Result { get; set; }

            public Func<T> Callback { get; set; }

            public ManualResetEvent WaitHandle { get; private set; }            
                 
        }
    }
}

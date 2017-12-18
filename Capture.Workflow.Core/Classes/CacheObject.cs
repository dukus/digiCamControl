using System;

namespace Capture.Workflow.Core.Classes
{
    public class CacheObject
    {
        public string Id { get; set; }
        public object Object { get; set; }

        public void DisposeObject()
        {
            if (Object == null)
                return;

            var o = Object as IDisposable;
            o?.Dispose();
        }
    }
}

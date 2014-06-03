using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Eos.Framework.Eventing
{
    public class EosExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }
    }
}

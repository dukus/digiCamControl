using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Eos.Framework.Eventing
{
    public class EosPropertyEventArgs: EventArgs
    {
        public uint PropertyId { get; set; }
    }
}

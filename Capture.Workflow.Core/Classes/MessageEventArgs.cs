using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class MessageEventArgs
    {
        public string Name { get; set; }
        public object Param { get; set; }

        public MessageEventArgs(string name, object param)
        {
            Name = name;
            Param = param;
        }
    }
}

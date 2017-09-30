using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class MessageEventArgs
    {
        private Context _context;
        public string Name { get; set; }
        public object Param { get; set; }


        public Context Context
        {
            get
            {
                if (_context == null)
                    return WorkflowManager.Instance.Context;
                return _context;
            }
            set { _context = value; }
        }

        public MessageEventArgs(string name, object param)
        {
            Name = name;
            Param = param;
        }
    }
}

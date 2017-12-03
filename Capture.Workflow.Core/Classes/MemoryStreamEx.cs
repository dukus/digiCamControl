using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class MemoryStreamEx: MemoryStream
    {
        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

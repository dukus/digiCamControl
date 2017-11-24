using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Scripting
{
    class TestScript:IEvaluateScript
    {
        public string SessionName = "";

        public object Evaluate(Context context)
        {
            SessionName = (string)context.WorkFlow.Variables["SessionName"].GetAsObject();
            return "";
        }
    }
}

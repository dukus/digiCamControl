using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Scripting
{
    public interface IEvaluateScript
    {
        object Evaluate(Context context);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using CSScriptLibrary;

namespace Capture.Workflow.Core.Scripting
{
    public class ScriptEngine
    {
        private static ScriptEngine _instance;

        public static ScriptEngine Instance
        {
            get
            {
                if(_instance==null)
                    _instance=new ScriptEngine();
                return _instance;
            }
            set { _instance = value; }
        }


        public string ExecuteLine(string code, Context context)
        {
            string codeString =
                @"using System;
                                     public class Script
                                     {
                                        "+ GetVariables(context.WorkFlow) + "\n" + @"  
                                         public object Evaluate()
                                         {
                                            " +  "\n return " + code+";";
            codeString+= @"                 
                                         }
                                     }";
            dynamic script = CSScript.Evaluator
                .LoadCode(codeString);
            
            return script.Evaluate().ToString();
        }

        public bool Evaluate(string code, Context context)
        {
            string codeString =
                @"using System;
                                     public class Script
                                     {
                                        " + GetVariables(context.WorkFlow) + "\n" + @"  
                                         public bool Evaluate()
                                         {
                                            " + "\n return " + code + ";";
            codeString += @"                 
                                         }
                                     }";
            dynamic script = CSScript.Evaluator
                .LoadCode(codeString);

            return script.Evaluate();
        }


        public string GetVariables(WorkFlow workFlow)
        {
            string res="";
            foreach (var variable in workFlow.Variables.Items)
            {
                res += variable.GetAsCsString() + ";" + Environment.NewLine;

            }
            return res;
        }
    }
}

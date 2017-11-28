using System;
using System.Collections.Generic;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using CSScriptLibrary;

namespace Capture.Workflow.Core.Scripting
{
    public class ScriptEngine
    {
        public Dictionary<string,IEvaluateScript> EvaluateScriptsCache { get; set; }


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

        public ScriptEngine()
        {
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom;
            CSScript.EvaluatorConfig.DebugBuild = true;
            EvaluateScriptsCache = new Dictionary<string, IEvaluateScript>();
        }

        public void ClearCache()
        {
            EvaluateScriptsCache.Clear();
        }

        public string ExecuteLine(string code, Context context)
        {
            try
            {
                if (EvaluateScriptsCache.ContainsKey(code))
                    return EvaluateScriptsCache[code].Evaluate(context).ToString();

                string codeString =
                    @"using System;
                      using System.IO;
                      using CameraControl.Devices;
                      using Capture.Workflow.Core.Scripting;
                      using Capture.Workflow.Core.Classes;
                                     public class Script:IEvaluateScript
                                     {
                                        " + GetVariables(context.WorkFlow) + "\n" + @"  
                                         public object Evaluate(Context context)
                                         {
                                            " + GetVariablesAssign(context.WorkFlow, "context") +
                    "\n return " + code + ";\n";
                codeString += @"                 
                                         }
                                     }";
                IEvaluateScript script = (IEvaluateScript) CSScript.Evaluator
                    .LoadCode(codeString);
                EvaluateScriptsCache.Add(code, script);
                return script.Evaluate(context).ToString();
            }
            catch (Exception e)
            {
                Log.Debug("Invalid script ",e);
               
            }
            return "";
        }

        public bool Evaluate(string code, Context context)
        {
            var res = ExecuteLine(code, context);
            return res == "True";
        }


        private string GetVariables(WorkFlow workFlow)
        {
            string res="";
            foreach (var variable in workFlow.Variables.Items)
            {
                res += variable.GetAsCsString() + ";" + Environment.NewLine;

            }
            return res;
        }

        private string GetVariablesAssign(WorkFlow workFlow, string varPrefix)
        {
            string res = "";
            foreach (var variable in workFlow.Variables.Items)
            {
                res+=string.Format("{0} = ({1}){2}.WorkFlow.Variables[\"{0}\"].GetAsObject();\n",variable.Name,variable.GetCSType(),varPrefix);
            }
            return res;
        }
    }
}

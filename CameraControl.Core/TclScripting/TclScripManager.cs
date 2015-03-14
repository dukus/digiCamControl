using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace CameraControl.Core.TclScripting
{
    public class TclScripManager
    {
        public event OutputEventHandler Output;

        public TclScripManager()
        {
            
        }


        public int ExecuteFile(string file)
        {
            return 0;
        }

        public int Execute(string commands)
        {
            Result result = null;
            var c = new ConsoleRedirect();
            var old_out = Console.Out;
            Console.SetOut(c);
            c.Output += c_Output;
            using (Interpreter interpreter = Interpreter.Create(ref result))
            {

                ICommand command = new DccCommand(new CommandData(
                    "dcc", null, null, null, typeof(DccCommand).FullName,
                    CommandFlags.None, null, 0));

                ReturnCode code;
                long token = 0;

                code = interpreter.AddCommand(
                    command, null, ref token, ref result);

                code = interpreter.AddCommand(new EchoCommand(new CommandData(
                    "echo", null, null, null, typeof(EchoCommand).FullName,
                    CommandFlags.None, null, 0)), null, ref token, ref result);

                var lines = commands.Split('\n');
                if (code == ReturnCode.Ok)
                {
                    int errorLine = 0;
                    int linenr = 1;

                        code = interpreter.EvaluateScript(commands,
                            ref result, ref errorLine);
                        interpreter.Host.WriteResult(code, result, errorLine, true);

                    //foreach (string line in lines)
                    //{
                    //    code = interpreter.EvaluateScript(line,
                    //        ref result, ref errorLine);
                    //    interpreter.Host.WriteResult(code, result, linenr, true);
                    //    linenr++;
                    //}
                    return (int)interpreter.ExitCode;
                }
                else
                {
                    interpreter.Host.WriteResult(code, result, true);
                }
            }
            Console.SetOut(old_out);
            return 0;
        }

        void c_Output(string message, bool newline)
        {
            if (Output != null)
                Output(message, newline);
        }
    }
}

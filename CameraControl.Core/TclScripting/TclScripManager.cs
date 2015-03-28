using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace CameraControl.Core.TclScripting
{
    public class TclScripManager
    {
        public event OutputEventHandler Output;
        private Interpreter _interpreter = null;

        public TclScripManager()
        {
            
        }


        public int ExecuteFile(string file)
        {
            return 0;
        }

        public int Execute(string commands)
        {
            Task.Factory.StartNew(() => ExecuteThread(commands));
            return 0;
        }

        private int ExecuteThread(string commands)
        {
            Result result = null;
            var c = new ConsoleRedirect();
            var old_out = Console.Out;
            Console.SetOut(c);
            c.Output += c_Output;
            _interpreter = Interpreter.Create(ref result);
           

                ICommand command = new DccCommand(new CommandData(
                    "dcc", null, null, null, typeof(DccCommand).FullName,
                    CommandFlags.None, null, 0));

                ReturnCode code;
                long token = 0;

                code = _interpreter.AddCommand(
                    command, null, ref token, ref result);

                code = _interpreter.AddCommand(new EchoCommand(new CommandData(
                    "echo", null, null, null, typeof(EchoCommand).FullName,
                    CommandFlags.None, null, 0)), null, ref token, ref result);

                if (code == ReturnCode.Ok)
                {
                    int errorLine = 0;

                    code = _interpreter.EvaluateScript(commands,
                        ref result, ref errorLine);
                    _interpreter.Host.WriteResult(code, result, errorLine, true);
                    return (int) _interpreter.ExitCode;
                }
                else
                {
                    _interpreter.Host.WriteResult(code, result, true);
                }
            _interpreter.Dispose();
            _interpreter = null;
            Console.SetOut(old_out);
            return 0;
        }

        public void Stop()
        {
            Result result = null;
            if (_interpreter != null)
                _interpreter.CancelAnyEvaluate(true, true, ReturnCode.Ok, ref result);

        }

        void c_Output(string message, bool newline)
        {
            if (Output != null)
                Output(message, newline);
        }
    }
}

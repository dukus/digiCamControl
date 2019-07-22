using System;
using System.IO;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Interfaces.Public;

namespace CameraControl.Core.TclScripting
{
    public class TclScripManager : IDisposable
    {
        public event OutputEventHandler Output;
        public event OutputEventHandler Error;

        private Interpreter _interpreter = null;

        #region Public Constructors
        public TclScripManager()
        {
        }
        #endregion

        #region Private Interpreter Lifetime Management
        private ReturnCode Initialize(ref Result error)
        {
            if (_interpreter != null) return ReturnCode.Ok;

            Interpreter localInterpreter;
            Result result; /* REUSED */
            long token; /* REUSED */

            result = null;

            localInterpreter = Interpreter.Create(ref result);

            if (localInterpreter == null)
            {
                error = result;
                return ReturnCode.Error;
            }

            token = 0;
            result = null;

            if (localInterpreter.AddCommand(new DccCommand(new CommandData(
                    "dcc", null, null, null, typeof(DccCommand).FullName,
                    CommandFlags.None, null, 0)), null, ref token,
                    ref result) != ReturnCode.Ok)
            {
                localInterpreter.Dispose();

                error = result;
                return ReturnCode.Error;
            }

            token = 0;
            result = null;

            if (localInterpreter.AddCommand(new EchoCommand(new CommandData(
                    "echo", null, null, null, typeof(EchoCommand).FullName,
                    CommandFlags.None, null, 0)), null, ref token,
                    ref result) != ReturnCode.Ok)
            {
                localInterpreter.Dispose();

                error = result;
                return ReturnCode.Error;
            }

            _interpreter = localInterpreter;
            return ReturnCode.Ok;
        }
        #endregion

        public int ExecuteFile(string file)
        {
            CheckDisposed();

            string commands = null;
            Result error = null;

            if (Engine.ReadScriptFile(
                    _interpreter, file, ref commands,
                    ref error) == ReturnCode.Ok)
            {
                return Execute(commands);
            }
            else
            {
                c_Error(Utility.FormatResult(
                    ReturnCode.Error, error), true);

                return (int)ExitCode.Failure;
            }
        }

        public int Execute(string commands)
        {
            CheckDisposed();

            return Execute(commands, true);
        }

        public int Execute(string commands, bool asynchronous)
        {
            CheckDisposed();

            if (asynchronous)
            {
                Task.Factory.StartNew(() => ExecuteThread(commands));
                return 0;
            }
            else
            {
                return ExecuteThread(commands);
            }
        }

        #region Save / Restore Console Output
        private void BeginRedirectedConsoleOutput(
            out TextWriter savedOutput
            )
        {
            savedOutput = Console.Out;

            ConsoleRedirect redirector = new ConsoleRedirect(c_Output);
            Console.SetOut(redirector);
        }

        private void EndRedirectedConsoleOutput(
            ref TextWriter savedOutput
            )
        {
            ConsoleRedirect redirector = Console.Out as ConsoleRedirect;
            Console.SetOut(savedOutput);

            if (redirector != null)
            {
                redirector.Dispose();
                redirector = null;
            }

            savedOutput = null;
        }
        #endregion

        #region Private Script Evaluation Helpers
        private bool HostWriteResult(
            ReturnCode code,
            Result result,
            int errorLine,
            bool newLine
            )
        {
            if (_interpreter == null)
            {
                c_Error(Utility.FormatResult(
                    code, result, errorLine), newLine);

                return false;
            }

            IDebugHost host = _interpreter.Host;

            if (host == null)
            {
                c_Error(Utility.FormatResult(
                    code, result, errorLine), newLine);

                return false;
            }

            return host.WriteResult(
                code, result, errorLine, newLine);
        }

        private ExitCode EvaluateScript(
            string commands
            )
        {
            if (_interpreter == null)
            {
                c_Error(Utility.FormatResult(ReturnCode.Error,
                    "cannot evaluate commands, no interpreter"), true);

                return ExitCode.Failure;
            }

            ReturnCode code;
            Result result = null;
            int errorLine = 0;

            code = _interpreter.EvaluateScript(
                commands, ref result, ref errorLine);

            HostWriteResult(code, result, errorLine, true);

            return _interpreter.ExitCode;
        }

        private int ExecuteThread(string commands)
        {
            TextWriter savedOutput;

            BeginRedirectedConsoleOutput(out savedOutput);

            try
            {
                ExitCode exitCode;
                ReturnCode code;
                Result result = null;

                code = Initialize(ref result);

                if (code == ReturnCode.Ok)
                {
                    exitCode = EvaluateScript(commands);

                    if (exitCode != ExitCode.Success)
                    {
                        c_Error(Utility.FormatResult(
                            ReturnCode.Error, result), true);
                    }
                }
                else
                {
                    c_Error(Utility.FormatResult(
                        code, result), true);

                    exitCode = ExitCode.Failure;
                }

                return (int)exitCode;
            }
            finally
            {
                EndRedirectedConsoleOutput(ref savedOutput);
            }
        }
        #endregion

        public void Stop()
        {
            CheckDisposed();

            if (_interpreter != null)
            {
                Result result = null;

                if (_interpreter.CancelAnyEvaluate(
                        null, CancelFlags.UnwindAndNotify,
                        ref result) != ReturnCode.Ok)
                {
                    c_Error(Utility.FormatResult(
                        ReturnCode.Error, result), true);
                }
            }
        }

        #region Event Wrappers
        private void c_Output(string message, bool newline)
        {
            if (Output != null)
                Output(message, newline);
        }

        private void c_Error(string message, bool newline)
        {
            if (Error != null)
                Error(message, newline);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IDisposable "Pattern" Members
        private bool disposed;
        private void CheckDisposed() /* throw */
        {
            if (disposed && Engine.IsThrowOnDisposed(_interpreter, false))
            {
                throw new ObjectDisposedException(
                    typeof(TclScripManager).Name);
            }
        }

        protected virtual void Dispose(
            bool disposing
            )
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ////////////////////////////////////
                    // dispose managed resources here...
                    ////////////////////////////////////

                    _interpreter.Dispose();
                    _interpreter = null;
                }

                //////////////////////////////////////
                // release unmanaged resources here...
                //////////////////////////////////////

                disposed = true;
            }
        }
        #endregion

        #region Destructor
        ~TclScripManager()
        {
            Dispose(false);
        }
        #endregion
    }
}

using Eagle._Components.Public;
using Eagle._Interfaces.Public;
using System;
using System.Threading.Tasks;

namespace CameraControl.Core.TclScripting
{
	public class TclScriptManager : IDisposable
	{
		public event OutputEventHandler Output;
		public event OutputEventHandler Error;
		public event EventHandler IsBusyChanged;


		private Interpreter _interpreter = null;
		private DccCommand _dccCommand = null;

		#region Public Constructors
		public TclScriptManager ()
		{
		}
		#endregion

		#region Private Interpreter Lifetime Management
		private ReturnCode Initialize (ref Result error)
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

			if (localInterpreter.AddCommand(
					_dccCommand = new DccCommand(
						new CommandData("dcc", null, null, null, typeof(DccCommand).FullName, CommandFlags.None, null, 0),
						() =>
						{
							Result isCancelled = null;
							return localInterpreter.IsCanceled(CancelFlags.NoLock | CancelFlags.Global, ref isCancelled) == ReturnCode.Error;
						}
					),
					null,
					ref token,
					ref result) != ReturnCode.Ok)
			{
				localInterpreter.Dispose();

				error = result;
				return ReturnCode.Error;
			}

			token = 0;
			result = null;

			if (localInterpreter.AddCommand(new EchoCommand(
				new CommandData(
						"echo", null, null, null, typeof(EchoCommand).FullName,
						CommandFlags.None, null, 0
					), (msg) => c_Output(msg, true)),
					null, ref token, ref result) != ReturnCode.Ok)
			{
				localInterpreter.Dispose();

				error = result;
				return ReturnCode.Error;
			}

			_interpreter = localInterpreter;
			return ReturnCode.Ok;
		}
		#endregion

		private bool _isBusy;

		public bool IsBusy
		{
			get => _isBusy;
			private set
			{
				if (_isBusy != value)
				{
					_isBusy = value;
					IsBusyChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public int ExecuteFile (string file)
		{
			if (IsBusy)
			{
				return (int)ExitCode.Failure;
			}
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

		public int Execute (string commands)
		{
			if (IsBusy)
			{
				return (int)ExitCode.Failure;
			}
			CheckDisposed();

			return Execute(commands, true);
		}

		public int Execute (string commands, bool asynchronous)
		{
			if (IsBusy)
			{
				return (int)ExitCode.Failure;
			}
			CheckDisposed();

			IsBusy = true;
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

		#region Private Script Evaluation Helpers
		private bool HostWriteResult (
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

		private ExitCode EvaluateScript (
			string commands
			)
		{
			if (_interpreter == null)
			{
				c_Error(Utility.FormatResult(ReturnCode.Error,
					"cannot evaluate commands, no interpreter"), true);

				return ExitCode.Failure;
			}

			try
			{
				ReturnCode code;
				Result result = null;
				int errorLine = 0;

				code = _interpreter.EvaluateScript(
					commands, ref result, ref errorLine);

				HostWriteResult(code, result, errorLine, true);

				return _interpreter.ExitCode;
			}
			finally
			{
				_dccCommand.ScriptExecutionCompleted();
			}
		}

		private int ExecuteThread (string commands)
		{
			var redirected = ThreadConsoleRedirect.Start(c_Output);

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
				ThreadConsoleRedirect.Stop(redirected);
				IsBusy = false;
			}
		}
		#endregion

		public void Stop ()
		{
			if (!IsBusy)
			{
				return;
			}
			CheckDisposed();

			if (_interpreter != null)
			{
				c_Output("About to stop script execution", false);
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
		private void c_Output (string message, bool newline)
		{
			if (Output != null)
				Output(message, newline);
		}

		private void c_Error (string message, bool newline)
		{
			if (Error != null)
				Error(message, newline);
		}
		#endregion

		#region IDisposable Members
		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region IDisposable "Pattern" Members
		private bool disposed;
		private void CheckDisposed () /* throw */
		{
			if (disposed && Engine.IsThrowOnDisposed(_interpreter, false))
			{
				throw new ObjectDisposedException(
					typeof(TclScriptManager).Name);
			}
		}

		protected virtual void Dispose (
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
		~TclScriptManager ()
		{
			Dispose(false);
		}
		#endregion
	}
}

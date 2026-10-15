using CameraControl.Core.Scripting;
using CameraControl.Devices;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using System;
using System.Collections;
using System.Linq;

namespace CameraControl.Core.TclScripting
{
	public class DccCommand : Eagle._Commands.Default
	{
		public DccCommand (ICommandData commandData, Func<bool> isCancellationRequested) : base(commandData)
		{
			this.Flags |= Utility.GetCommandFlags(GetType().BaseType) |
						  Utility.GetCommandFlags(this);
			_processor = new CommandLineProcessor(isCancellationRequested: isCancellationRequested);
		}

		private CommandLineProcessor _processor;

		public void ScriptExecutionCompleted ()
		{
			_processor.ScriptExecutionCompleted();
		}

		public override ReturnCode Execute (
			Interpreter interpreter,
			IClientData clientData,
			ArgumentList arguments,
			ref Result result
			)
		{
			if ((arguments == null) || (arguments.Count < 2))
			{
				result = Utility.WrongNumberOfArguments(
					this, 1, arguments, "command");

				return ReturnCode.Error;
			}

			try
			{
				var o = _processor.Pharse(arguments.Select(argument => (string)argument).Skip(1).ToArray());
				if (!(o is string) && o is IEnumerable)
					result = new StringList(o);
				else
				{
					result = o == null ? "" : new Variant(o).ToString();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Script error ", exception);
				result = "Error on command execution " + exception.Message;
				return ReturnCode.Error;
			}
			return ReturnCode.Ok;
		}
	}
}

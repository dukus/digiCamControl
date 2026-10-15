using CameraControl.Devices;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using System;
using System.Linq;

namespace CameraControl.Core.TclScripting
{
	public class EchoCommand : Eagle._Commands.Default
	{
		public EchoCommand (ICommandData commandData, Action<string> writeOutput)
			: base(commandData)
		{
			this.Flags |= Utility.GetCommandFlags(GetType().BaseType) |
						  Utility.GetCommandFlags(this);
			_writeOutput = writeOutput;
		}
		private Action<string> _writeOutput;


		public override ReturnCode Execute (
			Interpreter interpreter,
			IClientData clientData,
			ArgumentList arguments,
			ref Result result
			)
		{
			if ((arguments == null) || (arguments.Count == 0))
			{
				result = Utility.WrongNumberOfArguments(
					this, 1, arguments, "message");

				return ReturnCode.Error;
			}

			try
			{
				_writeOutput(string.Join(" ", arguments.Skip(1)));
				result = "";
			}
			catch (Exception exception)
			{
				Log.Error("Script error ", exception);
				result = "Error on command execution " + exception.Message;
			}
			return ReturnCode.Ok;
		}
	}
}

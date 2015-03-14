using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace CameraControl.Core.TclScripting
{
    public class EchoCommand : Eagle._Commands.Default
    {
        public EchoCommand(ICommandData commandData)
            : base(commandData)
        {
            this.Flags |= Utility.GetCommandFlags(GetType().BaseType) |
                          Utility.GetCommandFlags(this);
        }

        public override ReturnCode Execute(
            Interpreter interpreter,
            IClientData clientData,
            ArgumentList arguments,
            ref Result result
            )
        {
            if ((arguments == null) || (arguments.Count != 2))
            {
                result = Utility.WrongNumberOfArguments(
                    this, 1, arguments, "message");

                return ReturnCode.Error;
            }

            try
            {
                interpreter.Host.WriteResult(ReturnCode.Ok, arguments[1], true);
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

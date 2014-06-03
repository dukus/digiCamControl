#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Loop : BaseScript
    {
        private AsyncObservableCollection<IScriptCommand> _commands;

        public AsyncObservableCollection<IScriptCommand> Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                NotifyPropertyChanged("Commands");
            }
        }

        public override IScriptCommand Load(XmlNode loadnode)
        {
            base.Load(loadnode);
            foreach (XmlNode node in loadnode.ChildNodes)
            {
                foreach (var command in ServiceProvider.ScriptManager.AvaiableCommands)
                {
                    if (command.Name.ToLower() == node.Name.ToLower())
                        Commands.Add(((IScriptCommand) Activator.CreateInstance(command.GetType())).Load(node));
                }
            }
            return this;
        }

        public override bool Execute(ScriptObject scriptObject)
        {
            int loopcount;
            if (!int.TryParse(scriptObject.ParseString(LoadedParams["loopcount"]), out loopcount))
                loopcount = int.MaxValue;
            for (int i = 0; i < loopcount; i++)
            {
                scriptObject.Variabiles["loopno"] = i.ToString();
                if (ServiceProvider.ScriptManager.ShouldStop)
                    break;
                scriptObject.ExecuteCommands(Commands);
                if (scriptObject.ExitLoop)
                    break;
                // prevent CPU overloading
                Thread.Sleep(200);
            }
            scriptObject.ExitLoop = false;
            return true;
        }

        public Loop()
        {
            Name = "loop";
            Description = "execute a subset of commands for loopcount";
            DefaultValue = "loop loopcount=\"10\"";
            Commands = new AsyncObservableCollection<IScriptCommand>();
        }
    }
}
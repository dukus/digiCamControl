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
using System.Text.RegularExpressions;
using System.Xml;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class IfCommand : BaseScript
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

        public override IScriptCommand Load(System.Xml.XmlNode loadnode)
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
            bool cond = true;
            Regex splitter = new Regex("\\s*(.*?)\\s*(>=|<=|!=|=|<|>)\\s*(.*)$");

            Match match = splitter.Match(LoadedParams["condition"]);

            if (match.Groups.Count != 4)
            {
                ServiceProvider.ScriptManager.OutPut("wrong parameters for if");
                return true;
            }

            string left = match.Groups[1].Value;
            string op = match.Groups[2].Value;
            string right = match.Groups[3].Value;

            left = scriptObject.ParseString(left);
            right = scriptObject.ParseString(right);


            float leftNum = 0;
            float rightNum = 0;

            bool numeric = float.TryParse(left, out leftNum);
            numeric = numeric && float.TryParse(right, out rightNum);

            // try to process our test

            if (op == ">=")
            {
                if (numeric) cond = leftNum >= rightNum;
                else cond = left.CompareTo(right) >= 0;
            }
            else if (op == "<=")
            {
                if (numeric) cond = leftNum <= rightNum;
                else cond = left.CompareTo(right) <= 0;
            }
            else if (op == "!=")
            {
                if (numeric) cond = leftNum != rightNum;
                else cond = left.CompareTo(right) != 0;
            }
            else if (op == "=")
            {
                if (numeric) cond = leftNum == rightNum;
                else cond = left.CompareTo(right) == 0;
            }
            else if (op == "<")
            {
                if (numeric) cond = leftNum < rightNum;
                else cond = left.CompareTo(right) < 0;
            }
            else if (op == ">")
            {
                if (numeric) cond = leftNum > rightNum;
                else cond = left.CompareTo(right) > 0;
            }
            else
            {
                ServiceProvider.ScriptManager.OutPut("Wrong operator :" + op);
                return true;
            }

            if (cond)
            {
                scriptObject.ExecuteCommands(Commands);
            }
            return true;
        }

        public IfCommand()
        {
            Name = "if";
            Description = "if condition to run a set of scripts";
            DefaultValue = "if condition=\"${variable}==value\" ";
            Commands = new AsyncObservableCollection<IScriptCommand>();
        }
    }
}
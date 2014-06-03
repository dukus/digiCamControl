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
using System.Reflection;
using System.Text;
using CameraControl.Core.Classes;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public sealed class SetVariable : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            var varName = LoadedParams["name"];
            if (!string.IsNullOrEmpty(varName))
            {
                var val = scriptObject.ParseString(LoadedParams["value"]);
                int intval = 0;
                int intinc = 0;
                if (int.TryParse(val, out intval) &&
                    int.TryParse(scriptObject.ParseString(LoadedParams["inc"]), out intinc))
                {
                    intval += intinc;
                    val = intval.ToString();
                }
                if (varName.StartsWith("session."))
                {
                    IList<PropertyInfo> props = new List<PropertyInfo>(typeof (PhotoSession).GetProperties());
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.PropertyType == typeof (string) || prop.PropertyType == typeof (int) ||
                            prop.PropertyType == typeof (bool))
                        {
                            if (varName.Split('.')[1].ToLower() == prop.Name.ToLower())
                            {
                                if (prop.PropertyType == typeof (string))
                                {
                                    prop.SetValue(ServiceProvider.Settings.DefaultSession, val, null);
                                }
                                if (prop.PropertyType == typeof (bool))
                                {
                                    prop.SetValue(ServiceProvider.Settings.DefaultSession, val == "true", null);
                                }
                                if (prop.PropertyType == typeof (int))
                                {
                                    int i = 0;
                                    if (int.TryParse(val, out i))
                                        prop.SetValue(ServiceProvider.Settings.DefaultSession, i, null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    scriptObject.Variabiles[varName] = val;
                }
            }
            return base.Execute(scriptObject);
        }

        public SetVariable()
        {
            Name = "setvariable";
            Description = "Set value to a variable";
            DefaultValue = "setvariable  name=\"var_name\" value=\"value\" inc=\"0\"";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public sealed class SetVariable :BaseScript
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
                    IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool))
                        {
                            if (varName.Split('.')[1].ToLower() == prop.Name.ToLower())
                            {
                                if(prop.PropertyType == typeof(string))
                                {
                                    prop.SetValue(ServiceProvider.Settings.DefaultSession, val, null);
                                }
                                if (prop.PropertyType == typeof(bool))
                                {
                                    prop.SetValue(ServiceProvider.Settings.DefaultSession, val == "true", null);
                                }
                                if (prop.PropertyType == typeof(int))
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

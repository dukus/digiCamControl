using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    class SetCamera:BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            string property = scriptObject.ParseString(LoadedParams["property"].ToLower());
            string val = scriptObject.ParseString(LoadedParams["value"]).Trim();
            switch (property)
            {
                case "iso":
                    {
                        scriptObject.CameraDevice.IsoNumber.SetValue(val);
                        if (!scriptObject.CameraDevice.IsoNumber.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                    }
                    break;
                case "aperture":
                    {
                        val = val.Replace("f", "ƒ");
                        if(!val.Contains("/"))
                        {
                            val = "ƒ/" + val;
                        }
                        if (!val.Contains("."))
                            val = val + ".0";
                        if (!scriptObject.CameraDevice.FNumber.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                        scriptObject.CameraDevice.FNumber.SetValue(val);
                    }
                    break;
                case "shutter" :
                    {
                        if(!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
                        {
                            val += "s";
                        }
                        if (val.Equals("bulb"))
                        {
                            val = "Bulb";
                        }
                        scriptObject.CameraDevice.ShutterSpeed.SetValue(val);
                        if (!scriptObject.CameraDevice.ShutterSpeed.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                    }
                    break;
                case "ec":
                    {
                        scriptObject.CameraDevice.ExposureCompensation.SetValue(val);
                        if (!scriptObject.CameraDevice.ExposureCompensation.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                    }
                    break;
                case "wb":
                    {
                        scriptObject.CameraDevice.WhiteBalance.SetValue(val);
                        if (!scriptObject.CameraDevice.WhiteBalance.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                    }
                    break;
                case "cs":
                    {
                        scriptObject.CameraDevice.CompressionSetting.SetValue(val);
                        if (!scriptObject.CameraDevice.CompressionSetting.Values.Contains(val))
                            ServiceProvider.ScriptManager.OutPut(string.Format("Wrong value {1} for property {0}",
                                                                               property, val));
                    }
                    break;
                default:
                    {
                        ServiceProvider.ScriptManager.OutPut("Wrong property :" + property);
                    }
                    break;
            }
            return true;
        }

        public SetCamera()
        {
            Name = "setcamera";
            Description = "Set a camera property";
            DefaultValue = "setcamera";
        }
    }
}

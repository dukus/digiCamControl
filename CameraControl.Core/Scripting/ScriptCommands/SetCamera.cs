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

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    internal class SetCamera : BaseScript
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
                        if (!val.Contains("/"))
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
                case "shutter":
                    {
                        if (!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
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
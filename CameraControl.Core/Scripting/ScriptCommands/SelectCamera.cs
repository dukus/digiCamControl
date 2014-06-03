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
    public class SelectCamera : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            try
            {
                int cameranum = 0;
                if (int.TryParse(scriptObject.ParseString(LoadedParams["cameranum"]), out cameranum))
                {
                    if (cameranum > -1 && cameranum < ServiceProvider.DeviceManager.ConnectedDevices.Count)
                    {
                        ServiceProvider.DeviceManager.SelectedCameraDevice =
                            ServiceProvider.DeviceManager.ConnectedDevices[cameranum];
                        scriptObject.CameraDevice = ServiceProvider.DeviceManager.ConnectedDevices[cameranum];
                    }
                    else
                    {
                        ServiceProvider.ScriptManager.OutPut("Camera with number " + cameranum + "not exist");
                    }
                }
                else
                {
                    ServiceProvider.ScriptManager.OutPut("Wrong camera number");
                }
            }
            catch (Exception exception)
            {
                ServiceProvider.ScriptManager.OutPut("Exception on select camera " + exception.Message);
            }
            return true;
        }

        public SelectCamera()
        {
            Name = "selectcamera";
            Description = "If multiple cameras are connected select a camera identified by a number";
            DefaultValue = "selectcamera cameranum=\"1\"";
        }
    }
}
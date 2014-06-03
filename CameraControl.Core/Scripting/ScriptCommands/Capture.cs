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
using CameraControl.Core.Classes;
using CameraControl.Devices;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class Capture : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            try
            {
                ServiceProvider.ScriptManager.OutPut("Capture started");
                var thread = new Thread(() => CaptureAsync(scriptObject));
                thread.Start();
                Thread.Sleep(100);
            }
            catch (Exception exception)
            {
                ServiceProvider.ScriptManager.OutPut("Capture error " + exception.Message);
                Log.Debug("Script capture error", exception);
            }
            return true;
        }

        private void CaptureAsync(ScriptObject scriptObject)
        {
            try
            {
                scriptObject.CameraDevice.IsBusy = true;
                CameraHelper.Capture(scriptObject.CameraDevice);
            }
            catch (Exception exception)
            {
                scriptObject.CameraDevice.IsBusy = false;
                ServiceProvider.ScriptManager.OutPut("Capture error " + exception.Message);
                Log.Debug("Script capture error", exception);
            }
        }

        public Capture()
        {
            Name = "capture";
            Description = "Trigger capture command on camera";
            DefaultValue = "capture";
        }
    }
}
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
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    // Delegate for passing received message back to caller
    public delegate void DelegateMessage(string Reply);

    public class PipeServerT
    {
        private string _pipeName;

        public void Listen(string PipeName)
        {
            try
            {
                // Set to class level var so we can re-use in the async callback method
                _pipeName = PipeName;
                // Create the new async pipe 
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1,
                                                                             PipeTransmissionMode.Byte,
                                                                             PipeOptions.Asynchronous);

                // Wait for a connection
                pipeServer.BeginWaitForConnection(WaitForConnectionCallBack, pipeServer);
            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;
                // End waiting for the connection
                pipeServer.EndWaitForConnection(iar);

                StreamReader sr = new StreamReader(pipeServer);
                StreamWriter sw = new StreamWriter(pipeServer);


                var response = ProccesQueries(sr.ReadLine());
                sw.WriteLine(response);
                sw.Flush();
                pipeServer.WaitForPipeDrain();

                // Kill original sever and create new wait server
                pipeServer.Disconnect();
                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
                                                       PipeOptions.Asynchronous);

                // Recursively wait for the connection again and again....
                pipeServer.BeginWaitForConnection(WaitForConnectionCallBack, pipeServer);
            }
            catch (Exception e)
            {
                Log.Debug("Pipe server error", e);
            }
        }

        private string ProccesQueries(string query)
        {
            string res = ":;response:error;message:wrong query";
            try
            {
                var lines = Pharse(query);
                if (lines.ContainsKey("request"))
                {
                    switch (lines["request"])
                    {
                        case "session":
                        {
                            return lines.ContainsKey("format") && lines["format"] == "json"
                                ? JsonConvert.SerializeObject(ServiceProvider.Settings.DefaultSession,
                                    Formatting.Indented,
                                    new JsonSerializerSettings() {})
                                : GetSessionData();
                        }
                        case "cameras":
                        {
                            if (ServiceProvider.DeviceManager.ConnectedDevices.Count == 0)
                            {
                                return ":;response:error;message:no camera is connected";
                            }
                            return lines.ContainsKey("format") && lines["format"] == "json"
                                ? JsonConvert.SerializeObject(ServiceProvider.DeviceManager.ConnectedDevices,
                                    Formatting.Indented,
                                    new JsonSerializerSettings() {})
                                : GetCamerasData();
                        }
                        default:
                            return ":;response:error;message:unknown request";
                    }
                }
                if (lines.ContainsKey("command"))
                {
                    ICameraDevice device = null;
                    if (ServiceProvider.DeviceManager.ConnectedDevices.Count == 0)
                    {
                        return ":;response:error;message:no camera is connected";
                    }
                    if (lines.ContainsKey("serial"))
                    {
                        device = GetDevice(lines["serial"]);
                    }

                    if (device == null)
                    {
                        if (ServiceProvider.DeviceManager.SelectedCameraDevice == null)
                            return ":;response:error;message:No camera was found";
                        device = ServiceProvider.DeviceManager.SelectedCameraDevice;
                    }
                    switch (lines["command"])
                    {
                        case "capture":
                        {
                            var thread = new Thread(StartCapture);
                            thread.Start(device);
                            return ":;response:ok;";
                        }
                        case "starliveview":
                        {
                            ServiceProvider.WindowsManager.ExecuteCommand(
                                WindowsCmdConsts.LiveViewWnd_Show, device);
                            return ":;response:ok;";
                        }
                        case "captureliveview":
                        {
                            ServiceProvider.WindowsManager.ExecuteCommand(
                                CmdConsts.LiveView_Capture, device);
                            return ":;response:ok;";
                        }
                        case "manualfocus":
                        {
                            var time = DateTime.Now;
                            ServiceProvider.WindowsManager.ExecuteCommand(
                                CmdConsts.LiveView_ManualFocus + lines["step"], device);
                            return ":;response:ok;executiontime:" + (DateTime.Now - time);
                        }

                        case "stopliveview":
                        {
                            ServiceProvider.WindowsManager.ExecuteCommand(
                                WindowsCmdConsts.LiveViewWnd_Hide, device);
                            return ":;response:ok;";
                        }
                        case "dcc":
                        {
                            try
                            {
                                if (!lines.ContainsKey("param"))
                                {
                                    return ":;response:error;No parameters are specified";
                                }
                                var processor = new CommandLineProcessor();
                                var resp = processor.Pharse(lines["param"].Split(' '));
                                return string.Format(":;response:{0};", JsonConvert.SerializeObject(resp));
                            }
                            catch (Exception ex)
                            {
                                return ":;response:error;message:" + ex.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                res = ":;response:error;message:" + exception.Message;
            }
            return res;
        }

        private ICameraDevice GetDevice(string serial)
        {
            return ServiceProvider.DeviceManager.ConnectedDevices.FirstOrDefault(device => device.SerialNumber == serial);
        }

        private void StartCapture(object o)
        {
            try
            {
                CameraHelper.Capture(o);
            }
            catch (Exception exception)
            {
                Log.Error("Error capture:", exception);
                StaticHelper.Instance.SystemMessage = exception.Message;
            }
        }

        public static Dictionary<string, string> Pharse(string data)
        {
            var res = new Dictionary<string, string>();
            //the data length not enough 
            if (data.Length < 3)
                return null;
            string valueseparator = data.Substring(0, 1);
            char lineseparator = Convert.ToChar(data.Substring(1, 1));
            string[] lines = data.Substring(2).Split(lineseparator);
            if (lines.Length < 1)
                return null;
            foreach (string line in lines)
            {
                if (line.Contains(valueseparator))
                {
                    int seppos = line.IndexOf(valueseparator, StringComparison.Ordinal);
                    res.Add(line.Substring(0, seppos), line.Substring(seppos + 1));
                }
                else
                {
                    res.Add(valueseparator, string.Empty);
                }
            }
            return res;
        }

        private string GetSessionData()
        {
            string res = ":;response:session";
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(bool))
                {
                    var value = prop.GetValue(ServiceProvider.Settings.DefaultSession, null);
                    res += string.Format(";{0}:{1}", prop.Name.ToLower(), value);
                }
            }
            return res;
        }

        private string GetCamerasData()
        {
            string res = ":;response:cameras;count:" + ServiceProvider.DeviceManager.ConnectedDevices.Count;
            for (int i = 0; i < ServiceProvider.DeviceManager.ConnectedDevices.Count; i++)
            {
                CameraProperty property =
                    ServiceProvider.Settings.CameraProperties.Get(ServiceProvider.DeviceManager.ConnectedDevices[i]);
                res += string.Format(";serial{0}:{1};model{0}:{2};name{0}:{3}", i + 1,
                                     ServiceProvider.DeviceManager.ConnectedDevices[i].SerialNumber,
                                     ServiceProvider.DeviceManager.ConnectedDevices[i].DeviceName, property.DeviceName);
            }
            return res;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using CameraControl.Devices;
using Newtonsoft.Json;

namespace CameraControl.Core.Classes
{
    // Delegate for passing received message back to caller
    public delegate void DelegateMessage(string Reply);

    class PipeServerT
    {
        string _pipeName;

        public void Listen(string PipeName)
        {
            try
            {
                // Set to class level var so we can re-use in the async callback method
                _pipeName = PipeName;
                // Create the new async pipe 
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,PipeOptions.Asynchronous);

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
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

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
                if(lines.ContainsKey("request"))
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
                                if(ServiceProvider.DeviceManager.ConnectedDevices.Count==0)
                                {
                                    return ":;response:error;message:no camera is connected";
                                }
                                return lines.ContainsKey("format") && lines["format"] == "json"
                                           ? JsonConvert.SerializeObject(ServiceProvider.DeviceManager.ConnectedDevices,
                                                                         Formatting.Indented,
                                                                         new JsonSerializerSettings() { })
                                           : GetCamerasData();
                            }
                        default:
                            return ":;response:error;message:unknown request";

                    }
                }
                if (lines.ContainsKey("command"))
                {
                    switch (lines["command"])
                    {
                        case "capture":
                            {
                                if (ServiceProvider.DeviceManager.ConnectedDevices.Count == 0)
                                {
                                    return ":;response:error;message:no camera is connected";
                                }
                                if (lines.ContainsKey("serial"))
                                {
                                    foreach (ICameraDevice device in ServiceProvider.DeviceManager.ConnectedDevices)
                                    {
                                        if (device.SerialNumber == lines["serial"])
                                        {
                                            Log.Debug("Start capture: " + device.SerialNumber);
                                            var thread = new Thread(StartCapture);
                                            thread.Start(device);
                                            return ":;response:ok;";
                                        }
                                    }
                                    return ":;response:error;message:No camera was found" ;
                                }
                                else
                                {
                                    return ":;response:error;message:no serial as parameter";
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                res = ":;response:error;message:" + exception.Message;
            }
            return res;
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

        private Dictionary<string, string> Pharse(string data)
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
                if(line.Contains(valueseparator))
                {
                    int seppos = line.IndexOf(valueseparator, StringComparison.Ordinal);
                    res.Add(line.Substring(0, seppos), line.Substring(seppos+1));
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
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool))
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

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
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using CameraControl.Devices;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class PHDGuiding : BaseScript
    {
        public override bool Execute(ScriptObject scriptObject)
        {
            try
            {
                ServiceProvider.ScriptManager.OutPut("PHDGuiding started");
                Executing = true;
                TcpClient socket = new TcpClient("localhost", 4300);
                Thread.Sleep(200);
                switch (MoveType.ToLower())
                {
                    case "move 1":
                        SendReceiveTest2(socket, 3);
                        break;
                    case "move 2":
                        SendReceiveTest2(socket, 4);
                        break;
                    case "move 3":
                        SendReceiveTest2(socket, 5);
                        break;
                    case "move 4":
                        SendReceiveTest2(socket, 12);
                        break;
                    case "move 5":
                        SendReceiveTest2(socket, 13);
                        break;
                }
                ServiceProvider.ScriptManager.OutPut("PHDGuiding waiting....");
                for (int i = 1; i <= WaitTime/1000; i++)
                {
                    if (ServiceProvider.ScriptManager.ShouldStop)
                        break;
                    Thread.Sleep(i*1000);
                }
                socket.Close();
            }
            catch (Exception exception)
            {
                StaticHelper.Instance.SystemMessage = "PHDGuiding error " + exception.Message;
                Log.Error("PHDGuiding error", exception);
            }
            ServiceProvider.ScriptManager.OutPut("PHDGuiding done");
            return true;
        }

        public override IScriptCommand Create()
        {
            return new PHDGuiding();
        }

        public override XmlNode Save(XmlDocument doc)
        {
            XmlNode nameNode = doc.CreateElement("phdguiding");
            nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, "waittime", WaitTime.ToString()));
            nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, "movetype", MoveType));
            return nameNode;
        }

        public override IScriptCommand Load(XmlNode node)
        {
            PHDGuiding res = new PHDGuiding
                                 {
                                     WaitTime = Convert.ToInt32(ScriptManager.GetValue(node, "waittime")),
                                     MoveType = ScriptManager.GetValue(node, "movetype"),
                                     //Aperture = ScriptManager.GetValue(node, "Aperture")
                                 };
            return res;
        }

        public override UserControl GetConfig()
        {
            return new PHDGuidingControl(this);
        }

        public override string DisplayName
        {
            get { return string.Format("[{0}][MoveType={1}, WaitTime={2}]", Name, MoveType, WaitTime); }
            set { }
        }

        private int _waitTime;

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                _waitTime = value;
                NotifyPropertyChanged("WaitTime");
                NotifyPropertyChanged("DisplayName");
            }
        }

        private string _moveType;

        public string MoveType
        {
            get { return _moveType; }
            set
            {
                _moveType = value;
                NotifyPropertyChanged("MoveType");
                NotifyPropertyChanged("DisplayName");
            }
        }

        public PHDGuiding()
        {
            Name = "phdguiding";
            Description = "Send command to PHD.\nParameters: \nmovetype : value can be move 1..5";
            DefaultValue = "phdguiding movetype=\"move 1\"";
            WaitTime = 2000;
            MoveType = "move 1";
            HaveEditControl = true;
        }

        public static int SendReceiveTest2(TcpClient server, byte opersEnum)
        {
            byte[] bytes = new byte[256];
            try
            {
                // Blocks until send returns. 
                int byteCount = server.Client.Send(new[] {opersEnum}, SocketFlags.None);
                //Console.WriteLine("Sent {0} bytes.", byteCount);

                // Get reply from the server.
                byteCount = server.Client.Receive(bytes, SocketFlags.None);
                //Console.WriteLine(byteCount);
                //if (byteCount > 0)
                //    Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
            catch (SocketException e)
            {
                //Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                return (e.ErrorCode);
            }
            return 0;
        }
    }
}
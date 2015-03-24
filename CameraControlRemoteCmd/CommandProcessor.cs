using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using Newtonsoft.Json;

namespace CameraControlRemoteCmd
{
    public class CommandProcessor
    {
        public int Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Wrong usage");
                Console.WriteLine("Parameters");
                Console.WriteLine("/c <one line command>");
                Console.WriteLine("/host <host address>");
                Console.WriteLine("/clean");
                Console.ReadLine();
                return -1;
            }
            try
            {
                var arguments = new InputArguments(args, "/", true);
                if (!arguments.Contains("/c"))
                {
                    Console.WriteLine("Missing command");
                    Console.WriteLine("/c <one line command>");
                    return -1;
                }
                string mess = Send(arguments["/c"], "DCCPipe", arguments.Contains("/host") ? arguments["/host"] : ".",
                    15000);
                if (arguments.Contains("/clean"))
                {
                    var lines = PipeServerT.Pharse(mess);
                    if (lines.ContainsKey("response"))
                    {
                        if (lines["response"].StartsWith("["))
                        {
                            var list = JsonConvert.DeserializeObject<List<string>>(lines["response"]);
                            foreach (string s in list)
                            {
                                Console.WriteLine(s);
                            }
                        }
                        else
                        {
                            Console.WriteLine(lines["response"]);                            
                        }
                    }
                    else
                    {
                        Console.WriteLine(lines["message"]);
                    }
                }
                else
                {
                    Console.WriteLine(mess);                    
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        public string Send(string sendStr, string pipeName, string server=".",int timeOut = 1000)
        {
            sendStr = string.Format(":;command:dcc;param:{0}", sendStr);
            NamedPipeClientStream pipeStream = new NamedPipeClientStream(server, pipeName, PipeDirection.InOut);

            pipeStream.Connect(timeOut);


            var sr = new StreamReader(pipeStream);
            var sw = new StreamWriter(pipeStream);


            sw.WriteLine(sendStr);
            sw.Flush();

            string temp = sr.ReadToEnd();
            return temp;
        }


    }
}

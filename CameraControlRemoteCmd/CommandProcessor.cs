using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using CameraControl.Core.Classes;
using Newtonsoft.Json;

namespace CameraControlRemoteCmd
{
    public class CommandProcessor
    {
    private void ShowHelp()
        {
            Console.WriteLine("\nParameters");
            Console.WriteLine("/c <one line command> (parameter may be repeated multiple times)");
            Console.WriteLine("/host <host address>");
            Console.WriteLine("/clean - attempt to cleanup error messages");
            Console.WriteLine("/help - show this help text");
            Console.WriteLine("\npress enter to continue...");
            Console.ReadLine();
        }
        public int Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Wrong usage");
                ShowHelp();
                return -1;
            }
            try
            {
                var arguments = new InputArguments(args, "/", true);
                if (arguments.Contains("/help"))
                {
                    ShowHelp();
                    return 0;
                }
                if (!arguments.Contains("/c"))
                {
                    Console.WriteLine("ERROR: Missing command");
                    ShowHelp();
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
                        else if (lines.ContainsKey("message"))
                        {
                            Console.WriteLine(lines["response"] + " " + lines["message"]);
                        }
                        else if (!lines["response"].Equals("null"))
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
            catch (TimeoutException ex)
            {
                Console.WriteLine("Timeout Error (is the GUI running?):");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return -1;
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

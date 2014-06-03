using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CameraControl.Devices;

// origin: http://www.codeproject.com/Articles/1505/Create-your-own-Web-Server-using-C
namespace CameraControl.Core.Classes
{
    public class WebServerOld
    {
        public delegate void EventEventHandler(string cmd);

        public virtual event EventEventHandler Event;

        private bool _shouldStop;
        private TcpListener myListener;
        private int port = 5050; // Select any free port you wish

        //The constructor which make the TcpListener start listening on the
        //given port. It also calls a Thread on the method StartListen(). 
        public WebServerOld()
        {
        }

        public void Start(int pt)
        {
            try
            {
                _shouldStop = false;
                port = pt;
                var adrs = "127.0.0.1";
                if (!string.IsNullOrEmpty(ServiceProvider.Settings.Webaddress))
                    adrs = ServiceProvider.Settings.Webaddress.Substring(7).Split(':')[0];

                IPAddress localAddr = IPAddress.Parse(adrs);
                //start listing on the given port
                myListener = new TcpListener(localAddr, port);
                myListener.Start();
                Console.WriteLine("Web Server Running... Press ^C to Stop...");
                //start the thread which calls the method 'StartListen'
                Thread th = new Thread(new ThreadStart(StartListen));
                th.Start();

            }
            catch (Exception e)
            {
                Log.Error("Error start webserver", e);
            }
        }

        public void Stop()
        {
            _shouldStop = true;
            if (myListener != null)
                myListener.Stop();
        }

        /// <summary>
        /// Returns The Default File Name
        /// Input : WebServerRoot Folder
        /// Output: Default File Name
        /// </summary>
        /// <param name="sMyWebServerRoot"></param>
        /// <returns></returns>
        public string GetTheDefaultFileName(string sLocalDirectory)
        {
            String sLine = "index.html";

            if (File.Exists(sLocalDirectory + sLine) == true)
                return sLine;
            else
                return "";
        }



        /// <summary>
        /// This function takes FileName as Input and returns the mime type..
        /// </summary>
        /// <param name="sRequestedFile">To indentify the Mime Type</param>
        /// <returns>Mime Type</returns>
        public string GetMimeType(string sRequestedFile)
        {
            return "";

            //StreamReader sr;
            //String sLine = "";
            //String sMimeType = "";
            //String sFileExt = "";
            //String sMimeExt = "";

            //// Convert to lowercase
            //sRequestedFile = sRequestedFile.ToLower();

            //int iStartPos = sRequestedFile.IndexOf(".");

            //sFileExt = sRequestedFile.Substring(iStartPos);

            ////try
            ////{
            ////  //Open the Vdirs.dat to find out the list virtual directories
            ////  sr = new StreamReader("data\\Mime.Dat");

            ////  while ((sLine = sr.ReadLine()) != null)
            ////  {

            ////    sLine.Trim();

            ////    if (sLine.Length > 0)
            ////    {
            ////      //find the separator
            ////      iStartPos = sLine.IndexOf(";");

            ////      // Convert to lower case
            ////      sLine = sLine.ToLower();

            ////      sMimeExt = sLine.Substring(0, iStartPos);
            ////      sMimeType = sLine.Substring(iStartPos + 1);

            ////      if (sMimeExt == sFileExt)
            ////        break;
            ////    }
            ////  }
            ////}
            ////catch (Exception e)
            ////{
            ////  Console.WriteLine("An Exception Occurred : " + e.ToString());
            ////}

            //if (sMimeExt == sFileExt)
            //  return sMimeType;
            //else
            //  return "";
        }



        /// <summary>
        /// Returns the Physical Path
        /// </summary>
        /// <param name="sMyWebServerRoot">Web Server Root Directory</param>
        /// <param name="sDirName">Virtual Directory </param>
        /// <returns>Physical local Path</returns>
        public string GetLocalPath(string sMyWebServerRoot, string sDirName)
        {

            StreamReader sr;
            String sLine = "";
            String sVirtualDir = "";
            String sRealDir = "";
            int iStartPos = 0;


            //Remove extra spaces
            sDirName.Trim();



            // Convert to lowercase
            sMyWebServerRoot = sMyWebServerRoot.ToLower();

            // Convert to lowercase
            sDirName = sDirName.ToLower();

            //Remove the slash
            //sDirName = sDirName.Substring(1, sDirName.Length - 2);


            try
            {
                //Open the Vdirs.dat to find out the list virtual directories
                sr = new StreamReader("data\\VDirs.Dat");

                while ((sLine = sr.ReadLine()) != null)
                {
                    //Remove extra Spaces
                    sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator
                        iStartPos = sLine.IndexOf(";");

                        // Convert to lowercase
                        sLine = sLine.ToLower();

                        sVirtualDir = sLine.Substring(0, iStartPos);
                        sRealDir = sLine.Substring(iStartPos + 1);

                        if (sVirtualDir == sDirName)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred : " + e.ToString());
            }


            Console.WriteLine("Virtual Dir : " + sVirtualDir);
            Console.WriteLine("Directory   : " + sDirName);
            Console.WriteLine("Physical Dir: " + sRealDir);
            if (sVirtualDir == sDirName)
                return sRealDir;
            else
                return "";
        }



        /// <summary>
        /// This function send the Header Information to the client (Browser)
        /// </summary>
        /// <param name="sHttpVersion">HTTP Version</param>
        /// <param name="sMIMEHeader">Mime Type</param>
        /// <param name="iTotBytes">Total Bytes to be sent in the body</param>
        /// <param name="mySocket">Socket reference</param>
        /// <returns></returns>
        public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode,
                               ref Socket mySocket)
        {

            String sBuffer = "";

            // if Mime type is not provided set default to text/html
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/html"; // Default Mime Type is text/html
            }

            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";

            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);

            SendToBrowser(bSendData, ref mySocket);

            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());

        }



        /// <summary>
        /// Overloaded Function, takes string, convert to bytes and calls 
        /// overloaded sendToBrowserFunction.
        /// </summary>
        /// <param name="sData">The data to be sent to the browser(client)</param>
        /// <param name="mySocket">Socket reference</param>
        public void SendToBrowser(String sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }



        /// <summary>
        /// Sends data to the browser (client)
        /// </summary>
        /// <param name="bSendData">Byte Array</param>
        /// <param name="mySocket">Socket reference</param>
        public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;

            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                        Console.WriteLine("Socket Error cannot Send Packet");
                    else
                    {
                        Console.WriteLine("No. of bytes send {0}", numBytes);
                    }
                }
                else
                    Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);

            }
        }


        //This method Accepts new connection and
        //First it receives the welcome massage from the client,
        //Then it sends the Current date time to the Client.
        public void StartListen()
        {

            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;
            String sErrorMessage;
            String sLocalDir;
            String sMyWebServerRoot = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WebServer\\");
            String sPhysicalFilePath = "";
            String sFormattedMessage = "";
            String sResponse = "";



            while (true)
            {
                if (_shouldStop)
                    return;
                //Accept a new connection
                Socket mySocket = null;
                try
                {
                    mySocket = myListener.AcceptSocket();
                }
                catch (Exception)
                {
                }

                if (mySocket == null)
                    continue;

                //Console.WriteLine("Socket Type " + mySocket.SocketType);
                if (mySocket.Connected)
                {
                    //Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\n",
                    //                  mySocket.RemoteEndPoint);



                    //make a byte array and receive data from the client 
                    Byte[] bReceive = new Byte[1024];
                    int i = mySocket.Receive(bReceive, bReceive.Length, 0);



                    //Convert Byte to String
                    string sBuffer = Encoding.ASCII.GetString(bReceive);



                    //At present we will only deal with GET type
                    if (sBuffer.Substring(0, 3) != "GET")
                    {
                        Console.WriteLine("Only Get Method is supported..");
                        mySocket.Close();
                        continue;
                    }


                    // Look for HTTP request
                    iStartPos = sBuffer.IndexOf("HTTP", 1);


                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                    string sHttpVersion = sBuffer.Substring(iStartPos, 8);


                    // Extract the Requested Type and Requested file/directory
                    sRequest = sBuffer.Substring(0, iStartPos - 1);


                    //Replace backslash with Forward Slash, if Any
                    sRequest.Replace("\\", "/");


                    //If file name is not supplied add forward slash to indicate 
                    //that it is a directory and then we will look for the 
                    //default file name..
                    //if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                    //{
                    //  sRequest = sRequest + "/";
                    //}


                    //Extract the requested file name
                    iStartPos = sRequest.LastIndexOf("/") + 1;
                    sRequestedFile = sRequest.Substring(iStartPos);


                    //Extract The directory Name
                    sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);

                    string cmds = "";
                    if (sDirName.Contains("?"))
                    {
                        cmds = sDirName.Split('?')[1];
                        sDirName = sDirName.Split('?')[0];
                    }

                    /////////////////////////////////////////////////////////////////////
                    // Identify the Physical Directory
                    /////////////////////////////////////////////////////////////////////
                    if (sDirName == "/")
                        sLocalDir = sMyWebServerRoot;
                    else
                    {
                        //Get the Virtual Directory
                        sLocalDir = GetLocalPath(sMyWebServerRoot, sDirName);
                    }


                    Console.WriteLine("Directory Requested : " + sLocalDir);

                    //If the physical directory does not exists then
                    // dispaly the error message
                    if (sLocalDir.Length == 0)
                    {
                        sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                        //sErrorMessage = sErrorMessage + "Please check data\\Vdirs.Dat";

                        //Format The Message
                        SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);

                        //Send to the browser
                        SendToBrowser(sErrorMessage, ref mySocket);

                        mySocket.Close();

                        continue;
                    }


                    /////////////////////////////////////////////////////////////////////
                    // Identify the File Name
                    /////////////////////////////////////////////////////////////////////
                    if (sRequestedFile.Contains("?"))
                    {
                        cmds = sRequestedFile.Split('?')[1];
                        sRequestedFile = sRequestedFile.Split('?')[0];
                    }

                    //If The file name is not supplied then look in the default file list
                    if (sRequestedFile.Length == 0)
                    {
                        // Get the default filename
                        sRequestedFile = GetTheDefaultFileName(sLocalDir);

                        if (sRequestedFile == "")
                        {
                            sErrorMessage = "<H2>Error!! No Default File Name Specified</H2>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);

                            mySocket.Close();

                            continue;

                        }
                    }




                    /////////////////////////////////////////////////////////////////////
                    // Get TheMime Type
                    /////////////////////////////////////////////////////////////////////


                    if (!string.IsNullOrEmpty(cmds) && Event != null)
                        Event(cmds);

                    String sMimeType = GetMimeType(sRequestedFile);



                    //Build the physical path
                    sPhysicalFilePath = sLocalDir + sRequestedFile;
                    Console.WriteLine("File Requested : " + sPhysicalFilePath);


                    if (File.Exists(sPhysicalFilePath) == false)
                    {

                        sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                        SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                        SendToBrowser(sErrorMessage, ref mySocket);

                        Console.WriteLine(sFormattedMessage);
                    }

                    else
                    {
                        int iTotBytes = 0;

                        sResponse = "";




                        FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        // Create a reader that can read bytes from the FileStream.


                        BinaryReader reader = new BinaryReader(fs);
                        byte[] bytes = new byte[fs.Length];
                        int read;
                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Read from the file and write the data to the network
                            sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);

                            iTotBytes = iTotBytes + read;

                        }
                        reader.Close();
                        fs.Close();

                        SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);

                        SendToBrowser(bytes, ref mySocket);

                        //mySocket.Send(bytes, bytes.Length,0);

                    }
                    mySocket.Close();
                }
            }
        }
    }
}

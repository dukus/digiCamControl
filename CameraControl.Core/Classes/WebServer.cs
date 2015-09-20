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
using System.Net;
using System.Text;
using CameraControl.Devices;
using Griffin.Networking.Protocol.Http.Services.BodyDecoders;
using Griffin.WebServer;
using Griffin.WebServer.Files;
using Griffin.WebServer.Modules;
using Griffin.WebServer.Routing;

#endregion

namespace CameraControl.Core.Classes
{
    public class WebServer
    {
        public void Start(int port)
        {
            try
            {
                // Module manager handles all modules in the server
                var moduleManager = new ModuleManager();

                // Let's serve our downloaded files (Windows 7 users)
                var fileService = new DiskFileService("/", Settings.WebServerFolder);

                // Create the file module and allow files to be listed.
                var module = new FileModule(fileService) { ListFiles = false };

                var routerModule = new RouterModule();

                // Add the module
                //moduleManager.Add(module);
                moduleManager.Add(new WebServerModule());

                //moduleManager.Add(new BodyDecodingModule(new UrlFormattedDecoder()));

                // And start the server.
                var server = new HttpServer(moduleManager);

                server.Start(IPAddress.Any, port);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to start web server ", ex);
            }
        }

        public void Stop()
        {
        }
    }
}
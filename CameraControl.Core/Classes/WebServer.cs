using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Protocol.Http.Services.BodyDecoders;
using Griffin.WebServer;
using Griffin.WebServer.Files;
using Griffin.WebServer.Modules;
using Griffin.WebServer.Routing;

namespace CameraControl.Core.Classes
{
    public class WebServer
    {
        public void Start(int port)
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

        public void Stop()
        {
            
        }

    }
}

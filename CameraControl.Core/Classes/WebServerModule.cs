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
using System.Linq;
using System.Text;
using System.Threading;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using Eagle._Tests;
using Griffin.WebServer;
using Griffin.WebServer.Files;
using Griffin.WebServer.Modules;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class WebServerModule : IWorkerModule
    {
        /*        private string _lineFormat =
                    "{image : '@image@', title : 'Image Credit: Maria Kazvan', thumb : '@image_thumb@', url : '@image_url@'},";*/

        //private string _lineFormat =
        //    "            <div>  <img u=\"image\" src=\"@image@\" /><img u=\"thumb\" src=\"@image_thumb@\" /></div>";

        private string _lineFormat =
    "            <img u=\"image\" src=\"@image@\" />";

        private bool _liveViewFirstRun = true;

        #region Implementation of IHttpModule

        public void BeginRequest(IHttpContext context)
        {
        }

        public void EndRequest(IHttpContext context)
        {
        }

        public void HandleRequestAsync(IHttpContext context, Action<IAsyncModuleResult> callback)
        {
            callback(new AsyncModuleResult(context, HandleRequest(context)));
        }

        #endregion

        public ModuleResult HandleRequest(IHttpContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(context.Request.Uri.AbsolutePath) || context.Request.Uri.AbsolutePath == "/")
                {
                    string str = context.Request.Uri.Scheme + "://" + context.Request.Uri.Host;
                    if (context.Request.Uri.Port != 80)
                        str = str + (object) ":" + context.Request.Uri.Port;
                    string uriString = str + context.Request.Uri.AbsolutePath + "index.html";
                    if (!string.IsNullOrEmpty(context.Request.Uri.Query))
                        uriString = uriString + "?" + context.Request.Uri.Query;
                    context.Request.Uri = new Uri(uriString);
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/thumb/large"))
                {
                    string requestFile = Path.GetFileName(context.Request.Uri.AbsolutePath.Replace("/", "\\"));
                    foreach (FileItem item in ServiceProvider.Settings.DefaultSession.Files)
                    {
                        if (Path.GetFileName(item.LargeThumb) ==requestFile || item.Name == requestFile)
                        {
                            SendFile(context,
                                !File.Exists(item.LargeThumb)
                                    ? Path.Combine(Settings.WebServerFolder, "logo.png")
                                    : item.LargeThumb);
                            SendFile(context, item.LargeThumb);
                            return ModuleResult.Continue;
                        }
                    }
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/thumb/small"))
                {
                    string requestFile = Path.GetFileName(context.Request.Uri.AbsolutePath.Replace("/", "\\"));
                    foreach (FileItem item in ServiceProvider.Settings.DefaultSession.Files)
                    {
                        if (Path.GetFileName(item.SmallThumb) == requestFile || item.Name == requestFile)
                        {
                            SendFile(context,
                                !File.Exists(item.SmallThumb)
                                    ? Path.Combine(Settings.WebServerFolder, "logo.png")
                                    : item.SmallThumb);
                            return ModuleResult.Continue;
                        }
                    }
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/preview.jpg"))
                {
                    SendFile(context, ServiceProvider.Settings.SelectedBitmap.FileItem.LargeThumb);
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/session.json"))
                {
                    var s = JsonConvert.SerializeObject(ServiceProvider.Settings.DefaultSession, Formatting.Indented);
                    SendData(context, Encoding.ASCII.GetBytes(s));
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/settings.json"))
                {
                    var s = JsonConvert.SerializeObject(ServiceProvider.Settings, Formatting.Indented);
                    SendData(context, Encoding.ASCII.GetBytes(s));
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/filelist.json"))
                {
                    List<FileListItem> items =
                        ServiceProvider.Settings.DefaultSession.Files.Select(item => new FileListItem()
                        {
                            FileName = item.FileName,
                            LargeThumb = "/thumb/large/" + Path.GetFileName(item.LargeThumb),
                            SmallThumb = "/thumb/small/" + Path.GetFileName(item.SmallThumb),
                            Original = "/image/" + Path.GetFileName(item.FileName),
                            Name = item.Name,
                            Width = item.FileInfo.Width > 0 ? item.FileInfo.Width : 3000,
                            Height = item.FileInfo.Height > 0 ? item.FileInfo.Height : 2000,
                        }).ToList();
                    var s = JsonConvert.SerializeObject(items, Formatting.Indented);
                    SendData(context, Encoding.ASCII.GetBytes(s));
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/liveview.jpg") &&
                    ServiceProvider.DeviceManager.SelectedCameraDevice != null &&
                    ServiceProvider.DeviceManager.LiveViewImage.ContainsKey(
                        ServiceProvider.DeviceManager.SelectedCameraDevice))
                {
                    SendDataFile(context,
                        ServiceProvider.DeviceManager.LiveViewImage[ServiceProvider.DeviceManager.SelectedCameraDevice], MimeTypeProvider.Instance.Get("liveview.jpg"));
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/liveviewwebcam.jpg") &&
                    ServiceProvider.DeviceManager.SelectedCameraDevice != null )
                {
                    if (_liveViewFirstRun)
                    {
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Show);
                        Thread.Sleep(500);
                        ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.All_Minimize);
                        ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.LiveView_NoProcess);
                        _liveViewFirstRun = false;
                    }
                    if (ServiceProvider.DeviceManager.LiveViewImage.ContainsKey(
                        ServiceProvider.DeviceManager.SelectedCameraDevice))
                        SendDataFile(context,
                            ServiceProvider.DeviceManager.LiveViewImage[
                                ServiceProvider.DeviceManager.SelectedCameraDevice],
                            MimeTypeProvider.Instance.Get("liveview.jpg"));
                }

                if (context.Request.Uri.AbsolutePath.StartsWith("/image/"))
                {
                    foreach (FileItem item in ServiceProvider.Settings.DefaultSession.Files)
                    {
                        if (Path.GetFileName(item.FileName) ==
                            Path.GetFileName(context.Request.Uri.AbsolutePath.Replace("/", "\\")))
                        {
                            SendFile(context, item.FileName);
                            return ModuleResult.Continue;
                        }
                    }
                }

                var slc = context.Request.QueryString["slc"];
                if (ServiceProvider.Settings.AllowWebserverActions && !string.IsNullOrEmpty(slc))
                {
                    string response = "";
                    try
                    {
                        var processor = new CommandLineProcessor();
                        processor.SetCamera(context.Request.QueryString["camera"]);
                        var resp = processor.Pharse(new[] { context.Request.QueryString["slc"], context.Request.QueryString["param1"], context.Request.QueryString["param2"] });
                        var list = resp as IEnumerable<string>;
                        if (list != null)
                        {
                            foreach (var o in list)
                            {
                                response += o + "\n";
                            }
                        }
                        else
                        {
                            if (resp != null)
                                response = resp.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        response = ex.Message;
                    }
                    if (string.IsNullOrEmpty(response))
                        response = "OK";

                    byte[] buffer = Encoding.UTF8.GetBytes(response);

                    //response.ContentLength64 = buffer.Length;
                    context.Response.AddHeader("Content-Length", buffer.Length.ToString());
                    context.Response.ContentType = "text/html";
                    context.Response.Body = new MemoryStream();
                    
                    context.Response.Body.Write(buffer, 0, buffer.Length);
                    context.Response.Body.Position = 0;
                    return ModuleResult.Continue;
                }


                string fullpath = GetFullPath(context.Request.Uri);
                if (!string.IsNullOrEmpty(fullpath) && File.Exists(fullpath))
                {
                    if (Path.GetFileName(fullpath) == "slide.html")
                    {
                        string file = File.ReadAllText(fullpath);
                        string template= Path.Combine(Settings.WebServerFolder, "template.txt");
                        string jsontemplate = Path.Combine(Settings.WebServerFolder, "template.json");
                        bool json = false;
                        if (File.Exists(template))
                        {
                            _lineFormat = File.ReadAllText(template);
                        }
                        if (File.Exists(jsontemplate))
                        {
                            _lineFormat = File.ReadAllText(jsontemplate);
                            json = true;
                        }

                        StringBuilder builder = new StringBuilder();
                        foreach (FileItem item in ServiceProvider.Settings.DefaultSession.Files)
                        {
                            if (json && builder.Length>0)
                            {
                                builder.AppendLine(",");
                            }
                            string tempStr = _lineFormat.Replace("@image@",
                                "/thumb/large/" + Path.GetFileName(item.LargeThumb));
                            tempStr = tempStr.Replace("@image_thumb@",
                                "/thumb/small/" + Path.GetFileName(item.SmallThumb));
                            tempStr = tempStr.Replace("@image_url@", "/image/" + Path.GetFileName(item.FileName));
                            tempStr = tempStr.Replace("@title@", item.Name);
                            tempStr = tempStr.Replace("@width@",
                                (item.FileInfo != null && item.FileInfo.Width > 0)? item.FileInfo.Width.ToString(): "3000");
                            tempStr = tempStr.Replace("@height@",
                                (item.FileInfo != null && item.FileInfo.Height > 0) ? item.FileInfo.Height.ToString() : "2000");
                            tempStr = tempStr.Replace("@desc@",
                                (item.FileInfo != null) ? (item.FileInfo.InfoLabel ?? "") : "");
                            builder.AppendLine(json?CleanForJson(tempStr):tempStr);
                        }

                        file = file.Replace("@@image_list@@", builder.ToString());

                        byte[] buffer = Encoding.UTF8.GetBytes(file);

                        //response.ContentLength64 = buffer.Length;
                        context.Response.AddHeader("Content-Length", buffer.Length.ToString());

                        context.Response.Body = new MemoryStream();

                        context.Response.Body.Write(buffer, 0, buffer.Length);
                        context.Response.Body.Position = 0;
                    }
                    else
                    {
                        SendFile(context, fullpath);
                    }
                }
                string cmd = context.Request.QueryString["CMD"];
                string param = context.Request.QueryString["PARAM"];
                if (ServiceProvider.Settings.AllowWebserverActions && !string.IsNullOrEmpty(cmd))
                    ServiceProvider.WindowsManager.ExecuteCommand(cmd, param);
            }
            catch (Exception ex)
            {
                Log.Error("Web server error", ex);
            }
            return ModuleResult.Continue;
        }

        private  string CleanForJson(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        //sb.Append("\\n");
                        sb.Append(c);
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        //sb.Append("\\r");
                        sb.Append(c);
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        private void SendFile(IHttpContext context, string fullpath)
        {
            if (!File.Exists(fullpath))
                return;

            string str = MimeTypeProvider.Instance.Get(fullpath);
            FileStream fileStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read,
                                                   FileShare.Read | FileShare.Write);
            context.Response.AddHeader("Content-Disposition",
                                       "inline;filename=\"" + Path.GetFileName(fullpath) + "\"");
            context.Response.ContentType = str;
            context.Response.ContentLength = (int)fileStream.Length;
            context.Response.Body = fileStream;
        }

        private void SendData(IHttpContext context, byte[] data)
        {
            if (data == null)
                return;
            MemoryStream stream = new MemoryStream(data);
            //            context.Response.AddHeader("Content-Disposition",
            //"inline;filename=\"" + Path.GetFileName(fullpath) + "\"");
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = data.Length;
            context.Response.Body = stream;
        }

        private void SendDataFile(IHttpContext context, byte[] data, string mimet)
        {
            if (data == null)
                return;
            MemoryStream stream = new MemoryStream(data);
            //            context.Response.AddHeader("Content-Disposition",
            //"inline;filename=\"" + Path.GetFileName(fullpath) + "\"");
            context.Response.ContentType = mimet;
            context.Response.ContentLength = data.Length;
            context.Response.Body = stream;
        }

        private string GetFullPath(Uri uri)
        {
            // check first if there a branded version of the web server folder
            string file = Path.Combine(Settings.BrandingWebServerFolder,
                Uri.UnescapeDataString(uri.AbsolutePath.Remove(0, 1)).TrimStart(new[] { '/' })
                    .Replace('/', '\\'));
            if (File.Exists(file))
                return file;
            return Path.Combine(Settings.WebServerFolder,
                Uri.UnescapeDataString(uri.AbsolutePath.Remove(0, 1)).TrimStart(new[] {'/'})
                    .Replace('/', '\\'));
        }
    }
}
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
// for testing in 64 bit enviroment see: http://social.msdn.microsoft.com/Forums/vstudio/en-US/2e29a4aa-e587-43ef-bf50-329b7cd3eefb/debugging-wcf-service-with-x86-dependencies-on-a-x64-bit-machine?forum=wcf
#region

using System.IO;
using System.Reflection;
using CameraControl.Core.Classes;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

#endregion

namespace CameraControl.Core
{
    public class ServiceProvider : BaseFieldClass
    {
        private static readonly ILog _log = LogManager.GetLogger("DCC");
        private static PipeServerT _pipeServer;

        public static string AppName = "digiCamControl";


        public static Settings Settings { get; set; }

        public static CameraDeviceManager DeviceManager { get; set; }
        public static TriggerClass Trigger { get; set; }
        public static WindowsManager WindowsManager { get; set; }
        public static ActionManager ActionManager { get; set; }
        public static QueueManager QueueManager { get; set; }
        public static PluginManager PluginManager { get; set; }
        public static Branding Branding { get; set; }
        public static ScriptManager ScriptManager { get; set; }
        public static FilenameTemplateManager FilenameTemplateManager { get; set; }
        public static ExternalDeviceManager ExternalDeviceManager { get; set; }
        public static Analytics Analytics { get; set; }
        public static string LogFile;

        public static void Configure()
        {
            LogFile = Path.Combine(Settings.DataFolder, "Log", "app.log");
            Configure(LogFile);
        }

        public static void Configure(string logFile)
        {
            Configure(AppName, logFile);
            Log.LogDebug += Log_LogDebug;
            Log.LogError += Log_LogError;
            Log.Debug(
                "--------------------------------===========================Application starting===========================--------------------------------");
            try
            {
                Log.Debug("Application version : " + Assembly.GetEntryAssembly().GetName().Version);
            }
            catch {}
            Analytics = new Analytics();
            Log.Debug("Init : Analytics");
            //DeviceManager = new CameraDeviceManager(Path.Combine(Classes.Settings.ApplicationFolder, "Devices"));
            DeviceManager = new CameraDeviceManager();
            Log.Debug("Init : DeviceManager");
            ExternalDeviceManager = new ExternalDeviceManager();
            Log.Debug("Init : ExternalDeviceManager");
            Trigger = new TriggerClass();
            Log.Debug("Init : Trigger");
            ActionManager = new ActionManager();
            Log.Debug("Init : ActionManager");
            QueueManager = new QueueManager();
            Log.Debug("Init : QueueManager");
            //Branding = new Branding();
            ScriptManager = new ScriptManager();
            Log.Debug("Init : ScriptManager");
            PluginManager = new PluginManager();
            Log.Debug("Init : PluginManager");
            FilenameTemplateManager = new FilenameTemplateManager();
            _pipeServer = new PipeServerT();
            _pipeServer.Listen("DCCPipe");
            Log.Debug("Init : _pipeServer");
        }

        private static void Log_LogError(LogEventArgs e)
        {
            _log.Error(e.Message, e.Exception);
        }

        private static void Log_LogDebug(LogEventArgs e)
        {
            _log.Debug(e.Message, e.Exception);
        }

        public static void Configure(string appfolder, string logFile)
        {
            bool isConfigured = _log.Logger.Repository.Configured;
            if (!isConfigured)
            {
                // Setup RollingFileAppender
                var fileAppender = new RollingFileAppender
                                       {
                                           Layout =
                                               new PatternLayout(
                                               "%d [%t]%-5p %c [%x] - %m%n"),
                                           MaximumFileSize = "1000KB",
                                           MaxSizeRollBackups = 5,
                                           RollingStyle = RollingFileAppender.RollingMode.Size,
                                           AppendToFile = true,
                                           File = logFile,
                                           ImmediateFlush = true,
                                           LockingModel = new FileAppender.MinimalLock(),
                                           Name = "XXXRollingFileAppender"
                                       };
                fileAppender.ActivateOptions(); // IMPORTANT, creates the file
                BasicConfigurator.Configure(fileAppender);
#if DEBUG
                // Setup TraceAppender
                TraceAppender ta = new TraceAppender();
                ta.Layout = new PatternLayout("%d [%t]%-5p %c [%x] - %m%n");
                BasicConfigurator.Configure(ta);
#endif
            }
        }
    }
}
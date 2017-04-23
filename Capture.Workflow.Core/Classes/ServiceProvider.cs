using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace Capture.Workflow.Core.Classes
{
    public class ServiceProvider
    {

        private static readonly ILog _log = LogManager.GetLogger("Workflow");

        private static ServiceProvider _instance;

        public static ServiceProvider Instance
        {
            get { return _instance ?? (_instance = new ServiceProvider()); }
            set { _instance = value; }
        }

        public CameraDeviceManager DeviceManager { get; set; }
        public List<FileItem> FileItems { get; set; }
        public List<FileItem> InteriorFileItems { get; set; }

        public ServiceProvider()
        {
            DeviceManager = new CameraDeviceManager();
            FileItems = new List<FileItem>();
            InteriorFileItems = new List<FileItem>();
        }


        public static void Configure()
        {
            //var LogFile = Path.Combine(Settings.DataFolder, "Log", "app.log");
            var LogFile = Path.Combine("workflow.log");
            Configure(LogFile);

        }

        public static void Configure(string logFile)
        {
            Configure("WorkFlow", logFile);
            Log.LogDebug += Log_LogDebug;
            Log.LogError += Log_LogError;
            Log.Debug(
                "--------------------------------===========================Application starting===========================--------------------------------");
            try
            {
                Log.Debug("Application version : " + Assembly.GetEntryAssembly().GetName().Version);
            }
            catch { }
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
                    MaximumFileSize = "7000KB",
                    MaxSizeRollBackups = 2,
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

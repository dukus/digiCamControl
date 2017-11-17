using System.IO;
using System.Reflection;
using System.Windows;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace Capture.Workflow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogManager.GetLogger("Capture.Workflow");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Configure(Path.Combine(Settings.Instance.LogFolder, "app.log"));
            WorkflowManager.Instance.LoadPlugins("Capture.Workflow.Plugins.dll");
        }

        public static void Configure(string logFile)
        {
            Utils.CreateFolder(logFile);
            Configure("Capture.Workflow", logFile);
            Log.LogDebug += Log_LogDebug;
            Log.LogError += Log_LogError;
            Log.Debug("------------------------------===========================Application starting===========================------------------------------");
            try
            {
                Log.Debug("Application version : " + Assembly.GetEntryAssembly().GetName().Version);
                ServiceProvider.Instance.DeviceManager.AddFakeCamera();
                ServiceProvider.Instance.DeviceManager.ConnectToCamera();
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

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            QueueManager.Instance.Stop();
        }

    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Classes;
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
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;

            string procName = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name

            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                MessageBox.Show("Application already running !");
                Shutdown(-1);
                return;
            }
            Configure(Path.Combine(Settings.Instance.LogFolder, "Capture.Workflow.log"));
            GoogleAnalytics.Instance.TrackEvent("Application", "Start");
            WorkflowManager.Instance.LoadPlugins("Capture.Workflow.Plugins.dll");
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //#if DEBUG
            //      // In debug mode do not custom-handle the exception, let Visual Studio handle it

            //      e.Handled = false;

            //#else

            //          ShowUnhandeledException(e);    

            //#endif
            ShowUnhandeledException(e);
        }

        private void ShowUnhandeledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            Log.Error("Unhandled error ", e.Exception);
            string errorMessage =
                string.Format("Unhanded Exception {0}", e.Exception.Message + (e.Exception.InnerException != null
                                                        ? "\n" +
                                                          e.Exception.InnerException.Message
                                                        : null));

            GoogleAnalytics.Instance.TrackException(e.Exception.Message, true);

            if (e.Exception.GetType() == typeof(MissingMethodException))
            {
                Log.Error("Damaged installation. Application exiting ");
                MessageBox.Show(
                    "Application crash !! Damaged installation!\nPlease unintall the aplication from control panel and reinstall it!");
            }
            else
            {
                SendCrashReport(e.Exception);
                MessageBox.Show(errorMessage, "Application crash !!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Current?.Shutdown();
        }

        private void SendCrashReport(Exception e)
        {
            try
            {
                var body = "Version :" + Assembly.GetExecutingAssembly().GetName().Version + "\n" +
                       "Client Id" + (Settings.Instance.ClientId ?? "") + "\n" ;
                var error = "";
                if (e != null)
                {
                    error = e.Message;
                    body += e.StackTrace+"\n";
                    if (e.InnerException != null)
                    {
                        error = e.InnerException.Message;
                        body += "----------------------------------" + "\n";
                        body += e.InnerException.StackTrace;
                    }
                }
                Utils.SendEmail(body, "Capture.Workflow Crash report - "+error,"error_report@digicamcontrol.com" , "error_report@digicamcontrol.com",Path.Combine(Settings.Instance.LogFolder, "Capture.Workflow.log") );
            }
            catch (Exception )
            {
            }
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
                    MaximumFileSize = "10000KB",
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
            Settings.Instance.Save();
            GoogleAnalytics.Instance.TrackEvent("Application","Stop");
        }

    }
}

using System;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices
{
    public delegate void LogEventHandler(LogEventArgs e);

    public static class Log
    {
        public static event LogEventHandler LogDebug;
        public static event LogEventHandler LogError;
        public static event LogEventHandler LogInfo;



        public static void Debug(object message, Exception exception)
        {
            if (LogDebug != null)
                LogDebug(new LogEventArgs() { Exception = exception, Message = message });
        }

        public static void Debug(object message)
        {
            Debug(message, null);
        }

        public static void Error(object message, Exception exception)
        {
            if (LogError != null)
                LogError(new LogEventArgs() { Exception = exception, Message = message });
        }

        public static void Info(object message, Exception exception)
        {
            if (LogInfo != null)
                LogInfo(new LogEventArgs() { Exception = exception, Message = message });
        }

        public static void Error(object message)
        {
            Error(message, null);
        }


    }
}

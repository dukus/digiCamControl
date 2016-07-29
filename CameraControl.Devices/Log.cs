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
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices
{
    public delegate void LogEventHandler(LogEventArgs e);

    public static class Log
    {
        public static event LogEventHandler LogDebug;
        public static event LogEventHandler LogError;
        public static event LogEventHandler LogInfo;

        public static bool IsVerbose { get; set; } = false;

        /* The xxxWithWritexxx methods are for convenience instead of doing
         *    Console.WriteLine(String.Format("...format...", objects));
         *    Log.Verbose(String.Format("...format...", objects)); 
         * with all the overhead twice... */

        public static void Debug(object message, Exception exception)
        {
            if (LogDebug != null)
                LogDebug(new LogEventArgs() {Exception = exception, Message = message});
        }

        public static void Debug(object message)
        {
            Debug(message, null);
        }

        public static void Error(object message, Exception exception)
        {
            if (LogError != null)
                LogError(new LogEventArgs() {Exception = exception, Message = message});
        }

        public static void Error(object message)
        {
            Error(message, null);
        }

        public static void Info(object message, Exception exception)
        {
            if (LogInfo != null)
                LogInfo(new LogEventArgs() {Exception = exception, Message = message});
        }

        public static void Info(object message)
        {
            LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
        }

        public static void InfoWithWriteLine(object message)
        {
            Console.WriteLine(message);
            LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
        }

        public static void Verbose(object message)
        {
            if (IsVerbose)
                LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
        }

        public static void VerboseWithWriteLine(object message)
        {
            if (IsVerbose)
            {
                Console.WriteLine(message);
                LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
            }
        }

        public static void VerboseWithWriteLineAlways(object message)
        {
            Console.WriteLine(message);
            if (IsVerbose)
            {
                LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
            }
        }

        public static void VerboseWithWrite(object message)
        {
            if (IsVerbose)
            {
                Console.Write(message);
                LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
            }
        }

        public static void VerboseWithWriteAlways(object message)
        {
            Console.Write(message);
            if (IsVerbose)
            {
                LogInfo?.Invoke(new LogEventArgs() { Exception = null, Message = message });
            }
        }

    }
}
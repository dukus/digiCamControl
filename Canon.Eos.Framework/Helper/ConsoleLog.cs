using System;
using System.Text;
using Canon.Eos.Framework.Interfaces;

namespace Canon.Eos.Framework.Helper
{
    internal class ConsoleLog : IEosLog
    {
        public bool IsDebugEnabled { get; set; }
        
        public bool IsErrorEnabled { get; set; }

        public bool IsWarningEnabled { get; set; }

        public string LogProviderName
        {
            get { return this.GetType().FullName; }
        }

        public void Debug(string format, params object[] parameters)
        {
            if(this.IsDebugEnabled)
                Print(LogType.Debug, format, parameters);
        }

        public void Error(string format, params object[] parameters)
        {
            if(this.IsErrorEnabled || this.IsWarningEnabled || this.IsDebugEnabled)
                Print(LogType.Error, format, parameters);
        }

        public void Warn(string format, params object[] parameters)
        {
            if (this.IsWarningEnabled || this.IsDebugEnabled)
                Print(LogType.Warn, format, parameters);
        }

        private enum LogType { Debug, Error, Warn }

        private static void Print(LogType logType, string format, params object[] parameters)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(logType);
            stringBuilder.Append("::");
            stringBuilder.AppendFormat(format, parameters);
            Console.Error.WriteLine(stringBuilder);
        }
    }
}

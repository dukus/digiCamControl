namespace Canon.Eos.Framework.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEosLog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable debug logging
        /// </summary>
        /// <value>
        ///   <c>true</c> to enable log debug messages; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable error logging
        /// </summary>
        /// <value>
        ///   <c>true</c> to enable log error messages; otherwise, <c>false</c>.
        /// </value>
        bool IsErrorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable warning logging
        /// </summary>
        /// <value>
        ///   <c>true</c> to enable log warning messages; otherwise, <c>false</c>.
        /// </value>
        bool IsWarningEnabled { get; set; }

        /// <summary>
        /// Gets the name of the log provider.
        /// </summary>
        /// <value>
        /// The name of the log provider.
        /// </value>
        string LogProviderName { get; }

        /// <summary>
        /// Writes a debug message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void Debug(string format, params object[] parameters);

        /// <summary>
        /// Writes a error message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void Error(string format, params object[] parameters);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void Warn(string format, params object[] parameters);
    }
}

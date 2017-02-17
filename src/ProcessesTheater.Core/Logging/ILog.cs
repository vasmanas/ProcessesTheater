namespace ProcessesTheater.Core.Logging
{
    /// <summary>
    /// Interface for logging
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Only low level messages intended for debuging purposes.
        /// </summary>
        /// <param name="message"> Debug message. </param>
        void Debug(string message);

        /// <summary>
        /// Only low level messages intended for debuging purposes.
        /// </summary>
        /// <param name="message"> Debug message. </param>
        /// <param name="parameters"> Message format parameters. </param>
        void DebugFormat(string message, params object[] parameters);

        /// <summary>
        /// Only messages for tracking processes, but not details.
        /// </summary>
        /// <param name="message"> Information message. </param>
        void Info(string message);

        /// <summary>
        /// Only messages for tracking processes, but not details.
        /// </summary>
        /// <param name="message"> Information message. </param>
        /// <param name="parameters"> Message format parameters. </param>
        void InfoFormat(string message, params object[] parameters);

        /// <summary>
        /// Messages that warn of not propper use, but not of primary importance.
        /// </summary>
        /// <param name="message"> Warning message. </param>
        void Warn(string message);

        /// <summary>
        /// Messages that warn of not propper use, but not of primary importance.
        /// </summary>
        /// <param name="message"> Warning message. </param>
        /// <param name="parameters"> Message format parameters. </param>
        void WarnFormat(string message, params object[] parameters);

        /// <summary>
        /// Messages report of improper behavior in system.
        /// </summary>
        /// <param name="message"> Error message. </param>
        void Error(string message);

        /// <summary>
        /// Messages report of improper behavior in system.
        /// </summary>
        /// <param name="message"> Error message. </param>
        /// <param name="parameters"> Message format parameters. </param>
        void ErrorFormat(string message, params object[] parameters);
    }
}

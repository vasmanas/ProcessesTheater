namespace ProcessesTheater.Core.Logging
{
    /// <summary>
    /// Empty logger.
    /// </summary>
    public class EmptyLog : ILog
    {
        public void Debug(string message)
        {
        }

        public void DebugFormat(string message, params object[] parameters)
        {
        }

        public void Info(string message)
        {
        }

        public void InfoFormat(string message, params object[] parameters)
        {
        }

        public void Warn(string message)
        {
        }

        public void WarnFormat(string message, params object[] parameters)
        {
        }

        public void Error(string message)
        {
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
        }
    }
}

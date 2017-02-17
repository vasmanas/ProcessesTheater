using System;

namespace ProcessesTheater.Core.Logging
{
    /// <summary>
    /// Logging into console.
    /// </summary>
    public class ConsoleLog : ILog
    {
        public void Debug(string message)
        {
            Console.WriteLine("DEBUG: " + message);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            Console.WriteLine("DEBUG: " + message, parameters);
        }

        public void Info(string message)
        {
            Console.WriteLine("INFO: " + message);
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            Console.WriteLine("INFO: " + message, parameters);
        }

        public void Warn(string message)
        {
            Console.WriteLine("WARNING: " + message);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            Console.WriteLine("WARNING: " + message, parameters);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR: " + message);
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            Console.WriteLine("ERROR: " + message, parameters);
        }
    }
}

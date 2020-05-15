using System;
using System.IO;

namespace Logging
{
    public static class Log
    {

        #region Sub-Objects

        /// <summary>
        /// The different types of logs used for logging
        /// </summary>
        public enum LogTypes
        {
            None = 0,
            Warning = 1,
            Error = 2,
            Exception = 4,
            Message = 8
        }

        /// <summary>
        /// THe log event arguments class
        /// </summary>
        public class LogEventArgs : EventArgs
        {

            /// <summary>
            /// The log which occured
            /// </summary>
            public string Log { get; set; }

            /// <summary>
            /// The type of log which occured
            /// </summary>
            public LogTypes LogType { get; set; }

            /// <summary>
            /// The context in which the log occured
            /// </summary>
            public object LogContext { get; set; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate signature for event
        /// </summary>
        public delegate void LogOccuredEventHandler(LogEventArgs e);

        /// <summary>
        /// The event for when a log occurs
        /// </summary>
        public static event LogOccuredEventHandler LogOccured;

        #endregion

        #region Fields

        /// <summary>
        /// The format for how to print out the log date times
        /// </summary>
        private const string DATE_FORMAT = "MM/dd/yy|H:mm:ss.ffff|zzz";

        /// <summary>
        /// The stream writer which writes to the log
        /// </summary>
        private static StreamWriter writer;

        /// <summary>
        /// The allowed types of logging
        /// </summary>
        public static LogTypes allowed;

        /// <summary>
        /// Used to check if the stream has been set
        /// </summary>
        public static bool Streaming { get { return writer != null; } }

        /// <summary>
        /// Used to indicate if we additionally want to output to console
        /// </summary>
        public static bool ConsoleStream;

        #endregion

        #region Methods

        public static StreamWriter GetWriter()
        {
            return writer;
        }

        /// <summary>
        /// Sets the stream so that logging takes place
        /// <para></para>
        /// If a stream is already occuring, that stream gets closed, and replaced.
        /// <para></para>
        /// By default, all log types are enabled
        /// </summary>
        /// <param name="ToSet">The stream you would like the information to be logged to</param>
        /// <param name="AllowedLoggingTypes">The allowed log types</param>
        public static void SetStream(Stream ToSet, bool outputToConsole = false, LogTypes AllowedLoggingTypes = (LogTypes)15)
        {
            EndStream();
            allowed = AllowedLoggingTypes;
            writer = new StreamWriter(ToSet);
            ConsoleStream = outputToConsole;
        }

        /// <summary>
        /// Stops the stream(if one exists)
        /// </summary>
        public static void EndStream()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Writes a warning to the current log stream
        /// </summary>
        /// <param name="message">The message to be used for this warning</param>
        /// <param name="context">The context should you want this ot be passed</param>
        public static void Warning(string message, object context = null)
        {
            if ((allowed & LogTypes.Warning) == LogTypes.None)
                return;
            if (!Streaming)
                return;
            DateTime now = DateTime.Now;
            string line = now.ToString(DATE_FORMAT) + " - {Warning}||";
            line += "\t" + message;
            if (context != null)
                line += "\n{Context}" + context.ToString();
            if (ConsoleStream)
                Console.WriteLine(line);
            writer.WriteLine(line);
            writer.Flush();
            LogOccured?.Invoke(new LogEventArgs() { Log = message, LogContext = context, LogType = LogTypes.Warning });

        }

        /// <summary>
        /// Writes an Error to the current log stream
        /// </summary>
        /// <param name="message">The message to be used for this Error</param>
        /// <param name="context">The context should you want this ot be passed</param>
        public static void Error(string message, object context = null)
        {
            if ((allowed & LogTypes.Error) == LogTypes.None)
                return;
            if (!Streaming)
                return;
            DateTime now = DateTime.Now;
            string line = now.ToString(DATE_FORMAT) + " - {Error}||";
            line += "\t" + message;
            if (context != null)
                line += "\n{Context}" + context.ToString();
            if (ConsoleStream)
                Console.WriteLine(line);
            writer.WriteLine(line);
            writer.Flush();
            LogOccured?.Invoke(new LogEventArgs() { Log = message, LogContext = context, LogType = LogTypes.Error });
        }

        /// <summary>
        /// Writes an Exception to the current log stream
        /// </summary>
        /// <param name="message">The message to be used for this exception</param>
        /// <param name="context">The context should you want this ot be passed</param>
        public static void Exception(Exception e, object context = null)
        {
            if ((allowed & LogTypes.Exception) == LogTypes.None)
                return;
            if (!Streaming)
                return;
            DateTime now = DateTime.Now;
            string line = now.ToString(DATE_FORMAT) + " - {Exception}||";
            line += "\t" + e.ToString();
            if (context != null)
                line += "\n{Context}" + context.ToString();
            if (ConsoleStream)
                Console.WriteLine(line);
            writer.WriteLine(line);
            writer.Flush();
            LogOccured?.Invoke(new LogEventArgs() { Log = e.Message, LogContext = context, LogType = LogTypes.Exception });
        }

        /// <summary>
        /// Writes a message to the current log stream
        /// </summary>
        /// <param name="message">The message to be used for this warning</param>
        /// <param name="context">The context should you want this ot be passed</param>
        public static void Message(string message, object context = null)
        {
            if ((allowed & LogTypes.Message) == LogTypes.None)
                return;
            if (!Streaming)
                return;
            DateTime now = DateTime.Now;
            string line = now.ToString(DATE_FORMAT) + " - {Message}||";
            line += "\t" + message;
            if (context != null)
                line += "\n{Context}" + context.ToString();
            if (ConsoleStream)
                Console.WriteLine(line);
            writer.WriteLine(line);
            writer.Flush();
            LogOccured?.Invoke(new LogEventArgs() { Log = message, LogContext = context, LogType = LogTypes.Message });
        }

        #endregion

    }
}


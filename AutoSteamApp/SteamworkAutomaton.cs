using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSteamApp
{
    public class SteamworkAutomaton
    {

        public SteamworkAutomaton()
        {
            SetConsoleTitle();
            SetLogger();
        }

        /// <summary>
        /// Sets the console title of the app
        /// </summary>
        static void SetConsoleTitle()
        {
            Version ver = AutomatonConfiguration.ApplicationVersion;
            string title = "Steamworks Automaton V:" + ver.Major + "." + ver.Minor;
            title += "        ";
            title += "Supported MHW:IB Version: " + MHWMemoryValues.SupportedGameVersion;
            Console.Title = title;
        }

        /// <summary>
        /// Sets the proper log methods depending on the configuration found in the .config file
        /// </summary>
        static void SetLogger()
        {
            // Initialize the different log types
            Log.LogTypes LoggingTypes = Log.LogTypes.Message | Log.LogTypes.Error;

            // If we're in debug mode, heighten the logging which takes place
            if (AutomatonConfiguration.IsDebug)
            {
                LoggingTypes |= Log.LogTypes.Debug | Log.LogTypes.Exception | Log.LogTypes.Warning;

                // Check if we need to write logs to a file
                string logFile = AutomatonConfiguration.LogFile;
                if (!string.IsNullOrEmpty(logFile))
                {
                    // Try to create a stream using the log file
                    try
                    {
                        FileStream logStream = File.Create(logFile);
                        Log.SetStream(logStream, true, LoggingTypes);
                        return;
                    }
                    // In the event of failure, log to console instead
                    catch (Exception e)
                    {
                        Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
                        Log.Exception(e, logFile);
                        return;
                    }
                }
                // If no log file is specified, log to console instead
                Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
                Log.Warning("No log file specified. All logs written to console.");
                return;
            }
            // If not in debug mode, write messages to console
            Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
        }

        public void Run(CancellationToken cts)
        {
            Console.WriteLine("Error occured, exiting");
            Thread.Sleep(10000);
            Process p = StaticHelpers.GetMHWProcess();
            Environment.Exit(1);
        }
    }
}
using AutoSteamApp.Core;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public static class StaticHelpers
    {
        /// <summary>
        /// Returns the process which contains the process name as defined in the fields
        /// </summary>
        /// <returns></returns>
        public static Process GetMHWProcess()
        {
            //Retrieve all processes with defined process name
            var processes = Process.GetProcessesByName(AutomatonConfiguration.ProcessName);
            // Try to return the first one
            return processes.First(p => p != null && p.ProcessName.Equals(AutomatonConfiguration.ProcessName) && !p.HasExited);
        }

        /// <summary>
        /// Attempts to load a config file if one is supplied.
        /// </summary>
        /// <param name="args"></param>
        public static void SetConfig(string config)
        {
            // Set the configuration file to the specified path defined in the cmd line args
            AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = config;

            // If an incorrect config file is loaded, exit the application
            if (!AutomatonConfiguration.ConfigLoadedProperly)
            {
                Console.WriteLine("Defined config file could not be found. Defaulting to original.");
                AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = ".config";
            }
        }

        /// <summary>
        /// Sets the proper log methods depending on the configuration found in the .config file
        /// </summary>
        public static void SetLogger()
        {
            // Initialize the different log types
            Log.LogTypes LoggingTypes = Log.LogTypes.Message | Log.LogTypes.Warning;

            // If we're in debug mode, heighten the logging which takes place
            if (AutomatonConfiguration.IsDebug)
            {
                LoggingTypes |= Log.LogTypes.Debug | Log.LogTypes.Exception | Log.LogTypes.Error;

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

        /// <summary>
        /// Sets the console title of the app
        /// </summary>
        public static void SetConsoleTitle()
        {
            Version ver = AutomatonConfiguration.ApplicationVersion;
            string title = "Steamworks Automaton V:" + ver.Major + "." + ver.Minor;
            title += "        ";
            title += "Supported MHW:IB Version: " + MHWMemoryValues.SupportedGameVersion;
            Console.Title = title;
        }
    }
}

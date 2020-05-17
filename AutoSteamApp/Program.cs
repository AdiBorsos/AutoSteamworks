using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using AutoSteamApp.Process_Memory;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Keystroke.API;
using Logging;

namespace AutoSteamApp
{

    class Program
    {

        #region Main

        static void Main(string[] args)
        {

            // Attempts to load an external config file if one is supplied
            if (args.Length > 0)
            {
                SetConfig(args[0]);
            }

            // Set the console title
            SetConsoleTitle();

            // Initialize the logger
            SetLogger();

            /*
             * TODO: Print out some config details to the user for confirmation
             */

            // Wait for user input to signal ready
            Log.Message("Press Any Key to begin.");
            Console.ReadKey();

            // Create the automaton
            SteamworkAutomaton automaton = new SteamworkAutomaton();

            // Create a cancellation token for the thread
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Spawn a task to do the work in a separate thread
            Task t = Task.Run(() => { automaton.Run(token); });

            Log.Message("Enter quit to stop");
            while (Console.ReadLine() != "quit")
            {
                // Wait for exit command
            }

            Log.Message("Waiting for thread to exit.");
            //When exit is invoked, cancel the token
            cts.Cancel();

            // Wait for the thread to finish.
            t.Wait();
            Log.Message("Exiting.");

            // Quit, as well as all underlying threads
            Environment.Exit(0);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Attempts to load a config file if one is supplied.
        /// </summary>
        /// <param name="args"></param>
        private static void SetConfig(string config)
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

        #endregion

    }
}

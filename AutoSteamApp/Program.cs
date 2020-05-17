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
                StaticHelpers.SetConfig(args[0]);
            }

            // Set the console title
            StaticHelpers.SetConsoleTitle();

            // Initialize the logger
            StaticHelpers.SetLogger();

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

    }
}

using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using Logging;
using System;

namespace AutoSteamApp
{
    public class SteamAutomater
    {
        public void Init()
        {
            SetConsoleTitle();

            // Set the logging properties
            Log.LogTypes LoggingTypes = Log.LogTypes.Message | Log.LogTypes.Error;

            // If we're in debug mode, heighten the logging which takes place
            if (AutomatorConfiguration.IsDebug)
                LoggingTypes |= Log.LogTypes.Debug | Log.LogTypes.Exception | Log.LogTypes.Warning;
            Log.SetStream(null, true, LoggingTypes);

            Log.Debug("Debugging~!");
            Log.Message("Message~!");

        }

        /// <summary>
        /// Sets the console title of the app
        /// </summary>
        public static void SetConsoleTitle()
        {
            Version ver = AutomatorConfiguration.ApplicationVersion;
            string title = "Steamworks Automaton V:" + ver.Major + "." + ver.Minor;
            title += "        ";
            title += "Supported MHW:IB Version: " + MHWMemoryValues.SupportedGameVersion;
            Console.Title = title;
        }

    }
}

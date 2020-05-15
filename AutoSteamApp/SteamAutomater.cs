using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using Logging;
using System;

namespace AutoSteamApp
{
    public class SteamAutomater
    {

        private void Init()
        {
            SetConsoleTitle();
            if(StaticHelpers.IsDebug)
                Log.SetStream(Console.OpenStandardOutput(), true);
            Console.WriteLine(string.Empty);

            Console.WriteLine(
                string.Format(
                    "Based on the current settings, this run will consume: {0} fuel. If this was not intended, please change AutoSteamApp.exe.config.",
                    Settings.ShouldConsumeAllFuel ? "ALL the available" : "ONLY the Natural"));
            Console.WriteLine(string.Empty);

            WriteSeparator();
            Console.WriteLine($"Please select the type of run you want. When the run is finished, the app will close.");

            WriteSeparator();
            Console.WriteLine($"Press '{((KeyCode)Settings.KeyCodeStart).ToString()}' to ->");
            Console.WriteLine($"        Run with 100% Accuracy (requires correct game version)");

            Console.WriteLine($"Press '{((KeyCode)Settings.KeyCodeStartRandom).ToString()}' to ->");
            Console.WriteLine($"        Do a RANDOM run. This method will give unpredictable results, there is no check for values.");
            WriteSeparator();

            Console.WriteLine($"Press '{((KeyCode)Settings.KeyCodeStop).ToString()}' to end typing");
        }


        public static void SetConsoleTitle()
        {
            Version ver = StaticHelpers.ApplicationVersion;
            string title = "Steamworks Automaton V:" + ver.Major + "." + ver.Minor;
            title += "\t";
            title += "Supported MHW:IB Version: " + MHWMemoryValues.SupportedGameVersion;
            Console.Title = title;
        }


    }
}

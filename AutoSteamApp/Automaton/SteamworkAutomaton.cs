using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using AutoSteamApp.Process_Memory;
using GregsStack.InputSimulatorStandard;
using Logging;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace AutoSteamApp
{

    public class SteamworkAutomaton
    {

        #region Fields

        /// <summary>
        /// Process loaded in the constructor.
        /// </summary>
        Process _Process;

        /// <summary>
        /// Save data loaded in the constructor.
        /// </summary>
        SaveData _SaveData;

        /// <summary>
        /// Steamworks data loaded in constructor.
        /// </summary>
        SteamworksData _SteamworksData;

        /// <summary>
        /// Field used to flag whether or not the currently running MHW:IB version is supported.
        /// </summary>
        bool _SupportedVersion = false;

        /// <summary>
        /// The input simulator used to mock button presses.
        /// </summary>
        InputSimulator _InputSimulator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor which loads all required process memory and configuration values.
        /// </summary>
        public SteamworkAutomaton()
        {
            try
            {
                LoadAndVerifyProcess();
                _SteamworksData = new SteamworksData(_Process);
                _SaveData = new SaveData(_Process);
                _InputSimulator = new InputSimulator();
            }
            catch (Exception e)
            {
                Log.Exception(new Exception("Failed to initialze Steamworks Automaton\n\t", e));
                Log.Warning("Something went wrong reading the MHW:IB process. Press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the main logic loop of reading the steamworks sequence, 
        /// then performing actions depending on the configurations.
        /// </summary>
        /// <param name="cts">Cancellation token used to signal an exit.</param>
        public void Run(CancellationToken cts)
        {
            try
            {
            }
            catch (Exception e)
            {
                Log.Exception(new Exception("Failed reading/inputting sequence\n\t", e));
                Log.Warning("Something went wrong trying to automate the steamworks. Press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads and verifies the current MHW:IB process against the currently supported version.
        /// </summary>
        /// TODO: Make it more oo by making this the constructor for an abstract automaton.
        /// Then have inhertied automatons work based on different versions.
        /// This would allow for backward compatability
        /// 
        private void LoadAndVerifyProcess()
        {
            Log.Message("Attempting to load MHW:IB Process.");
            // First set the mhw process
            _Process = StaticHelpers.GetMHWProcess();

            Log.Message("Process Loaded. Retrieving version");
            // Now verify the version
            Match match = Regex.Match(_Process.MainWindowTitle, MHWMemoryValues.SupportedVersionRegex);
            // If the match is made
            if (match.Success)
                // And we have a capture group
                if (match.Groups.Count > 1)
                    // Try to turn it into a number
                    if (int.TryParse(match.Groups[1].Value, out int result))
                    {
                        Log.Message("Version found: " + result);
                        // If it is a numeber and is the same as the supported version
                        if (result == MHWMemoryValues.SupportedGameVersion)
                        {
                            // Set the flag
                            _SupportedVersion = true;
                            return;
                        }
                        Log.Warning(
                            "Version unsupported. Currently supported version: " + MHWMemoryValues.SupportedGameVersion +
                            "\n\t\tAutomaton will still run, however, correct sequences cannot be read"
                                    );
                        return;
                    }
            Log.Error(
                "Could not verify game version. This is most likely due to a different versioning system being used by Capcom." +
                "\n\t\tAutomaton will still run, however, correct sequences cannot be read"
                     );
        }

        #endregion

    }

}
using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using AutoSteamApp.Process_Memory;
using Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSteamApp
{
    public class SteamworkAutomaton
    {

        /// <summary>
        /// Process loaded in the constructor.
        /// </summary>
        Process mhwProcess;

        /// <summary>
        /// Save data loaded in the constructor.
        /// </summary>
        SaveData saveData;

        /// <summary>
        /// Field used to flag whether or not the currently running MHW:IB version is supported.
        /// </summary>
        bool SupportedVersion = false;

        /// <summary>
        /// Constructor which loads all required process memory and configuration values.
        /// </summary>
        public SteamworkAutomaton()
        {
            try
            {
                LoadAndVerifyProcess();
            }
            catch (Exception e)
            {
                Exception error = new Exception("Failed to initialze Steamworks Automaton.", e);
                Log.Exception(error);
                throw error;
            }
        }

        public void Run(CancellationToken cts)
        {

            Environment.Exit(1);
        }

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
            // First set the mhw process
            mhwProcess = StaticHelpers.GetMHWProcess();

            // Now verify the version
            Match match = Regex.Match(mhwProcess.MainWindowTitle, AutomatonConfiguration.SupportedVersionRegex);
            // If the match is made
            if (match.Success)
                // And we have a capture group
                if (match.Groups.Count > 1)
                    // Try to turn it into a number
                    if (int.TryParse(match.Groups[1].Value, out int result))
                        // If it is a numeber and is the same as the supported version
                        if (result == AutomatonConfiguration.SupportedGameVersion)
                            // Set the flag
                            SupportedVersion = true;
        }

        #endregion

    }
}
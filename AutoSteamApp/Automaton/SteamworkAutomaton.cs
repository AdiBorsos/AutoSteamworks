using AutoSteamApp.Helpers;
using AutoSteamApp.ProcessMemory;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Logging;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;

namespace AutoSteamApp.Automaton
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
                Environment.Exit(1);
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
                while (!cts.IsCancellationRequested)
                {
                    CheckSteamworksState(cts);
                    ExtractAndEnterSequence(cts);
                }
            }
            catch (Exception e)
            {
                Log.Exception(new Exception("Failed automating steamworks\n\t", e));
                Log.Warning("Something went wrong trying to automate the steamworks. Press any key to exit");
                Console.ReadKey();
                Environment.Exit(1);
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

        /// <summary>
        /// Extracts from the process the correct sequence to input, then inputs it.
        /// </summary>
        /// <param name="cts">Cancellation token used to signal when to stop.</param>
        void ExtractAndEnterSequence(CancellationToken cts)
        {
            try
            {
                // Generate a sequence to input
                VirtualKeyCode[] sequence;
                if (_SupportedVersion)
                    // If the version is supported, use the extracted sequence
                    sequence = _SteamworksData.ExtractSequence();
                else
                {
                    // If the version is unsuported, use a random sequence
                    sequence = StaticHelpers.RandomSequence();
                    // Press the buttons and wait 30ms then press start (in case the cutscene plays)
                    // Press the keys
                    StaticHelpers.PressKey(_InputSimulator, sequence[0]);
                    Thread.Sleep(50);
                    StaticHelpers.PressKey(_InputSimulator, sequence[1]);
                    Thread.Sleep(50);
                    StaticHelpers.PressKey(_InputSimulator, sequence[2]);
                    Thread.Sleep(50);
                    StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE);
                    return;
                }
                if (sequence == null)
                {
                    Log.Debug("Could not find a valid sequence. Are you sure you are in the game?");
                    Thread.Sleep(1000);
                    return;
                }
                // For each key in the sequence
                for (int i = 0; i < sequence.Length; i++)
                {
                    // Record our pre-registered value for input
                    byte beforeKeyPressValue = _SteamworksData.InputPressStateCheck;
                    byte afterKeyPressValue = beforeKeyPressValue;
                    // While our input has not been recognized and we haven't been signalled to quit
                    while (afterKeyPressValue == beforeKeyPressValue && !cts.IsCancellationRequested)
                    {
                        // Wait until we have focus
                        if (!_Process.HasFocus())
                            Log.Message("Waiting for MHW to have focus.");
                        while (!_Process.HasFocus() && !cts.IsCancellationRequested) { };

                        // Press the key
                        StaticHelpers.PressKey(_InputSimulator, sequence[i]);
                        afterKeyPressValue = _SteamworksData.InputPressStateCheck;
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error in extracting and entering sequence.", e);
            }
        }

        /// <summary>
        /// Method used to ensure synchronization with the steamworks process and the inputting of key codes.
        /// <para></para>
        /// Kudos to <a href="https://github.com/UNOWEN-OwO">UNOWEN-OwO</a> for his work on this
        /// </summary>
        /// <param name="cts">Cancellation token used to signal when to stop.</param>
        private void CheckSteamworksState(CancellationToken cts)
        {
            try
            {
                // Check the current Button Press Check Value
                byte currentButtonPressState = _SteamworksData.InputPressStateCheck;
                // While the current game input state does not signify the beginning of the gamew
                while (currentButtonPressState != (byte)ButtonPressedState.Beginning && 
                       !cts.IsCancellationRequested)
                {

                    if (currentButtonPressState == (byte)ButtonPressedState.End)
                    {
                        // While we are not waiting for input, it means we are doing something else so wait
                        while (_SteamworksData.PhaseValue != (byte)PhaseState.WaitingForInput && 
                               !cts.IsCancellationRequested)
                        {

                            // If we're in the cutscene phase, press the skip cutscene key
                            while (_SteamworksData.PhaseValue == (byte)PhaseState.Cutscene && 
                                   !cts.IsCancellationRequested)
                            {
                                StaticHelpers.PressKey(_InputSimulator, (VirtualKeyCode)ConfigurationReader.KeyCutsceneSkip, 100);
                                Thread.Sleep(100);
                            }

                            // If the "Press start" is being shown press space
                            while (_SteamworksData.PhaseValue == (byte)PhaseState.Fuel && 
                                   !cts.IsCancellationRequested)
                            {
                                StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                Thread.Sleep(100);
                            }

                            // If the rewards phase is being shown press escape
                            while (_SteamworksData.PhaseValue == (byte)PhaseState.Rewards && 
                                   !cts.IsCancellationRequested)
                            {
                                StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.ESCAPE, 100);
                                Thread.Sleep(100);
                            }

                            // While in the bonus or settled phase
                            // What this stage essentially does is upon winning, waits for all the rewards
                            // to be shown, then exits the minigame, and rejoins it
                            bool cleared = false;
                            while (_SteamworksData.PhaseValue == (byte)PhaseState.Settled ||
                                   _SteamworksData.PhaseValue == (byte)PhaseState.Bonus && 
                                   !cts.IsCancellationRequested)
                            {
                                // If we've cleared the screen press space to start
                                if (cleared)
                                {
                                    StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                    Thread.Sleep(100);
                                }

                                // Otherwise we exit out of the menu, and replay the game. Refer to https://github.com/UNOWEN-OwO
                                // Here we need to refer to the second phase check because the game is technically over.
                                else
                                {
                                    // While settled or bonus and in rewards have finished
                                    while ((_SteamworksData.PhaseValue == (byte)PhaseState.Settled ||
                                            _SteamworksData.PhaseValue == (byte)PhaseState.Bonus) &&
                                            _SteamworksData.SecondPhaseValue == 1 && 
                                            !cts.IsCancellationRequested)
                                    {
                                        StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.LEFT, 100);
                                        Thread.Sleep(1000);
                                        StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                        Thread.Sleep(1000);
                                    }
                                    // This escape command resets the timing properly after winning.
                                    // If this doesn't occur, read values think we are waiting to collect our reward still
                                    while (_SteamworksData.PhaseValue != (byte)PhaseState.Idle &&
                                           _SteamworksData.PhaseValue != (byte)PhaseState.Fuel &&
                                           _SteamworksData.SecondPhaseValue == 0 && 
                                           !cts.IsCancellationRequested)
                                    {
                                        StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.ESCAPE, 100);
                                        Thread.Sleep(1000);
                                    }
                                }
                            }

                            // If we're in idle phase, press the space button three times
                            // No ide why this is needed. Refer to https://github.com/UNOWEN-OwO
                            while (_SteamworksData.PhaseValue == (byte)PhaseState.Idle)
                            {
                                StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                Thread.Sleep(1000);
                                StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                Thread.Sleep(500);
                                StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, 100);
                                Thread.Sleep(100);
                                cleared = true;
                            }
                        }
                    }
                    // Reread the current button press state
                    currentButtonPressState = _SteamworksData.InputPressStateCheck;
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error in checking steamworks state", e);
            }
        }

        #endregion

    }

}
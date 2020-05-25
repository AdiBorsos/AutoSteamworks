using AutoSteamApp.Configuration;
using AutoSteamApp.Helpers;
using AutoSteamApp.ProcessMemory;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AutoSteamApp.Automaton
{

    public class SteamworkAutomaton
    {

        #region Fields

        /// <summary>
        /// Process loaded in the constructor.
        /// </summary>
        private readonly Process _Process;

        /// <summary>
        /// Save data loaded in the constructor.
        /// </summary>
        private SaveData _SaveData;

        /// <summary>
        /// Steamworks data loaded in constructor.
        /// </summary>
        private SteamworksData _SteamworksData;

        /// <summary>
        /// Field used to flag whether or not the currently running MHW:IB version is supported.
        /// </summary>
        private bool _SupportedVersion = false;

        /// <summary>
        /// The input simulator used to mock button presses.
        /// </summary>
        private readonly InputSimulator _InputSimulator;

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
                if (_SupportedVersion && !ConfigurationReader.RandomRun)
                {
                    _SteamworksData = new SteamworksData(_Process);
                    _SaveData = new SaveData(_Process);
                }
                _InputSimulator = new InputSimulator();
            }
            catch (Exception e)
            {
                Log.Exception(new Exception("Failed to initialze Steamworks Automaton\n\t", e));
                Log.Warning("Something went wrong reading the MHW:IB process.");
                if (!ConfigurationReader.ShouldAutoQuit)
                {
                    Log.Message("Press any key to exit.");
                    Console.ReadKey();
                }
                Environment.Exit(-1);

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
                    if (_SupportedVersion && !ConfigurationReader.RandomRun)
                    {
                        // If we have satisfied all exit conditions
                        if (CheckForExitCondition())
                            // If we should auto exit, quit immediately
                            if (ConfigurationReader.ShouldAutoQuit)
                                Environment.Exit(0);
                            // Otherwise wait for quit to be typed
                            else
                                return;

                        // Otherwise we need to extract the sequence
                        ExtractAndEnterSequence(cts);

                        // Then check the steamworks state
                        CheckSteamworksState(cts);
                    }
                    else
                    {
                        EnterRandomSequence(cts);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(new Exception("Failed automating steamworks\n\t", e));
                Log.Warning("Something went wrong trying to automate the steamworks.");
                // If we should auto exit, quit immediately
                if (ConfigurationReader.ShouldAutoQuit)
                    Environment.Exit(-1);
                // Otherwise wait for quit to be typed
                else
                    return;
            }
        }

        private void EnterRandomSequence(CancellationToken cts)
        {
            // Generate a sequence to input
            VirtualKeyCode[] sequence = StaticHelpers.GetRandomSequence();
            // If the version is unsuported, use a random sequence

            // Press the buttons and wait 30ms then press start (in case the cutscene plays)
            Log.Debug("Random Sequence: [" + string.Join(", ", sequence.Select(x => x.ToString())) + "]");
            // Wait until we have focus
            if (!_Process.HasFocus())
                Log.Message("Waiting for MHW to have focus.");
            while (!_Process.HasFocus() && !cts.IsCancellationRequested) { };

            for (int i = 0; i < sequence.Length; i++)
            {
                StaticHelpers.PressKey(_InputSimulator, sequence[i], ConfigurationReader.RandomInputDelay);
                Thread.Sleep(ConfigurationReader.RandomInputDelay);
            }
            StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, ConfigurationReader.RandomInputDelay);
            Thread.Sleep(ConfigurationReader.RandomInputDelay);
            StaticHelpers.PressKey(_InputSimulator, ConfigurationReader.KeyCutsceneSkip, ConfigurationReader.RandomInputDelay);
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Checks if we have met the satisfying conditions provided by the config file.
        /// </summary>
        /// <returns></returns>
        private bool CheckForExitCondition()
        {
            if (ConfigurationReader.OnlyUseNaturalFuel)
            {
                // true if we have either equal to or less fuel than specified in the config file.
                if (_SaveData.NaturalFuelLeft <= ConfigurationReader.StopAtFuelAmount)
                {
                    Log.Message("Hit minimum natural fuel reserve.");
                    return true;
                }
                return false;
            }
            else
            {
                // true if we have either equal to or less fuel than specified in the config file.
                if (_SaveData.StoredFuelLeft <= ConfigurationReader.StopAtFuelAmount)
                {
                    Log.Message("Hit minimum stored fuel reserve.");
                    return true;
                }
                return false;
            }
        }

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
        private void ExtractAndEnterSequence(CancellationToken cts)
        {
            try
            {
                // Generate a sequence to input
                VirtualKeyCode[] sequence;

                // If the version is supported, use the extracted sequence
                sequence = _SteamworksData.ExtractSequence();

                if (sequence == null)
                {
                    Log.Debug("Could not find a valid sequence. Are you sure you are in the game?");
                    Thread.Sleep(1000);
                    return;
                }

                //Here we need to check the probability of us winning based on the rarity of the reward
                float probability = ConfigurationReader.CommonSuccessRate;
                if (_SteamworksData.RewardRarityValue == RewardRarity.Rare)
                {
                    Log.Debug("Rare reward detected.");
                    probability = ConfigurationReader.RareSuccessRate;
                }

                // Use rng to check if we win or not
                // TODO: maybe not create a new instance every time? I'll need to consult with someone about the probability distribution
                // or System.Random when doing it this way.
                Random rng = new Random();

                // If we fail the rng check, reverse the inputs
                if (rng.NextDouble() > probability)
                {
                    Log.Debug("Failed rng check. shifting sequence to guarantee incorrect input.");
                    sequence = new VirtualKeyCode[] { sequence[1], sequence[2], sequence[0] };
                    // Sometimes the input being shifted doesnt change the input?
                    Thread.Sleep(50);
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
            catch (Exception e)
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
                // While we are not in the input mode
                while (currentButtonPressState != (byte)ButtonPressedState.Beginning &&
                       !cts.IsCancellationRequested)
                {
                    // Sleep so the animation plays a bit
                    Thread.Sleep(50);
                    // Then press skip cutscene with a minor delay
                    StaticHelpers.PressKey(_InputSimulator, ConfigurationReader.KeyCutsceneSkip, ConfigurationReader.RandomInputDelay);

                    // If the cutscene is over
                    if (currentButtonPressState == (byte)ButtonPressedState.End)
                    {
                        // When the steam gauge has reset, it means we can press space to start again.
                        if (_SteamworksData.SteamGuageValue == 0)
                            StaticHelpers.PressKey(_InputSimulator, VirtualKeyCode.SPACE, ConfigurationReader.RandomInputDelay);
                    }
                    // Reread the current button press state
                    currentButtonPressState = _SteamworksData.InputPressStateCheck;

                    // Wait until we have focus
                    if (!_Process.HasFocus())
                        Log.Message("Waiting for MHW to have focus.");
                    while (!_Process.HasFocus() && !cts.IsCancellationRequested) { };

                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in checking steamworks state", e);
            }
        }

        #endregion

    }

}
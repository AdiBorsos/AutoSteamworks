using AutoSteamApp.Helpers;
using GregsStack.InputSimulatorStandard.Native;
using Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace AutoSteamApp.ProcessMemory
{

    public class SteamworksData
    {

        #region Fields

        /// <summary>
        /// The Monster Hunter World: Iceborne process to load player data from.
        /// </summary>
        private readonly Process MHWProcess;

        /// <summary>
        /// The address which begins the "steamworks" object.
        /// </summary>
        ulong SteamworksAddress;

        /// <summary>
        /// Address which holds the current expected sequence of the game.
        /// </summary>
        ulong SequenceAddress;

        /// <summary>
        /// Address which indicates whether the button input was registered.
        /// </summary>
        ulong ButtonPressedCheckAddress;

        /// <summary>
        /// Address used to read what phase the steamworks is in. 
        /// </summary>
        ulong PhaseAddress;

        /// <summary>
        /// Address used to read what sub-phase the steamworks is in. When in the bonus or settled phase.
        /// </summary>
        ulong SecondPhaseAddress;

        /// <summary>
        /// Address used to read the rarity of the reward.
        /// </summary>
        ulong RarityAddress;

        /// <summary>
        /// Address used to read the current steam gauge progress.
        /// </summary>
        ulong SteamGaugeAddress;
        
        #endregion

        #region Properties

        /// <summary>
        /// The value used to check whether the game has registered the input press.
        /// </summary>
        public byte InputPressStateCheck
        {
            get
            {
                return MemoryHelper.Read<byte>(MHWProcess, ButtonPressedCheckAddress);
            }
        }

        /// <summary>
        /// what phase the steamworks is in. 
        /// </summary>
        public byte PhaseValue
        {
            get
            {
                return MemoryHelper.Read<byte>(MHWProcess, PhaseAddress);
            }
        }

        /// <summary>
        /// what phase the steamworks is in when in the main menu of the steamworks.
        /// </summary>
        public byte SecondPhaseValue
        {
            get
            {
                return MemoryHelper.Read<byte>(MHWProcess, SecondPhaseAddress);
            }
        }

        /// <summary>
        /// what phase the steamworks is in when in the main menu of the steamworks.
        /// </summary>
        public RewardRarity RewardRarityValue
        {
            get
            {
                return (RewardRarity)MemoryHelper.Read<byte>(MHWProcess, RarityAddress);
            }
        }

        /// <summary>
        /// the current progress of the steam gauge.
        /// </summary>
        public short SteamGuageValue
        {
            get
            {
                return MemoryHelper.Read<short>(MHWProcess, SteamGaugeAddress);
            }
        }

        #endregion

        #region Constructor

        public SteamworksData(Process mhwProcess)
        {
            Log.Debug("Loading Steamworks data values");
            MHWProcess = mhwProcess;
            LoadData();
            Log.Debug("Steamworks data values loaded.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads/Reloads the Save Data of the current MHW Process
        /// </summary>
        public void LoadData()
        {
            // Load the address of the steamworks data
            SteamworksAddress = MemoryHelper.Read<ulong>(MHWProcess, MHWMemoryValues.SteamworksDataPointer);
            Log.Debug("Steamworks Address: " + SteamworksAddress);
            // Offset to find the sequence address
            SequenceAddress = SteamworksAddress + MHWMemoryValues.OffsetToSequence;
            Log.Debug("Sequence Address: " + SequenceAddress);
            // Offset to find the button pressed check address
            ButtonPressedCheckAddress = SteamworksAddress + MHWMemoryValues.OffsetToButtonCheck;
            Log.Debug("Button-Pressed-Check Address: " + ButtonPressedCheckAddress);
            // Offset to find the phase address
            PhaseAddress = SteamworksAddress + MHWMemoryValues.OffsetToSteamPhase;
            Log.Debug("Phase Address: " + PhaseAddress);
            // Offset to find the secondary phase check address
            SecondPhaseAddress = SteamworksAddress + MHWMemoryValues.OffsetToSteamSecondPhase;
            Log.Debug("2nd Phase Address: " + SecondPhaseAddress);
            // Offset to find the rarity
            RarityAddress = SteamworksAddress + MHWMemoryValues.OffsetToGameRarity;
            Log.Debug("Rarity Address: " + RarityAddress);
            SteamGaugeAddress = SteamworksAddress + MHWMemoryValues.OffsetToSteamGauge;
            Log.Debug("Steam Gauge Address: " + SteamGaugeAddress);
        }

        /// <summary>
        /// Returns a tuple of keycodes to press corresponding to the correct sequence of moves .
        /// </summary>
        /// <returns>A Tuple </returns>
        public VirtualKeyCode[] ExtractSequence()
        {
            // Read the byte representation of the sequence from the game
            byte[] sequence = new byte[3];
            sequence[0] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress);
            sequence[1] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress + 1);
            sequence[2] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress + 2);

            // return null if they are are all zero (indicates the minigame hasnt started)
            if (sequence.AllIdentical())
                if (sequence[0] == 0)
                    return null;
                else
                    throw new Exception("Error interpretting sequence [" + string.Join(", ", sequence.Select(x => x.ToString())) + "]");

            // Capcom has likely done some manipulation so we need to do some twidling because the sequences aren't as straightforward as we'd like.
            // Kudos to https://github.com/Geobryn for figuring this out,
            if (sequence[0] == 2 && sequence[1] == 0 && sequence[2] == 1)
            {
                sequence[0] = 1;
                sequence[1] = 2;
                sequence[2] = 0;
            }
            else
                if (sequence[0] == 1 && sequence[1] == 2 && sequence[2] == 0)
            {
                sequence[0] = 2;
                sequence[1] = 0;
                sequence[2] = 1;
            }

            // Return the array of key code presses extracted from the bytes
            return StaticHelpers.KeyCodeSequenceFromBytes(sequence);
        }

        #endregion

    }

    /// <summary>
    /// Enum used for determining which state of button press are we in
    /// </summary>
    public enum ButtonPressedState
    {
        Beginning = 0,
        End = 7
    }

    /// <summary>
    /// Enum used for determining which state of the steamworks animations are we in
    /// </summary>
    public enum PhaseState
    {
        Idle = 0,
        Burning = 1,
        Fuel = 2,
        Bonus = 4,
        Settled = 5,
        WaitingForInput = 8,
        Cutscene = 12,
        Rewards = 13,
    }

    /// <summary>
    /// The rarity of the reward of the steamworks
    /// </summary>
    public enum RewardRarity
    {
        Common = 0,
        Rare = 1
    }

}
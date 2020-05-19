using AutoSteamApp.Helpers;
using GregsStack.InputSimulatorStandard.Native;
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
        Process MHWProcess;

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
        /// Address used to read the rarity of the reward.
        /// </summary>
        ulong RarityAddress;

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
        /// Address used to read what phase the steamworks is in. 
        /// </summary>
        public byte PhaseValue
        {
            get
            {
                return MemoryHelper.Read<byte>(MHWProcess, PhaseAddress);
            }
        }

        #endregion

        #region Constructor

        public SteamworksData(Process mhwProcess)
        {
            MHWProcess = mhwProcess;
            LoadData();
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
            // Offset to find the sequence address
            SequenceAddress = SteamworksAddress + MHWMemoryValues.OffsetToSequence;
            // Offset to find the button pressed check address
            ButtonPressedCheckAddress = SteamworksAddress + MHWMemoryValues.OffsetToButtonCheck;
            // Offset to find the phase address
            PhaseAddress = SteamworksAddress + MHWMemoryValues.OffsetToSteamPhase;
            // Offset to find the rarity
            RarityAddress = SteamworksAddress + MHWMemoryValues.OffsetToGameRarity;
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
        Fuel = 2,
        Bonus = 4,
        Settled = 5,
        WaitingForInput = 8,
        Cutscene = 12
    }

}
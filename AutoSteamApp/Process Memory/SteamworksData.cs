using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoSteamApp.Process_Memory
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
        /// <para>Phase values:</para>
        /// <para>8 = Waiting for input</para>
        /// <para>12 = Overload cutscene</para>
        /// <para>5 = Press "space" to start</para>
        /// <para>4 = Bonus begin animation</para>
        /// </summary>
        ulong PhaseAddress;

        /// <summary>
        /// Address used to read the rarity of the reward.
        /// </summary>
        ulong RarityAddress;

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

        public Queue<VirtualKeyCode> ExtractSequence()
        {
            byte[] bytes = new byte[3];
            bytes[0] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress);
            bytes[1] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress + 1);
            bytes[2] = MemoryHelper.Read<byte>(MHWProcess, SequenceAddress + 2);

            // return null if they are are all zero (indicates the minigame hasnt started)
            if (bytes.AllIdentical())
                if (bytes[0] == 0)
                    return null;
                else
                    throw new Exception("Error reading correct sequence. An unknown sequence has been encountered. This is most likely due to an incompatible game version");

            //Capcom has likely done some manipulation so we need to do some twidling because the sequences aren't as straightforward as we'd like.



            throw new NotImplementedException();
        }

        #endregion

    }

}
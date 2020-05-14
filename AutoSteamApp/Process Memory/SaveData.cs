using AutoSteamApp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSteamApp.Process_Memory
{
    public class SaveData
    {

        /// <summary>
        /// The Monster Hunter World: Iceborne process to load player data from.
        /// </summary>
        Process MHWProcess;

        /// <summary>
        /// The cancellation token used for invoking a cnacellation of the program.
        /// </summary>
        CancellationTokenSource CancellationToken;


        #region Constructor

        /// <summary>
        /// Constructor used to generate player data from.
        /// </summary>
        /// <param name="mhwProcess">The Monster Hunter World: Iceborne process to load player data from.</param>
        /// <param name="cancellationToken">The cancellation token used for invoking a cnacellation of the program.</param>
        /// <param name="Pointer1Value">The value found in the po</param>
        public SaveData(Process mhwProcess, CancellationTokenSource cancellationToken)
        {
            MHWProcess = mhwProcess;
            CancellationToken = cancellationToken;
            LoadData();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads/Reloads the Save Data of the current MHW Process
        /// </summary>
        public void LoadData()
        {
            /* First we need to find out which save slot the player is currently using
             *  We cleverly check this by looking for the "Play Time" value of each slot after a certain time.
             * The slot whose play time increased is the slot we are looking for. This may take a few tries as
             */

            //First we find the address pointed to by the save data pointer
            ulong SaveDataAddress = MemoryHelper.Read<ulong>(MHWProcess, MHWMemoryValues.SaveDataPointer);
            int slotNumber = GetSlotNumber(SaveDataAddress);
            

        }


        int GetSlotNumber(ulong SaveDataAddress)
        {
            // We know that the pointer to the slot data is X bytes after the save data pointer value
            ulong SlotDataPointer = SaveDataAddress + MHWMemoryValues.OffsetToSlotDataPointer;

            int[] SlotPlayTimes = new int[3] { -1, -1, -1 };

            //We loop through each slot to set the initial playtime. 
            for (int slotNumber = 0; slotNumber < 3; slotNumber++)
            {
                // Grab the slot data address from the slot data pointer
                ulong SlotDataAddress = MemoryHelper.Read<ulong>(MHWProcess, SlotDataPointer);

                // We need to go to the correct slot though which is the slot size * slot number
                SlotDataAddress += MHWMemoryValues.SlotSize * (ulong)slotNumber;

                // The pointer is x bytes into the slot
                ulong PlayTimePointer = SlotDataAddress + MHWMemoryValues.OffsetToPlayTimePointer;

                // Get the playtime from the playtime pointer
                SlotPlayTimes[slotNumber] = MemoryHelper.Read<int>(MHWProcess, PlayTimePointer);
            }

            // I have explicitly duplicated the above code to make it more clear to a reader, rather than
            // Doing it in a more "programmy" way.
            int retVal = -1;
            while (!CancellationToken.IsCancellationRequested)
            {
                // Repeat the same as above until one of the slots changes or cancellation is requested. 
                // This indicates that the changed slot is being played currently.
                //We loop through each slot to set the initial playtime. 
                for (int slotNumber = 0; slotNumber < 3; slotNumber++)
                {
                    // Grab the slot data address from the slot data pointer
                    ulong SlotDataAddress = MemoryHelper.Read<ulong>(MHWProcess, SlotDataPointer);

                    // We need to go to the correct slot though which is the slot size * slot number
                    SlotDataAddress += MHWMemoryValues.SlotSize * (ulong)slotNumber;

                    // The pointer is x bytes into the slot
                    ulong PlayTimePointer = SlotDataAddress + MHWMemoryValues.OffsetToPlayTimePointer;

                    // If the playtime has been updated, we have identified which slot the player is using
                    if (MemoryHelper.Read<int>(MHWProcess, PlayTimePointer) != SlotPlayTimes[slotNumber])
                    {
                        retVal = slotNumber;
                        break;
                    }
                }
            }
            return retVal;
            
        }

        #endregion

    }
}

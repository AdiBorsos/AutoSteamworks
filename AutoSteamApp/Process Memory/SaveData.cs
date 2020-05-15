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

        /// <summary>
        /// The pointer to the steamworks save data.
        /// </summary>
        ulong SteamworksSaveDataPointer;

        /// <summary>
        /// The integer value of the amount of natural fuel the player has left.
        /// </summary>
        public int NaturalFuelLeft
        {
            get
            {
                return MemoryHelper.Read<int>(MHWProcess, SteamworksSaveDataPointer + MHWMemoryValues.OffsetToNaturalFuel);
            }
        }

        /// <summary>
        /// The integer value of the amount of stored fuel the player has left.
        /// </summary>
        public int StoredFuel
        {
            get
            {
                return MemoryHelper.Read<int>(MHWProcess, SteamworksSaveDataPointer + MHWMemoryValues.OffsetToStoredFuel);
            }
        }

        /// <summary>
        /// The short value showing how much 
        /// </summary>
        public short SteamGauge
        {
            get
            {
                return MemoryHelper.Read<short>(MHWProcess, SteamworksSaveDataPointer + MHWMemoryValues.OffsetToSteamGauge);
            }
        }

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
            // Load the address of the save data
            ulong SaveDataAddress = MemoryHelper.Read<ulong>(MHWProcess, MHWMemoryValues.SaveDataPointer);
            // The slot data is an offset from the save data
            ulong SlotDataPointer = SlotDataPointer = SaveDataAddress + MHWMemoryValues.OffsetToSlotDataPointer;
            // Get which slot number we are using so we know where to find the current steamworks values
            int slotNumber = GetSlotNumber(SlotDataPointer);
            // The steamworks save data pointer is found in the slot data so first go there.
            SteamworksSaveDataPointer = MemoryHelper.Read<ulong>(MHWProcess, SlotDataPointer);
            // Then offset to the proper slot (slot no. * slot size)
            SteamworksSaveDataPointer += ((ulong)slotNumber * MHWMemoryValues.SlotSize);
        }

        /// <summary>
        /// Iterates over the different player slots in the save data.
        /// Returns the slot which has the playtime increase after some time.
        /// </summary>
        /// <param name="SlotDataPointer">The pointer to the slot data address.</param>
        /// <returns>The slot number the player is currently using.</returns>
        int GetSlotNumber(ulong SlotDataPointer)
        {
            // We know that the pointer to the slot data is X bytes after the save data pointer value          
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
            bool found = false;
            while (!found && !CancellationToken.IsCancellationRequested)
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
                        found = true;
                        break;
                    }
                }
            }
            return retVal;
        }

        #endregion

    }
}

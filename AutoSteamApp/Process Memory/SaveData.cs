using AutoSteamApp.Configuration;
using AutoSteamApp.Helpers;
using Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace AutoSteamApp.ProcessMemory
{

    public class SaveData
    {

        #region Fields

        /// <summary>
        /// The Monster Hunter World: Iceborne process to load player data from.
        /// </summary>
        private readonly Process MHWProcess;

        /// <summary>
        /// The address of the steamworks save data.
        /// </summary>
        private ulong SteamworksSaveDataAddress;

        #endregion

        #region Properties

        /// <summary>
        /// The integer value of the amount of natural fuel the player has left.
        /// </summary>
        public int NaturalFuelLeft
        {
            get
            {
                return MemoryHelper.Read<int>(MHWProcess, SteamworksSaveDataAddress + MHWMemoryValues.OffsetToNaturalFuel);
            }
        }

        /// <summary>
        /// The integer value of the amount of stored fuel the player has left.
        /// </summary>
        public int StoredFuelLeft
        {
            get
            {
                return MemoryHelper.Read<int>(MHWProcess, SteamworksSaveDataAddress + MHWMemoryValues.OffsetToStoredFuel);
            }
        }

        /// <summary>
        /// The short value showing how much 
        /// </summary>
        public short SteamGauge
        {
            get
            {
                return MemoryHelper.Read<short>(MHWProcess, SteamworksSaveDataAddress + MHWMemoryValues.OffsetToSteamGauge);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to generate player data from.
        /// </summary>
        /// <param name="mhwProcess">The Monster Hunter World: Iceborne process to load player data from.</param>
        /// <param name="cancellationToken">The cancellation token used for invoking a cnacellation of the program.</param>
        /// <param name="Pointer1Value">The value found in the po</param>
        public SaveData(Process mhwProcess)
        {
            Log.Debug("Loading Save data values.");
            MHWProcess = mhwProcess;
            LoadData();
            Log.Debug("Save data values loaded.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads/Reloads the Save Data of the current MHW Process
        /// </summary>
        private void LoadData()
        {
            // Load the address of the save data
            ulong SaveDataAddress = MemoryHelper.Read<ulong>(MHWProcess, MHWMemoryValues.SaveDataPointer);
            // The slot data is an offset from the save data
            ulong SlotDataPointer = SlotDataPointer = SaveDataAddress + MHWMemoryValues.OffsetToSlotDataPointer;
            // Get which slot number we are using so we know where to find the current steamworks values
            int slotNumber = GetSlotNumber(SlotDataPointer);
            // The steamworks save data pointer is found in the slot data so first go there.
            SteamworksSaveDataAddress = MemoryHelper.Read<ulong>(MHWProcess, SlotDataPointer);
            // Then offset to the proper slot (slot no. * slot size)
            SteamworksSaveDataAddress += ((ulong)slotNumber * MHWMemoryValues.SlotSize);
            Log.Debug("Steamworks Save Data Address: " + SteamworksSaveDataAddress);
        }

        /// <summary>
        /// Iterates over the different player slots in the save data.
        /// Returns the slot which has the playtime increase after some time.
        /// </summary>
        /// <param name="SlotDataPointer">The pointer to the slot data address.</param>
        /// <returns>The slot number the player is currently using.</returns>
        private int GetSlotNumber(ulong SlotDataPointer)
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
            Log.Message("Attempting to determine slot: Will attempt for a maximum of " + ConfigurationReader.MaxTimeSlotNumberSeconds + " seconds");
            // I have explicitly duplicated the above code to make it more clear to a reader, rather than
            // Doing it in a more "programmy" way.
            int retVal = -1;
            bool found = false;
            DateTime start = DateTime.Now;
            int attempts = 0;
            while (!found)
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
                        Log.Message("Slot found: " + retVal);
                        break;
                    }
                    attempts++;
                    if (attempts % 10 == 0)
                        Log.Debug("attempt " + attempts + " at determining currently used slot.");
                    // Sleep while we let the timer run up
                    Thread.Sleep(1000);
                }
                if ((DateTime.Now - start).TotalSeconds > ConfigurationReader.MaxTimeSlotNumberSeconds)
                    throw new TimeoutException("There was an issue determining the currently used slot number.");
            }
            return retVal;
        }

        #endregion

    }

}
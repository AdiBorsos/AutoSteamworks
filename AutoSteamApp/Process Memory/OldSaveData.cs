using System;
using System.Diagnostics;
using System.Threading;
using AutoSteamApp.Core;

namespace AutoSteamApp
{
    public class OldSaveData
    {

        /// <summary>
        /// The MHW Process
        /// </summary>
        private Process _mhw = null;

        /// <summary>
        /// The Start address to the Save Data of a player. Offset_to_Base + Offset_to_SaveData
        /// </summary>
        private ulong StartAddress = Settings.Off_Base + Settings.Off_SaveData;

        // Pointer to where the 
        private ulong offset1;
        private ulong offset2;

        public int NaturalFuel => MemoryHelper.Read<int>(_mhw, offset2 + 0x102FDC);
        public int StoredFuel => MemoryHelper.Read<int>(_mhw, offset2 + 0x102FE0);
        public short SteamGauge => MemoryHelper.Read<short>(_mhw, offset2 + 0x102FE4);
        public byte BonusTime => MemoryHelper.Read<byte>(_mhw, offset2 + 0x102FE6);


        public OldSaveData(Process mhw, CancellationTokenSource cts)
        {
            // Set the proper process for the save data object
            _mhw = mhw;

            PlayerData.Load(_mhw, cts);

            offset1 = MemoryHelper.Read<ulong>(_mhw, StartAddress) + 0xA8;
            offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)PlayerData.CurrentSlot * Settings.Off_DiffSlot;
        }
    }

    public static class PlayerData
    {

        /// <summary>
        /// Address of the user's save data (slot independent). This is the offset of the base game plus the offset to the save data
        /// </summary>
        private static ulong StartAddress = Settings.Off_Base + Settings.Off_SaveData;

        public static int CurrentSlot { get; private set; } = -1;

        public static void Load(Process mhw, CancellationTokenSource cts)
        {

            // Get the pointer found 0xA8 bytes into the start address
            var offset1 = MemoryHelper.Read<ulong>(mhw, StartAddress) + 0xA8;

            int[] slotPlayTimes = new int[3] { -1, -1, -1 };

            DateTime dt = DateTime.Now;
            while (CurrentSlot == -1 && !cts.IsCancellationRequested)
            {
                Logger.LogInfo($"Awaiting slot number!");
                Thread.Sleep(1000);

                for (int slotId = 0; slotId < 3; slotId++)
                {
                    var pointer1 = MemoryHelper.Read<ulong>(mhw, offset1) + (ulong)slotId * Settings.Off_DiffSlot;
                    var p1Value = MemoryHelper.Read<int>(mhw, pointer1 + 0xA0);

                    if (slotPlayTimes[slotId] == -1)
                    {
                        slotPlayTimes[slotId] = p1Value;
                    }
                    else 
                        if (slotPlayTimes[slotId] != p1Value)
                        {
                            CurrentSlot = slotId;
                            Logger.LogInfo($"Identified slot number: {slotId + 1}");

                            return;
                        }
                }

                if (dt.AddSeconds(10) < DateTime.Now)
                {
                    Logger.LogInfo($"Slot Number couldn't be found after 10 seconds!");
                }
            }
        }
    }
}
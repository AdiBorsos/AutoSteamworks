using System;
using System.Diagnostics;
using System.Threading;
using AutoSteamApp.Core;

namespace AutoSteamApp
{
    public class SaveData
    {
        private Process _mhw = null;
        private ulong StartAddress = Settings.Off_Base + Settings.Off_SaveData;
        private ulong offset1;
        private ulong offset2;

        public int NaturalFuel => MemoryHelper.Read<int>(_mhw, offset2 + 0x102FDC);
        public int StoredFuel => MemoryHelper.Read<int>(_mhw, offset2 + 0x102FE0);
        public short SteamGauge => MemoryHelper.Read<short>(_mhw, offset2 + 0x102FE4);
        public byte BonusTime => MemoryHelper.Read<byte>(_mhw, offset2 + 0x102FE6);

        public SaveData(Process mhw, CancellationTokenSource cts)
        {
            _mhw = mhw;

            PlayerData.Load(_mhw, cts);

            offset1 = MemoryHelper.Read<ulong>(_mhw, StartAddress) + 0xA8;
            offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)PlayerData.CurrentSlot * Settings.Off_DiffSlot;
        }
    }

    public static class PlayerData
    {
        private static ulong StartAddress = Settings.Off_Base + Settings.Off_SaveData;

        public static int CurrentSlot { get; private set; } = -1;

        public static void Load(Process mhw, CancellationTokenSource cts)
        {
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
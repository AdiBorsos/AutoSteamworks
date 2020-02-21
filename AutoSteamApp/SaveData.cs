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

        public SaveData(Process mhw)
        {
            _mhw = mhw;

            PlayerData.Load(_mhw);

            offset1 = MemoryHelper.Read<ulong>(_mhw, StartAddress) + 0xA8;
            offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)PlayerData.CurrentSlot * Settings.Off_DiffSlot;
        }
    }

    public static class PlayerData
    {
        private static ulong StartAddress = Settings.Off_Base + Settings.Off_SaveData;

        public static int CurrentSlot { get; private set; } = -1;

        public static void Load(Process mhw)
        {
            var offset1 = MemoryHelper.Read<ulong>(mhw, StartAddress) + 0xA8;

            int[] slotPlayTimes = new int[3] { -1, -1, -1 };

            while (CurrentSlot == -1)
            {
                for (int slotId = 0; slotId < 3; slotId++)
                {
                    var pointer1 = MemoryHelper.Read<ulong>(mhw, offset1) + (ulong)slotId * Settings.Off_DiffSlot;
                    var p1Value = MemoryHelper.Read<int>(mhw, pointer1 + 0xA0);

                    if (slotPlayTimes[slotId] == -1)
                    {
                        slotPlayTimes[slotId] = p1Value;
                    }
                    else if (slotPlayTimes[slotId] != p1Value)
                    {
                        CurrentSlot = slotId;
                        Logger.LogInfo($"Identified slot number: {slotId + 1}");
                        return;
                    }
                }

                Logger.LogInfo($"Awaiting slot number!");
                Thread.Sleep(1000);
            }
        }
    }
}
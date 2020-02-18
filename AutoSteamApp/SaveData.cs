using System.Diagnostics;
using AutoSteamApp.Core;

namespace AutoSteamApp
{
    public class SaveData
    {
        private Process _mhw = null;
        private ulong StartAddress = 0x140000000 + 0x4DF3F00;
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
            offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)(PlayerData.CurrentSlot * 0x27E9F0);
        }
    }

    public static class PlayerData
    {
        private static ulong StartAddress = 0x140000000 + 0x4DF3F00;

        private static int _currentSlot = -1;
        public static int CurrentSlot
        {
            get
            {
                return _currentSlot != -1 ? _currentSlot : 0;
            }
        }

        public static void Load(Process mhw)
        {
            var offset1 = MemoryHelper.Read<ulong>(mhw, StartAddress) + 0xA8;

            //try slot1 
            int play1 = -1;
            int play2 = -1;
            int play3 = -1;

            for (int i = 0; i < 3; i++)
            {
                var p1 = MemoryHelper.Read<ulong>(mhw, offset1) + (ulong)(0 * 0x27E9F0);
                var p1Val = MemoryHelper.Read<int>(mhw, p1 + 0xA0);
                if (play1 == -1)
                {
                    play1 = p1Val;
                }
                else if (play1 != p1Val)
                {
                    _currentSlot = 0;
                    Logger.LogInfo($"Identified slot number: 1");
                    return;
                }

                var p2 = MemoryHelper.Read<ulong>(mhw, offset1) + (ulong)(1 * 0x27E9F0);
                var p2Val = MemoryHelper.Read<int>(mhw, p2 + 0xA0);
                if (play2 == -1)
                {
                    play2 = p2Val;
                }
                else if (play2 != p2Val)
                {
                    _currentSlot = 1;
                    Logger.LogInfo($"Identified slot number: 2");
                    return;
                }

                var p3 = MemoryHelper.Read<ulong>(mhw, offset1) + (ulong)(2 * 0x27E9F0);
                var p3Val = MemoryHelper.Read<int>(mhw, p3 + 0xA0);
                if (play3 == -1)
                {
                    play3 = p3Val;
                }
                else if (play3 != p3Val)
                {
                    _currentSlot = 2;
                    Logger.LogInfo($"Identified slot number: 3");
                    return;
                }
            }
        }
    }
}
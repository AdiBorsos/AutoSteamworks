using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BotanistWarning.Core.Utility;

namespace BotanistWarning.Model
{
    public class DataFeeder
    {
        private Process _mhw = null;

        private ulong offset1;
        private ulong offset2;

        public readonly List<CultivateSlot> CultivateSlots = new List<CultivateSlot>();
        public readonly List<FertilizerItem> FertilizerItems = new List<FertilizerItem>();
        public int HarvestBoxCount { get; private set; }


        public DataFeeder(Process mhw)
        {
            _mhw = mhw;

            PlayerData.Load(_mhw);

            var baseAddr = Settings.Off_Base + Settings.Off_SaveData;
            offset1 = MemoryHelper.Read<ulong>(_mhw, baseAddr) + 0xA8;
            offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)PlayerData.CurrentSlot * Settings.Off_DiffSlot;
        }

        public void Load()
        {
            CultivateSlots.Clear();
            for (ulong i = 0; i < 4; i++)
            {
                CultivateSlots.Add(new CultivateSlot()
                {
                    SlotNumber = (int)i + 1,
                    ItemId = MemoryHelper.Read<int>(_mhw, offset2 + 0x103030 + i * 0x10)
                });
            }

            // 4x Fertilizers - each per 0x10 
            for (ulong i = 0; i < 5; i++)
            {
                FertilizerItems.Add(new FertilizerItem()
                {
                    FertilizerId = MemoryHelper.Read<int>(_mhw, offset2 + 0x103070 + i * 0x10),
                    Duration = MemoryHelper.Read<int>(_mhw, offset2 + 0x103074 + i * 0x10)
                });
            }

            // Items start after 0x10 more - go for 40 max slots - 0x10 each
            ulong itemsAddress = offset2 + 0x103070 + (ulong)5 * 0x10;

            HarvestBoxCount = 0;
            // max 40 item brackets 
            for (ulong i = 0; i < 40; i++)
            {
                int cultivatedItemId = MemoryHelper.Read<int>(_mhw, itemsAddress + i * 0x10);

                if (cultivatedItemId > 0)
                {
                    HarvestBoxCount++;
                }
            }
            
            //HarvestBoxCount = 0;
            //for (ulong iAddress = itemsAddress; iAddress < itemsAddress + 0x330; iAddress += 0x10)
            //{
            //    int memValue = MemoryHelper.Read<int>(_mhw, iAddress);
            //    if (memValue > 0)
            //    {
            //        HarvestBoxCount++;
            //    }
            //}
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
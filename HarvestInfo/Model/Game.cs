using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HarvestInfo.Core.Utility;

namespace HarvestInfo.Model
{
    public class Game
    {
        private const string ProcessName = "MonsterHunterWorld";
        private static Process _mhw = GetMHW();

        public static HarvestBox Harvest = new HarvestBox();

        public delegate void GameEventHandler(object source, EventArgs args);
        public static event GameEventHandler OnHarvestCountLow;
        public static event GameEventHandler OnFertilizerCountLow;

        protected static void _onHarvestCountLow()
        {
            OnHarvestCountLow?.Invoke(typeof(Game), EventArgs.Empty);
        }

        // On game close
        protected static void _onFertilizerCountLow()
        {
            OnFertilizerCountLow?.Invoke(typeof(Game), EventArgs.Empty);
        }

        public static void LoadMemData()
        {
            if (_mhw != null)
            {
                PlayerData.Load(_mhw);

                var baseAddr = Settings.Off_Base + Settings.Off_SaveData;
                var offset1 = MemoryHelper.Read<ulong>(_mhw, baseAddr) + 0xA8;
                var offset2 = MemoryHelper.Read<ulong>(_mhw, offset1) + (ulong)PlayerData.CurrentSlot * Settings.Off_DiffSlot;

                for (int i = 0; i < 4; i++)
                {
                    Harvest.CultivateSlots[i].SlotNumber = i + 1;
                    Harvest.CultivateSlots[i].ItemId = MemoryHelper.Read<int>(_mhw, offset2 + 0x103030 + (ulong)i * 0x10);
                }

                // 4x Fertilizers - each per 0x10 
                for (int i = 0; i < 5; i++)
                {
                    Harvest.Fertilizers[i].FertilizerId = MemoryHelper.Read<int>(_mhw, offset2 + 0x103070 + (ulong)i * 0x10);
                    Harvest.Fertilizers[i].Duration = MemoryHelper.Read<int>(_mhw, offset2 + 0x103074 + (ulong)i * 0x10);
                }

                // Items start after 0x10 more - go for 40 max slots - 0x10 each
                ulong itemsAddress = offset2 + 0x103070 + (ulong)5 * 0x10;

                var HarvestBoxCount = 0;
                // max 40 item brackets 
                for (ulong i = 0; i < 40; i++)
                {
                    int cultivatedItemId = MemoryHelper.Read<int>(_mhw, itemsAddress + i * 0x10);

                    if (cultivatedItemId > 0)
                    {
                        HarvestBoxCount++;
                    }
                }

                Harvest.Counter = HarvestBoxCount;

                if (Harvest.Counter > Settings.ThresholdHarvest)
                {
                    _onHarvestCountLow();
                }

                if (Harvest.Fertilizers.Any(fert => fert.Duration < Settings.ThresholdDuration && fert.Duration != 0))
                {
                    _onFertilizerCountLow();
                }
            }
            else
            {
                Logger.LogInfo($"Waiting for '{ProcessName}' process.");

                _mhw = GetMHW();
            }
        }

        public static Process GetMHW()
        {
            var processes = Process.GetProcesses();
            try
            {
                return processes.FirstOrDefault(p => p != null && p.ProcessName.Equals(ProcessName) && !p.HasExited);
            }
            catch
            {
                Logger.LogError($"Error trying to find '{ProcessName}' process.");
            }

            Logger.LogError($"Looks like the game is not running. It should...");

            return null;
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
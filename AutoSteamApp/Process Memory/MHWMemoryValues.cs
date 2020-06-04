using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.ProcessMemory
{
    public static class MHWMemoryValues
    {

        #region Process Version

        /// <summary>
        /// The name of the process. Ideally this should be MonsterHunterWorld as this is likely never to change.
        /// </summary>
        public const string ProcessName = "MonsterHunterWorld";

        /// <summary>
        /// The MHW:IB version which is the currently supported.
        /// </summary>
        public const int SupportedGameVersion = 410918;

        /// <summary>
        /// Regex used to match the supported version in the process title.
        /// </summary>
        public const string SupportedVersionRegex = @"\(([0-9]+)\)";

        #endregion

        #region Offsets

        /// <summary>
        /// The offset from the starting address to the save data of the player
        /// </summary>
        public const ulong OffsetToSaveData = 0x4F53580;

        /// <summary>
        /// The offset from the base address to the steamworks combo values
        /// </summary>
        public const ulong OffsetToSteamworksValues = 0x4EC68C0;

        /// <summary>
        /// The offset from the start address pointer value to the save data pointer address
        /// </summary>
        public const ulong OffsetToSlotDataPointer = 0xA8;

        /// <summary>
        /// The size in bytes of each save slot. This is multiplied by the desired slot number to 
        /// get the corresponding offset required to get the memory values of that slot.
        /// </summary>
        public const ulong SlotSize = 0x27E9F0;

        /// <summary>
        /// The offset from the Slot Data Pointer value to the natural fuel level of the player.
        /// </summary>
        public const ulong OffsetToNaturalFuel = 0x102FDC;

        /// <summary>
        /// The offset from the Slot Data Pointer value to the stored fuel level of the player.
        /// </summary>
        public const ulong OffsetToStoredFuel = 0x102FE0;

        /// <summary>
        /// The offset from the Slot Data Pointer value to the steam gauge of the player.
        /// </summary>
        public const ulong OffsetToSteamGauge = 0x102FE4;

        /// <summary>
        /// The offset from the Slot Data Pointer value to the remaining bonus time in the steamworks of the player.
        /// </summary>
        public const ulong OffsetToSteamBonusTime = 0x102FE6;

        /// <summary>
        /// The offset into each save slot data for the current play time.
        /// </summary>
        public const ulong OffsetToPlayTimePointer = 0xA0;

        /// <summary>
        /// Offset from the steamworks data address to the phase value.
        /// </summary>
        public const ulong OffsetToSteamPhase = 0x57C;

        /// <summary>
        /// Offset to the secondary phase value which determines which phase we're in after winning.
        /// </summary>
        public const ulong OffsetToSteamSecondPhase = 0x5C5;

        /// <summary>
        /// Offset from the steamworks data address to the sequence data.
        /// </summary>
        public const ulong OffsetToSequence = 0x350;

        /// <summary>
        /// Offset from the steamworks data to the input button pressed check value.
        /// </summary>
        public const ulong OffsetToButtonCheck = 0x358;

        /// <summary>
        /// Offset from the steamworks data to the byte representing the rarity of the reward.
        /// </summary>
        public const ulong OffsetToGameRarity = 0x5E1;

        #endregion

        #region Addresses

        /// <summary>
        /// The base address to read the game memory.
        /// </summary>
        public const ulong BaseAddress = 0x140000000;

        #endregion

        #region Pointers

        /// <summary>
        /// The pointer to where save data begins.
        /// </summary>
        public static ulong SaveDataPointer = BaseAddress + OffsetToSaveData;

        /// <summary>
        /// The pointer to where the steamworks data begins.
        /// </summary>
        public static ulong SteamworksDataPointer = BaseAddress + OffsetToSteamworksValues;

        #endregion

    }
}


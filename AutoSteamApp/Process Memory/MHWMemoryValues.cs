using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Core
{
    public static class MHWMemoryValues
    {

        #region Process Version

        /// <summary>
        /// The supported MHW:IB version in string format
        /// </summary>
        public static string SupportedGameVersion = "410014";

        #endregion

        #region Offsets

        /// <summary>
        /// The offset from the starting address to the save data of the player
        /// </summary>
        public const ulong OffsetToSaveData = 0x4F54590;

        /// <summary>
        /// The offset from the base address to the steamworks combo values
        /// </summary>
        public const ulong OffsetToSteamworksValues = 0x4EC78C0;

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

        #endregion

        #region Addresses

        /// <summary>
        /// The base address to read the game memory.
        /// </summary>
        public const ulong BaseAddress = 0x140000000;

        #endregion

        #region Pointers

        /// <summary>
        /// The address in which save data begins.
        /// </summary>
        public static ulong SaveDataPointer = BaseAddress + OffsetToSaveData;

        #endregion

    }
}


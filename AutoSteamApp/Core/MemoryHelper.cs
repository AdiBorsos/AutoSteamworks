using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoSteamApp.Core
{
    public static class MemoryHelper
    {
        public static T Read<T>(Process process, ulong address) where T : struct
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];

            int lpNumberOfBytesRead = 0;
            WindowsApi.ReadProcessMemory(process.Handle, (IntPtr)address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            T result;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;
        }
    }
}

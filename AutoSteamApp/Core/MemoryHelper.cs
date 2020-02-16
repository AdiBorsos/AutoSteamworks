using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoSteamApp.Core
{
    public static class MemoryHelper
    {
        public static T Read<T>(Process process, ulong address) where T : struct
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
            byte[] bytes = new byte[Marshal.SizeOf(outputType)];

            int lpNumberOfBytesRead = 0;
            WindowsApi.ReadProcessMemory(process.Handle, (IntPtr)address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            T result;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static byte[] ReadUnmanaged(Process process, ulong address, int length)
        {
            byte[] bytes = new byte[length];

            int lpNumberOfBytesRead = 0;
            WindowsApi.ReadProcessMemory(process.Handle, (IntPtr)address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            return bytes;
        }
    }
}

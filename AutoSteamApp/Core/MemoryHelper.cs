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

        public static bool Write<T>(Process process, IntPtr lpBaseAddress, T value) where T : struct
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
            var bytes = new T[Marshal.SizeOf(outputType)];

            bytes[0] = value;

            return WindowsApi.WriteProcessMemory(process.Handle, lpBaseAddress, bytes, Marshal.SizeOf<T>(), out var bytesread);
        }

        public static bool WriteUnmanaged<T>(Process process, IntPtr lpBaseAddress, byte[] value)
        {
            return WindowsApi.WriteProcessMemory(process.Handle, lpBaseAddress, value, value.Length, out var bytesread);
        }
    }
}
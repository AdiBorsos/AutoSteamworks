using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoSteamApp.Core
{
    public static class MemoryHelper
    {

        /// <summary>
        /// Reads a struct <typeparamref name="T"/> of memory from a proccess at a specified address.
        /// </summary>
        /// <typeparam name="T">What type of struct to read.</typeparam>
        /// <param name="process">What process to read from.</param>
        /// <param name="address">The adress to begin reading from.</param>
        /// <returns>The desired struct representation of the the address read</returns>
        public static T Read<T>(Process process, ulong address) where T : struct
        {
            // Get the underlying base type of the struct to read
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

            // Initialize an array with size corresponding to the size of the desired T type
            byte[] bytes = new byte[Marshal.SizeOf(outputType)];


            // Read bytes from process into bytes[] buffer of this process
            int lpNumberOfBytesRead = 0;
            WindowsApi.ReadProcessMemory(process.Handle, (IntPtr)address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            // Setup reference to resulting value
            T result;

            // Create a handle to the managed bytes (now unmanaged)
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                // Blit bytes to the desired structure type
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
            }
            finally
            {
                // On completion or if anything goes wrong, clean the memory allocated for the result
                handle.Free();
            }

            // Return the resulting value
            return result;
        }

        /// <summary>
        /// Reads a series of bytes from a process starting at the desired address
        /// </summary>
        /// <param name="process">What process to read from.</param>
        /// <param name="address">The adress to begin reading from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>A byte array of the read bytes</returns>
        public static byte[] ReadUnmanaged(Process process, ulong address, int length)
        {
            // Create a byte array of the desired length
            byte[] bytes = new byte[length];

            // Read bytes from process into bytes[] buffer of this process
            int lpNumberOfBytesRead = 0;
            WindowsApi.ReadProcessMemory(process.Handle, (IntPtr)address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            // Return the read bytes
            return bytes;
        }

    }
}
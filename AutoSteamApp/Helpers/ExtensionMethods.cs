using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public static class ExtensionMethods
    {

        #region array

        /// <summary>
        /// Returns true if all memebers of the array are identical to one another
        /// </summary>
        /// <typeparam name="T">The type of array to check.</typeparam>
        /// <param name="array">The array whose contents are checked.</param>
        /// <returns></returns>
        public static bool AllIdentical<T>(this T[] array)
        {
            if (array == null)
            {
                throw new Exception("Equality test cannot be calculated on a null array");
            }

            if (array.Length < 2)
            {
                return true;
            }

            for (int i = 1; i < array.Length; i++)
            {
                if (!array[i].Equals(array[i - 1]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Shuffles an array using the fischer-yates algorithm.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static T[] Shuffle<T>(this T[] array, Random rng)
        {
            int n = array.Length;
            for (int i = 0; i < (n - 1); i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = i + rng.Next(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }

            return array;
        }

        /// <summary>
        /// Checks if the desired process has the current focus.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool HasFocus(this Process process)
        {
            var activatedHandle = WindowsApi.GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            int activeProcId;
            WindowsApi.GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == process.Id;
        }

        #endregion

    }
}

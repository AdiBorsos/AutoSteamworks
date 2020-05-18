using System;
using System.Collections.Generic;
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
                throw new Exception("Equality test cannot be calculated on a null array");
            if (array.Length < 2)
                return true;

            for (int i = 1; i < array.Length; i++)
            {
                if (!array[i].Equals(array[i - 1]))
                    return false;
            }
            return true;
        }

        #endregion

    }
}

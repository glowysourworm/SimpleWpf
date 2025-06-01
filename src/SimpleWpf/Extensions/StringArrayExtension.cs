using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWpf.Extensions
{
    public static class StringArrayExtension
    {
        /// <summary>
        /// Returns the string with a capitalized first letter - the rest being lower case.
        /// </summary>
        public static string[] ToCapitalCase(this string[] stringArray)
        {
            for (int index = 0; index < stringArray.Length; index++)
            {
                stringArray[index] = stringArray[index].ToCapitalCase();
            }

            return stringArray;
        }

        /// <summary>
        /// Returns substring of characters before the specified character
        /// </summary>
        public static string[] Before(this string[] stringArray, char token)
        {
            for (int index = 0; index < stringArray.Length; index++)
            {
                stringArray[index] = stringArray[index].Before(token);
            }

            return stringArray;
        }

        /// <summary>
        /// Joins string array using specified separator
        /// </summary>
        public static string Join(this string[] stringArray, string joinString)
        {
            return string.Join(joinString, stringArray);
        }
    }
}

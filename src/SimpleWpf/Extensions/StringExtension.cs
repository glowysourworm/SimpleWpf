using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWpf.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns the string with a capitalized first letter - the rest being lower case.
        /// </summary>
        public static string ToCapitalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        /// <summary>
        /// Returns substring of characters before the specified character
        /// </summary>
        public static string Before(this string str, char token)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            for (int index = 0; index < str.Length; index++)
            {
                if (str[index] == token)
                    return str.Substring(0, index);
            }

            return str;
        }

        /// <summary>
        /// Splits the string into two pieces - the second of which starts at the supplied index.
        /// </summary>
        /// <param name="index">The starting index of the second substring</param>
        public static string[] Splice(this string theString, int index)
        {
            var beforeString = theString.Substring(0, index + 1);
            var afterString = theString.Substring(index + 1, theString.Length - index - 2);

            return new[] { beforeString, afterString };
        }

        public static string ReplaceUntilMismatch(this string theString, string otherString, int startIndex, string replaceString)
        {
            if (string.IsNullOrWhiteSpace(theString) ||
                string.IsNullOrWhiteSpace(otherString))
                return theString;

            if (otherString.Length < startIndex + 1)
                throw new ArgumentException("Invalid 'theString' length:  StringExtension.ReplaceSubstring");

            if (theString.Length < startIndex + 1)
                throw new ArgumentException("Invalid 'otherString' length:  StringExtension.ReplaceSubstring");

            for (int index = startIndex;
                 index < theString.Length &&
                 index < otherString.Length;
                 index++)
            {
                // Mismatch
                if (theString[index] != otherString[index])
                {
                    // Calculate the pieces of the substring
                    var pieces = theString.Splice(index);

                    // Replace the first piece with the desired replace string
                    pieces[0] = replaceString;

                    // Join the result
                    return pieces.Join("");
                }
            }

            return theString; 
        }
    }
}

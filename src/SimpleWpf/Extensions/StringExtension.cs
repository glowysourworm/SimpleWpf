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
        /// Left aligned string limiting function with options for ellipses
        /// </summary>
        public static string LimitLengthLeft(this string str, uint length = uint.MaxValue, bool useEllipses = true)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length > length)
            {
                if (!useEllipses)
                    return str.Substring(0, (int)length);

                else
                {
                    if (length > 3)
                        return str.Substring(0, (int)length - 3) + "...";

                    else
                        return str.Substring(0, (int)length);
                }
            }

            return str;
        }

        /// <summary>
        /// Right-aligned string limiting function with option for ellipses
        /// </summary>
        public static string LimitLengthRight(this string str, uint length = uint.MaxValue, bool useEllipses = true)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length > length)
            {
                if (!useEllipses)
                    return str.Substring(str.Length - (int)length, (int)length);

                else
                {
                    if (str.Length - length > 3)
                        return "..." + str.Substring(str.Length - (int)length + 3, (int)length - 3);

                    else
                        return str.Substring(str.Length - (int)length, (int)length);
                }
            }

            return str;
        }

        /// <summary>
        /// Right aligns string, padding to the left, and ellipses option
        /// </summary>
        public static string ForceLengthRight(this string str, uint length = 10, bool useEllipses = true)
        {
            if (length > 10000)
                throw new ArgumentException("Trying to force string length to very large number. Please use local method. (or limit to 10000 chars)");

            if (string.IsNullOrWhiteSpace(str))
                return str;

            // Limit
            if (str.Length > length)
                return str.LimitLengthRight(length, useEllipses);

            // Pad
            else
            {
                var result = string.Empty;
                var canUseEllipses = str.Length - length >= 3;



                // Padding chars
                for (int index = 0; index < length; index++)
                {
                    // Ellipses
                    if (useEllipses && canUseEllipses)
                    {
                        if (index < length - str.Length - 3)
                            result += ' ';

                        else if (index < length - str.Length)
                            result += '.';

                        else
                            result += str[index - ((int)length - str.Length)];
                    }

                    // No Ellipses
                    else
                    {
                        if (index < length - str.Length)
                            result += ' ';

                        else
                            result += str[index - ((int)length - str.Length)];
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Right aligns padded string, with option for ellipses
        /// </summary>
        public static string ForceLengthLeft(this string str, uint length = 10, bool useEllipses = false)
        {
            if (length > 10000)
                throw new ArgumentException("Trying to force string length to very large number. Please use local method. (or limit to 10000 chars)");

            if (string.IsNullOrWhiteSpace(str))
                return str;

            // Limit
            if (str.Length > length)
                return str.LimitLengthLeft(length, useEllipses);

            // Pad
            else
            {
                var result = string.Empty;
                var canUseEllipses = str.Length - length >= 3;

                // Padding chars
                for (int index = 0; index < length; index++)
                {
                    if (index < str.Length)
                        result += str[index];

                    else if (index < str.Length + 3 && useEllipses && canUseEllipses)
                        result += '.';

                    else
                        result += ' ';
                }

                return result;

            }
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

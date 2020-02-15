using System;

namespace Utilities
{
    public static class StringUtils
    {
        public static string SubstringBetweenIndexes(this string str, int startIndex, int endIndex)
        {
            var betweenTwoIndexesCount = endIndex - startIndex;
            var r = str.Substring(startIndex, betweenTwoIndexesCount);
            return r;
        }
    }
}

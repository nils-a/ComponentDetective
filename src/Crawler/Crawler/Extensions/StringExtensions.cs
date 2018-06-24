using System;

namespace ComponentDetective.Crawler.Extensions
{
    internal static class StringExtensions
    {
        internal static bool Contains(this string input, string value, StringComparison comparisonType)
        {
            return input.IndexOf(value, comparisonType) > -1;
        }
    }
}

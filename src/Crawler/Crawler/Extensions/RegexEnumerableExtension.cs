using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComponentDetective.Crawler.Extensions
{
    internal static class RegexEnumerableExtension
    {
        internal static bool IsMatch(this IEnumerable<Regex> regexes, string input)
        {
            if(regexes == null)
            {
                return false;
            }

            return regexes.Any(r => r.IsMatch(input));
        }
    }
}

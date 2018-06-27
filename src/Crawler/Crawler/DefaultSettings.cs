using System.Collections.Generic;
using System.Text.RegularExpressions;
using ComponentDetective.Contracts;

namespace ComponentDetective.Crawler
{
    internal class DefaultSettings : ICrawlerSettings
    {
        public IEnumerable<Regex> LibraryNameExcludesRegex => null;

        public IEnumerable<Regex> PathExcludesRegex => null;
    }
}
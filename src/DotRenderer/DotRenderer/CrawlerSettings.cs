using System.Collections.Generic;
using System.Text.RegularExpressions;
using ComponentDetective.Contracts;

namespace ComponentDetective.Render
{
    internal class CrawlerSettings : ICrawlerSettings
    {
        //
        // TODO: All the settings, should be configureable at commandline-level
        //

        public IEnumerable<Regex> LibraryNameExcludesRegex
        {
            get
            {
                yield return new Regex("^System$", RegexOptions.IgnoreCase|RegexOptions.Compiled);
                yield return new Regex("^System\\..*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                yield return new Regex("^Microsoft\\..*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        public IEnumerable<Regex> PathExcludesRegex
        {
            get
            {
                yield return new Regex("\\.localhistory", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }
    }
}
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ComponentDetective.Contracts
{
    public interface ICrawlerSettings
    {
        /// <summary>
        /// Set to a list of regex to exclude found libraries by name
        /// </summary>
        IEnumerable<Regex> LibraryNameExcludesRegex {get;}

        /// <summary>
        /// Set to a list of regex to exclude found projects and solutions by name
        /// </summary>
        IEnumerable<Regex> PathExcludesRegex { get; }
    }
}

using System.Collections.Generic;
using Contracts.Models;

namespace Crawler.Models
{
    internal class ProjectInformation : IProjectInformation
    {
        public ProjectType Type { get; internal set; }

        public string Path { get; internal set; }

        internal IEnumerable<LibraryReference> References { get; set; }
        IEnumerable<ILibraryReference> IProjectInformation.References { get { return References; } }
    }
}
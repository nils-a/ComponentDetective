using System.Collections.Generic;
using Contracts.Models;

namespace Crawler.Models
{
    internal class ProjectInformation : IProjectInformation
    {
        public ProjectType Type { get; internal set; }

        public string Path { get; internal set; }

        public IEnumerable<string> OutputPaths { get; internal set; }

        public IEnumerable<IProjectReference> ProjectReferences { get; internal set; }

        internal IEnumerable<LibraryReference> LibraryReferences { get; set; }
        IEnumerable<ILibraryReference> IProjectInformation.LibraryReferences { get { return LibraryReferences; } }
    }
}
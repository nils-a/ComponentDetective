using System.Collections.Generic;

namespace Contracts.Models
{
    public interface IProjectInformation
    {
        ProjectType Type { get; }

        string Path { get; }

        IEnumerable<ILibraryReference> LibraryReferences { get; }

        IEnumerable<IProjectReference> ProjectReferences { get; }

        IEnumerable<string> OutputPaths { get; }
    }
}
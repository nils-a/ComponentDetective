using System.Collections.Generic;

namespace Contracts.Models
{
    public interface IProjectInformation
    {
        ProjectType Type { get; }

        string Path { get; }

        IEnumerable<ILibraryReference> References { get; }
    }
}
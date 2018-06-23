using System.Collections.Generic;

namespace Contracts.Models
{
    public interface ISolutionInformation
    {
        string Path { get; }

        string Name { get; }

        IEnumerable<IProjectReference> Projects { get; }
    }
}
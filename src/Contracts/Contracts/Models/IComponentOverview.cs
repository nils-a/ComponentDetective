using System.Collections.Generic;

namespace Contracts.Models
{
    public interface IComponentOverview
    {
        IEnumerable<ISolutionInformation> Solutions { get; }

        IEnumerable<IProjectInformation> Projects { get; }

        IEnumerable<ILibraryReference> References { get; }
    }
}

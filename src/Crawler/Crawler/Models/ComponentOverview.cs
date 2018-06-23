using System.Collections.Generic;
using Contracts.Models;

namespace Crawler.Models
{
    internal class ComponentOverview : IComponentOverview
    {
        internal IEnumerable<SolutionInformation> Solutions { get; set; }

        internal IEnumerable<ProjectInformation> Projects { get; set; }

        internal IEnumerable<LibraryReference> References { get; set; }

        IEnumerable<ISolutionInformation> IComponentOverview.Solutions { get { return Solutions; } }
        IEnumerable<IProjectInformation> IComponentOverview.Projects { get { return Projects; } }
        IEnumerable<ILibraryReference> IComponentOverview.References { get { return References; } }
    }
}
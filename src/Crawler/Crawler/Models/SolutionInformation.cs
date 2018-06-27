using System.Collections.Generic;
using Contracts.Models;

namespace ComponentDetective.Crawler.Models
{
    internal class SolutionInformation : ISolutionInformation
    {
        public string Path { get; set; }

        public string Name { get; set; }

        internal IEnumerable<ProjectReference> Projects { get; set; }

        IEnumerable<IProjectReference> ISolutionInformation.Projects { get { return Projects; } }
    }
}
using Contracts.Models;

namespace Crawler.Models
{
    internal class ProjectReference : IProjectReference
    {
        public string Path { get; set; }

        public string Name { get; set; }
    }
}
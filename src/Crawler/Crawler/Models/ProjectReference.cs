using Contracts.Models;

namespace ComponentDetective.Crawler.Models
{
    internal class ProjectReference : IProjectReference
    {
        public string Path { get; set; }

        public string Name { get; set; }
    }
}
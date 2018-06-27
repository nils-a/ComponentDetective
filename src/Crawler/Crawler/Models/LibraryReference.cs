using Contracts.Models;

namespace ComponentDetective.Crawler.Models
{
    internal class LibraryReference : ILibraryReference
    {
        public string HintPath { get; set; }

        public string Name { get; set; }
    }
}
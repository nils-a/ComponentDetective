using ComponentDetective.Contracts;
using Contracts.Models;
using Crawler.Comparer;
using Crawler.Models;
using System.IO;
using System.Linq;

namespace Crawler
{
    public class DependencyCrawler : ICrawler
    {
        private readonly ILogger logger;

        public DependencyCrawler(ILogger logger)
        {
            this.logger = logger;
        }

        public IComponentOverview Crawl(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                return null;
            }

            var slns = Directory.GetFiles(basePath, "*.sln", SearchOption.AllDirectories);
            var projs = Directory.GetFiles(basePath, "*.??proj", SearchOption.AllDirectories);

            var parsedProjects = (new ProjectParser(logger)).ParseAll(projs);

            return new ComponentOverview
            {
                Solutions = (new SolutionParser(logger)).ParseAll(slns),
                Projects = parsedProjects,
                References = parsedProjects.SelectMany(p => p.LibraryReferences).Distinct(new LibraryReferenceComparer())
            };
        }
    }
}

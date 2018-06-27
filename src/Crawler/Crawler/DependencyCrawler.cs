using ComponentDetective.Contracts;
using Contracts.Models;
using ComponentDetective.Crawler.Comparer;
using ComponentDetective.Crawler.Models;
using System.IO;
using System.Linq;

namespace ComponentDetective.Crawler
{
    public class DependencyCrawler : ICrawler
    {
        private readonly ILogger logger;
        private readonly ICrawlerSettings settings;

        public DependencyCrawler(ILogger logger)
            : this(logger, new DefaultSettings())
        {}

        public DependencyCrawler(ILogger logger, ICrawlerSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public IComponentOverview Crawl(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                return null;
            }

            var slns = Directory.GetFiles(basePath, "*.sln", SearchOption.AllDirectories);
            var projs = Directory.GetFiles(basePath, "*.??proj", SearchOption.AllDirectories);

            var parsedProjects = (new ProjectParser(logger, settings)).ParseAll(projs);

            return new ComponentOverview
            {
                Solutions = (new SolutionParser(logger, settings)).ParseAll(slns),
                Projects = parsedProjects,
                References = parsedProjects.SelectMany(p => p.LibraryReferences).Distinct(new LibraryReferenceComparer())
            };
        }
    }
}

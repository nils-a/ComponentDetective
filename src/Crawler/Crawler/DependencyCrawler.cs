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
        public IComponentOverview Crawl(string basePath)
        {
            if (!Directory.Exists(basePath))
            {
                return null;
            }

            var slns = Directory.GetFiles(basePath, "*.sln", SearchOption.AllDirectories);
            var projs = Directory.GetFiles(basePath, "*.??proj", SearchOption.AllDirectories);

            var parsedProjects = (new ProjectParser()).ParseAll(projs);

            return new ComponentOverview
            {
                Solutions = (new SolutionParser()).ParseAll(slns),
                Projects = parsedProjects,
                References = parsedProjects.SelectMany(p => p.References).Distinct(new LibraryReferenceComparer())
            };
        }
    }
}

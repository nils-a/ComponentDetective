using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ComponentDetective.Contracts;
using ComponentDetective.Crawler.Extensions;
using ComponentDetective.Crawler.Models;

namespace ComponentDetective.Crawler
{
    internal class SolutionParser
    {
        private ILogger logger;
        private readonly ICrawlerSettings settings;

        public SolutionParser(ILogger logger, ICrawlerSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        internal IEnumerable<SolutionInformation> ParseAll(string[] slns)
        {
            var result = new List<SolutionInformation>();

            foreach (var sln in slns)
            {
                if(settings.PathExcludesRegex.IsMatch(sln))
                {
                    logger.Verbose($"Skipping: {sln}: Exclude by path");
                    continue;
                }
                logger.Verbose($"Parsing Solution: {sln}");
                try
                {
                    var fullPath = Path.GetFullPath(sln);
                    result.Add(new SolutionInformation
                    {
                        Path = fullPath,
                        Name = Path.GetFileNameWithoutExtension(sln),
                        Projects = ParseProjects(fullPath)
                    });
                }
                catch(Exception e)
                {
                    logger.Error($"{e.GetType().Name} while parsing {sln}. Skipping!", e);
                }
            }

            return result;
        }

        private IEnumerable<ProjectReference> ParseProjects(string path)
        {
            var content = File.ReadLines(path);
            var expression = new Regex("^Project.* = \"(?<name>[^\"]+)\", \"(?<ref>[^\"]+)\", \".+$"); // https://regex101.com/r/itACjq/1

            var slnFolder = Path.GetFullPath(Path.GetDirectoryName(path));

            foreach(var line in content)
            {
                var match = expression.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                var name = match.Groups["name"].Value;
                var reference = match.Groups["ref"].Value;

                if(name == reference)
                {
                    continue;
                }
                if(name.Equals("Solution Items", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                yield return new ProjectReference
                {
                    Name = name,
                    Path = Path.GetFullPath(Path.Combine(slnFolder, reference))
                };
            }
        }
    }
}
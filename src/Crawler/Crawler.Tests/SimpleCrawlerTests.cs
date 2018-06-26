using ComponentDetective.Contracts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Linq;

namespace Crawler.Tests
{
    [TestClass]
    public class SimpleCrawlerTests
    {
        [TestMethod]
        public void CrawlingThisStructure_GetsNoErrors()
        {
            var logger = Substitute.For<ILogger>();
            var thisProjectbasePath = Path.Combine(GetProjectDir(), "../../..");
            var sut = new DependencyCrawler(logger);
            var result = sut.Crawl(thisProjectbasePath);

            var thisSolution = Path.GetFileNameWithoutExtension(Directory.GetFiles(Path.Combine(GetProjectDir(), "../"), "*.sln").Single());
            result.Solutions.FirstOrDefault(x => x.Name.Equals(thisSolution)).Should().NotBeNull("this Solution");

            var thisProjectOutPath = Path.Combine(
                Path.GetFullPath(Path.Combine(GetProjectDir(), "bin/debug")),
                $"{GetType().Assembly.GetName().Name}.dll");
            var allOutPaths = result.Projects.SelectMany(p => p.OutputPaths);
            allOutPaths.FirstOrDefault(x => x.Equals(thisProjectOutPath, StringComparison.InvariantCultureIgnoreCase))
                .Should().NotBeNull("this project");
        }

        private string GetProjectDir()
        {
            var dllLoc = GetType().Assembly.Location;
            var dllPath = Path.GetDirectoryName(dllLoc);
            var projPath = Path.Combine(dllPath, "../.."); //output is to bin/debug...

            return projPath;
        }

        [TestMethod]
        public void CrawlingThisStructure_CrawlerReferencesContractsAsProjectRef()
        {
            var logger = Substitute.For<ILogger>();
            var thisProjectbasePath = Path.Combine(GetProjectDir(), "../../..");
            var crawlerProjectPath = Path.GetFullPath(Path.Combine(GetProjectDir(), "../Crawler/Crawler.csproj"));
            File.Exists(crawlerProjectPath).Should().BeTrue("It's a prerequisite this project exists");

            var sut = new DependencyCrawler(logger);
            var actual = sut.Crawl(thisProjectbasePath);

            var crawlerProj = actual.Projects.Single(x => x.Path.Equals(crawlerProjectPath, StringComparison.InvariantCultureIgnoreCase));

            crawlerProj.LibraryReferences.FirstOrDefault(l => l.HintPath.Contains($"{typeof(ICrawler).Assembly.GetName().Name}.dll"))
                .Should().BeNull("the dll should no longer be there");

            crawlerProj.ProjectReferences.FirstOrDefault(p => Path.GetFileName(p.Path).Equals("Contracts.csproj", StringComparison.InvariantCultureIgnoreCase))
                .Should().NotBeNull("the project is now referenced");
        }
    }
}

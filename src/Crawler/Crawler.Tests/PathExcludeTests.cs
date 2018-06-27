using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ComponentDetective.Contracts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ComponentDetective.Crawler.Tests
{
    [TestClass]
    public class PathExcludeTests
    {
        [TestMethod]
        public void GivenOnlyOneSolution_TheSolutionWillBeParsed()
        {
            var logger = Substitute.For<ILogger>();
            var thisProjectbasePath = Path.Combine(GetProjectDir(), "../../..");
            var settings = Substitute.For<ICrawlerSettings>();
            settings.PathExcludesRegex.Returns(new[]{
                new Regex("^((?!Crawler.sln).)*$", RegexOptions.IgnoreCase|RegexOptions.Compiled) //negative lookahead - exclude everything but crawler.sln
            });
            var sut = new DependencyCrawler(logger,settings);
            var result = sut.Crawl(thisProjectbasePath);
            result.Projects.Count().Should().Be(0, "No project matches crawler.sln");
            result.Solutions.Count().Should().Be(1, "only crawler.sln matches crawler.sln");
            result.Solutions.Single().Path.EndsWith("crawler.sln", System.StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetProjectDir()
        {
            var dllLoc = GetType().Assembly.Location;
            var dllPath = Path.GetDirectoryName(dllLoc);
            var projPath = Path.Combine(dllPath, "../.."); //output is to bin/debug...

            return projPath;
        }

        [TestMethod]
        public void ExcludingSystem_WillNotShowNSystem()
        {
            const string dll = "system.core";
            // check it's found:
            var logger = Substitute.For<ILogger>();
            var thisProjectbasePath = Path.Combine(GetProjectDir(), "../../..");
            var sut = new DependencyCrawler(logger);
            var result = sut.Crawl(thisProjectbasePath);
            if(!result.References.Any(r => r.Name.IndexOf(dll, StringComparison.InvariantCultureIgnoreCase) > -1))
            {
                Assert.Inconclusive(dll+" is not used - so this test is no good!");
            }
            // This is only to ensure the dll is really referenced!!!


            var settings = Substitute.For<ICrawlerSettings>();
            settings.LibraryNameExcludesRegex.Returns(new[]{
                new Regex(dll, RegexOptions.IgnoreCase|RegexOptions.Compiled)
            });
            sut = new DependencyCrawler(logger, settings);
            result = sut.Crawl(thisProjectbasePath);
            result.References.Any(r => r.Name.IndexOf(dll, StringComparison.InvariantCultureIgnoreCase) > -1).Should().Be(false);
        }
    }
}

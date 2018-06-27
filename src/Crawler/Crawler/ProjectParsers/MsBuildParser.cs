using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ComponentDetective.Contracts;
using Contracts.Models;
using ComponentDetective.Crawler.Models;
using ComponentDetective.Crawler.Extensions;

namespace ComponentDetective.Crawler.ProjectParsers
{
    internal class MsBuildParser : IProjParser
    {
        const string xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
        private readonly ILogger logger;
        private readonly ICrawlerSettings settings;
        private readonly ProjectType type;

        public MsBuildParser(ILogger logger, ICrawlerSettings settings, ProjectType type)
        {
            this.logger = logger;
            this.settings = settings;
            this.type = type;
        }

        public ProjectInformation Parse(string projFilePath)
        {
            XDocument xml;
            using (var s = new StreamReader(projFilePath))
            {
                xml = XDocument.Load(s);
            }

            var projDir = Path.GetDirectoryName(projFilePath);

            return new ProjectInformation
            {
                Type = type,
                Path = Path.GetFullPath(projFilePath),
                LibraryReferences = GetLibraryReferences(xml, projDir),
                OutputPaths = GetOutPaths(xml, projDir),
                ProjectReferences = GetProjectReferences(xml, projDir)
            };
        }

        private IEnumerable<IProjectReference> GetProjectReferences(XDocument xml, string projectBaseDir)
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("x", xmlns);

            var references = new List<ProjectReference>();
            foreach (var elm in xml.XPathSelectElements("//x:ProjectReference", namespaceManager))
            {
                var inc = elm.Attribute("Include").Value;
                var name = elm.Descendants(XName.Get("Name", xmlns)).First().Value;

                if (!Path.IsPathRooted(inc))
                {
                    inc = Path.Combine(projectBaseDir, inc);
                }
                inc = Path.GetFullPath(inc);

                references.Add(new ProjectReference
                {
                    Name = name,
                    Path = inc
                });
            }
            return references;
        }

        private IEnumerable<string> GetOutPaths(XDocument xml, string projectBaseDir)
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("x", xmlns);

            var paths = new HashSet<string>();
            foreach (var elm in xml.XPathSelectElements("//x:OutputPath", namespaceManager))
            {
                if (!string.IsNullOrEmpty(elm.Value))
                {
                    if (Path.IsPathRooted(elm.Value))
                    {
                        paths.Add(elm.Value);
                    }
                    else
                    {
                        paths.Add(Path.Combine(projectBaseDir, elm.Value));
                    }
                }
            }

            var assemblyName = xml.XPathSelectElements("//x:AssemblyName", namespaceManager).First().Value;

            var outType = xml.XPathSelectElements("//x:OutputType", namespaceManager).First().Value.ToLowerInvariant();
            var extension = "UNKNOWN";
            switch (outType)
            {
                case "library":
                    extension = "dll";
                    break;
                case "exe":
                    extension = "exe";
                    break;
                case "winexe":
                    extension = "exe";
                    break;
                default:
                    logger.Error($"Output-Type {outType} is unknown. Project will NOT HAVE a correct out-path!");
                    break;
            }

            foreach(var p in paths)
            {
                yield return Path.GetFullPath(Path.Combine(p, $"{assemblyName}.{extension}"));
            }
        }

        private IEnumerable<LibraryReference> GetLibraryReferences(XDocument xml, string projectBaseDir)
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("x", xmlns);

            var references = new List<LibraryReference>();
            foreach (var elm in xml.XPathSelectElements("//x:Reference", namespaceManager))
            {
                var inc = elm.Attribute("Include").Value;

                if (settings.LibraryNameExcludesRegex.IsMatch(inc))
                {
                    logger.Verbose($"Skipping {inc}: Exclude by name.");
                    continue;
                }

                var hintElm = elm.Descendants(XName.Get("HintPath", xmlns)).FirstOrDefault();
                var hintPath = string.Empty;
                if (hintElm != null)
                {
                    hintPath = hintElm.Value;
                    if (!Path.IsPathRooted(hintPath))
                    {
                        hintPath = Path.Combine(projectBaseDir, hintPath);
                    }

                    hintPath = Path.GetFullPath(hintPath);
                }

                references.Add(new LibraryReference
                {
                    Name = inc,
                    HintPath = hintPath
                });
            }
            return references;
        }
    }
}

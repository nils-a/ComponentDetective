using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Crawler.Models;

namespace Crawler
{
    internal class ProjectParser
    {
        internal IEnumerable<ProjectInformation> ParseAll(string[] projs)
        {
            var result = new List<ProjectInformation>();
            foreach (var proj in projs)
            {
                try
                {
                    var fullPath = Path.GetFullPath(proj);
                    var extension = Path.GetExtension(fullPath).Substring(1).ToUpperInvariant();
                    switch (extension)
                    {
                        case "CSPROJ":
                            result.Add(ParseCsProj(fullPath));
                            break;
                        default:
                            //logging??
                            break;
                    }
                }
                catch
                {
                    //logging?
                    continue;
                }
            }
            return result;
        }

        private ProjectInformation ParseCsProj(string fullPath)
        {
            XDocument xml;
            using(var s = new StreamReader(fullPath))
            {
                xml = XDocument.Load(s);
            }

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

            var references = new List<LibraryReference>();
            foreach (var elm in xml.XPathSelectElements("//x:Reference", namespaceManager))
            {
                var inc = elm.Attribute("Include").Value;
                var hintElm = elm.Descendants("HintPath").FirstOrDefault();
                var hintPath = string.Empty;
                if(hintElm != null)
                {
                    hintPath = hintElm.Value;
                }

                references.Add(new LibraryReference
                {
                    Name = inc,
                    HintPath = hintPath
                });
            }

            return new ProjectInformation
            {
                Type = Contracts.Models.ProjectType.CsProj,
                Path = fullPath,
                References = references
            };
        }
    }
}
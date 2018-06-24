using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComponentDetective.Crawler.Extensions;
using ComponentDetective.Crawler.ProjectParsers;
using Crawler.Models;

namespace Crawler
{
    internal class ProjectParser
    {
        internal IEnumerable<ProjectInformation> ParseAll(string[] projs)
        {
            var result = new List<ProjectInformation>();

            var parsers = new Dictionary<string, IProjParser>
            {
                { "csproj", new CsProjParser() }
            };

            foreach (var proj in projs)
            {
                try
                {
                    var fullPath = Path.GetFullPath(proj);
                    var extension = Path.GetExtension(fullPath).Substring(1).ToLowerInvariant();
                    if (!parsers.TryGetValue(extension, out IProjParser parser))
                    {
                        //logging??
                        continue;
                    }

                    result.Add(parser.Parse(proj));
                }
                catch
                {
                    //logging?
                    continue;
                }
            }

            // now, if projects were referencened by out-path, they are in Library-References and not in proejct-references...
            var pathLookup = result.SelectMany(x => x.OutputPaths.Select(y => new { Path=y, Proj=x })).ToDictionary(x => x.Path, y => y.Proj);
            foreach(var p in result)
            {
                var libRefs = p.LibraryReferences.ToList();
                var projRefs = p.ProjectReferences.ToList();
                var modified = false;
                foreach(var libRef in libRefs.ToList())
                {
                    if (string.IsNullOrEmpty(libRef.HintPath))
                    {
                        continue;
                    }

                    if(!pathLookup.TryGetValue(libRef.HintPath, out var projectInformation))
                    {
                        continue;
                    }

                    // we found a lib-ref that's actually a known project...
                    modified = true;
                    libRefs.Remove(libRef);

                    //libref-name is a full assembly-name, and projectref-name is the name the project has in the sln. So let's make something up...
                    var name = libRef.Name;
                    if(name.Contains(",", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = name.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    projRefs.Add(new ProjectReference
                    {
                        Name = name,
                        Path = projectInformation.Path
                    });
                }

                if (modified)
                {
                    p.LibraryReferences = libRefs;
                    p.ProjectReferences = projRefs;
                }
            }
            return result;
        }
    }
}
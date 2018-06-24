using ComponentDetective.DotRenderer.Extensions;
using Contracts.Models;
using Crawler;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Render
{
    class Program
    {
        static int Main(string[] args)
        {
            var showHelp = false;
            var startFolder = string.Empty;
            var targetFile = string.Empty;
            var appName = Path.GetFileName(typeof(Program).Assembly.Location);

            var p = new OptionSet() {
                { "f|folder=", "the {FOLDER} to start the crawl.",
                   f => startFolder = f },
                { "t|target=", "the {TARGET} to write the dot into",
                    t => targetFile = t },
                { "h|help",  "show this message and exit",
                   v => showHelp = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Error.WriteLine($"{appName}: {e.Message}");
                Console.Error.WriteLine($"Try '{appName} --help' for more information.");
                return 99;
            }

            if (showHelp)
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (string.IsNullOrEmpty(startFolder))
            {
                Console.Error.WriteLine($"folder is needed.");
                Console.Error.WriteLine($"Try '{appName} --help' for more information.");
                return 99;
            }

            if (!Directory.Exists(startFolder))
            {
                Console.Error.WriteLine($"The folder {startFolder} does not exist.");
                return 99;
            }

            if (!string.IsNullOrEmpty(targetFile) && File.Exists(targetFile))
            {
                Console.Error.WriteLine($"The output-file {targetFile} already exist.");
                return 99;
            }

            var crawler = new DependencyCrawler();
            var result =  crawler.Crawl(startFolder);
            var dot = GenerateDotFile(result);

            TextWriter output;
            if (!string.IsNullOrEmpty(targetFile))
            {
                output = new StreamWriter(targetFile, false, Encoding.Unicode);
            }
            else
            {
                output = Console.Out;
            }

            using (output)
            {
                output.Write(dot);
            }

            return 0;
        }

        private static string GenerateDotFile(IComponentOverview input)
        {
            var excludes = new[] { "^System$", "^System\\..*", "^Microsoft\\..*" };
            var excludeExpression = new Regex(string.Join("|", excludes.Select(x => $"({x})")), RegexOptions.Compiled);

            var solutions = input.Solutions.ToList();
            var projects = input.Projects.ToList();
            var libraries = input.References.ToList();


            var sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            sb.AppendLine("{");

            // first, the nodes...
            foreach (var sln in solutions)
            {
                sb.AppendLine($" {sln.GetId()} [shape=doubleoctagon label=\"{sln.Name.Replace("\\", "\\\\")}\"]");
            }
            foreach (var proj in projects)
            {
                var projName = Path.GetFileNameWithoutExtension(proj.Path);
                sb.AppendLine($" {proj.GetId()} [shape=octagon label=\"{projName.Replace("\\", "\\\\")}\"]");
            }
            foreach (var lib in libraries)
            {
                if (excludeExpression.IsMatch(lib.Name))
                {
                    continue;
                }

                var libName = lib.Name;
                if (libName.Contains(","))
                {
                    var parts = libName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        libName = $"{parts[0].Trim()} ({parts[1].Trim()})";
                    }
                    else
                    {
                        libName = parts[0].Trim();
                    }
                }
                sb.AppendLine($" {lib.GetId()} [shape=ellipse label=\"{libName.Replace("\\", "\\\\")}\"]");
            }
            sb.AppendLine("}");

            // now, the references...
            foreach (var sln in solutions)
            {
                foreach (var p in sln.Projects)
                {
                    sb.AppendLine($"{sln.GetId()} -> {p.GetId()}");
                }
            }
            foreach (var proj in projects)
            {
                foreach (var p in proj.ProjectReferences)
                {
                    sb.AppendLine($"{proj.GetId()} -> {p.GetId()}");
                }
                foreach (var r in proj.LibraryReferences)
                {
                    if (excludeExpression.IsMatch(r.Name))
                    {
                        continue;
                    }
                    sb.AppendLine($"{proj.GetId()} -> {r.GetId()}");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

}

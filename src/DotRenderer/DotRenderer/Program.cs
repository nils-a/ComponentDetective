using ComponentDetective.Crawler;
using ComponentDetective.DotRender;
using ComponentDetective.DotRenderer.Extensions;
using Contracts.Models;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComponentDetective.Render
{
    class Program
    {
        static int Main(string[] args)
        {
            var showHelp = false;
            var verbose = false;
            var startFolder = string.Empty;
            var targetFile = string.Empty;
            var appName = Path.GetFileName(typeof(Program).Assembly.Location);

            var p = new OptionSet() {
                { "f|folder=", "the {FOLDER} to start the crawl.",
                   f => startFolder = f },
                { "t|target", "the {TARGET} to write the dot into",
                    t => targetFile = t },
                { "v|verbose",  "print verbose messages",
                   v => verbose = v != null },
                { "h|help",  "show this message and exit",
                   h => showHelp = h != null },
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
            var logger = new ConsoleLogger(verbose);
            var settings = new CrawlerSettings();
            var crawler = new DependencyCrawler(logger, settings);
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
                logger.Verbose("Output:");
            }

            using (output)
            {
                output.Write(dot);
            }

            return 0;
        }

        private static string GenerateDotFile(IComponentOverview input)
        {
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
                    sb.AppendLine($"{proj.GetId()} -> {r.GetId()}");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

}

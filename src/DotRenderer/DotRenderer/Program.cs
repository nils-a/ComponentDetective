using ComponentDetective.DotRenderer.Extensions;
using Contracts.Models;
using Crawler;
using System;
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
            if (args.Length < 1 || args.Length > 2)
            {
                Console.Error.WriteLine($"Call {typeof(Program).Assembly.Location} with infolder as first and optional output as second param.");
                return 99;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.Error.WriteLine($"The folder {args[0]} does not exist.");
                return 99;
            }

            if (args.Length > 1 && File.Exists(args[1]))
            {
                Console.Error.WriteLine($"The output-file {args[1]} already exist.");
                return 99;
            }

            var crawler = new DependencyCrawler();
            var result =  crawler.Crawl(args[0]);
            var dot = GenerateDotFile(result);

            TextWriter output;
            if (args.Length > 1)
            {
                output = new StreamWriter(args[1], false, Encoding.Unicode);
            }
            else
            {
                output = Console.Out;
            }

            using (output)
            {
                output.Write(dot);
            }

#if DEBUG
            Console.ReadKey();
#endif
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

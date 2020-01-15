using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace TrailingSpaceRemover
{
    class Program
    {
        private static readonly string[] AllowedFileExtensions = new[] { "cs", "json", "xml", "cscfg", "csdef", "manifest", "cmd", "sln", "proj", "csproj", "props", "ps1", "config", "targets" };

        static async Task<int> Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "TrailingSpaceRemover",
                Description = "Batch remove trailing spaces."
            };

            app.HelpOption("-?|-h|--help");

            var path = app
                .Argument("path", "The file/folder path.")
                .Accepts(validation => validation.RegularExpression(".*", "Invalid path argument."));

            var extensions = app
                .Option("-e|--extensions", "Allowed file extensions", CommandOptionType.SingleValue)
                .Accepts(validation => validation.RegularExpression(".*", "Invalid extensions option."));

            app.OnExecuteAsync(async cancellationToken =>
            {
                Console.WriteLine("TrailingSpaceRemover v0.1");

                if (!string.IsNullOrWhiteSpace(path.Value) && File.Exists(path.Value))
                {
                    var changed = await ProcessFile(path.Value);
                    Console.WriteLine($"{changed} lines changed in {path.Value}.");
                    return 0;
                }

                var root = path.Value ?? Environment.CurrentDirectory;
                if (Directory.Exists(root))
                {
                    Console.WriteLine($"Listing files under {root}...");
                    var files = GetFileList(root, AllowedFileExtensions);
                    Console.WriteLine($"{files.Length} files found. Start processing...");
                    foreach (var file in files)
                    {
                        var changed = await ProcessFile(file);
                        Console.WriteLine($"{changed} lines changed in {file}.");
                    }

                    return 0;
                }

                Console.WriteLine($"Invalid root directory: '{root}'.");
                return 1;
            });

            return await app.ExecuteAsync(args);
        }

        private static async Task<int> ProcessFile(string path)
        {
            var lines = await File.ReadAllLinesAsync(path);
            var count = 0;
            var newLines = lines.Select(line =>
            {
                var trimmed = line.TrimEnd();
                if (trimmed != line)
                {
                    count++;
                    return trimmed;
                }

                return line;
            }).ToArray();

            if (count > 0)
            {
                await File.WriteAllLinesAsync(path, newLines, Encoding.UTF8);
            }

            return count;
        }

        private static string[] GetFileList(string root, string[] allowedFileExtensions)
        {
            return allowedFileExtensions
                .SelectMany(extension => Directory.GetFiles(root, $"*.{extension}", SearchOption.AllDirectories))
                .ToArray();
        }
    }
}

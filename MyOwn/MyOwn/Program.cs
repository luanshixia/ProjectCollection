using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using McMaster.Extensions.CommandLineUtils;

namespace MyOwn
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "myown",
                Description = "My own productivity tool."
            };

            app.HelpOption("-?|-h|--help");

            app.Command("browse", browseCommand =>
            {
                browseCommand.Description = "Open Web browser with URI.";
                browseCommand.HelpOption(inherited: true);

                var uri = browseCommand
                    .Argument("uri", "The URI to browse.")
                    .Accepts(validation => validation.RegularExpression(".*", "Invalid URI."))
                    .IsRequired();

                browseCommand.OnExecute(() =>
                {
                    Console.WriteLine("Opening URI in browser...");
                    Program.OpenUrl(WithScheme(uri.Value));
                });
            });

            app.Command("google", googleCommand =>
            {
                googleCommand.Description = "Google with keyword.";
                googleCommand.HelpOption(inherited: true);

                var kwd = googleCommand
                    .Argument("kwd", "The keyword to search.")
                    .Accepts(validation => validation.RegularExpression(".*", "Invalid keyword."))
                    .IsRequired();

                googleCommand.OnExecute(() =>
                {
                    Console.WriteLine("Opening Google in browser...");
                    Program.OpenUrl(WithScheme($"https://www.google.com/#q=${kwd.Value}"));
                });
            });

            app.Command("maps", mapsCommand =>
            {
                mapsCommand.Description = "Show maps with keyword.";
                mapsCommand.HelpOption(inherited: true);

                var kwd = mapsCommand
                    .Argument("kwd", "The keyword to search.")
                    .Accepts(validation => validation.RegularExpression(".*", "Invalid keyword."))
                    .IsRequired();

                mapsCommand.OnExecute(() =>
                {
                    Console.WriteLine("Opening Google Maps in browser...");
                    Program.OpenUrl(WithScheme($"https://www.google.com/maps/search/{kwd.Value}"));
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("MyOwn productivity tool v0.1");
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        private static void OpenUrl(string url)
        {
            // Solution from https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private static string WithScheme(string uri)
        {
            return uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase) 
                ? uri
                : "http://" + uri;
        }
    }
}

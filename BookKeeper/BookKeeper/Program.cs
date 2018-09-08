using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace BookKeeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "bookkeeper"
            };

            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                Console.WriteLine("Hello World!");
                return 0;
            });

            app.Command("hide", (command) =>
            {
                command.Description = "Instruct the ninja to hide in a specific location.";
                command.HelpOption("-?|-h|--help");

                var locationArgument = command.Argument("[location]", "Where the ninja should hide.");

                command.OnExecute(() =>
                {
                    Console.WriteLine("Ninja is hidden " + locationArgument.Value ?? "under a turtle");
                    return 0;
                });
            });

            app.Command("attack", (command) =>
            {
                command.Description = "Instruct the ninja to go and attack!";
                command.HelpOption("-?|-h|--help");

                var excludeOption = command.Option(
                    "-e|--exclude <exclusions>",
                    "Things to exclude while attacking.",
                    CommandOptionType.MultipleValue);

                var screamOption = command.Option(
                    "-s|--scream",
                    "Scream while attacking",
                    CommandOptionType.NoValue);

                command.OnExecute(() =>
                {
                    var exclusions = excludeOption.Values;
                    var attacking = (new List<string>
                        {
                            "dragons",
                            "badguys",
                            "civilians",
                            "animals"
                        }).Where(x => !exclusions.Contains(x));

                    Console.Write("Ninja is attacking " + string.Join(", ", attacking));

                    if (screamOption.HasValue())
                    {
                        Console.Write(" while screaming");
                    }

                    Console.WriteLine();

                    return 0;

                });
            });

            app.Execute(args);
        }
    }
}

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

            app.Command("book", bookCommand =>
            {
                bookCommand.Description = "Book related commands.";
                bookCommand.HelpOption("-?|-h|--help");

                bookCommand.Command("search", searchCommand =>
                {
                    searchCommand.Description = "Search a book on Amazon.";
                    searchCommand.Argument("kwd", "The keyword");

                    searchCommand.OnExecute(() =>
                    {
                        var kwd = searchCommand.Arguments.SingleOrDefault();
                        searchCommand.Out.WriteLine($"About to search for '{kwd.Value}'.");

                        return 0;
                    });
                });
            });

            app.Execute(args);
        }
    }
}

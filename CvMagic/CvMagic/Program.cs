using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;
using CompiledSpell = System.Func<string, string>;

namespace CvMagic
{
    public class Program
    {
        static CompiledSpell[] CompiledSpells;

        static async Task Main(string[] args)
        {
            await CompileSpells();

            Console.WriteLine("CV Magic started.");

            var breaker = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                breaker.Cancel();
                Console.WriteLine("CV Magic stopped.");
            };

            var prevText = string.Empty;

            while (!breaker.IsCancellationRequested)
            {
                var text = await ClipboardService.GetTextAsync(breaker.Token);
                if (text != prevText)
                {
                    var output = await CastAll(input: text);
                    Console.WriteLine($"[{DateTime.Now}] Clipboard content changed: '{text}'. Transformed to: '{output}'.");
                    await ClipboardService.SetTextAsync(output, breaker.Token);
                    prevText = output;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), breaker.Token);
            }
        }

        static async Task CompileSpells()
        {
            Console.WriteLine("Loading spells...");

            var list = new List<CompiledSpell>();
            await foreach (var spell in LoadSpellsAsync())
            {
                var compiledSpell = await CSharpScript.EvaluateAsync<CompiledSpell>(code: spell);
                list.Add(compiledSpell);
            }

            CompiledSpells = list.ToArray();
            Console.WriteLine($"Loaded {list.Count} spells.");
        }

        static async Task<string> CastAll(string input)
        {
            foreach (var spell in CompiledSpells)
            {
                input = await Cast(spell, input);
            }

            return input;
        }

        static async IAsyncEnumerable<string> LoadSpellsAsync()
        {
            foreach (var file in Directory
                .GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Spells"))
                .Where(file => file.EndsWith(".csx", StringComparison.OrdinalIgnoreCase)))
            {
                yield return await File.ReadAllTextAsync(file);
            }
        }

        static Task<string> Cast(CompiledSpell spell, string input)
        {
            return CSharpScript.EvaluateAsync<string>(
                code: "spell(input)",
                globals: new Globals { spell = spell, input = input });
        }

        public class Globals
        {
            public CompiledSpell spell { get; set; }

            public string input { get; set; }
        }
    }
}

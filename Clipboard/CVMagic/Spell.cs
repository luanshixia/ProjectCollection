using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CVMagic
{
    public class Spell
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public string Template { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public bool Validate()
        {
            //Regex.Replace(input: this.Template, pattern: @"\{([a-zA-Z_][a-zA-Z0-9_]*)\}", replacement: "");
            return true;
        }

        public async Task<string> Cast()
        {
            var output = this.Template;
            try
            {
                foreach (var parameter in this.Parameters)
                {
                    var argument = await CSharpScript.EvaluateAsync(
                        code: parameter.Value,
                        options: ScriptOptions.Default.WithImports("System"));

                    output = output.Replace($"{{{parameter.Key}}}", argument.ToString());
                }
            }
            catch (CompilationErrorException ex)
            {
                return string.Join(Environment.NewLine, ex.Diagnostics);
            }

            return output;
        }
    }
}

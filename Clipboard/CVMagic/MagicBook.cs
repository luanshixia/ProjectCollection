using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVMagic
{
    public static class MagicBook
    {
        public static List<Spell> Spells { get; } = new List<Spell>
        {
            new Spell
            {
                Name = "Ago to UTC",
                Pattern = @"TIMESTAMP\s*>\s*ago1\((?<number>[0-9]+)(?<unit>[dhms])\)",
                Template = "TIMESTAMP > datetime({ago}) and TIMESTAMP < datetime({now})",
                Parameters = new Dictionary<string, string>
                {
                    { "ago", "DateTime.UtcNow" },
                    { "now", "DateTime.UtcNow" }
                }
            }
        };
    }
}

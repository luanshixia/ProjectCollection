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
                Pattern = @"TIMESTAMP\s*>\s*ago\((?<number>[0-9]+)(?<unit>[dhms])\)",
                Template = "TIMESTAMP > datetime({ago}) and TIMESTAMP < datetime({now})",
                Parameters = new Dictionary<string, string>
                {
                    { "ago", "match.Groups['unit'].Value == 'd' ? DateTime.UtcNow.AddDays(-int.Parse(match.Groups['number'].Value)) : DateTime.UtcNow".Replace("'", "\"") },
                    { "now", "DateTime.UtcNow" }
                }
            },
            new Spell
            {
                Name = "US date to ISO date",
                Pattern = @"(?<month>\d\d)/(?<day>\d\d)/(?<year>\d\d\d\d)",
                Substitution = "${year}-${month}-${day}"
            },
            new Spell
            {
                Name = "Project 01",
                Pattern = "project01",
                Substitution = "| project TIMESTAMP, correlationId, operationName, targetUri, httpStatusCode, durationInMilliseconds, SourceNamespace, Role"
            }
        };
    }
}

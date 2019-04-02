using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CosmosDBQueryCharge
{
    class Program
    {
        private static readonly int[][] DataGenerationPatterns =
        {
            new[] { 3, 4, 5 },
            new[] { 2 },
            new[] { 1 },
        };

        static void Main(string[] args)
        {
            if (args.First().Equals("generate"))
            {
                var resourceGenerator = new ResourceGenerator(DataGenerationPatterns);
                foreach (var resource in resourceGenerator.GenerateAll())
                {
                    var json = JsonConvert.SerializeObject(resource);
                    Console.WriteLine(json);
                    Console.WriteLine(Encoding.UTF8.GetBytes(json).Length);
                }
            }

            Console.ReadKey();
        }
    }
}

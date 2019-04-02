using System;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Azure.Documents.Client;
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

        private static readonly int[][] DbDataGenerationPatterns =
        {
            new[] { 3, 4, 5 },
            new[] { 2 },
            new[] { 1 },
        };

        static void Main(string[] args)
        {
            var documentClient = new DocumentClient(
                serviceEndpoint: new Uri("https://arm-cits-globaldata.documents.azure.com:443/"),
                authKey: GetAuthKey());

            if (args.First().Equals("generate"))
            {
                var resourceGenerator = new ResourceGenerator(DataGenerationPatterns);
                foreach (var resource in resourceGenerator.GenerateAll())
                {
                    var json = JsonConvert.SerializeObject(resource, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    Console.WriteLine(json);
                    Console.WriteLine(Encoding.UTF8.GetBytes(json).Length);
                }
            }
            else if (args.First().Equals("dbgenerate"))
            {
                var resourceGenerator = new ResourceGenerator(DbDataGenerationPatterns);
                foreach (var resource in resourceGenerator.GenerateAll())
                {
                    var response = documentClient
                        .CreateDocumentAsync(
                            documentCollectionUri: UriFactory.CreateDocumentCollectionUri(
                                databaseId: "ARMLocalData",
                                collectionId: "resources"),
                            document: resource)
                        .Result;

                    Console.WriteLine($"Size={Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resource)).Length}|Charge={response.RequestCharge}");
                }
            }

            Console.ReadKey();
        }

        private static SecureString GetAuthKey()
        {
            var encodedKey = "";
            var secureString = new SecureString();
            foreach (var c in Encoding.UTF8.GetString(Convert.FromBase64String(encodedKey)))
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }
    }
}

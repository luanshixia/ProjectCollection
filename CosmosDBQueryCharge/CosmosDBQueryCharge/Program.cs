using System;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
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
            new[] { 1 },
            new[] { 16 },
            new[] { 128 },
            new[] { 1024 },
            // new[] { 2048 },
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
                var total = DbDataGenerationPatterns.SelectMany(sub => sub).Sum();
                var current = 0;
                foreach (var resource in resourceGenerator.GenerateAll())
                {
                    var response = documentClient
                        .CreateDocumentAsync(
                            documentCollectionUri: UriFactory.CreateDocumentCollectionUri(
                                databaseId: "ARMLocalData",
                                collectionId: "resources"),
                            document: resource)
                        .Result;

                    current++;
                    Console.WriteLine($"({current} of {total}) Size={Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resource)).Length}|Charge={response.RequestCharge}");
                }

                Console.WriteLine();

                foreach (var subscription in resourceGenerator.Subscriptions)
                {
                    foreach (var resourceGroup in subscription.ResourceGroups)
                    {
                        Console.WriteLine($"Subscription={subscription.Id}|ResourceGroup={resourceGroup.Name}|Location={resourceGroup.Location}");
                    }
                }
            }
            else if (args.First().Equals("readsmalldoc"))
            {
                TestReadDocument(
                    documentClient: documentClient,
                    databaseId: "ARMLocalData",
                    collectionId: "resources",
                    partitionKey: "173ed1f4-308c-4502-85fb-b75bf1ee9af6",
                    documentId: "l1JRPUbt2StY1YGM-Microsoft:2ECompute:2Fdisks:2FHZeNAeuuEX6dF7ofDYIZ8QMh-eastasia");

                TestReadPartition(
                    documentClient: documentClient,
                    databaseId: "ARMLocalData",
                    collectionId: "resources",
                    partitionKey: "173ed1f4-308c-4502-85fb-b75bf1ee9af6");
            }
            else if (args.First().Equals("querydocs"))
            {
                //var partitions = new[]
                //{
                //    "109abf51-e148-4bc6-bbf9-a8a045cc711c",
                //    "2d16309f-8923-4c85-bdd2-33165b264e03",
                //    "173ed1f4-308c-4502-85fb-b75bf1ee9af6"
                //};

                var partitions = new[]
                {
                    "54294b98-3f59-499d-920b-f2ec0071afd3",
                    "c1abe804-9a53-4d7f-a607-da40655c6200",
                    "98902bdb-ec67-42d4-ae61-cd293a24b2cb",
                    "9d97f971-d2f2-425d-89f4-fd274556e457"
                };

                foreach (var partition in partitions)
                {
                    TestReadPartition(
                        documentClient: documentClient,
                        databaseId: "ARMLocalData",
                        collectionId: "resources",
                        partitionKey: partition);
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

        private static void TestReadDocument(DocumentClient documentClient, string databaseId, string collectionId, string partitionKey, string documentId)
        {
            var response = documentClient
                .ReadDocumentAsync<Resource>(
                    documentUri: UriFactory.CreateDocumentUri(
                        databaseId: databaseId,
                        collectionId: collectionId,
                        documentId: documentId),
                    options: new RequestOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey)
                    })
                .Result;

            Console.WriteLine($"[Read] Size={response.Document.GetDocumentSize()}|Charge={response.RequestCharge}");
        }

        private static void TestReadPartition(DocumentClient documentClient, string databaseId, string collectionId, string partitionKey)
        {
            Console.WriteLine($"[PartitionKey={partitionKey}]");

            var feedResponse = documentClient
                .ReadDocumentFeedAsync(
                    documentsLink: $"/dbs/{databaseId}/colls/{collectionId}/docs/",
                    options: new FeedOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey),
                        MaxItemCount = -1
                    })
                .Result;

            Console.WriteLine($"[ReadFeed] Count={feedResponse.Count}|TotalSize={feedResponse.Sum(doc => ((object)doc).GetDocumentSize())}|Charge={feedResponse.RequestCharge}");

            var queryResponse = documentClient
                .CreateDocumentQuery<Resource>(
                    documentCollectionUri: UriFactory.CreateDocumentCollectionUri(
                        databaseId: databaseId,
                        collectionId: collectionId),
                    feedOptions: new FeedOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey),
                        MaxItemCount = -1
                    })
                .AsDocumentQuery()
                .ExecuteNextAsync<Resource>()
                .Result;

            Console.WriteLine($"[Query] Count={queryResponse.Count}|TotalSize={queryResponse.Sum(doc => doc.GetDocumentSize())}|Charge={queryResponse.RequestCharge}");

            Console.WriteLine();
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static readonly Subscription[] DbDataGenerationSubscriptions =
        {
            new Subscription
            {
                Id = "54294b98-3f59-499d-920b-f2ec0071afd3",
                ResourceGroups = new[]
                {
                    new ResourceGroup { Name = "iXiEpmqqAAb8wcq88lI", Location = "japanwest", ResourceCount = 1 }
                }
            },
            new Subscription
            {
                Id = "c1abe804-9a53-4d7f-a607-da40655c6200",
                ResourceGroups = new[]
                {
                    new ResourceGroup { Name = "qOAUwnBXpRv5ia5ZXrhQwJLKNTgjT", Location = "westeurope", ResourceCount = 16 }
                }
            },
            new Subscription
            {
                Id = "98902bdb-ec67-42d4-ae61-cd293a24b2cb",
                ResourceGroups = new[]
                {
                    new ResourceGroup { Name = "IfTecupY3mQeF0PF4tYq3", Location = "eastasia", ResourceCount = 128 }
                }
            },
            new Subscription
            {
                Id = "9d97f971-d2f2-425d-89f4-fd274556e457",
                ResourceGroups = new[]
                {
                    new ResourceGroup { Name = "JHQLLW2p6T7S3WmSR5fG0U", Location = "centralus", ResourceCount = 1024 }
                }
            }
        };

        private static readonly Subscription[] DbDataGenerationSubscriptions1 =
        {
            new Subscription
            {
                Id = "c6aa9b42-d1aa-4c80-acf1-bf748805d8af",
                ResourceGroups = new[]
                {
                    new ResourceGroup { Name = "sub301spike200", Location = "westus", ResourceCount = 301 }
                }
            }
        };

        static void Main(string[] args)
        {
            var documentClient = DocumentClientPool.GetDocumentClient();

            if (args.First().Equals("generate", StringComparison.InvariantCultureIgnoreCase))
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
            else if (args.First().Equals("dbgenerate", StringComparison.InvariantCultureIgnoreCase))
            {
                var definition = DbDataGenerationSubscriptions1;
                var resourceGenerator = new ResourceGenerator(definition);
                var total = definition.SelectMany(sub => sub.ResourceGroups).Sum(rg => rg.ResourceCount);
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
            else if (args.First().Equals("readsmalldoc", StringComparison.InvariantCultureIgnoreCase))
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
            else if (args.First().Equals("querydocs", StringComparison.InvariantCultureIgnoreCase))
            {
                var partitions = new[]
                {
                    ("54294b98-3f59-499d-920b-f2ec0071afd3", "iXiEpmqqAAb8wcq88lI"),
                    ("c1abe804-9a53-4d7f-a607-da40655c6200", "qOAUwnBXpRv5ia5ZXrhQwJLKNTgjT"),
                    ("98902bdb-ec67-42d4-ae61-cd293a24b2cb", "IfTecupY3mQeF0PF4tYq3"),
                    ("9d97f971-d2f2-425d-89f4-fd274556e457", "JHQLLW2p6T7S3WmSR5fG0U")
                };

                foreach (var partition in partitions)
                {
                    TestReadPartition(
                        documentClient: documentClient,
                        databaseId: "ARMLocalData",
                        collectionId: "resources",
                        partitionKey: partition.Item1);

                    TestInPartitionQuery(
                        documentClient: documentClient,
                        databaseId: "ARMLocalData",
                        collectionId: "resources",
                        partitionKey: partition.Item1,
                        resourceGroupName: partition.Item2);
                }
            }
            else if (args.First().Equals("loadtest", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    var requestGenerator = new RequestGenerator("c6aa9b42-d1aa-4c80-acf1-bf748805d8af");
                    var documentIds = requestGenerator.GetDocumentIds();
                    var requestBunches = new[]
                    {
                        requestGenerator.GenerateReadPartition(targetRps: 15, delayStart: TimeSpan.FromSeconds(0), duration: TimeSpan.FromSeconds(120)),
                        //requestGenerator.GenerateReadPartition(targetRps: 200, delayStart: TimeSpan.FromSeconds(20), duration: TimeSpan.FromSeconds(1)),
                        //requestGenerator.GenerateReadDocument(targetRps: 100, delayStart: TimeSpan.FromSeconds(0), duration: TimeSpan.FromSeconds(30), documentIds: documentIds),
                        //requestGenerator.Generate(targetRps: 50, delayStart: TimeSpan.FromSeconds(10), duration: TimeSpan.FromSeconds(1)),
                        //requestGenerator.Generate(targetRps: 50, delayStart: TimeSpan.FromSeconds(20), duration: TimeSpan.FromSeconds(1)),
                        //requestGenerator.Generate(targetRps: 1000, delayStart: TimeSpan.FromSeconds(30), duration: TimeSpan.FromSeconds(1)),
                        //requestGenerator.Generate(targetRps: 1000, delayStart: TimeSpan.FromSeconds(40), duration: TimeSpan.FromSeconds(1)),
                        //requestGenerator.Generate(targetRps: 1000, delayStart: TimeSpan.FromSeconds(50), duration: TimeSpan.FromSeconds(1)),
                    };

                    var requests = requestBunches.SelectMany(bunch => bunch).ToArray();
                    Task.WhenAll(requests).Wait();
                    Console.WriteLine($"Sent {requests.Length} requests.");

                    RequestAnalyzer.Instance.Analyze();
                }
                catch (DocumentClientException ex)
                {
                }
            }
            else if (args.First().Equals("query", StringComparison.InvariantCultureIgnoreCase))
            {
                DocumentClientPool.NewDocumentClient2();
            }

            Console.ReadKey();
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

        private static void TestInPartitionQuery(DocumentClient documentClient, string databaseId, string collectionId, string partitionKey, string resourceGroupName)
        {
            Console.WriteLine($"[PartitionKey={partitionKey}|ResourceGroup={resourceGroupName}]");

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
                .Where(resource => resource.ResourceGroupName == resourceGroupName)
                .AsDocumentQuery()
                .ExecuteNextAsync<Resource>()
                .Result;

            Console.WriteLine($"[Query] Count={queryResponse.Count}|TotalSize={queryResponse.Sum(doc => doc.GetDocumentSize())}|Charge={queryResponse.RequestCharge}");

            Console.WriteLine();
        }
    }
}

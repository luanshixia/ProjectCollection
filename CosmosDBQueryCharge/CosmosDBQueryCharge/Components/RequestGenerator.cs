using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The request generator.
    /// </summary>
    public class RequestGenerator
    {
        private readonly DocumentClient DocumentClient;
        private readonly string DatabaseId = "ARMLocalData";
        private readonly string CollectionId = "resources";
        private readonly string PartitionKey;

        public RequestGenerator(DocumentClient documentClient, string partitionKey)
        {
            this.DocumentClient = documentClient;
            this.PartitionKey = partitionKey;
        }

        public Task<FeedResponse<Resource>>[] Generate(int targetRps, TimeSpan delayStart, TimeSpan duration)
        {
            var totalRequests = targetRps * (int)duration.TotalSeconds;
            return Enumerable
                .Range(0, totalRequests)
                .Select(async i =>
                {
                    await Task.Delay(delayStart + TimeSpan.FromSeconds(1.0 / targetRps * i));

                    var requestId = Guid.NewGuid();
                    RequestAnalyzer.Instance.Log(DateTime.Now, requestId, false, null);

                    try
                    {
                        var response = await ReadPartition(this.DocumentClient, this.DatabaseId, this.CollectionId, this.PartitionKey);
                        RequestAnalyzer.Instance.Log(DateTime.Now, requestId, true, new { response.Count, response.RequestCharge });
                        return response;
                    }
                    catch (DocumentClientException ex)
                    {
                        RequestAnalyzer.Instance.Log(DateTime.Now, requestId, true, new { ex.StatusCode, ex.Error, ex.Message, ex.RequestCharge });
                    }

                    return null;
                })
                .ToArray();
        }

        private static Task<FeedResponse<Resource>> ReadPartition(DocumentClient documentClient, string databaseId, string collectionId, string partitionKey)
        {
            return documentClient
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
                .ExecuteNextAsync<Resource>();
        }
    }
}

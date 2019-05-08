using System;
using System.Linq;
using System.Threading;
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
        private readonly string DatabaseId = "ARMLocalData";
        private readonly string CollectionId = "resources";
        private readonly string PartitionKey;

        private static readonly DocumentClient DocumentClient = DocumentClientPool.NewDocumentClient();

        public RequestGenerator(string partitionKey)
        {
            this.PartitionKey = partitionKey;
        }

        public string[] GetDocumentIds()
        {
            var response = ReadPartition(RequestGenerator.DocumentClient, this.DatabaseId, this.CollectionId, this.PartitionKey).Result;
            return response.Select(resource => resource.Id).ToArray();
        }

        public Task<FeedResponse<Resource>>[] GenerateReadPartition(int targetRps, TimeSpan delayStart, TimeSpan duration)
        {
            var totalRequests = targetRps * (int)duration.TotalSeconds;
            return Enumerable
                .Range(0, totalRequests)
                .Select(async i =>
                {
                    await Task.Delay(delayStart + TimeSpan.FromSeconds(1.0 / targetRps * i));

                    var requestId = Guid.NewGuid();
                    RequestAnalyzer.Instance.LogRequestStart(DateTime.Now, requestId);

                    try
                    {
                        var response = await ReadPartition(RequestGenerator.DocumentClient, this.DatabaseId, this.CollectionId, this.PartitionKey);
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, false, new { response.Count, response.RequestCharge });
                        return response;
                    }
                    catch (DocumentClientException ex)
                    {
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, true, new { StatusCode = (int)ex.StatusCode, ex.Error, ex.Message, ex.RequestCharge });
                    }
                    catch (OperationCanceledException ex)
                    {
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, true, new { StatusCode = 0, ex.Message, RequestCharge = 0 });
                    }

                    return null;
                })
                .ToArray();
        }

        public Task<DocumentResponse<Resource>>[] GenerateReadDocument(int targetRps, TimeSpan delayStart, TimeSpan duration, string[] documentIds)
        {
            var totalRequests = targetRps * (int)duration.TotalSeconds;
            return Enumerable
                .Range(0, totalRequests)
                .Select(async i =>
                {
                    await Task.Delay(delayStart + TimeSpan.FromSeconds(1.0 / targetRps * i));

                    var requestId = Guid.NewGuid();
                    RequestAnalyzer.Instance.LogRequestStart(DateTime.Now, requestId);

                    try
                    {
                        var response = await ReadDocument(RequestGenerator.DocumentClient, this.DatabaseId, this.CollectionId, this.PartitionKey, documentIds[i % documentIds.Length]);
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, false, new { Count = 1, response.RequestCharge });
                        return response;
                    }
                    catch (DocumentClientException ex)
                    {
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, true, new { ex.StatusCode, ex.Error, ex.Message, ex.RequestCharge });
                    }
                    catch (OperationCanceledException ex)
                    {
                        RequestAnalyzer.Instance.LogRequestEnd(DateTime.Now, requestId, true, new { StatusCode = 0, ex.Message, RequestCharge = 0 });
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
                .ExecuteNextAsync<Resource>(new CancellationTokenSource(delay: TimeSpan.FromSeconds(30)).Token);
        }

        private static Task<DocumentResponse<Resource>> ReadDocument(DocumentClient documentClient, string databaseId, string collectionId, string partitionKey, string documentId)
        {
            return documentClient
                .ReadDocumentAsync<Resource>(
                    documentUri: UriFactory.CreateDocumentUri(
                        databaseId: databaseId,
                        collectionId: collectionId,
                        documentId: documentId),
                    options: new RequestOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey)
                    },
                    cancellationToken: new CancellationTokenSource(delay: TimeSpan.FromSeconds(30)).Token);
        }
    }
}

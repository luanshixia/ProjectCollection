using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ArmRevokeTokenConsumer
{
    public class UserToken
    {
        public string TenantId { get; set; } = "usertoken";

        [JsonProperty(PropertyName = "id")]
        public string ObjectId { get; set; }

        public string InvalidTime { get; set; }
    }

    public class SimpleEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");

                var jtoken = JToken.Parse(data);
                var userToken = new UserToken
                {
                    ObjectId = jtoken["User"]["ObjectId"].ToObject<string>(),
                    InvalidTime = ((long)(jtoken["EnforcementTimestamp"].ToObject<DateTime>() - Program.Epoch).TotalSeconds).ToString()
                };

                await Program.DocumentClient.UpsertDocumentAsync(
                    documentCollectionUri: UriFactory.CreateDocumentCollectionUri("usertoken", "usertoken"),
                    document: userToken);
            }

            await context.CheckpointAsync();
        }
    }

    class Program
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly DocumentClient DocumentClient = new DocumentClient(
            serviceEndpoint: new Uri("https://arm-cits-globaldata.documents.azure.com:443/"),
            authKey: GetAuthKey());

        private const string EventHubConnectionString = "Endpoint=sb://armeventhub0.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=";
        private const string EventHubName = "testeventhub2";
        private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=eventhub0sa;AccountKey=;EndpointSuffix=core.windows.net";
        private const string StorageContainerName = "testeventhub2blob";

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private static SecureString GetAuthKey()
        {
            var input = "";
            var secureString = new SecureString();
            input.ToList().ForEach(character => secureString.AppendChar(character));
            return secureString;
        }
    }
}

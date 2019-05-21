using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmRevokeTokenProducer
{
    public class User
    {
        public string ObjectId { get; set; }
    }

    public class Message
    {
        public User User { get; set; }
        
        public DateTime EnforcementTimestamp { get; set; }
    }

    class Program
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://armeventhub0.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=";
        private const string EventHubName = "testeventhub2";

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            var messages = GetMessages(numMessagesToSend);

            foreach (var messageObject in messages)
            {
                try
                {
                    var message = JsonConvert.SerializeObject(messageObject);
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }

        private static Message[] GetMessages(int numMessages)
        {
            return Enumerable.Range(0, numMessages).Select(i => new Message
            {
                User = new User
                {
                    ObjectId = Guid.NewGuid().ToString()
                },
                EnforcementTimestamp = DateTime.UtcNow
            }).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Azure.Documents.Client;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The document client pool.
    /// </summary>
    public static class DocumentClientPool
    {
        public const int Capacity = 5;

        private static readonly List<DocumentClient> Clients;
        private static int Counter;

        static DocumentClientPool()
        {
            DocumentClientPool.Clients = Enumerable
                .Range(0, DocumentClientPool.Capacity)
                .Select(i => new DocumentClient(
                    serviceEndpoint: new Uri("https://arm-cits-globaldata.documents.azure.com:443/"),
                    authKey: GetAuthKey()))
                .ToList();
        }

        public static DocumentClient GetDocumentClient()
        {
            DocumentClientPool.Counter %= DocumentClientPool.Clients.Count;
            return DocumentClientPool.Clients[DocumentClientPool.Counter++];
        }

        public static DocumentClient NewDocumentClient()
        {
            return new DocumentClient(
                serviceEndpoint: new Uri("https://arm-cits-globaldata.documents.azure.com:443/"),
                authKey: GetAuthKey());
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

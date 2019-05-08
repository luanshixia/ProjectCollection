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
                serviceEndpoint: new Uri("https://arm-cits-localdata.documents.azure.com:443/"),
                authKey: GetAuthKey());
        }

        public static DocumentClient NewDocumentClient2()
        {
            return new DocumentClient(
                serviceEndpoint: new Uri("https://arm-test-localdata.documents.azure.com:443/"),
                authKey: GetAuthKey2());
        }

        private static SecureString GetAuthKey()
        {
            //var encodedKey = "S1c3VWE0NWduZ2dTbmdndjI5MWRKZm9pbkdQN0E4c1NrQjJJd0o2TlBNejE3bHdLSUhrTmwweWVsV1FiekVrbHdueklkdWtJS2NJUFpYaHlVVlU4SHc9PQ==";
            var key = "";
            var secureString = new SecureString();
            foreach (var c in key)
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }

        private static SecureString GetAuthKey2()
        {
            var key = "";
            var secureString = new SecureString();
            foreach (var c in key)
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }
    }
}

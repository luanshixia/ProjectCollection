using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The Cosmos DB entity.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets the partition key.
        /// </summary>
        public abstract string PartitionKey { get; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the changed time.
        /// </summary>
        public DateTime? ChangedTime { get; set; }

        /// <summary>
        /// Gets or sets the eTag value used by Cosmos DB to implement optimistic concurrency.
        /// </summary>
        [JsonProperty(PropertyName = "_ETag", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityTag { get; set; }
    }
}

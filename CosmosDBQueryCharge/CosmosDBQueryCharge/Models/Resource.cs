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
    /// The resource entity representation in Cosmos DB.
    /// </summary>
    public class Resource : Entity
    {
        /// <summary>
        /// Gets or sets the subscription ID.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the resource group name.
        /// </summary>
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the resource ID.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the resource SKU.
        /// </summary>
        public ResourceSku Sku { get; set; }

        /// <summary>
        /// Gets or sets the resource kind.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the managed by property.
        /// </summary>
        public string ManagedBy { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the availability zones.
        /// </summary>
        public string[] Zones { get; set; }

        /// <summary>
        /// Gets or sets the resource group location.
        /// </summary>
        public string ResourceGroupLocation { get; set; }

        /// <summary>
        /// Gets or sets the provisioning state.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProvisioningState ProvisioningState { get; set; }

        /// <summary>
        /// Gets or sets the resource plan.
        /// </summary>
        public ResourcePlan Plan { get; set; }

        /// <summary>
        /// Gets or sets the tags associated with resource group.
        /// TODO handle compression.
        /// </summary>
        public TagsDictionary Tags { get; set; }

        /// <summary>
        /// Gets or sets the resource scale.
        /// </summary>
        public ResourceScale Scale { get; set; }

        /// <summary>
        /// Gets or sets the resource identity.
        /// TODO handle compression.
        /// </summary>
        public ResourceIdentity Identity { get; set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "id")]
        public override string Id
        {
            get
            {
                return $"{this.ResourceGroupName}-{this.ResourceId}-{this.Location}";
            }
        }

        /// <summary>
        /// Gets the partition key of the entity.
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "partitionKey")]
        public override string PartitionKey
        {
            get
            {
                return this.SubscriptionId;
            }
        }
    }
}

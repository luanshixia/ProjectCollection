using System.Text;
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
                return $"{this.ResourceGroupName}-{EscapeStorageKey(this.ResourceId)}-{this.Location}";
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

        /// <summary>
        /// The escaped storage keys.
        /// </summary>
        private static readonly string[] EscapedStorageKeys = new string[128]
        {
            ":00", ":01", ":02", ":03", ":04", ":05", ":06", ":07", ":08", ":09", ":0A", ":0B", ":0C", ":0D", ":0E", ":0F",
            ":10", ":11", ":12", ":13", ":14", ":15", ":16", ":17", ":18", ":19", ":1A", ":1B", ":1C", ":1D", ":1E", ":1F",
            ":20", ":21", ":22", ":23", ":24", ":25", ":26", ":27", ":28", ":29", ":2A", ":2B", ":2C", ":2D", ":2E", ":2F",
            "0",   "1",   "2",   "3",   "4",   "5",   "6",   "7",   "8",   "9", ":3A", ":3B", ":3C", ":3D", ":3E", ":3F",
            ":40",   "A",   "B",   "C",   "D",   "E",   "F",   "G",   "H",   "I",   "J",   "K",   "L",   "M",   "N",   "O",
            "P",   "Q",   "R",   "S",   "T",   "U",   "V",   "W",   "X",   "Y",   "Z", ":5B", ":5C", ":5D", ":5E", ":5F",
            ":60",   "a",   "b",   "c",   "d",   "e",   "f",   "g",   "h",   "i",   "j",   "k",   "l",   "m",   "n",   "o",
            "p",   "q",   "r",   "s",   "t",   "u",   "v",   "w",   "x",   "y",   "z", ":7B", ":7C", ":7D", ":7E", ":7F",
        };

        /// <summary>
        /// Escapes the storage key.
        /// </summary>
        /// <param name="storageKey">The storage key.</param>
        public static string EscapeStorageKey(string storageKey)
        {
            var sb = new StringBuilder(storageKey.Length);
            for (var index = 0; index < storageKey.Length; ++index)
            {
                var c = storageKey[index];
                if (c < 128)
                {
                    sb.Append(EscapedStorageKeys[c]);
                }
                else if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
                else if (c < 0x100)
                {
                    sb.Append(':');
                    sb.Append(((int)c).ToString("X2"));
                }
                else
                {
                    sb.Append(':');
                    sb.Append(':');
                    sb.Append(((int)c).ToString("X4"));
                }
            }

            return sb.ToString();
        }
    }
}

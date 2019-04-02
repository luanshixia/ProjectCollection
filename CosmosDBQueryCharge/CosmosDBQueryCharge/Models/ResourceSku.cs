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
    /// The resource <c>sku</c> object.
    /// </summary>
    public class ResourceSku
    {
        /// <summary>
        /// Gets or sets the <c>sku</c> name.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> tier.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Tier { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> size.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> family.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Family { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> model.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> capacity.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int? Capacity { get; set; }
    }
}

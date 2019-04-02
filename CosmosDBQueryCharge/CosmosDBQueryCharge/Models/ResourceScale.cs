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
    /// The resource scale object.
    /// </summary>
    public class ResourceScale
    {
        /// <summary>
        /// Gets or sets the scale capacity.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the scale maximum.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the scale minimum.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int? Minimum { get; set; }
    }
}

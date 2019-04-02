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
    /// The resource plan object.
    /// </summary>
    public class ResourcePlan
    {
        /// <summary>
        /// Gets or sets the plan name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the plan's promotion code.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string PromotionCode { get; set; }

        /// <summary>
        /// Gets or sets the plan's product code.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the plan's publisher.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the plan's version.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Version { get; set; }
    }
}

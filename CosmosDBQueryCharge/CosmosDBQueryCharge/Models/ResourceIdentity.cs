using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// Represents the resource identity.
    /// </summary>
    public class ResourceIdentity
    {
        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the tenant id.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the resource identity type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public ResourceIdentityType Type { get; set; }

        /// <summary>
        /// Gets or sets the explicit identity ids.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string[] IdentityIds { get; set; }

        /// <summary>
        /// Gets or sets the user assigned identities.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, JToken> UserAssignedIdentities { get; set; }

        /// <summary>
        /// Represents the resource identity type.
        /// </summary>
        [Flags]
        public enum ResourceIdentityType
        {
            /// <summary>
            /// The identity type is not specified.
            /// </summary>
            NotSpecified = 0,

            /// <summary>
            /// The identity is assigned by the system.
            /// </summary>
            SystemAssigned = 1 << 0,

            /// <summary>
            /// The identity is assigned by the user.
            /// </summary>
            UserAssigned = 1 << 1,

            /// <summary>
            /// The identity is removed.
            /// </summary>
            None = 1 << 2
        }
    }
}

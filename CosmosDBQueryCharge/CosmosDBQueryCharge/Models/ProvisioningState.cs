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
    /// The provisioning state.
    /// </summary>
    public enum ProvisioningState
    {
        /// <summary>
        /// The provisioning state is not specified.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// The provisioning state is accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// The provisioning state is running.
        /// </summary>
        Running,

        /// <summary>
        /// The provisioning state is creating.
        /// </summary>
        Creating,

        /// <summary>
        /// The provisioning state is created.
        /// </summary>
        Created,

        /// <summary>
        /// The provisioning state is deleting.
        /// </summary>
        Deleting,

        /// <summary>
        /// The provisioning state is deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// The provisioning state is canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The provisioning state is failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The provisioning state is succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The provisioning state is moving resources.
        /// </summary>
        MovingResources,

        /// <summary>
        /// The provisioning state is transient failure (for Azure-AsyncOperation).
        /// </summary>
        TransientFailure,
    }
}

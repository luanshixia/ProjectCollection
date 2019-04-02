namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The subscription.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the subscription ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the resource groups.
        /// </summary>
        public ResourceGroup[] ResourceGroups { get; set; }
    }
}

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The resource group.
    /// </summary>
    public class ResourceGroup
    {
        /// <summary>
        /// Gets or sets the resource group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource group location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the resource count.
        /// </summary>
        public int ResourceCount { get; set; }
    }
}

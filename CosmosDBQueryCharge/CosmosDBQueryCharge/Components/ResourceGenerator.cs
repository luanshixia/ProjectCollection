using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The resource generator.
    /// </summary>
    public class ResourceGenerator
    {
        private readonly Subscription[] Subscriptions;

        private readonly string[] ResourceTypes =
        {
            "Microsoft.Compute/virtualMachines",
            "Microsoft.Compute/virtualMachineScaleSets",
            "Microsoft.Compute/disks",
            "Microsoft.Storage/storageAccounts",
            "Microsoft.Network/virtualNetworks",
            "Microsoft.Network/networkInterfaces",
            "Microsoft.Network/publicIPAddresses",
            "Microsoft.Web/sites",
            "Microsoft.Sql/servers",
            "Microsoft.DocumentDb/databaseAccounts",
        };

        private readonly string[] Locations =
        {
            "eastus",
            "eastus2",
            "westus",
            "westus2",
            "centralus",
            "westeurope",
            "northeurope",
            "australiasoutheast",
            "eastasia",
            "southeastasia",
            "japanwest",
            "ukwest",
            "canadacentral"
        };

        private readonly string[] Zones =
        {
            "1",
            "2",
            "3"
        };

        private readonly ProvisioningState[] ProvisioningStates =
        {
            ProvisioningState.Running,
            ProvisioningState.Created,
            ProvisioningState.Created,
            ProvisioningState.Created
        };

        private readonly Dictionary<string, ResourceSku> Skus = new Dictionary<string, ResourceSku>
        {
            { "Microsoft.Storage/storageAccounts", new ResourceSku { Name = "Standard_LRS", Tier = "Standard" } }
        };

        private readonly Dictionary<string, string> Kinds = new Dictionary<string, string>
        {
            { "Microsoft.Storage/storageAccounts", "Storage" }
        };

        private readonly HashSet<string> MsiResourceTypes = new HashSet<string>
        {
            "Microsoft.Compute/virtualMachines",
            "Microsoft.Web/sites"
        };

        private readonly int[] ExplicitIdentityCounts =
        {
            0,
            0,
            0,
            0,
            1,
            2,
            4
        };

        public ResourceGenerator(int[][] patterns)
        {
            this.Subscriptions = patterns
                .Select(sub => new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    ResourceGroups = sub
                        .Select(rg => new ResourceGroup
                        {
                            Name = RandomIdGenerator.GetBase62(RandomNumber.Next(5, 64)),
                            Location = this.Locations.PickRandomItem(),
                            ResourceCount = rg
                        })
                        .ToArray()
                })
                .ToArray();
        }

        public IEnumerable<Resource> GenerateAll(double sizeFactor = 1.0)
        {
            foreach (var subscription in this.Subscriptions)
            {
                foreach (var resourceGroup in subscription.ResourceGroups)
                {
                    foreach (var resource in Enumerable.Range(0, resourceGroup.ResourceCount))
                    {
                        yield return this.Next(subscription.Id, resourceGroup.Name, resourceGroup.Location, sizeFactor);
                    }
                }
            }
        }

        public Resource Next(string subscriptionId, string resourceGroupName, string resourceGroupLocation, double sizeFactor = 1.0)
        {
            var resourceType = this.ResourceTypes.PickRandomItem();
            return new Resource
            {
                SubscriptionId = subscriptionId,
                ResourceGroupName = resourceGroupName,
                ResourceId = $"{resourceType}/{RandomIdGenerator.GetBase62(RandomNumber.Next(8, 96))}",
                Location = this.Locations.PickRandomItem(),
                Zones = this.Zones.PickRandomItems(),
                ResourceGroupLocation = resourceGroupLocation,
                ProvisioningState = this.ProvisioningStates.PickRandomItem(),
                ManagedBy = RandomIdGenerator.GetBase62(16),
                Sku = this.Skus.GetValueOrDefault(resourceType),
                Kind = this.Kinds.GetValueOrDefault(resourceType),
                Tags = this.GenerateTags(sizeFactor),
                Plan = null,
                Scale = null,
                Identity = this.MsiResourceTypes.Contains(resourceType)
                    ? this.GenerateResourceIdentity(this.ExplicitIdentityCounts.PickRandomItem(), subscriptionId, resourceGroupName)
                    : null,
                CreatedTime = DateTime.Now,
                ChangedTime = DateTime.Now,
                EntityTag = RandomIdGenerator.GetBase36(16)
            };
        }

        private TagsDictionary GenerateTags(double sizeFactor = 1.0)
        {
            const int maxNumberOfTags = 15;
            const int maxTagKeyLength = 512;
            const int maxTagValueLength = 256;

            var factor = sizeFactor / 8.0;
            var tagCount = new[] { 0, 0, RandomNumber.Next(1, maxNumberOfTags + 1) }.PickRandomItem();

            return new TagsDictionary(
                dictionary: Enumerable
                    .Range(0, tagCount)
                    .ToDictionary(
                        i => RandomIdGenerator.GetBase62(RandomNumber.Next(3, (int)(maxTagKeyLength * factor))),
                        i => RandomIdGenerator.GetBase62(RandomNumber.Next(3, (int)(maxTagValueLength * factor)))));
        }

        private ResourceIdentity GenerateResourceIdentity(int identityCount, string subscriptionId, string resourceGroupName)
        {
            if (identityCount == 0)
            {
                return new ResourceIdentity
                {
                    Type = ResourceIdentity.ResourceIdentityType.SystemAssigned,
                    TenantId = Guid.NewGuid().ToString(),
                    PrincipalId = Guid.NewGuid().ToString()
                };
            }

            return new ResourceIdentity
            {
                Type = ResourceIdentity.ResourceIdentityType.UserAssigned,
                IdentityIds = Enumerable
                    .Range(0, identityCount)
                    .Select(i => $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{RandomIdGenerator.GetBase62(8)}")
                    .ToArray()
            };
        }
    }
}

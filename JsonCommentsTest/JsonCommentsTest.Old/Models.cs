using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonCommentsTest
{
    /// <summary>
    /// The template.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the template schema.
        /// </summary>
        [JsonProperty(PropertyName = "$schema", Required = Required.Always)]
        public string Schema { get; set; }

        /// <summary>
        /// Gets or sets the content version.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ContentVersion { get; set; }

        /// <summary>
        /// Gets or sets the API profile.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ApiProfile { get; set; }

        /// <summary>
        /// Gets or sets the template parameters.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, TemplateInputParameter> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the template variables.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, JToken> Variables { get; set; }

        /// <summary>
        /// Gets or sets the template resources.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public TemplateResource[] Resources { get; set; }

        /// <summary>
        /// Gets or sets the template outputs.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, TemplateOutputParameter> Outputs { get; set; }
    }

    /// <summary>
    /// The template resource.
    /// </summary>
    public class TemplateResource : JsonLineInfo
    {
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the resource metadata.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JToken Metadata { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> of the resource.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Sku { get; set; }

        /// <summary>
        /// Gets or sets the kind of the resource.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the managed by property.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ManagedBy { get; set; }

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource <c>api</c>-version.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the resource group.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the availability zones.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Zones { get; set; }

        /// <summary>
        /// Gets or sets the resource plan.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Plan { get; set; }

        /// <summary>
        /// Gets or sets the resource identity.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Identity { get; set; }

        /// <summary>
        /// Gets or sets the resource tags.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Tags { get; set; }

        /// <summary>
        /// Gets or sets the resource scale.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Scale { get; set; }

        /// <summary>
        /// Gets or sets the resource properties.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Properties { get; set; }

        /// <summary>
        /// Gets or sets the list of nested resources.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public TemplateResource[] Resources { get; set; }

        /// <summary>
        /// Gets or sets the list of depends on resources.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string[] DependsOn { get; set; }

        /// <summary>
        /// Gets or sets the condition on the resource. 
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Condition { get; set; }
    }

    /// <summary>
    /// The template parameter type.
    /// </summary>
    public enum TemplateParameterType
    {
        /// <summary>
        /// The template parameter type is not specified.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// The template parameter type is string.
        /// </summary>
        String,

        /// <summary>
        /// The template parameter type is secure string.
        /// </summary>
        SecureString,

        /// <summary>
        /// The template parameter type is integer.
        /// </summary>
        Int,

        /// <summary>
        /// The template parameter type is boolean.
        /// </summary>
        Bool,

        /// <summary>
        /// The template parameter type is JSON array.
        /// </summary>
        Array,

        /// <summary>
        /// The template parameter type is JSON object.
        /// </summary>
        Object,

        /// <summary>
        /// The template parameter type is JSON secure object.
        /// </summary>
        SecureObject,
    }

    /// <summary>
    /// The template parameter.
    /// </summary>
    public abstract class TemplateParameter : JsonLineInfo
    {
        /// <summary>
        /// Gets or sets the parameter type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public TemplateParameterType Type { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Value { get; set; }

        /// <summary>
        /// Gets or sets the parameter metadata.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JToken Metadata { get; set; }
    }

    /// <summary>
    /// The template input parameter.
    /// </summary>
    public class TemplateInputParameter : TemplateParameter
    {
        /// <summary>
        /// Gets or sets the default parameter value.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JToken DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the allowed parameter values.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JArray AllowedValues { get; set; }

        /// <summary>
        /// Gets or sets the minimum parameter value.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum parameter value.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum parameter length.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum parameter length.
        /// </summary>
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? MaxLength { get; set; }
    }

    /// <summary>
    /// The template output parameter.
    /// </summary>
    public class TemplateOutputParameter : TemplateParameter
    {
    }
}

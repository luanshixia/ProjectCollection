using Dreambuild.Collections;
using Dreambuild.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BubbleFlow
{
    public class Workflow
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public WorkflowMetadata Metadata { get; set; }
        public JToken Properties { get; set; }

        [JsonIgnore]
        public Dictionary<Guid, FlowNode> NodesStore { get; internal set; }

        public List<FlowNode> Nodes
        {
            get => this.NodesStore?.Values.ToList(); // NOTE: get() should return null for JSON deserializaiton to work.
            set => this.NodesStore = value.ToDictionary(node => node.ID, node => node);
        }

        [JsonIgnore]
        public DoubleDictionary<Guid, Guid, FlowLink> LinksStore { get; internal set; }

        public List<FlowLink> Links
        {
            get => this.LinksStore?.RealValues.ToList(); // NOTE: get() should return null for JSON deserializaiton to work.
            set => this.LinksStore = value.ToDoubleDictionary(link => link.From, link => link.To, link => link);
        }

        public List<FlowLabel> Labels { get; set; }

        public static Workflow FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Workflow>(
                value: json,
                settings: new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(
                value: this,
                settings: new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                });
        }
    }

    public class FlowNode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public FlowNodeStatus Status { get; set; }
        public FlowElementMetadata Metadata { get; set; }
        public JToken Properties { get; set; }
    }

    public class FlowLink
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public string Label { get; set; }
    }

    public class FlowLabel
    {
        public string Text { get; set; }
        public FlowElementMetadata Metadata { get; set; }
    }

    public class WorkflowMetadata
    {
        public string FontFamily { get; set; }
        public double? FontSize { get; set; }
    }

    public class FlowElementMetadata
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public string FontFamily { get; set; }
        public double? FontSize { get; set; }
    }

    public enum FlowNodeStatus
    {
        Default,
        Active,
        Completed
    }
}

using Dreambuild.Collections;
using Dreambuild.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            get => this.NodesStore.Values.ToList();
            set => this.NodesStore = value.ToDictionary(node => node.ID, node => node);
        }

        [JsonIgnore]
        public DoubleDictionary<Guid, Guid, FlowLink> LinksStore { get; internal set; }

        public List<FlowLink> Links
        {
            get => this.LinksStore.RealValues.ToList();
            set => this.LinksStore = value.ToDoubleDictionary(link => link.From, link => link.To, link => link);
        }

        public List<FlowLabel> Labels { get; set; }

        public static Workflow FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Workflow>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class FlowNode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public FlowNodeStatus Status { get; set; }
        public FlowElementMetadata Metadata { get; set; }
        public JToken Properties { get; set; }

        //public string user { get; set; }
        //public string role { get; set; }
        //public double xpos { get; set; }
        //public double ypos { get; set; }
        //public string status { get; set; } // "passed", "current", "unreached"
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

        //public double xpos { get; set; }
        //public double ypos { get; set; }
        //public bool centerAligned { get; set; }
        //public double fontSize { get; set; }
        //public string fontFamily { get; set; }
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

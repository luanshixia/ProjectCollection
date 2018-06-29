using System;
using System.Collections.Generic;
using System.Linq;

namespace BubbleFlow
{
    public class Workflow
    {
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public List<FlowNode> Nodes { get; private set; }

        public Workflow()
        {
            Nodes = new List<FlowNode>();
        }

        public WorkflowJsonObject ToJsonObject()
        {
            WorkflowJsonObject json = new WorkflowJsonObject();
            json.nodes = this.Nodes.Select(x => new FlowNodeJsonObject { name = x.Name, user = x.AllowedUsers, role = x.AllowedRoles, xpos = x.FlowChartPositionX, ypos = x.FlowChartPositionY }).ToList();
            json.connections = this.Nodes.SelectMany(x => x.ToNodes.Select(y => new NodeConnectionJsonObject { from = this.Nodes.IndexOf(x), to = this.Nodes.IndexOf(y), label = GetLabel(x, y) })).ToList();
            return json;
        }

        private static string GetLabel(FlowNode from, FlowNode to)
        {
            if (string.IsNullOrEmpty(from.ToLabels))
            {
                return string.Empty;
            }
            var labels = from.ToLabels.Split('|');
            int index = from.ToNodes.IndexOf(to);
            if (index >= labels.Length)
            {
                return string.Empty;
            }
            return labels[from.ToNodes.IndexOf(to)];
        }

        public void DeleteNode(FlowNode node)
        {
            Nodes.Remove(node);
            Nodes.ForEach(x => x.ToNodes.Remove(node));
        }
    }

    public class FlowNode
    {
        public string Name { get; set; }
        //public List<FlowNode> FromNodes { get; set; }
        public List<FlowNode> ToNodes { get; set; }
        public string DisplayPage { get; set; }
        public string AllowedUsers { get; set; }
        public string AllowedRoles { get; set; }
        //public string FlowChartPosition { get; set; }
        public double FlowChartPositionX { get; set; }
        public double FlowChartPositionY { get; set; }
        public string ToLabels { get; set; }

        public FlowNode()
        {
            ToNodes = new List<FlowNode>();
        }

        public FlowNode(string name, string users = "*", string roles = "*")
        {
            Name = name;
            //FromNodes = new List<FlowNode>();
            ToNodes = new List<FlowNode>();
            AllowedUsers = users;
            AllowedRoles = roles;
        }

        public void ConnectTo(FlowNode node)
        {
            this.ToNodes.Add(node);
        }
    }

    public class WorkflowJsonObject
    {
        public string id { get; set; }
        public List<FlowNodeJsonObject> nodes { get; set; }
        public List<NodeConnectionJsonObject> connections { get; set; }
        public List<TextLabelJsonObject> labels { get; set; }

        public void MoveNodesToPositive()
        {
            double nodeSize = 100;
            double minx = nodes.Min(x => x.xpos) - 1 * nodeSize;
            double miny = nodes.Min(x => x.ypos) - 1 * nodeSize;
            if (minx < 0)
            {
                nodes.ForEach(x => x.xpos += -minx);
                labels.ForEach(x => x.xpos += -minx);
            }
            if (miny < 0)
            {
                nodes.ForEach(x => x.ypos += -miny);
                labels.ForEach(x => x.ypos += -miny);
            }
        }
    }

    public class FlowNodeJsonObject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string user { get; set; }
        public string role { get; set; }
        public double xpos { get; set; }
        public double ypos { get; set; }
        public string status { get; set; } // "passed", "current", "unreached"
    }

    public class NodeConnectionJsonObject
    {
        public int from { get; set; }
        public int to { get; set; }
        public string label { get; set; }
    }

    public class TextLabelJsonObject
    {
        public string text { get; set; }
        public double xpos { get; set; }
        public double ypos { get; set; }
        public bool centerAligned { get; set; }
        public double fontSize { get; set; }
        public string fontFamily { get; set; }
    }
}

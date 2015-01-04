using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TongJi.Web.Flow
{
    public class Workflow
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public string Group { get; set; }
        //public bool Enabled { get; set; }
        public List<FlowNode> Nodes { get; set; }
        //public int? TimeLimit { get; set; } // newly 20130709
    }

    public class FlowNode
    {
        //public int ID { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid WorkflowID { get; set; }
        public List<FlowNode> FromNodes { get; set; }
        public List<FlowNode> ToNodes { get; set; }
        public string DisplayPage { get; set; }
        public string AllowedUsers { get; set; }
        public string AllowedRoles { get; set; }
        public double FlowChartPositionX { get; set; }
        public double FlowChartPositionY { get; set; }
        public string ToLabels { get; set; }

        public FlowNode()
        {
        }

        public FlowNode(Guid id, string name, string users = "*", string roles = "*", double posx = 0, double posy = 0, string label = "")
        {
            ID = id;
            Name = name;
            FromNodes = new List<FlowNode>();
            ToNodes = new List<FlowNode>();
            AllowedUsers = users;
            AllowedRoles = roles;
            FlowChartPositionX = posx;
            FlowChartPositionY = posy;
            ToLabels = label;
        }
    }

    [Table("FlowNodeFlowNodes")]
    public class FlowNodeFlowNode1 // newly 20140107
    {
        [Key, Column(Order = 1)]
        public Guid? FlowNode_ID { get; set; }
        [Key, Column(Order = 2)]
        public Guid? FlowNode_ID1 { get; set; }
    }

    public class FlowInstance
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; } // newly 20130708
        public string Comment { get; set; } // newly 20130708
        public string Status { get; set; } // newly 20130711
        public Guid WorkflowID { get; set; }
        //public int CurrentNodeID { get; set; }
        public Guid CurrentNodeID { get; set; }
        public DateTime StartTime { get; set; }
        public bool Completed { get; set; }
        public DateTime EndTime { get; set; } // newly 20130605 20130708重新解释为：最后操作时间，而非结束时间
        //public DateTime? Deadline { get; set; } // newly 20130709

        public FlowInstance()
        {
        }

        public FlowInstance(string name, Guid flowID, Guid startNodeID)
        {
            Name = name;
            WorkflowID = flowID;
            CurrentNodeID = startNodeID;
            StartTime = DateTime.Now;
            Completed = false;
            EndTime = DateTime.Now;
        }
    }

    public class FlowInstanceAction
    {
        public int ID { get; set; }
        public int FlowInstanceID { get; set; }
        public DateTime PerformTime { get; set; }
        public string Performer { get; set; }
        //public int FromNodeID { get; set; }
        //public int ToNodeID { get; set; }
        public Guid FromNodeID { get; set; }
        public Guid ToNodeID { get; set; }

        public FlowInstanceAction()
        {
        }

        public FlowInstanceAction(int instID, Guid fromNodeID, Guid toNodeID, string performer)
        {
            FlowInstanceID = instID;
            FromNodeID = fromNodeID;
            ToNodeID = toNodeID;
            PerformTime = DateTime.Now;
            Performer = performer;
        }
    }

    public class FlowChartTextLabel
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public Guid WorkflowID { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool CenterAligned { get; set; }
        public double FontSize { get; set; }
        public string FontFamily { get; set; }
    }
}

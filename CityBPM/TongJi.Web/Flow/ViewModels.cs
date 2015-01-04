using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongJi.Web.Flow
{

    //
    // ViewModels
    //

    public class TodoListItem
    {
        public int ID { get; set; }
        public string FlowName { get; set; }
        public string InstName { get; set; }
        public string InstNumber { get; set; } // newly 20130708
        public string InstComment { get; set; } // newly 20130708
        public string CurrentNode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; } // 最后操作时间
        public int InstID { get; set; }
    }

    public class TrackBackListItem
    {
        public int ID { get; set; }
        public DateTime PerformTime { get; set; }
        public string Performer { get; set; }
        public string FlowName { get; set; }
        public string InstName { get; set; }
        public string InstNumber { get; set; }
        public string FromNodeName { get; set; }
        public string ToNodeName { get; set; }
    }

    public class FlowListItem
    {
        public string ID { get; set; }
        public string FlowName { get; set; }
        public string Group { get; set; }
        public DateTime CreateTime { get; set; }
        public int NodeCount { get; set; }
        public int InstanceCount { get; set; }
        public int ActiveInstanceCount { get; set; }
    }

    public class NodeListItem
    {
        public string ID { get; set; }
        public string NodeName { get; set; }
        public string FromNodes { get; set; }
        public string ToNodes { get; set; }
        public string Users { get; set; }
        public string Roles { get; set; }
    }
}

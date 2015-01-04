using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongJi.Web.Flow
{

    //
    // Json Objects
    //

    public class WorkflowJsonObject
    {
        public string id { get; set; }
        public List<FlowNodeJsonObject> nodes { get; set; }
        public List<NodeConnectionJsonObject> connections { get; set; }
        public List<TextLabelJsonObject> labels { get; set; }
    }

    public class FlowNodeJsonObject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string user { get; set; }
        public string role { get; set; }
        public double xpos { get; set; }
        public double ypos { get; set; }
        public string status { get; set; }
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

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.Generic;
using System.Linq;

using System.Json;

namespace SlFlowLib
{
    public static class DataManager
    {
        public const double NodeSize = 100;

        public static string ToJson(WorkflowJsonObject workflowJson)
        {
            JsonObject json = new JsonObject(new List<KeyValuePair<string,JsonValue>>());
            
            JsonArray jsonnodes = new JsonArray(new List<JsonValue>());
            JsonArray jsonconns = new JsonArray(new List<JsonValue>());

            foreach (var node in workflowJson.nodes)
            {
                JsonObject jsonNodeVals = new JsonObject(new List<KeyValuePair<string, JsonValue>>());

                JsonPrimitive value_id = new JsonPrimitive(node.id);
                JsonPrimitive value_name = new JsonPrimitive(node.name);
                JsonPrimitive value_user = new JsonPrimitive(node.user);
                JsonPrimitive value_role = new JsonPrimitive(node.role);
                JsonPrimitive value_xpos = new JsonPrimitive(node.xpos);
                JsonPrimitive value_ypos = new JsonPrimitive(node.ypos);
                JsonPrimitive value_status = new JsonPrimitive("");

                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("id", value_id));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("name", value_name));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("user", value_user));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("role", value_role));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("xpos", value_xpos));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("ypos", value_ypos));
                jsonNodeVals.Add(new KeyValuePair<string, JsonValue>("status", value_status));

                jsonnodes.Add(jsonNodeVals);
            }

            foreach (var conn in workflowJson.connections)
            {
                JsonObject jsonConnVals = new JsonObject(new List<KeyValuePair<string, JsonValue>>());
                
                JsonPrimitive value_from = new JsonPrimitive(conn.from);
                JsonPrimitive value_to = new JsonPrimitive(conn.to);
                JsonPrimitive value_label = new JsonPrimitive(conn.label);

                jsonConnVals.Add(new KeyValuePair<string, JsonValue>("from", value_from));
                jsonConnVals.Add(new KeyValuePair<string, JsonValue>("to", value_to));
                jsonConnVals.Add(new KeyValuePair<string, JsonValue>("label", value_label));

                jsonconns.Add(jsonConnVals);
            }

            json.Add(new KeyValuePair<string, JsonValue>("id", workflowJson.id));
            json.Add(new KeyValuePair<string, JsonValue>("nodes", jsonnodes));
            json.Add(new KeyValuePair<string, JsonValue>("connections", jsonconns));

            return json.ToString();
        }

        public static WorkflowJsonObject ParseJson(string json)
        {
            var data = JsonValue.Parse(json);
            var result = new WorkflowJsonObject();
            result.nodes = new List<FlowNodeJsonObject>();
            result.connections = new List<NodeConnectionJsonObject>();
            result.labels = new List<TextLabelJsonObject>();
            foreach (JsonValue node in data["nodes"])
            {
                var nodeResult = new FlowNodeJsonObject();
                nodeResult.id = node["id"];
                nodeResult.name = node["name"];
                nodeResult.user = node["user"];
                nodeResult.role = node["role"];
                nodeResult.xpos = node["xpos"];
                nodeResult.ypos = node["ypos"];
                nodeResult.status = node["status"];
                result.nodes.Add(nodeResult);
            }
            foreach (JsonValue conn in data["connections"])
            {
                var connResult = new NodeConnectionJsonObject();
                connResult.from = conn["from"];
                connResult.to = conn["to"];
                connResult.label = conn["label"];
                result.connections.Add(connResult);
            }
            foreach (JsonValue label in data["labels"])
            {
                var labelResult = new TextLabelJsonObject();
                labelResult.text = label["text"];
                labelResult.xpos = label["xpos"];
                labelResult.ypos = label["ypos"];
                labelResult.centerAligned = label["centerAligned"];
                labelResult.fontSize = label["fontSize"];
                labelResult.fontFamily = label["fontFamily"];
                result.labels.Add(labelResult);
            }
            result.id = data["id"];
            return result;
        }

        public static void DrawToCanvas(Canvas canvas, WorkflowJsonObject flow)
        {
            flow.MoveNodesToPositive();
            canvas.Children.Clear();

            foreach (var node in flow.nodes)
            {
                Node nodeMark = new Node();
                nodeMark.Text = node.name;
                //if (node.pos.Length > 0)
                //{
                //    nodeMark.Position = ParsePoint(node.pos);
                //}
                nodeMark.Position = new Point(node.xpos, node.ypos);
                if (node.status == "passed")
                {
                    nodeMark.FillColor = Color.FromArgb(255, 200, 250, 100);
                }
                else if (node.status == "current")
                {
                    nodeMark.FillColor = Colors.Orange;
                    nodeMark.NeedAlert = true;
                }

                nodeMark.ReadyControl();
                Canvas.SetZIndex(nodeMark, 100);
                canvas.Children.Add(nodeMark);
            }

            foreach (var conn in flow.connections)
            {
                //Arrow arrow = new Arrow();
                BezierLink arrow = new BezierLink();
                arrow.StartPoint = ParsePoint(flow.nodes[conn.from]);
                arrow.EndPoint = ParsePoint(flow.nodes[conn.to]);
                arrow.StartOffset = NodeSize / 2;
                arrow.EndOffset = NodeSize / 2;
                //arrow.ArrowSize = NodeSize / 10;
                //arrow.LabelText = conn.label;

                arrow.ReadyControl();
                canvas.Children.Add(arrow);
            }

            foreach (var label in flow.labels)
            {
                TextBlock textLabel = new TextBlock();
                textLabel.Text = label.text;
                textLabel.FontSize = 9;
                textLabel.Foreground = new SolidColorBrush(Colors.Gray);
                Canvas.SetLeft(textLabel, label.xpos);
                Canvas.SetTop(textLabel, label.ypos);
                canvas.Children.Add(textLabel);
            }

            //canvas.Width = flow.nodes.Max(x => x.xpos) + 2 * NodeSize;
            //canvas.Height = flow.nodes.Max(x => x.ypos) + 2 * NodeSize;
        }

        private static Point ParsePoint(string pointStr)
        {
            var coords = pointStr.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            return new Point(coords[0], coords[1]);
        }

        private static Point ParsePoint(FlowNodeJsonObject node)
        {
            return new Point(node.xpos, node.ypos);
        }
    }
}

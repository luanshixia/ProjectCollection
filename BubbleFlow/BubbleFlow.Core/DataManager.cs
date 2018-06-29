using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BubbleFlow
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
            var result = new WorkflowJsonObject
            {
                nodes = new List<FlowNodeJsonObject>(),
                connections = new List<NodeConnectionJsonObject>(),
                labels = new List<TextLabelJsonObject>()
            };

            foreach (JsonValue node in data["nodes"])
            {
                var nodeResult = new FlowNodeJsonObject
                {
                    id = node["id"],
                    name = node["name"],
                    user = node["user"],
                    role = node["role"],
                    xpos = node["xpos"],
                    ypos = node["ypos"],
                    status = node["status"]
                };
                result.nodes.Add(nodeResult);
            }

            foreach (JsonValue conn in data["connections"])
            {
                var connResult = new NodeConnectionJsonObject
                {
                    from = conn["from"],
                    to = conn["to"],
                    label = conn["label"]
                };
                result.connections.Add(connResult);
            }

            foreach (JsonValue label in data["labels"])
            {
                var labelResult = new TextLabelJsonObject
                {
                    text = label["text"],
                    xpos = label["xpos"],
                    ypos = label["ypos"],
                    centerAligned = label["centerAligned"],
                    fontSize = label["fontSize"],
                    fontFamily = label["fontFamily"]
                };
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
                var nodeMark = new Node
                {
                    Text = node.name,
                    //if (node.pos.Length > 0)
                    //{
                    //    nodeMark.Position = ParsePoint(node.pos);
                    //}
                    Position = new Point(node.xpos, node.ypos)
                };

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
                var arrow = new BezierLink
                {
                    StartPoint = ParsePoint(flow.nodes[conn.from]),
                    EndPoint = ParsePoint(flow.nodes[conn.to]),
                    StartOffset = NodeSize / 2,
                    EndOffset = NodeSize / 2
                };
                //Arrow arrow = new Arrow();
                //arrow.ArrowSize = NodeSize / 10;
                //arrow.LabelText = conn.label;

                arrow.ReadyControl();
                canvas.Children.Add(arrow);
            }

            foreach (var label in flow.labels)
            {
                var textLabel = new TextBlock
                {
                    Text = label.text,
                    FontSize = 9,
                    Foreground = new SolidColorBrush(Colors.Gray)
                };

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

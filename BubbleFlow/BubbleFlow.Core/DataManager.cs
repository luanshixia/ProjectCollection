using Dreambuild.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BubbleFlow
{
    public static class DataManager
    {
        public const double NodeSize = 100;

        public static string CurrentFileName { get; private set; }

        public static Workflow CurrentDocument { get; private set; }

        public static void New()
        {
            DataManager.CurrentFileName = null;
            DataManager.CurrentDocument = new Workflow
            {
                ID = Guid.NewGuid(),
                Name = "Untitled workflow",
                Metadata = new WorkflowMetadata
                {
                    FontFamily = "Arial",
                    FontSize = 16
                },
                NodesStore = new Dictionary<Guid, FlowNode>(),
                LinksStore = new DoubleDictionary<Guid, Guid, FlowLink>(),
                Labels = new List<FlowLabel>()
            };
        }

        public static void Open(string fileName)
        {
            DataManager.CurrentFileName = fileName;
            DataManager.CurrentDocument = Workflow.FromJson(File.ReadAllText(fileName));
        }

        public static bool Validate()
        {
            return true;
        }

        public static void SaveAs(string fileName)
        {
            DataManager.CurrentFileName = fileName;
            File.WriteAllText(fileName, DataManager.CurrentDocument.ToJson());
        }

        public static void DrawToCanvas(this Workflow flow, Canvas canvas)
        {
            //flow.MoveNodesToPositive();
            canvas.Children.Clear();

            foreach (var node in flow.Nodes)
            {
                var bubble = new NodeBubble
                {
                    Text = node.Name,
                    Position = node.GetPosition()
                };

                if (node.Status == FlowNodeStatus.Completed)
                {
                    bubble.FillColor = Color.FromArgb(255, 200, 250, 100);
                }
                else if (node.Status == FlowNodeStatus.Active)
                {
                    bubble.FillColor = Colors.Orange;
                    bubble.NeedAlert = true;
                }

                bubble.ReadyControl();
                Canvas.SetZIndex(bubble, 100);
                canvas.Children.Add(bubble);
            }

            foreach (var link in flow.Links)
            {
                var arrow = new BezierLink
                {
                    StartPoint = flow.NodesStore[link.From].GetPosition(),
                    EndPoint = flow.NodesStore[link.To].GetPosition(),
                    StartOffset = DataManager.NodeSize / 2,
                    EndOffset = DataManager.NodeSize / 2
                };

                arrow.ReadyControl();
                canvas.Children.Add(arrow);
            }

            foreach (var label in flow.Labels)
            {
                var textLabel = new TextBlock
                {
                    Text = label.Text,
                    FontSize = 9,
                    Foreground = new SolidColorBrush(Colors.Gray)
                };

                Canvas.SetLeft(textLabel, label.Metadata.Left);
                Canvas.SetTop(textLabel, label.Metadata.Top);
                canvas.Children.Add(textLabel);
            }
        }

        public static Point GetPosition(this FlowNode node)
        {
            return new Point(node.Metadata.Left, node.Metadata.Top);
        }

        public static void MoveNodesToPositive(this Workflow flow)
        {
            double minLeft = flow.Nodes.Min(node => node.Metadata.Left) - 1 * DataManager.NodeSize;
            double minTop = flow.Nodes.Min(node => node.Metadata.Top) - 1 * DataManager.NodeSize;

            if (minLeft < 0)
            {
                flow.Nodes.ForEach(node => node.Metadata.Left += -minLeft);
                flow.Labels.ForEach(label => label.Metadata.Left += -minLeft);
            }

            if (minTop < 0)
            {
                flow.Nodes.ForEach(node => node.Metadata.Top += -minTop);
                flow.Labels.ForEach(label => label.Metadata.Top += -minTop);
            }
        }
    }
}

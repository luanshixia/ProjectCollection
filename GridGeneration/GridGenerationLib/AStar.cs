using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridGenerationLib
{
    public class Node
    {
        public int x { get; protected set; }
        public int y { get; protected set; }
        public double f;
        public double g;
        public double h;
        public bool walkable;
        public Node parent;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            walkable = true;
        }
    }

    public class NodeGrid
    {
        public Node[,] nodes { get; protected set; }
        public int numCols { get; protected set; }
        public int numRows { get; protected set; }

        //
        // Constructor
        //
        public NodeGrid(int numCols, int numRows)
        {
            this.numCols = numCols;
            this.numRows = numRows;
            nodes = new Node[numCols, numRows];

            for (int i = 0; i < numCols; i++)
            {
                for (int j = 0; j < numRows; j++)
                {
                    nodes[i, j] = new Node(i, j);
                }
            }
        }

        //
        // Methods
        //
    }

    public class AStar
    {
        private List<Node> _open;
        private List<Node> _closed;
        private NodeGrid _grid;
        private Node _startNode;
        private Node _endNode;

        public List<Node> path { get; protected set; }
        public Func<Node, double> heuristic;

        public const double straightCost = 1.0;
        public const double diagCost = 100.0;
        public const double turnCost = 10.0;
        public const double hMag = 1.8;

        public AStar(NodeGrid grid, int startX, int startY, int endX, int endY)
        {
            this._open = new List<Node>();
            this._closed = new List<Node>();
            this._grid = grid;
            this._startNode = grid.nodes[startX, startY];
            this._endNode = grid.nodes[endX, endY];
            this.path = new List<Node>();

            heuristic = this.manhattan;

            _startNode.g = 0;
            _startNode.h = heuristic(_startNode);
            _startNode.f = _startNode.g + hMag * _startNode.h;
        }

        public bool search()
        {
            Node node = _startNode;
            while (node != _endNode)
            {
                int startX = Math.Max(0, node.x - 1);
                int endX = Math.Min(_grid.numCols - 1, node.x + 1);
                int startY = Math.Max(0, node.y - 1);
                int endY = Math.Min(_grid.numRows - 1, node.y + 1);

                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        Node test = _grid.nodes[i, j];
                        if (test == node || !test.walkable)
                        {
                            continue;
                        }

                        double cost = straightCost;
                        if (!((node.x == test.x) || (node.y == test.y)))
                        {
                            cost = diagCost;
                        }
                        //if (_closed.Count > 0)
                        //{
                        //    Node last = _closed.Last();
                        //    if (!((last.x == node.x && node.x == test.x) || (last.y == node.y && node.y == test.y)))
                        //    {
                        //        cost += turnCost;
                        //    }
                        //}
                        double g = node.g + cost;
                        double h = heuristic(test);
                        double f = g + hMag * h;
                        if (_open.Contains(test) || _closed.Contains(test))
                        {
                            if (test.f > f)
                            {
                                test.f = f;
                                test.g = g;
                                test.h = h;
                                test.parent = node;
                            }
                        }
                        else
                        {
                            test.f = f;
                            test.g = g;
                            test.h = h;
                            test.parent = node;
                            _open.Add(test);
                        }
                    }
                }
                _closed.Add(node);
                if (_open.Count == 0)
                {
                    return false;
                }
                _open = _open.OrderBy(x => x.f).ToList();
                node = _open.First();
                _open.RemoveAt(0);
            }
            buildPath();
            return true;
        }

        private void buildPath()
        {
            path.Clear();
            Node node = _endNode;
            path.Add(node);
            while (node != _startNode)
            {
                node = node.parent;
                path.Insert(0, node);
            }
            path.RemoveAt(path.Count - 1);
            path.RemoveAt(0);
        }

        public List<Node> getPathSides()
        {
            return path.SelectMany(n =>
            {
                int startX = Math.Max(0, n.x - 1);
                int endX = Math.Min(_grid.numCols - 1, n.x + 1);
                int startY = Math.Max(0, n.y - 1);
                int endY = Math.Min(_grid.numRows - 1, n.y + 1);

                List<Node> nodes = new List<Node>();
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        if (_grid.nodes[i, j] != _startNode && _grid.nodes[i, j] != _endNode)
                        {
                            nodes.Add(_grid.nodes[i, j]);
                        }
                    }
                }
                return nodes;
            }).Distinct().Where(n => !path.Contains(n)).ToList();
        }

        public double manhattan(Node node)
        {
            return Math.Abs(node.x - _endNode.x) * straightCost + Math.Abs(node.y - _endNode.y) * straightCost;
        }

        public double euclidian(Node node)
        {
            double dx = node.x - _endNode.x;
            double dy = node.y - _endNode.y;
            return Math.Sqrt(dx * dx + dy * dy) * straightCost;
        }

        public double diagonal(Node node)
        {
            double dx = Math.Abs(node.x - _endNode.x);
            double dy = Math.Abs(node.y - _endNode.y);
            double diag = Math.Min(dx, dy);
            double straight = dx + dy;
            return diagCost * diag + straightCost * (straight - 2 * diag);
        }
    }
}

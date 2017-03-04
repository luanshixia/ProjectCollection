using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Gis.Topology
{
    public struct Position
    {
        public double X;
        public double Y;

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position b)
        {
            var a = this;
            double tol = 1e-6;
            double delta = (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
            if (delta < tol)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0:0.##},{1:0.##}]", X, Y);
        }
    }

    /// <summary>
    /// 拓扑边
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// 获取或设置起点.
        /// </summary>
        public Position StartPoint { get; set; }
        /// <summary>
        /// 获取或设置终点.
        /// </summary>
        public Position EndPoint { get; set; }
        /// <summary>
        /// 获取或设置边长.
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 获取或设置阻力系数.
        /// </summary>
        public double ResistanceFactor { get; set; }
        /// <summary>
        /// 获取阻力.
        /// </summary>
        public double Resistance { get { return Length * ResistanceFactor; } }

        //改进
        /// <summary>
        /// 获取一条边的另一点.
        /// </summary>
        /// <param name="oneNode">已知点</param>
        /// <param name="topo">拓扑网络</param>
        /// <returns></returns>
        public Node GetTheOtherNode(Node oneNode, TopoNetwork topo)
        {
            if (oneNode.Position.Equals(StartPoint))
            {
                return topo.Nodes.Single(x => x.Position.Equals(EndPoint));
            }
            else if (oneNode.Position.Equals(EndPoint))
            {
                return topo.Nodes.Single(x => x.Position.Equals(StartPoint));
            }
            return new Node();
        }
    }

    /// <summary>
    /// 拓扑结点
    /// </summary>
    public class Node
    {
        private Position _position;
        private List<Edge> _edges;
        /// <summary>
        /// 获取几何点.
        /// </summary>
        public Position Position { get { return _position; } }
        /// <summary>
        /// 获取该点发出的所有边.
        /// </summary>
        public List<Edge> Edges { get { return _edges; } } //改进

        public Node()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">几何点</param>
        public Node(Position position)
        {
            _position = position;
            _edges = new List<Edge>(); //********
        }

        //改进
        /// <summary>
        /// 求解该点发出的所有边.
        /// </summary>
        /// <param name="topoNet">拓扑网络</param>
        public void FindEdges(TopoNetwork topoNet)
        {
            Position pos = _position;
            _edges = topoNet.Edges.Where(x => x.StartPoint.Equals(pos) || x.EndPoint.Equals(pos)).ToList<Edge>();
        }

        //改进
        /// <summary>
        /// 沿一条边前进,获取下一点.
        /// </summary>
        /// <param name="overThisEdge">沿哪一条边</param>
        /// <param name="topo">拓扑网络</param>
        /// <returns></returns>
        public Node NextNode(Edge overThisEdge, TopoNetwork topo)
        {
            return overThisEdge.GetTheOtherNode(this, topo);
        }

        /// <summary>
        /// 获取结点的坐标表示.
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return string.Format("({0:0.##},{1:0.##})", _position.X, _position.Y);
        }
    }

    /// <summary>
    /// 拓扑网络
    /// </summary>
    public class TopoNetwork
    {
        private List<Node> _nodes;
        private List<Edge> _edges;
        /// <summary>
        /// 获取所有结点.
        /// </summary>
        public List<Node> Nodes { get { return _nodes; } }
        /// <summary>
        /// 获取所有边.
        /// </summary>
        public List<Edge> Edges { get { return _edges; } }

        /// <summary>
        /// 创建拓扑网络,只需传入边,自动生成结点.
        /// </summary>
        /// <param name="edges"></param>
        public TopoNetwork(List<Edge> edges)
        {
            _edges = edges;
            var points = edges.Select(x => x.StartPoint).Union(edges.Select(y => y.EndPoint)).OrderBy(x => x.X).Distinct().ToList();
            List<Position> points1 = new List<Position>();
            if (points.Count > 0)
            {
                points1.Add(points[0]);
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (!points[i + 1].Equals(points[i]))
                {
                    points1.Add(points[i + 1]);
                }
            }
            _nodes = points1.Select(x => new Node(x)).ToList<Node>();

            int n = _nodes.Count;
            for (int i = 0; i < n; i++) //不能用foreach, 变量不能更改.
            {
                _nodes[i].FindEdges(this);
            }

        }

        private TopoNetwork()
        {
        }
    }

    public class BestPathMatrix
    {
        private TopoNetwork TopoNet;
        public int MatrixLevel
        {
            get
            {
                return TopoNet.Nodes.Count;
            }
        }
        public int MaxIterationStep;
        public double[,] Matrix;
        public int[,] PathMatrix;

        public BestPathMatrix(TopoNetwork topoNet)
        {
            TopoNet = topoNet;
            Matrix = new double[MatrixLevel, MatrixLevel];
            PathMatrix = new int[MatrixLevel, MatrixLevel];
            MaxIterationStep = (int)Math.Floor(Math.Log10(MatrixLevel - 1) / Math.Log10(2)) + 1;

            InitializeMatrix();
        }

        private void InitializeMatrix()
        {
            //工作矩阵全置正无穷，路径矩阵全置-1
            for (int i = 0; i < MatrixLevel; i++)
            {
                for (int j = 0; j < MatrixLevel; j++)
                {
                    Matrix[i, j] = double.PositiveInfinity;
                    PathMatrix[i, j] = -1;
                }
            }

            //工作矩阵对角线置0
            for (int i = 0; i < MatrixLevel; i++)
            {
                Matrix[i, i] = 0;
            }

            //工作矩阵设置
            List<Position> topoNetPoints = TopoNet.Nodes.Select(x => x.Position).ToList();
            foreach (Edge edg in TopoNet.Edges)
            {
                int i = topoNetPoints.IndexOf(edg.StartPoint);
                int j = topoNetPoints.IndexOf(edg.EndPoint);
                Matrix[i, j] = edg.Resistance;
                Matrix[j, i] = edg.Resistance;
            }
        }

        public void CalculateMatrix()
        {
            for (int n = 0; n < MaxIterationStep; n++)
            {
                bool IsDifferent = false;
                for (int i = 0; i < MatrixLevel; i++)
                {
                    for (int j = 0; j < MatrixLevel; j++)
                    {
                        if (i == j) continue;
                        double min = double.MaxValue;
                        int minIndex = -1;
                        for (int l = 0; l < MatrixLevel; l++)
                        {
                            double temp = Matrix[i, l] + Matrix[l, j];
                            if (temp < min)
                            {
                                min = temp;
                                minIndex = l;
                            }
                        }
                        if (min < Matrix[i, j])
                        {
                            Matrix[i, j] = min;
                            PathMatrix[i, j] = minIndex;
                            IsDifferent = true;
                        }
                    }
                }
                if (!IsDifferent) break; //提前结束迭代
            }
        }
    }
    public class NetworkCleaner
    {
        private List<PointString> _roadPolys;

        public NetworkCleaner(List<PointString> polys)
        {
            _roadPolys = polys;
        }

        private List<PointString> DivOnePoly(int index)
        {
            PointString poly = _roadPolys[index];
            List<double> divs = new List<double>();
            for (int i = 0; i < _roadPolys.Count; i++)
            {
                if (i != index)
                {
                    PointString poly2 = _roadPolys[i];
                    for (int j = 0; j < poly.Points.Count - 1; j++)
                    {
                        LineSeg seg = poly.GetSegAt(j);
                        for (int k = 0; k < poly2.Points.Count - 1; k++)
                        {
                            LineSeg seg2 = poly2.GetSegAt(k);
                            LineSegIntersect intr = new LineSegIntersect(seg, seg2);
                            if (intr.Intersect())
                            {
                                divs.Add(j + intr.RatioAB);
                            }
                        }
                    }
                }
            }
            divs.Insert(0, 0);
            divs.Add(poly.Points.Count - 1);
            divs = divs.Distinct().OrderBy(x => x).ToList();
            List<PointString> result = new List<PointString>();
            for (int i = 0; i < divs.Count - 1; i++)
            {
                PointString subPoly = poly.GetSubPoly(divs[i], divs[i + 1]);
                if (subPoly.Length() > 0.1)  // subtle
                {
                    result.Add(subPoly);
                }
            }
            return result;
        }

        public List<PointString> GetAllDivs()
        {
            return _roadPolys.SelectMany(x => DivOnePoly(_roadPolys.IndexOf(x))).ToList();
        }
    }

    public class PathFinder
    {
        private BestPathMatrix matrix;
        private TopoNetwork network;

        public List<PointString> divPolys;

        public PathFinder(List<PointString> roads)
        {
            NetworkCleaner cleaner = new NetworkCleaner(roads);
            List<PointString> divs = cleaner.GetAllDivs();
            divPolys = divs;
            List<Edge> edges = divs.Select(v => new Edge()
            {
                StartPoint = new Position(v.Points.First().X, v.Points.First().Y),
                EndPoint = new Position(v.Points.Last().X, v.Points.Last().Y),
                Length = v.Length(),
                ResistanceFactor = 1
            }).ToList();
            network = new TopoNetwork(edges);
            matrix = new BestPathMatrix(network);
            matrix.CalculateMatrix();
        }

        public List<Node> Nodes
        {
            get
            {
                return network.Nodes;
            }
        }

        public List<Edge> Edges
        {
            get
            {
                return network.Edges;
            }
        }

        private double GetDist(int i, int j)
        {
            return matrix.Matrix[i, j];
        }

        private List<int> GetPath(int i, int j)
        {
            List<int> result = new List<int>() { i, j };
            Func<int, bool> ins = (k) =>
            {
                int t = matrix.PathMatrix[result[k], result[k + 1]];
                if (t != -1)
                {
                    if (t != result[k] && t != result[k + 1])
                    {
                        result.Insert(k + 1, t);
                        return true;
                    }
                }
                return false;
            };
            int count = 0;
            while (result.Count != count)
            {
                count = result.Count;
                for (int k = 0; k < result.Count - 1; k++)
                {
                    if (ins(k))
                    {
                        k++;
                    }
                }
            }
            return result;
        }

        private PointString GetPathPoly(int i, int j)
        {
            return new PointString(GetPath(i, j).Select(x => new Vector(Nodes[x].Position.X, Nodes[x].Position.Y)).ToList());
        }

        public double GetDist(Vector p1, Vector p2)
        {
            double d1 = Nodes.Min(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)));
            double d2 = Nodes.Min(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)));
            Node n1 = Nodes.First(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)) == d1);
            Node n2 = Nodes.First(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)) == d2);
            return GetDist(Nodes.IndexOf(n1), Nodes.IndexOf(n2));
        }

        public PointString GetPathPoly(Vector p1, Vector p2)
        {
            double d1 = Nodes.Min(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)));
            double d2 = Nodes.Min(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)));
            Node n1 = Nodes.First(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)) == d1);
            Node n2 = Nodes.First(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)) == d2);
            return GetPathPoly(Nodes.IndexOf(n1), Nodes.IndexOf(n2));
        }

        public List<PointString> GetRealPath(Vector p1, Vector p2)
        {
            double d1 = Nodes.Min(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)));
            double d2 = Nodes.Min(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)));
            Node n1 = Nodes.First(x => p1.Dist(new Vector(x.Position.X, x.Position.Y)) == d1);
            Node n2 = Nodes.First(x => p2.Dist(new Vector(x.Position.X, x.Position.Y)) == d2);
            List<int> path = GetPath(Nodes.IndexOf(n1), Nodes.IndexOf(n2));
            List<PointString> result = new List<PointString>();
            try
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var t1 = new Vector(Nodes[path[i]].Position.X, Nodes[path[i]].Position.Y);
                    var t2 = new Vector(Nodes[path[i + 1]].Position.X, Nodes[path[i + 1]].Position.Y);
                    PointString polySeg = divPolys.First(x => ((x.Points.First().Equals(t1) && x.Points.Last().Equals(t2)) || (x.Points.First().Equals(t2) && x.Points.Last().Equals(t1))));
                    result.Add(polySeg);
                }
            }
            catch
            {
                result = new List<PointString>();
            }
            return result;
        }
    }
}

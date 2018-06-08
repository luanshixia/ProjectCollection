using Dreambuild.Collections;
using Dreambuild.Data;
using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Algorithms
{
    /// <summary>
    /// All pairs shortest paths - Floyd-Warshall algorithm.
    /// https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm
    /// </summary>
    public class AllPairsShortestPaths
    {
        private Graph<int, double> Graph;

        private long[] Nodes;

        private DoubleDictionary<long, long, double> Dist = new DoubleDictionary<long, long, double>();

        private DoubleDictionary<long, long, long> Next = new DoubleDictionary<long, long, long>();

        public AllPairsShortestPaths(Graph<int, double> graph, bool directed = false)
        {
            this.Graph = graph;
            this.Nodes = graph.GetVertices().Select(vertex => vertex.ID).ToArray();

            this.Nodes.Cross(this.Nodes, (t, u) => this.Dist[t][u] = double.PositiveInfinity);
            this.Nodes.Cross(this.Nodes, (t, u) => this.Next[t][u] = -1);

            graph.GetEdges().ForEach(edge =>
            {
                this.Dist[edge.Source][edge.Destination] = edge.Property;
                this.Next[edge.Source][edge.Destination] = edge.Destination;

                if (!directed)
                {
                    this.Dist[edge.Destination][edge.Source] = edge.Property;
                    this.Next[edge.Destination][edge.Source] = edge.Source;
                }
            });

            this.Nodes.ForEach(id => this.Dist[id][id] = 0);

            foreach (var k in this.Nodes)
            {
                foreach (var i in this.Nodes)
                {
                    foreach (var j in this.Nodes)
                    {
                        if (this.Dist[i][j] > this.Dist[i][k] + this.Dist[k][j])
                        {
                            this.Dist[i][j] = this.Dist[i][k] + this.Dist[k][j];
                            this.Next[i][j] = this.Next[i][k];
                        }
                    }
                }
            }
        }

        public double GetDist(long i, long j)
        {
            return this.Dist[i][j];
        }

        public IList<long> GetPath(long i, long j)
        {
            if (this.Next[i][j] == -1)
            {
                return EmptyList<long>.Instance;
            }

            var path = i.WrapInList();
            while (i != j)
            {
                i = this.Next[i][j];
                path.Add(i);
            }

            return path;
        }
    }
}

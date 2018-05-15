using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Data
{
    public class Vertex<TProperty>
    {
        public long ID { get; set; }

        public TProperty Property { get; set; }

        public Vertex(long id, TProperty property = default(TProperty))
        {
            this.ID = id;
            this.Property = property;
        }
    }

    public class Edge<TProperty>
    {
        public long Source { get; set; }

        public long Destination { get; set; }

        public TProperty Property { get; set; }

        public Edge(long source, long destination, TProperty property = default(TProperty))
        {
            this.Source = source;
            this.Destination = destination;
            this.Property = property;
        }
    }

    public class Graph<TVertex, TEdge>
    {
        public Dictionary<long, Vertex<TVertex>> Vertices { get; protected set; }

        public Dictionary<long, Edge<TEdge>> OutgoingEdges { get; protected set; }

        public Dictionary<long, Edge<TEdge>> IncomingEdges { get; protected set; }

        protected Graph()
        {
            this.Vertices = new Dictionary<long, Vertex<TVertex>>();
            this.OutgoingEdges = new Dictionary<long, Edge<TEdge>>();
            this.IncomingEdges = new Dictionary<long, Edge<TEdge>>();
        }

        public Graph(IEnumerable<Vertex<TVertex>> vertices, IEnumerable<Edge<TEdge>> edges)
        {
        }
    }
}

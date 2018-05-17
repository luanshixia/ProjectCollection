using Dreambuild.Collections;
using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Edge<TProperty> Reverse()
        {
            return new Edge<TProperty>(
                source: this.Destination,
                destination: this.Source,
                property: this.Property);
        }
    }

    public enum EdgeDirection
    {
        Both,
        Either,
        Incoming,
        Outgoing
    }

    public class EdgeTriplet<TVertex, TEdge> : Edge<TEdge>
    {
        public TVertex SourceProperty { get; set; }

        public TVertex DestinationProperty { get; set; }

        public EdgeTriplet(
            long source, 
            long destination, 
            TVertex sourceProperty = default(TVertex), 
            TEdge edgeProperty = default(TEdge), 
            TVertex destinationProperty = default(TVertex))
            : base(source, destination, edgeProperty)
        {
            this.SourceProperty = sourceProperty;
            this.DestinationProperty = destinationProperty;
        }
    }

    public class Graph<TVertex, TEdge>
    {
        public IEnumerable<Vertex<TVertex>> Vertices { get; protected set; }

        public IEnumerable<Edge<TEdge>> Edges { get; protected set; }

        protected Dictionary<long, Vertex<TVertex>> VerticesIndex { get; set; }

        protected DoubleDictionary<long, long, Edge<TEdge>> OutgoingEdges { get; set; }

        protected DoubleDictionary<long, long, Edge<TEdge>> IncomingEdges { get; set; }

        protected Graph()
        {
            this.Vertices = new List<Vertex<TVertex>>();
            this.Edges = new List<Edge<TEdge>>();
            this.VerticesIndex = new Dictionary<long, Vertex<TVertex>>();
            this.OutgoingEdges = new DoubleDictionary<long, long, Edge<TEdge>>();
            this.IncomingEdges = new DoubleDictionary<long, long, Edge<TEdge>>();
        }

        public Graph(IEnumerable<Vertex<TVertex>> vertices, IEnumerable<Edge<TEdge>> edges)
            : this()
        {
            this.Vertices = vertices.ToList();
            this.Edges = edges.ToList();
            this.VerticesIndex = vertices.ToDictionary(vertex => vertex.ID, vertex => vertex);

            // TODO: remove duplicate edges
            edges.ForEach(edge =>
            {
                this.OutgoingEdges.Add(edge.Source, edge.Destination, edge);
                this.IncomingEdges.Add(edge.Destination, edge.Source, edge);
            });
        }

        public IEnumerable<EdgeTriplet<TVertex, TEdge>> GetTriplets()
        {
            return this.Edges
                .Select(edge => new EdgeTriplet<TVertex, TEdge>(
                    source: edge.Source,
                    destination: edge.Destination,
                    sourceProperty: this.VerticesIndex[edge.Source].Property,
                    edgeProperty: edge.Property,
                    destinationProperty: this.VerticesIndex[edge.Destination].Property))
                .ToList();
        }

        public IDictionary<long, int> GetOutgoingDegrees() => this.Vertices.ToDictionary(vertex => vertex.ID, vertex => this.OutgoingEdges[vertex.ID].Count);

        public IDictionary<long, int> GetIncomingDegrees() => this.Vertices.ToDictionary(vertex => vertex.ID, vertex => this.IncomingEdges[vertex.ID].Count);

        public IDictionary<long, int> GetDegrees() => this.Vertices.ToDictionary(vertex => vertex.ID, vertex => this.OutgoingEdges[vertex.ID].Count + this.IncomingEdges[vertex.ID].Count);

        public Graph<TVertex2, TEdge> MapVertices<TVertex2>(Func<long, TVertex, TVertex2> mapper)
        {
            var newVertices = this.Vertices
                .Select(vertex => new Vertex<TVertex2>(
                    id: vertex.ID, 
                    property: mapper(vertex.ID, vertex.Property)))
                .ToList();

            // TODO: optimize so that we don't re-index
            return new Graph<TVertex2, TEdge>(vertices: newVertices, edges: this.Edges);
        }

        public Graph<TVertex, TEdge2> MapEdges<TEdge2>(Func<TEdge, TEdge2> mapper)
        {
            var newEdges = this.Edges
                .Select(edge => new Edge<TEdge2>(
                    source: edge.Source,
                    destination: edge.Destination,
                    property: mapper(edge.Property)))
                .ToList();

            // TODO: optimize so that we don't re-index
            return new Graph<TVertex, TEdge2>(vertices: this.Vertices, edges: newEdges);
        }

        public Graph<TVertex, TEdge> Reverse()
        {
            return new Graph<TVertex, TEdge>(
                vertices: this.Vertices, 
                edges: this.Edges.Select(edge => edge.Reverse()).ToList());
        }

        public Graph<TVertex, TEdge> GetSubgraph(
            Predicate<Vertex<TVertex>> vertexPredicate, 
            Predicate<EdgeTriplet<TVertex, TEdge>> edgePredicate)
        {
            var newVertices = this.Vertices
                .Where(vertex => vertexPredicate(vertex))
                .ToList();

            var newEdges = this.GetTriplets()
                .Where(triplet => edgePredicate(triplet))
                .ToList();

            return new Graph<TVertex, TEdge>(vertices: newVertices, edges: newEdges);
        }

        public IDictionary<long, List<Vertex<TVertex>>> GetNeighbors(EdgeDirection direction)
        {
            var outgoingNeighbors = this.Vertices.ToDictionary(
                keySelector: vertex => vertex.ID, 
                elementSelector: vertex => this.OutgoingEdges[vertex.ID].Keys
                    .Select(id => this.VerticesIndex[id])
                    .ToList());

            var incomingNeighbors = this.Vertices.ToDictionary(
                keySelector: vertex => vertex.ID,
                elementSelector: vertex => this.IncomingEdges[vertex.ID].Keys
                    .Select(id => this.VerticesIndex[id])
                    .ToList());

            if (direction == EdgeDirection.Outgoing)
            {
                return outgoingNeighbors;
            }
            else if (direction == EdgeDirection.Incoming)
            {
                return incomingNeighbors;
            }
            else if (direction == EdgeDirection.Either)
            {
                return this.Vertices.ToDictionary(
                    keySelector: vertex => vertex.ID, 
                    elementSelector: vertex => outgoingNeighbors[vertex.ID].Union(incomingNeighbors[vertex.ID]).Distinct().ToList());
            }
            else // direction == EdgeDirection.Both
            {
                return this.Vertices.ToDictionary(
                    keySelector: vertex => vertex.ID,
                    elementSelector: vertex => outgoingNeighbors[vertex.ID].Intersect(incomingNeighbors[vertex.ID]).Distinct().ToList());
            }
        }

        public IDictionary<long, TMessage> MapReduce<TMessage>(
            Func<EdgeTriplet<TVertex, TEdge>, TMessage> sourceMapper, 
            Func<EdgeTriplet<TVertex, TEdge>, TMessage> destinationMapper, 
            Func<TMessage, TMessage, TMessage> reducer)
        {
            var messages = new SafeDictionary<long, List<TMessage>>(
                createOnMiss: true, 
                valueGenerator: key => new List<TMessage>());

            this.GetTriplets().ForEach(triplet =>
            {
                messages[triplet.Source].Add(sourceMapper(triplet));
                messages[triplet.Destination].Add(destinationMapper(triplet));
            });

            return this.Vertices.ToDictionary(
                keySelector: vertex => vertex.ID, 
                elementSelector: vertex => messages[vertex.ID].Aggregate(reducer));
        }

        //public IDictionary<long, TMsg> AggregateMessages<TMsg>(
        //    Action<EdgeTriplet<TVertex, TEdge>, Action<TMsg>> sendMessage, 
        //    Func<TMsg, TMsg, TMsg> mergeMessage)
        //{

        //}
    }

    public class GraphBuilder
    {

    }
}

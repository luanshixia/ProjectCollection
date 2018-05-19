using Dreambuild.Collections;
using Dreambuild.Extensions;
using Dreambuild.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        protected Dictionary<long, TVertex> Vertices { get; set; }

        protected DoubleDictionary<long, long, TEdge> OutgoingEdges { get; set; }

        protected DoubleDictionary<long, long, TEdge> IncomingEdges { get; set; }

        protected Graph()
        {
            this.Vertices = new Dictionary<long, TVertex>();
            this.OutgoingEdges = new DoubleDictionary<long, long, TEdge>();
            this.IncomingEdges = new DoubleDictionary<long, long, TEdge>();
        }

        public Graph(IEnumerable<Vertex<TVertex>> vertices, IEnumerable<Edge<TEdge>> edges)
            : this()
        {
            this.Vertices = vertices.ToDictionary(vertex => vertex.ID, vertex => vertex.Property);

            // TODO: remove duplicate edges
            edges.ForEach(edge =>
            {
                this.OutgoingEdges.Add(edge.Source, edge.Destination, edge.Property);
                this.IncomingEdges.Add(edge.Destination, edge.Source, edge.Property);
            });
        }

        public string ToDebugString()
        {
            return Json.Encode(new { this.Vertices, this.OutgoingEdges, this.IncomingEdges });
        }

        public IList<EdgeTriplet<TVertex, TEdge>> GetTriplets()
        {
            return this.GetEdges()
                .Select(edge => new EdgeTriplet<TVertex, TEdge>(
                    source: edge.Source,
                    destination: edge.Destination,
                    sourceProperty: this.Vertices[edge.Source],
                    edgeProperty: edge.Property,
                    destinationProperty: this.Vertices[edge.Destination]))
                .ToList();
        }

        public IList<Vertex<TVertex>> GetVertices() => this.Vertices.Select(vertex => new Vertex<TVertex>(vertex.Key, vertex.Value)).ToList();

        public IList<Edge<TEdge>> GetEdges() => this.OutgoingEdges.SelectMany(source => source.Value.Select(destination => new Edge<TEdge>(source.Key, destination.Key, destination.Value))).ToList();

        public IDictionary<long, int> GetOutgoingDegrees() => this.Vertices.ToDictionary(vertex => vertex.Key, vertex => this.OutgoingEdges[vertex.Key].Count);

        public IDictionary<long, int> GetIncomingDegrees() => this.Vertices.ToDictionary(vertex => vertex.Key, vertex => this.IncomingEdges[vertex.Key].Count);

        public IDictionary<long, int> GetDegrees() => this.Vertices.ToDictionary(vertex => vertex.Key, vertex => this.OutgoingEdges[vertex.Key].Count + this.IncomingEdges[vertex.Key].Count);

        public Vertex<TVertex> GetVertex(long id)
        {
            if (!this.Vertices.ContainsKey(id))
            {
                return null;
            }

            return new Vertex<TVertex>(id, this.Vertices[id]);
        }

        public Edge<TEdge> GetEdge(long source, long destination)
        {
            if (!this.OutgoingEdges.ContainsKey(source) || !this.OutgoingEdges[source].ContainsKey(destination))
            {
                return null;
            }

            return new Edge<TEdge>(source, destination, this.OutgoingEdges[source][destination]);
        }

        public Graph<TVertex2, TEdge> MapVertices<TVertex2>(Func<long, TVertex, TVertex2> mapper)
        {
            var newVertices = this.Vertices
                .Select(vertex => new Vertex<TVertex2>(
                    id: vertex.Key,
                    property: mapper(vertex.Key, vertex.Value)))
                .ToList();

            // TODO: optimize so that we don't re-index
            return new Graph<TVertex2, TEdge>(vertices: newVertices, edges: this.GetEdges());
        }

        public Graph<TVertex, TEdge2> MapEdges<TEdge2>(Func<TEdge, TEdge2> mapper)
        {
            var newEdges = this.GetEdges()
                .Select(edge => new Edge<TEdge2>(
                    source: edge.Source,
                    destination: edge.Destination,
                    property: mapper(edge.Property)))
                .ToList();

            // TODO: optimize so that we don't re-index
            return new Graph<TVertex, TEdge2>(vertices: this.GetVertices(), edges: newEdges);
        }

        public Graph<TVertex, TEdge> Reverse()
        {
            return new Graph<TVertex, TEdge>(
                vertices: this.GetVertices(),
                edges: this.GetEdges().Select(edge => edge.Reverse()).ToList());
        }

        public Graph<TVertex, TEdge> GetSubgraph(
            Predicate<Vertex<TVertex>> vertexPredicate,
            Predicate<EdgeTriplet<TVertex, TEdge>> edgePredicate)
        {
            var newVertices = this.GetVertices()
                .Where(vertex => vertexPredicate(vertex))
                .ToList();

            var newEdges = this.GetTriplets()
                .Where(triplet => edgePredicate(triplet))
                .ToList();

            return new Graph<TVertex, TEdge>(vertices: newVertices, edges: newEdges);
        }

        public IDictionary<long, List<long>> GetNeighbors(EdgeDirection direction)
        {
            var outgoingNeighbors = this.Vertices.ToDictionary(
                keySelector: vertex => vertex.Key,
                elementSelector: vertex => this.OutgoingEdges[vertex.Key].Keys.OrderBy(id => id).ToList());

            var incomingNeighbors = this.Vertices.ToDictionary(
                keySelector: vertex => vertex.Key,
                elementSelector: vertex => this.IncomingEdges[vertex.Key].Keys.OrderBy(id => id).ToList());

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
                    keySelector: vertex => vertex.Key,
                    elementSelector: vertex => outgoingNeighbors[vertex.Key]
                        .Union(incomingNeighbors[vertex.Key])
                        .Distinct()
                        .OrderBy(id => id)
                        .ToList());
            }
            else // direction == EdgeDirection.Both
            {
                return this.Vertices.ToDictionary(
                    keySelector: vertex => vertex.Key,
                    elementSelector: vertex => outgoingNeighbors[vertex.Key]
                        .Intersect(incomingNeighbors[vertex.Key])
                        .Distinct()
                        .OrderBy(id => id)
                        .ToList());
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
                keySelector: vertex => vertex.Key,
                elementSelector: vertex => messages[vertex.Key].Aggregate(reducer));
        }
    }

    public static class GraphLoader
    {
        public static Graph<int, int> FromEdgeList(IEnumerable<(long, long)> edgeList)
        {
            edgeList = edgeList.ToList();

            var vertexList = edgeList
                .SelectMany(edge => new[] { edge.Item1, edge.Item2 })
                .Distinct()
                .OrderBy(vertex => vertex)
                .ToList();

            return new Graph<int, int>(
                vertices: vertexList.Select(id => id.ToVertex(0)).ToArray(),
                edges: edgeList.Select(edge => edge.ToEdge(0)).ToArray());
        }

        public static Graph<int, int> FromEdgeListLines(IEnumerable<string> edgeListLines, string fieldDelimiter = "\t")
        {
            return GraphLoader.FromEdgeList(
                edgeList: edgeListLines
                    .Select(line => 
                    {
                        var items = line.Split(fieldDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (items.Length < 2)
                        {
                            throw new ArgumentException();
                        }

                        if (items.Take(2).Any(item => !item.CanParseToInt64()))
                        {
                            throw new ArgumentException();
                        }

                        return (items[0].ParseToInt64(), items[1].ParseToInt64());
                    }));
        }

        public static Graph<int, int> FromEdgeListFile(string fileName)
        {
            return GraphLoader.FromEdgeListLines(
                edgeListLines: File.ReadAllLines(fileName));
        }

        public static async Task<Graph<int, int>> FromEdgeListFileAsync(string fileName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GraphLoader.FromEdgeListLines(
                edgeListLines: await File.ReadAllLinesAsync(fileName, cancellationToken));
        }

        public static Vertex<TVertex> ToVertex<TVertex>(this long vertexId, TVertex property)
        {
            return new Vertex<TVertex>(vertexId, property);
        }

        public static Edge<TEdge> ToEdge<TEdge>(this (long, long) edge, TEdge property)
        {
            return new Edge<TEdge>(edge.Item1, edge.Item2, property);
        }
    }

    public class GraphBuilder
    {

    }
}

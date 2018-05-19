using Dreambuild.Data;
using Dreambuild.Extensions;
using Dreambuild.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dreambuild.Common.Test
{
    public class GraphTests
    {
        [Fact]
        public void Loading()
        {
            var edgeListLines = this.GetSimpleEdgeListLines();

            var graph = GraphLoader.FromEdgeListLines(edgeListLines);
            var vertices = graph.GetVertices();
            var edges = graph.GetEdges();

            Assert.Equal(expected: 4, actual: vertices.Count);
            Assert.Equal(expected: 6, actual: edges.Count);
        }

        [Fact]
        public void Indexing()
        {
            var edgeListLines = this.GetSimpleEdgeListLines();

            var graph = GraphLoader.FromEdgeListLines(edgeListLines);

            Assert.Equal(
                expected: "{\"Vertices\":{\"1\":0,\"2\":0,\"3\":0,\"4\":0},\"OutgoingEdges\":{\"1\":{\"2\":0,\"3\":0,\"4\":0},\"2\":{\"3\":0,\"4\":0},\"3\":{\"4\":0}},\"IncomingEdges\":{\"2\":{\"1\":0},\"3\":{\"1\":0,\"2\":0},\"4\":{\"1\":0,\"2\":0,\"3\":0}}}",
                actual: graph.ToDebugString());
        }

        [Fact]
        public void Degrees()
        {
            var edgeListLines = this.GetSimpleEdgeListLines();

            var graph = GraphLoader.FromEdgeListLines(edgeListLines);
            var outgoingDegrees = graph.GetOutgoingDegrees();
            var incomingDegrees = graph.GetIncomingDegrees();
            var degrees = graph.GetDegrees();

            Assert.Equal(expected: 4, actual: outgoingDegrees.Count);
            Assert.Equal(expected: 4, actual: incomingDegrees.Count);
            Assert.Equal(expected: 4, actual: degrees.Count);

            Assert.Equal(expected: 3, actual: outgoingDegrees[1]);
            Assert.Equal(expected: 2, actual: outgoingDegrees[2]);
            Assert.Equal(expected: 1, actual: outgoingDegrees[3]);
            Assert.Equal(expected: 0, actual: outgoingDegrees[4]);

            Assert.Equal(expected: 0, actual: incomingDegrees[1]);
            Assert.Equal(expected: 1, actual: incomingDegrees[2]);
            Assert.Equal(expected: 2, actual: incomingDegrees[3]);
            Assert.Equal(expected: 3, actual: incomingDegrees[4]);

            Assert.Equal(expected: 3, actual: degrees[1]);
            Assert.Equal(expected: 3, actual: degrees[2]);
            Assert.Equal(expected: 3, actual: degrees[3]);
            Assert.Equal(expected: 3, actual: degrees[4]);
        }

        [Fact]
        public void Degrees_RandomGraph()
        {
            var edgeList = this.GetRandomEdgeList(100, 100);
            var graph = GraphLoader.FromEdgeList(edgeList);

            var outgoingDegrees = graph.GetOutgoingDegrees();
            var incomingDegrees = graph.GetIncomingDegrees();
            var degrees = graph.GetDegrees();

            Assert.Equal(
                expected: degrees.Sum(vertex => vertex.Value),
                actual: outgoingDegrees.Sum(vertex => vertex.Value) + incomingDegrees.Sum(vertex => vertex.Value));

            Assert.Equal(
                expected: 2 * graph.GetEdges().Count,
                actual: degrees.Sum(vertex => vertex.Value));
        }

        [Fact]
        public void Reverse_RandomGraph()
        {
            var edgeList = this.GetRandomEdgeList(100, 100);
            var graph = GraphLoader.FromEdgeList(edgeList);
            var graphPrime = graph.Reverse();

            TestHelpers.DictionaryEqual(graph.GetOutgoingDegrees(), graphPrime.GetIncomingDegrees());
            TestHelpers.DictionaryEqual(graph.GetIncomingDegrees(), graphPrime.GetOutgoingDegrees());

            Assert.Equal(
                Json.Encode(graph.GetNeighbors(EdgeDirection.Outgoing)), 
                Json.Encode(graphPrime.GetNeighbors(EdgeDirection.Incoming)));

            Assert.Equal(
                Json.Encode(graph.GetNeighbors(EdgeDirection.Incoming)),
                Json.Encode(graphPrime.GetNeighbors(EdgeDirection.Outgoing)));
        }

        [Fact]
        public void Neighbors()
        {
            var edgeListLines = this.GetSimpleEdgeListLines();

            var graph = GraphLoader.FromEdgeListLines(edgeListLines);

            var outgoingNeighbors = graph.GetNeighbors(EdgeDirection.Outgoing);
            Assert.Equal(
                expected: "{\"1\":[2,3,4],\"2\":[3,4],\"3\":[4],\"4\":[]}",
                actual: Json.Encode(outgoingNeighbors));

            var incomingNeighbors = graph.GetNeighbors(EdgeDirection.Incoming);
            Assert.Equal(
                expected: "{\"1\":[],\"2\":[1],\"3\":[1,2],\"4\":[1,2,3]}",
                actual: Json.Encode(incomingNeighbors));

            var eitherNeighbors = graph.GetNeighbors(EdgeDirection.Either);
            Assert.Equal(
                expected: "{\"1\":[2,3,4],\"2\":[1,3,4],\"3\":[1,2,4],\"4\":[1,2,3]}",
                actual: Json.Encode(eitherNeighbors));

            var bothNeighbors = graph.GetNeighbors(EdgeDirection.Both);
            Assert.Equal(
                expected: "{\"1\":[],\"2\":[],\"3\":[],\"4\":[]}",
                actual: Json.Encode(bothNeighbors));
        }

        private string[] GetSimpleEdgeListLines()
        {
            return new[]
            {
                "1\t2",
                "1\t3",
                "1\t4",
                "2\t3",
                "2\t4",
                "3\t4",
            };
        }

        private (long, long)[] GetRandomEdgeList(int maxVertexId, int numEdges)
        {
            var rand = new Random();
            return Enumerable
                .Range(0, numEdges)
                .Select(i => ((long)rand.Next(1, maxVertexId), (long)rand.Next(1, maxVertexId)))
                .ToArray();
        }
    }
}

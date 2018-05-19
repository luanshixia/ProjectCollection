using Dreambuild.Data;
using Dreambuild.Extensions;
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
    }
}

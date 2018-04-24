using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dreambuild.Common.Test
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void Chunk()
        {
            var chunkSize = 10;
            var originalText = "xUnit support two different types of unit test, Fact and Theory. We use xUnit Fact when we have some criteria that always must be met, regardless of data. For example, when we test a controller's action to see if it's returning the correct view. xUnit Theory on the other hand depends on set of parameters and its data, our test will pass for some set of data and not the others. We have a theory which postulate that with this set of data, this will happen. In this post, I'm going to discuss what are our options when we need to feed a theory with a set of data and see why and when to use them.";

            var originalBytes = Encoding.UTF8.GetBytes(originalText);
            var chunks = originalBytes.Chunk(chunkSize).ToArray();

            var restoredBytes = chunks.Flatten().ToArray();
            var restoredText = Encoding.UTF8.GetString(restoredBytes);

            Assert.True(chunks.Length >= originalBytes.Length / chunkSize);
            Assert.True(chunks.Length <= originalBytes.Length / chunkSize + 1);
            Assert.Equal(expected: originalBytes.Length, actual: restoredBytes.Length);
            Assert.Equal(expected: originalText, actual: restoredText);
        }

        [Fact]
        public void Chunk_OneChunkOnly()
        {
            var chunkSize = 10;
            var originalText = "xUnit";
            var chunks = originalText.Chunk(chunkSize).ToArray();

            Assert.Single(collection: chunks);
            Assert.Equal(expected: originalText, actual: new string(chunks.First()));
        }

        [Fact]
        public async void ElementsAt_Optimized()
        {
            var n = 99999;
            var source = 0.SeqTo(n);
            var indices = 0.SeqTo(n / 2);

            await Task
                .Run(() => source.ElementsAt(indices).ToArray())
                .MustFinishWithin(TimeSpan.FromMilliseconds(100));

            await Task
                .Run(() => fetch_Slow(indices, source).ToArray())
                .CannotFinishWithin(TimeSpan.FromMilliseconds(1000));
        }

        private static IEnumerable<T> fetch_Slow<T>(IEnumerable<int> indices, IEnumerable<T> source)
        {
            return indices.Select(i => source.ElementAt(i));
        }

        [Fact]
        public void MinAndMax()
        {
            var source = 1.SeqTo(1000);

            Assert.Equal(expected: 0, actual: source.Min(selector: x => x, resultSelector: (x, i) => i));
            Assert.Equal(expected: 999, actual: source.Max(selector: x => x, resultSelector: (x, i) => i));
        }

        [Fact]
        public void Recurrent()
        {
            Assert.Equal(expected: new[] { 64.0, 32.0, 16.0, 8.0, 4.0, 2.0, 1.0, 0.5, 0.25, 0.125 }, actual: 10.Recurrent(x => x / 2, 64.0));
            Assert.Equal(expected: new[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 }, actual: 10.Recurrent((x, y) => x + y, 1, 1));
        }
    }
}

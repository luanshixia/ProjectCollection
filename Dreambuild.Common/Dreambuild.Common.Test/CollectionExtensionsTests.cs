using System;
using System.Linq;
using System.Text;
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
    }
}

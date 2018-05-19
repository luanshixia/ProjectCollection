using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dreambuild.Common.Test
{
    public static class TestHelpers
    {
        public static async Task MustFinishWithin(this Task task, TimeSpan timeout, string message = null)
        {
            var taskFinishedFirst = await Task.WhenAny(task, Task.Delay(timeout));
            Assert.True(condition: taskFinishedFirst == task, userMessage: message);
        }

        public static async Task CannotFinishWithin(this Task task, TimeSpan timeout, string message = null)
        {
            var taskFinishedFirst = await Task.WhenAny(task, Task.Delay(timeout));
            Assert.False(condition: taskFinishedFirst == task, userMessage: message);
        }

        public static void DictionaryEqual<K, V>(IDictionary<K, V> expected, IDictionary<K, V> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            var expectedKeys = new HashSet<K>(expected.Keys);
            var actualKeys = new HashSet<K>(actual.Keys);

            Assert.Subset(expectedKeys, actualKeys);
            Assert.Superset(expectedKeys, actualKeys);

            expectedKeys.ForEach(key =>
            {
                Assert.Equal(expected[key], actual[key]);
            });
        }
    }
}

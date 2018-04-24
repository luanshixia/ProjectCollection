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
    }
}

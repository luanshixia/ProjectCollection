namespace CosmosDBQueryCharge
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the value for the key or the default value if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key.</param>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            return source != null && source.TryGetValue(key, out TValue value) ? value : default;
        }

        public static int GetDocumentSize(this object documentObject)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(documentObject)).Length;
        }

        public static T PickRandomItem<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(RandomNumber.Next(source.Count()));
        }

        public static T[] PickRandomItems<T>(this IEnumerable<T> source)
        {
            return source.Aggregate(seed: new List<T>(), func: (list, item) =>
            {
                if (RandomNumber.Next(2) == 1)
                {
                    list.Add(item);
                }

                return list;
            }).ToArray();
        }

        /// <summary>
        /// Adds the value to collection and return collection instance back.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">Type of the item.</typeparam>
        /// <param name="source">The collection to add to.</param>
        /// <param name="item">The value to add.</param>
        public static TCollection WithItem<TCollection, TItem>(this TCollection source, TItem item)
            where TCollection : ICollection<TItem>
        {
            source.Add(item);
            return source;
        }

        /// <summary>
        /// Breaks the enumerable into a group of enumerable values and executes the action using the selector concurrently.
        /// </summary>
        /// <typeparam name="TElement">Type stored in the enumerable.</typeparam>
        /// <typeparam name="TResult">Type of object returned by the selector.</typeparam>
        /// <param name="source">The source enumerable to batch.</param>
        /// <param name="selector">The function to select result from each operation.</param>
        /// <param name="concurrencyLimit">The concurrency limit.</param>
        public static Task<List<TResult>> SelectConcurrently<TElement, TResult>(
            this IEnumerable<TElement> source,
            Func<TElement, Task<TResult>> selector,
            int concurrencyLimit)
        {
            return source.AggregateOperationConcurrently(
                operation: selector,
                concurrencyLimit: concurrencyLimit,
                seed: new List<TResult>(),
                accumulator: (state, result) => state.WithItem(result));
        }

        /// <summary>
        /// Breaks the enumerable into a group of enumerable values and aggregates the action using the selector concurrently.
        /// </summary>
        /// <typeparam name="TElement">Type stored in the enumerable.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulate.</typeparam>
        /// <param name="source">The source enumerable to batch.</param>
        /// <param name="operation">The operation that needs to be executed concurrently.</param>
        /// <param name="concurrencyLimit">The concurrency limit.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="accumulator">The accumulator function.</param>
        public static async Task<TAccumulate> AggregateOperationConcurrently<TElement, TResult, TAccumulate>(
            this IEnumerable<TElement> source,
            Func<TElement, Task<TResult>> operation,
            int concurrencyLimit,
            TAccumulate seed,
            Func<TAccumulate, TResult, TAccumulate> accumulator)
        {
            var queue = new Queue<Task<TResult>>();
            var pending = new HashSet<Task<TResult>>();
            try
            {
                foreach (var element in source)
                {
                    while (queue.Any() && !pending.Contains(queue.Peek()))
                    {
                        seed = accumulator(seed, queue.Dequeue().Result);
                    }

                    if (pending.Count == concurrencyLimit)
                    {
                        var completed = pending.FirstOrDefault(task => task.IsCompleted) ?? await Task.WhenAny(pending).ConfigureAwait(continueOnCapturedContext: false);
                        if (completed.Status != TaskStatus.RanToCompletion)
                        {
                            break;
                        }

                        pending.Remove(completed);
                    }

                    var started = operation(element);
                    queue.Enqueue(started);
                    pending.Add(started);
                }
            }
            finally
            {
                await Task.WhenAll(pending).WrapMultipleExceptionsForAwait().ConfigureAwait(continueOnCapturedContext: false);
            }

            foreach (var completed in queue)
            {
                seed = accumulator(seed, completed.Result);
            }

            return seed;
        }

        /// <summary>
        /// Wraps the multiple exceptions as single aggregate exception for await operator.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by task.</typeparam>
        /// <param name="task">The asynchronous task.</param>
        public static Task<T> WrapMultipleExceptionsForAwait<T>(this Task<T> task)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            task.ContinueWith(
                continuationAction: ignored => CompleteTaskAndWrapMultipleExceptions(task, tcs),
                continuationOptions: TaskContinuationOptions.ExecuteSynchronously,
                cancellationToken: CancellationToken.None,
                scheduler: TaskScheduler.Default);

            return tcs.Task;
        }

        /// <summary>
        /// Completes the task completion source and wraps multiple exceptions as single aggregate exception.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by task.</typeparam>
        /// <param name="task">The asynchronous task.</param>
        /// <param name="completionSource">The task completion source.</param>
        private static void CompleteTaskAndWrapMultipleExceptions<T>(Task task, TaskCompletionSource<T> completionSource)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    completionSource.SetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    completionSource.SetResult(task is Task<T> genericTask ? genericTask.Result : default);
                    break;
                case TaskStatus.Faulted:
                    completionSource.SetException(task.Exception.InnerExceptions.Count > 1 ? task.Exception : task.Exception.InnerException);
                    break;
            }
        }
    }
}

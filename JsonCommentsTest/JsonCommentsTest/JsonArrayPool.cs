using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;

namespace JsonCommentsTest
{
    /// <summary>
    /// The JSON array pool.
    /// </summary>
    internal sealed class JsonArrayPool : IArrayPool<char>
    {
        /// <summary>
        /// Rent an array from the pool. This array must be returned when it is no longer needed.
        /// </summary>
        /// <param name="minimumLength">The minimum required length of the array. The returned array may be longer.</param>
        /// <returns>
        /// The rented array from the pool. This array must be returned when it is no longer needed.
        /// </returns>
        public char[] Rent(int minimumLength)
        {
            return ArrayPool<char>.Shared.Rent(minimumLength);
        }

        /// <summary>
        /// Return an array to the pool.
        /// </summary>
        /// <param name="array">The array that is being returned.</param>
        public void Return(char[] array)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }

    /// <summary>
    /// Provides a resource pool that enables reusing instances of type.
    /// </summary>
    /// <typeparam name="T">The type of the array items.</typeparam>
    public abstract class ArrayPool<T>
    {
        /// <summary>
        /// The lazily-initialized shared pool instances.
        /// </summary>
        private static ArrayPool<T>[] sharedInstances = null;

        /// <summary>
        /// Gets a shared <see cref="ArrayPool{T}"/> instance.
        /// </summary>
        public static ArrayPool<T> Shared
        {
            get
            {
                var instances = Volatile.Read(ref ArrayPool<T>.sharedInstances) ?? ArrayPool<T>.EnsureSharedCreated();
                return instances[MemoryUtility.CurrentExecutionId & (instances.Length - 1)];
            }
        }

        /// <summary>
        /// Ensures that <see cref="sharedInstances" /> has been initialized to a pool and returns it.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArrayPool<T>[] EnsureSharedCreated()
        {
            Interlocked.CompareExchange(
                location1: ref ArrayPool<T>.sharedInstances,
                value: Enumerable.Range(0, MemoryUtility.GetNextPowerOf2(Environment.ProcessorCount)).Select(index => new SharedArrayPool<T>()).ToArray(),
                comparand: null);

            return ArrayPool<T>.sharedInstances;
        }

        /// <summary>
        /// Gets the maximum length of array in the pool.
        /// </summary>
        public abstract int MaximumLength { get; }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array needed.</param>
        public abstract T[] Rent(int minimumLength);

        /// <summary>
        /// Returns to the pool an array that was previously obtained via <see cref="Rent" /> on the same
        /// <see cref="ArrayPool{T}" /> instance.
        /// </summary>
        /// <param name="array">The buffer previously obtained from <see cref="Rent" /> to return to the pool.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "By design.")]
        public abstract void Return(T[] array);
    }

    /// <summary>
    /// The internal memory management utility.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "PInvoke methods are not exposed.")]
    internal static class MemoryUtility
    {
        #region CurrentExecutionId

        /// <summary>
        /// The execution identifier cache shift.
        /// </summary>
        private const int ExecutionIdCacheShift = 16;

        /// <summary>
        /// The execution identifier cache count down mask.
        /// </summary>
        private const int ExecutionIdCacheCountDownMask = (1 << MemoryUtility.ExecutionIdCacheShift) - 1;

        /// <summary>
        /// The execution identifier refresh rate.
        /// </summary>
        private const int ExecutionIdRefreshRate = 1000;

        /// <summary>
        /// The thread local execution identifier cache.
        /// </summary>
        [ThreadStatic]
        private static int threadExecutionIdCache;

        /// <summary>
        /// Gets the cached processor number used as a hint for which per-core resource to access.
        /// It is periodically refreshed to trail the actual thread core affinity.
        /// </summary>
        internal static int CurrentExecutionId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var executionIdCache = MemoryUtility.threadExecutionIdCache--;
                if ((executionIdCache & MemoryUtility.ExecutionIdCacheCountDownMask) == 0)
                {
                    executionIdCache = MemoryUtility.RefreshExecutionIdCache();
                }

                return executionIdCache >> MemoryUtility.ExecutionIdCacheShift;
            }
        }

        /// <summary>
        /// Refreshes the execution identifier cache.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int RefreshExecutionIdCache()
        {
            return MemoryUtility.threadExecutionIdCache = (MemoryUtility.GetCurrentProcessorNumber() << MemoryUtility.ExecutionIdCacheShift) | MemoryUtility.ExecutionIdRefreshRate;
        }

        /// <summary>
        /// Gets the number of the processor the current thread was running on during the call to this function.
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetCurrentProcessorNumber();

        #endregion

        /// <summary>
        /// Disposes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="memoryManager">The memory manager.</param>
        public static void Dispose(ref byte[] buffer, MemoryBufferManager memoryManager)
        {
            var obj = Interlocked.Exchange(ref buffer, null);
            if (obj != null && memoryManager != null)
            {
                memoryManager.ReturnBuffer(obj);
            }
        }

        /// <summary>
        /// Disposes the specified buffers.
        /// </summary>
        /// <param name="buffers">The buffers.</param>
        /// <param name="memoryManager">The memory manager.</param>
        internal static void Dispose(ref List<byte[]> buffers, MemoryBufferManager memoryManager)
        {
            var obj = Interlocked.Exchange(ref buffers, null);
            if (obj != null && memoryManager != null)
            {
                foreach (var buffer in obj)
                {
                    memoryManager.ReturnBuffer(buffer);
                }
            }
        }

        /// <summary>
        /// Disposes the specified disposable.
        /// </summary>
        /// <typeparam name="T">The type of disposable object.</typeparam>
        /// <param name="disposable">The disposable object.</param>
        internal static void Dispose<T>(ref T disposable)
            where T : class, IDisposable
        {
            var obj = Interlocked.Exchange(ref disposable, null);
            if (obj != null)
            {
                obj.Dispose();
            }
        }

        /// <summary>
        /// Selects the index of the array pool bucket.
        /// </summary>
        /// <param name="bufferSize">Size of the requested buffer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Performance-critical code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Performance-critical code.")]
        internal static int GetBucketIndex(int bufferSize)
        {
            uint bitsRemaining = ((uint)bufferSize - 1) >> 4;

            int poolIndex = 0;
            if (bitsRemaining > 0xFFFF) { bitsRemaining >>= 16; poolIndex = 16; }
            if (bitsRemaining > 0xFF) { bitsRemaining >>= 8; poolIndex += 8; }
            if (bitsRemaining > 0xF) { bitsRemaining >>= 4; poolIndex += 4; }
            if (bitsRemaining > 0x3) { bitsRemaining >>= 2; poolIndex += 2; }
            if (bitsRemaining > 0x1) { bitsRemaining >>= 1; poolIndex += 1; }

            return poolIndex + (int)bitsRemaining;
        }

        /// <summary>
        /// Gets the buffer size for the array pool bucket.
        /// </summary>
        /// <param name="index">Index of the array pool bucket.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetBucketBufferSize(int index)
        {
            return 16 << index;
        }

        /// <summary>
        /// Gets the next power of 2.
        /// </summary>
        /// <param name="num">The number.</param>
        internal static int GetNextPowerOf2(int num)
        {
            num--;

            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;

            return num + 1;
        }
    }

    /// <summary>
    /// The shared resource pool implementation.
    /// </summary>
    /// <typeparam name="T">The type of the array items.</typeparam>
    internal sealed class SharedArrayPool<T> : ArrayPool<T>
    {
        /// <summary>
        /// The maximum length of each array in the pool (2^22).
        /// </summary>
        private const int MaxArrayLength = 4 * 1024 * 1024;

        /// <summary>
        /// The maximum number of arrays per bucket that are available for rent.
        /// </summary>
        private const int MaxNumberOfArraysPerBucket = 24;

        /// <summary>
        /// The thread-safe buckets containing buffers that can be rented and returned.
        /// </summary>
        private readonly Bucket[] buckets;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedArrayPool{T}"/> class.
        /// </summary>
        internal SharedArrayPool()
        {
            this.buckets = new Bucket[MemoryUtility.GetBucketIndex(SharedArrayPool<T>.MaxArrayLength) + 1];

            for (var i = 0; i < this.buckets.Length; i++)
            {
                this.buckets[i] = new Bucket(
                    bufferLength: MemoryUtility.GetBucketBufferSize(i),
                    numberOfBuffers: SharedArrayPool<T>.MaxNumberOfArraysPerBucket);
            }
        }

        /// <summary>
        /// Gets the maximum length of array in the pool.
        /// </summary>
        public override int MaximumLength
        {
            get { return SharedArrayPool<T>.MaxArrayLength; }
        }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array needed.</param>
        public override T[] Rent(int minimumLength)
        {
            if (minimumLength == 0)
            {
                return EmptyArray<T>.Instance;
            }

            var index = MemoryUtility.GetBucketIndex(minimumLength);
            return index < this.buckets.Length ? this.buckets[index].Rent() : new T[minimumLength];
        }

        /// <summary>
        /// Returns to the pool an array that was previously obtained.
        /// </summary>
        /// <param name="array">The buffer previously obtained to return to the pool.</param>
        public override void Return(T[] array)
        {
            if (array.Length == 0)
            {
                return;
            }

            // Determine with what bucket this array length is associated.
            var index = MemoryUtility.GetBucketIndex(array.Length);
            if (index < this.buckets.Length)
            {
                // Return the buffer to its bucket. In the future, we might consider having Return return false
                // instead of dropping a bucket, in which case we could try to return to a lower-sized bucket,
                // just as how in Rent we allow renting from a higher-sized bucket.
                this.buckets[index].Return(array);
            }
        }

        /// <summary>
        /// Provides a thread-safe bucket containing buffers that can be rented and returned.
        /// </summary>
        private sealed class Bucket
        {
            /// <summary>
            /// The bucket buffers length.
            /// </summary>
            private readonly int bufferLength;

            /// <summary>
            /// The bucket buffers to rent and return.
            /// </summary>
            private readonly T[][] buffers;

            /// <summary>
            /// The bucket lock. Do not make this readonly; it's a mutable struct.
            /// </summary>
            private SpinLock buffersLock;

            /// <summary>
            /// The current bucket index.
            /// </summary>
            private int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="Bucket"/> class.
            /// </summary>
            /// <param name="bufferLength">Length of the buffer.</param>
            /// <param name="numberOfBuffers">The number of buffers.</param>
            internal Bucket(int bufferLength, int numberOfBuffers)
            {
                this.bufferLength = bufferLength;
                this.buffers = new T[numberOfBuffers][];
                this.buffersLock = new SpinLock(enableThreadOwnerTracking: Debugger.IsAttached);
            }

            /// <summary>
            /// Takes an array from the bucket. If the bucket is empty, returns null.
            /// </summary>
            internal T[] Rent()
            {
                T[] buffer = null;

                // While holding the lock, grab whatever is at the next available index and
                // update the index.  We do as little work as possible while holding the spin
                // lock to minimize contention with other threads.  The try/finally is
                // necessary to properly handle thread aborts on platforms which have them.
                bool lockTaken = false;
                try
                {
                    this.buffersLock.Enter(ref lockTaken);

                    if (this.index < this.buffers.Length)
                    {
                        buffer = this.buffers[this.index];
                        this.buffers[this.index++] = null;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        this.buffersLock.Exit(false);
                    }
                }

                // While we were holding the lock, we grabbed whatever was at the next available index, if
                // there was one. If we tried and if we got back null, that means we hadn't yet allocated
                // for that slot, in which case we should do so now.
                return buffer ?? new T[this.bufferLength];
            }

            /// <summary>
            /// Attempts to return the buffer to the bucket.  If successful, the buffer will be stored
            /// in the bucket and true will be returned; otherwise, the buffer won't be stored, and false
            /// will be returned.
            /// </summary>
            /// <param name="array">The array to return.</param>
            internal void Return(T[] array)
            {
                if (array.Length != this.bufferLength)
                {
                    return;
                }

                // While holding the spin lock, if there's room available in the bucket,
                // put the buffer into the next available slot.  Otherwise, we just drop it.
                // The try/finally is necessary to properly handle thread aborts on platforms
                // which have them.
                bool lockTaken = false;
                try
                {
                    this.buffersLock.Enter(ref lockTaken);

                    if (this.index > 0)
                    {
                        this.buffers[--this.index] = array;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        this.buffersLock.Exit(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// The resource stack memory buffer manager.
    /// </summary>
    public sealed class MemoryBufferManager
    {
        #region Default memory manager

        /// <summary>
        /// The buffer size for small allocations.
        /// </summary>
        /// <remarks>
        /// The value was chosen during performance testing as a compromise between using
        /// as little buffer as possible and still getting large part of performance benefit.
        /// </remarks>
        public const int SmallAllocationsBufferSize = 16 * 1024;

        /// <summary>
        /// The buffer size for large allocations.
        /// </summary>
        /// <remarks>
        /// The value was chosen during performance testing as a compromise between using
        /// as little buffer as possible and still getting large part of performance benefit.
        /// </remarks>
        public const int LargeAllocationsBufferSize = 128 * 1024;

        /// <summary>
        /// The default maximum pool bytes for small allocations.
        /// </summary>
        /// <remarks>
        /// The value was chosen during performance testing targeting high-performance
        /// server applications and scales linearly by number of processors increases.
        /// </remarks>
        public static readonly int DefaultSmallAllocationsMaximumPoolBytes = 8 * 1024 * 1024 * Environment.ProcessorCount;

        /// <summary>
        /// The default maximum pool bytes for large allocations.
        /// </summary>
        /// <remarks>
        /// The value was chosen during performance testing targeting high-performance
        /// server applications and scales linearly by number of processors increases.
        /// </remarks>
        public static readonly int DefaultLargeAllocationsMaximumPoolBytes = 16 * 1024 * 1024 * Environment.ProcessorCount;

        /// <summary>
        /// The instance of memory buffer manager for small allocations.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Field is intentionally mutable to give opportunity for host to override behavior if necessary.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Field is intentionally mutable to give opportunity for host to override behavior if necessary.")]
        public static MemoryBufferManager SmallAllocations = new MemoryBufferManager(
            bufferSize: MemoryBufferManager.SmallAllocationsBufferSize,
            maximumPoolBytes: MemoryBufferManager.DefaultSmallAllocationsMaximumPoolBytes);

        /// <summary>
        /// The instance of memory buffer manager for large allocations.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Field is intentionally mutable to give opportunity for host to override behavior if necessary.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Field is intentionally mutable to give opportunity for host to override behavior if necessary.")]
        public static MemoryBufferManager LargeAllocations = new MemoryBufferManager(
            bufferSize: MemoryBufferManager.LargeAllocationsBufferSize,
            maximumPoolBytes: MemoryBufferManager.DefaultLargeAllocationsMaximumPoolBytes);

        /// <summary>
        /// Gets the instance of memory buffer manager.
        /// </summary>
        /// <param name="capacity">The buffer capacity.</param>
        public static MemoryBufferManager GetInstance(long capacity)
        {
            return capacity < MemoryBufferManager.LargeAllocationsBufferSize
                ? MemoryBufferManager.SmallAllocations
                : MemoryBufferManager.LargeAllocations;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Gets or sets the maximum pool bytes.
        /// </summary>
        public long MaximumPoolBytes { get; set; }

        /// <summary>
        /// Gets the free pool bytes.
        /// </summary>
        public long FreePoolBytes
        {
            get { return this.freePoolBytes; }
        }

        /// <summary>
        /// Gets the allocated pool bytes.
        /// </summary>
        public long AllocatedPoolBytes
        {
            get { return this.allocatedPoolBytes; }
        }

        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// The buffer pool.
        /// </summary>
        private readonly ConcurrentStack<byte[]> pool = new ConcurrentStack<byte[]>();

        /// <summary>
        /// The free pool bytes.
        /// </summary>
        private long freePoolBytes;

        /// <summary>
        /// The allocated pool bytes.
        /// </summary>
        private long allocatedPoolBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBufferManager"/> class.
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="maximumPoolBytes">The maximum pool bytes.</param>
        public MemoryBufferManager(int bufferSize, long maximumPoolBytes)
        {
            this.BufferSize = bufferSize;
            this.MaximumPoolBytes = maximumPoolBytes;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Takes the buffer.
        /// </summary>
        public byte[] TakeBuffer()
        {
            byte[] buffer;
            if (this.pool.TryPop(out buffer))
            {
                Interlocked.Add(ref this.freePoolBytes, -this.BufferSize);
                return buffer;
            }

            Interlocked.Add(ref this.allocatedPoolBytes, this.BufferSize);
            return new byte[this.BufferSize];
        }

        /// <summary>
        /// Returns the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void ReturnBuffer(byte[] buffer)
        {
            if (buffer.Length == this.BufferSize && this.FreePoolBytes < this.MaximumPoolBytes)
            {
                Interlocked.Add(ref this.freePoolBytes, this.BufferSize);
                this.pool.Push(buffer);
            }
        }

        #endregion
    }

    /// <summary>
    /// Empty array collection. Useful in number of places that return an empty array to avoid unnecessary memory allocation.
    /// </summary>
    /// <typeparam name="T">The type of empty array elements.</typeparam>
    public static class EmptyArray<T>
    {
        /// <summary>
        /// The empty array instance.
        /// </summary>
        public static readonly T[] Instance = new T[0];
    }
}

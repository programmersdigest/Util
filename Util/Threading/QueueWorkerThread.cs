using System;
using System.Collections.Concurrent;
using System.Threading;

namespace programmersdigest.Util.Threading
{
    /// <summary>
    /// Starts a new thread executing the given callback on every item being enqueued. Runs
    /// until it gets disposed.
    /// </summary>
    /// <typeparam name="T">The type of the items being enqueued.</typeparam>
    public sealed class QueueWorkerThread<T> : IDisposable
    {
        private readonly WorkerThread _workerThread;
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly Action<T> _callback;
        private readonly int _idleSleepTime;

        /// <summary>
        /// Creates a new <see cref="QueueWorkerThread{T}"/>.
        /// </summary>
        /// <param name="callback">Method to be executed for each item enqueued in the queue.</param>
        /// <param name="idleSleepTime">The time the thread sleeps when there is no item in the queue.</param>
        /// <exception cref="ArgumentNullException">In case <paramref name="callback"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">In case <paramref name="idleSleepTime"/> is begative.</exception>
        public QueueWorkerThread(Action<T> callback, int idleSleepTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (idleSleepTime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idleSleepTime));
            }

            _callback = callback;
            _idleSleepTime = idleSleepTime;

            _workerThread = new WorkerThread(DoWork);
        }

        /// <summary>
        /// Creates a new <see cref="QueueWorkerThread{T}"/> with an idle sleep time of 0 ms.
        /// </summary>
        /// <param name="callback">Method to be executed for each item enqueued in the queue.</param>
        public QueueWorkerThread(Action<T> callback) : this(callback, 0)
        {
        }

        /// <summary>
        /// Disposes the <see cref="QueueWorkerThread{T}"/>.
        /// Requests the underlying <see cref="WorkerThread"/> to stop execution.
        /// </summary>
        public void Dispose()
        {
            _workerThread.Dispose();
        }

        /// <summary>
        /// Enqueues the given item into the worker queue.
        /// </summary>
        /// <param name="item">The item to be enqueued.</param>
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        /// <summary>
        /// Worker method for <see cref="_workerThread"/>.
        /// Tries to dequeue an item from the <see cref="_queue"/>. If there is no item, the thread is put to
        /// sleep for <see cref="_idleSleepTime"/> ms. If there IS an item, <see cref="_callback"/> is executed
        /// for this item.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token provided by the <see cref="_workerThread"/>. Unused.</param>
        private void DoWork(CancellationToken cancellationToken)
        {
            if (!_queue.TryDequeue(out var item))
            {
                Thread.Sleep(_idleSleepTime);
                return;
            }

            _callback(item);
        }
    }
}

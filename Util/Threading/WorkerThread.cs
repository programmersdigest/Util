using System;
using System.Threading;

namespace programmersdigest.Util.Threading
{
    /// <summary>
    /// Starts a new thread executing the given callback in a loop until it gets disposed.
    /// </summary>
    public class WorkerThread : IDisposable
    {
        private readonly Thread _thread;
        private readonly Action<CancellationToken> _callback;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Creates a new <see cref="WorkerThread"/> to execute the given <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">
        /// The callback to run in a loop on the new thread.
        /// Disposing the <see cref="WorkerThread"/> will cancel the <see cref="CancellationToken"/> and
        /// stop the threads loop. If <paramref name="callback"/> is a long running operation, it may be
        /// necesssary to explicitly listen to the <see cref="CancellationToken"/> and abort the operation
        /// if necessary.
        /// </param>
        /// <exception cref="ArgumentNullException">In case <paramref name="callback"/> is null.</exception>
        public WorkerThread(Action<CancellationToken> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _callback = callback;

            _thread = new Thread(DoWork);
            _thread.Start(_cancellationTokenSource.Token);
        }

        /// <summary>
        /// Disposes the <see cref="WorkerThread"/> requesting the underlying thread to stop.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Worker method for <see cref="_thread"/>.
        /// Executes <see cref="_callback"/> in a loop until the <see cref="CancellationToken"/> has
        /// been cancelled (which happens on disposal of the <see cref="WorkerThread"/>).
        /// </summary>
        /// <param name="state">The <see cref="CancellationToken"/> retrieved from <see cref="_cancellationTokenSource"/>.</param>
        private void DoWork(object state)
        {
            var cancellationToken = (CancellationToken)state;

            while (!cancellationToken.IsCancellationRequested)
            {
                _callback(cancellationToken);
            }
        }
    }
}

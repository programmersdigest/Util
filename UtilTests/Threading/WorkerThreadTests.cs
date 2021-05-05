using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util.Threading;
using System;
using System.Threading;

namespace programmersdigest.UtilTests.Threading
{
    [TestClass]
    public class WorkerThreadTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CallbackIsNull_ShouldThrowArgumentNullException()
        {
            new WorkerThread(null!);
        }

        [TestMethod]
        public void Constructor_ValidCallback_ShouldCreateInstance()
        {
            Action<CancellationToken> callback = (cancellationToken) => Thread.Sleep(0);

            var workerThread = new WorkerThread(callback);
            Assert.IsNotNull(workerThread);
        }

        [TestMethod]
        public void DoWork_CallbackShouldGetExecuted()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<CancellationToken> callback = (cancellationToken) =>
            {
                resetEvent.Set();
            };
            new WorkerThread(callback);
            var firstReset = resetEvent.WaitOne(100);

            Assert.IsTrue(firstReset);
        }

        [TestMethod]
        public void DoWork_CallbackShouldBeExecutedInALoop()
        {
            var countdownEvent = new CountdownEvent(5);

            Action<CancellationToken> callback = (cancellationToken) =>
            {
                if (!countdownEvent.IsSet)
                {
                    countdownEvent.Signal();
                }
            };
            new WorkerThread(callback);

            var hasExecutedFiveTimes = countdownEvent.Wait(100);
            Assert.IsTrue(hasExecutedFiveTimes);
        }

        [TestMethod]
        public void DoWork_CallbackShouldRunOnDifferentThread()
        {
            var resetEvent = new AutoResetEvent(false);

            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            var callbackThreadId = 0;

            Action<CancellationToken> callback = (cancellationToken) =>
            {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                resetEvent.Set();
            };
            new WorkerThread(callback);
            resetEvent.WaitOne(100);

            Assert.AreNotEqual(0, callbackThreadId);
            Assert.AreNotEqual(currentThreadId, callbackThreadId);
        }

        [TestMethod]
        public void Dispose_ShouldShutdownThread()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<CancellationToken> callback = (cancellationToken) =>
            {
                resetEvent.Set();
            };
            var workerThread = new WorkerThread(callback);

            workerThread.Dispose();

            Thread.Sleep(3);   // Give the thread some time to shut down.

            resetEvent.Reset();     // Thread should be stopped, we should not get another signal.
            var signalled = resetEvent.WaitOne(3);

            Assert.IsFalse(signalled);
        }

        [TestMethod]
        public void Dispose_CallbackShouldBeSignalledCancellation()
        {
            var controlResetEvent = new AutoResetEvent(false);
            var resultResetEvent = new AutoResetEvent(false);

            Action<CancellationToken> callback = (cancellationToken) =>
            {
                controlResetEvent.Set();    // Signal the test to dispose().
                controlResetEvent.WaitOne(100);
                if (cancellationToken.IsCancellationRequested)
                {
                    resultResetEvent.Set();
                }
            };
            var workerThread = new WorkerThread(callback);

            controlResetEvent.WaitOne(100);
            workerThread.Dispose();
            controlResetEvent.Set();  // Signal the thread to check the cancellation token.

            var cancellationRequested = resultResetEvent.WaitOne(100);
            Assert.IsTrue(cancellationRequested);
        }
    }
}

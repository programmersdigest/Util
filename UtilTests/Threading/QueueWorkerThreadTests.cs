using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util.Threading;
using System;
using System.Diagnostics;
using System.Threading;

namespace programmersdigest.UtilTests.Threading
{
    [TestClass]
    public class QueueWorkerThreadTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CallbackIsNull_ShouldThrowArgumentNullException()
        {
            new QueueWorkerThread<string>(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ValidCallback_IdleSleepTimeIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            Action<string> callback = (item) => { };

            new QueueWorkerThread<string>(callback, -1);
        }

        [TestMethod]
        public void Constructor_ValidCallback_NoIdleSleepTime_ShouldCreateInstance()
        {
            Action<string> callback = (item) => { };

            var queueWorkerThread = new QueueWorkerThread<string>(callback);
            Assert.IsNotNull(queueWorkerThread);
        }

        [TestMethod]
        public void Constructor_ValidCallback_IdleSleepTimeIs0_ShouldCreateInstance()
        {
            Action<string> callback = (item) => { };

            var queueWorkerThread = new QueueWorkerThread<string>(callback, 0);
            Assert.IsNotNull(queueWorkerThread);
        }

        [TestMethod]
        public void Constructor_ValidCallback_IdleSleepTimeIsPositive_ShouldCreateInstance()
        {
            Action<string> callback = (item) => { };

            var queueWorkerThread = new QueueWorkerThread<string>(callback, 1);
            Assert.IsNotNull(queueWorkerThread);
        }

        [TestMethod]
        public void DoWork_NoEnqueuedItems_CallbackShouldNotBeExecuted()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<string> callback = (item) =>
            {
                resetEvent.Set();
            };
            new QueueWorkerThread<string>(callback);
            var signalled = resetEvent.WaitOne(5);

            Assert.IsFalse(signalled);
        }

        [TestMethod]
        public void DoWork_OneEnqueuedItem_CallbackShouldBeExecutedOnce()
        {
            var countdownEvent = new CountdownEvent(1);
            var tooManyExecutions = false;

            Action<string> callback = (item) =>
            {
                if (!countdownEvent.IsSet)
                {
                    countdownEvent.Signal();
                }
                else
                {
                    tooManyExecutions = true;
                }
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback);

            queueWorkerThread.Enqueue("Test 1");

            var executedOnce = countdownEvent.Wait(100);

            Assert.IsTrue(executedOnce);
            Assert.IsFalse(tooManyExecutions);
        }

        [TestMethod]
        public void DoWork_MultipleEnqueuedItems_CallbackShouldRunOncePerItem()
        {
            var countdownEvent = new CountdownEvent(5);
            var tooManyExecutions = false;

            Action<string> callback = (item) =>
            {
                if (!countdownEvent.IsSet)
                {
                    countdownEvent.Signal();
                }
                else
                {
                    tooManyExecutions = true;
                }
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback);

            queueWorkerThread.Enqueue("Test 1");
            queueWorkerThread.Enqueue("Test 2");
            queueWorkerThread.Enqueue("Test 3");
            queueWorkerThread.Enqueue("Test 4");
            queueWorkerThread.Enqueue("Test 5");

            var allItemsExecuted = countdownEvent.Wait(100);

            Assert.IsTrue(allItemsExecuted);
            Assert.IsFalse(tooManyExecutions);
        }

        [TestMethod]
        public void DoWork_OneEnqueuedItem_CallbackShouldReceiveTheEnqueuedItem()
        {
            var resetEvent = new AutoResetEvent(false);
            string receivedItem = null!;

            Action<string> callback = (item) =>
            {
                receivedItem = item;
                resetEvent.Set();
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback);

            queueWorkerThread.Enqueue("Test 1");

            resetEvent.WaitOne(100);

            Assert.AreEqual("Test 1", receivedItem);
        }

        [TestMethod]
        public void DoWork_CallbackShouldRunOnDifferentThread()
        {
            var resetEvent = new AutoResetEvent(false);

            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            var callbackThreadId = 0;

            Action<string> callback = (item) =>
            {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                resetEvent.Set();
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback);
            queueWorkerThread.Enqueue("Test 1");

            resetEvent.WaitOne(100);

            Assert.AreNotEqual(0, callbackThreadId);
            Assert.AreNotEqual(currentThreadId, callbackThreadId);
        }

        [TestMethod]
        public void DoWork_IdleSleepTimeIsDefault_ShouldNotSleepWhenIdle()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<string> callback = (item) =>
            {
                resetEvent.Set();
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback);

            queueWorkerThread.Enqueue("Test 1");
            resetEvent.WaitOne(100);
            queueWorkerThread.Enqueue("Test 2");
            var executedItem2 = resetEvent.WaitOne(5);  // Executing item 2 may at most take 5 ms.

            Assert.IsTrue(executedItem2);
        }

        [TestMethod]
        public void DoWork_IdleSleepTimeIs50ms_ShouldSleep50msWhenIdle()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<string> callback = (item) =>
            {
                resetEvent.Set();
            };
            var queueWorkerThread = new QueueWorkerThread<string>(callback, 50);

            // This test is not exact - we try getting close by measuring the time it takes between
            // execution of two items with a forced idle time in between.
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Let the first item be executed.
            queueWorkerThread.Enqueue("Test 1");
            resetEvent.WaitOne(100);

            // Wait for a moment, after which we should be idling for 50ms.
            Thread.Sleep(10);

            // Add another item and see how long it takes for execution.
            queueWorkerThread.Enqueue("Test 2");
            resetEvent.WaitOne(100);

            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 50); // Should almost always be == 50, but the measurement is not 100% exact.
        }

        [TestMethod]
        public void Dispose_ShouldShutdownThread()
        {
            var resetEvent = new AutoResetEvent(false);

            Action<string> callback = (item) =>
            {
                resetEvent.Set();
            };
            var queueuWorkerThread = new QueueWorkerThread<string>(callback);

            queueuWorkerThread.Dispose();
            Thread.Sleep(3);   // Give the thread some time to shut down.

            // Reset the resetEvent and enqueue an item, which should trigger
            // the callback if the thread is still running (which it should not be).
            resetEvent.Reset();
            queueuWorkerThread.Enqueue("Test 1");
            var signaled = resetEvent.WaitOne(3);

            Assert.IsFalse(signaled);
        }
    }
}
